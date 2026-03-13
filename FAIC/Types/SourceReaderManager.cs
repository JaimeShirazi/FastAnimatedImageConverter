using SharpGen.Runtime;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vortice.MediaFoundation;
using static Vortice.MediaFoundation.MediaFactory;

namespace FAIC.Types
{
    internal class SourceReaderManager : IDisposable
    {
        public event Action OnSupportChange = () => { };
        public struct StepResult : IDisposable
        {
            public struct Metadata
            {
                public int Width => HasAperture ? ApertureWidth : CodedWidth;
                public int Height => HasAperture ? ApertureHeight : CodedHeight;
                public bool HasAperture;
                public int CodedWidth, CodedHeight;
                public int Stride;
                public short OffsetX, OffsetXFrac, OffsetY, OffsetYFrac; //Not actually used - would require some changes to properly support
                public int ApertureWidth, ApertureHeight;
                public Metadata(IMFSourceReader reader)
                {
                    IMFMediaType newType = reader.GetCurrentMediaType(SourceReaderIndex.FirstVideoStream);

                    //Get packed frame size (UINT64)
                    ulong packedSize = newType.GetUInt64(MediaTypeAttributeKeys.FrameSize);

                    //Extract width/height
                    CodedWidth = (int)(packedSize >> 32);
                    CodedHeight = (int)(packedSize & 0xFFFFFFFF);

                    if (newType.GetUInt32(MediaTypeAttributeKeys.DefaultStride, out uint s).Success)
                    {
                        Stride = (int)s;
                    }
                    else
                    {
                        Stride = CodedWidth * 4;
                    }

                    byte[] blob = new byte[16];
                    if (newType.GetBlob(MediaTypeAttributeKeys.GeometricAperture, blob).Success)
                    {
                        HasAperture = true;

                        using var ms = new MemoryStream(blob);
                        using var br = new BinaryReader(ms);

                        OffsetX = br.ReadInt16();
                        OffsetXFrac = br.ReadInt16();
                        OffsetY = br.ReadInt16();
                        OffsetYFrac = br.ReadInt16();

                        ApertureWidth = br.ReadInt32();
                        ApertureHeight = br.ReadInt32();
                    }

                    newType.Dispose();
                }
            }

            public bool Exists => sample != null;

            private IMFSample sample;
            public long timestamp;
            public Metadata meta;

            public double GetSeconds() => TimestampToSeconds(timestamp);

            internal static StepResult Empty => new();

            public void DisposeAndOverwrite(StepResult from)
            {
                if (from.Exists)
                {
                    Overwrite(from.sample, from.timestamp, from.meta);
                }
                else Dispose();
            }
            public void Overwrite(IMFSample sample, long timestamp, Metadata currentMetadata)
            {
                Dispose();
                this.sample = sample;
                this.timestamp = timestamp;
                meta = currentMetadata;
            }

            public bool TryBlit(ref WriteableBitmap readerFrame)
            {
                if (!Exists) return false;

                nint ptr = 0;
                int pitch = 0;
                int length = 0;

                IMF2DBuffer? buffer2D = null;
                bool locked2D = false;
                IMFMediaBuffer? buffer = null;
                bool locked = false;
                bool readerFrameLocked = false;

                try
                {
                    if (sample.BufferCount == 1)
                    {
                        buffer = sample.GetBufferByIndex(0);

                        buffer2D = buffer.QueryInterfaceOrNull<IMF2DBuffer>();
                        if (buffer2D != null)
                        {
                            buffer2D.Lock2D(out ptr, out pitch);
                            locked2D = true;
                            length = pitch * meta.Height;
                        }
                    }

                    if (buffer2D == null)
                    {
                        buffer ??= sample.ConvertToContiguousBuffer();
                        buffer.Lock(out ptr, out int maxLength, out length);
                        locked = true;
                        pitch = meta.Stride;
                    }

                    if (readerFrame == null
                        || readerFrame.PixelWidth != meta.Width
                        || readerFrame.PixelHeight != meta.Height)
                    {
                        readerFrame = new WriteableBitmap(
                            meta.Width,
                            meta.Height,
                            96,
                            96,
                            PixelFormats.Bgr32,
                            null);
                    }

                    readerFrame.Lock();
                    readerFrameLocked = true;
                    readerFrame.WritePixels(new Int32Rect(0, 0, meta.Width, meta.Height), ptr, length, pitch);

                    return true;
                }
                catch (Exception ex)
                {
                    Program.TryOutput($"Failed to preview from source media: {ex.Message}");
                    return false;
                }
                finally
                {
                    if (locked2D) buffer2D.Unlock2D();
                    if (locked) buffer.Unlock();
                    if (readerFrameLocked) readerFrame.Unlock();
                    buffer2D?.Dispose();
                    buffer?.Dispose();
                }
            }
            public void Dispose()
            {
                sample?.Dispose();
                sample = null;
            }

            private const double SECOND_IN_TIMESTAMP = 10_000_000.0;
            public static double TimestampToSeconds(long timestamp) => timestamp / SECOND_IN_TIMESTAMP;
            public static long SecondsToTimestamp(double seconds) => (long)(seconds * SECOND_IN_TIMESTAMP);
            /// <remarks>Can throw. If throws, the whole reader should be considered unsupported.</remarks>
            /// <returns>If false, don't try to step again. True does not garuantee <paramref name="result"/> to have a value, just that the stepping can continue.</returns>
            internal static bool StepReader(IMFSourceReader reader, ref Metadata current, out StepResult result)
            {
                result = default;

                IMFSample? sample = null;
                SourceReaderFlag flags = SourceReaderFlag.None;
                long timestamp = 0;

                sample = reader.ReadSample(
                        SourceReaderIndex.FirstVideoStream,
                        SourceReaderControlFlag.None,
                        out int actualStreamIndex,
                        out flags,
                        out timestamp);

                //End of stream
                if ((flags & SourceReaderFlag.EndOfStream) != 0)
                {
                    sample?.Dispose();
                    return false;
                }

                if ((flags & SourceReaderFlag.CurrentMediaTypeChanged) != 0)
                {
                    current = new(reader);
                    sample?.Dispose();
                    return true;
                }

                if ((flags & SourceReaderFlag.StreamTick) != 0)
                {
                    sample?.Dispose();
                    return true; //Gap in stream
                }

                result.Overwrite(sample, timestamp, current);
                return true;
            }
        }
        public struct StepCache : IDisposable
        {
            public StepResult forwardCache;
            public long timestampNow;
            public bool Exists { get; private set; }
            public long GetNextTimeThreshold(long epsilon)
            {
                if (forwardCache.Exists) return (forwardCache.timestamp - timestampNow) - epsilon;
                return epsilon;
            }
            /// <summary>
            /// Expected to always be called at least once per request
            /// </summary>
            public void NewStep(long timestamp)
            {
                Exists = true;
                timestampNow = timestamp;
            }
            public void End(StepResult from)
            {
                forwardCache.DisposeAndOverwrite(from);
            }
            public StepResult PopCache()
            {
                StepResult result = forwardCache;
                forwardCache = StepResult.Empty;
                return result;
            }
            public void Clear()
            {
                forwardCache = StepResult.Empty;
                timestampNow = -1;
                Exists = false;
            }
            public void Dispose()
            {
                forwardCache.Dispose();
                Clear();
            }
        }
        IMFSourceReader reader;
        public bool SourceReaderSupported
        {
            get => sourceReaderSupported;
            private set
            {
                if (sourceReaderSupported != value)
                {
                    sourceReaderSupported = value;
                    OnSupportChange.Invoke();
                }
            }
        }
        public int LatestWidth => latestMetadata.Width;
        public int LatestHeight => latestMetadata.Height;
        private bool sourceReaderSupported;
        private double estimatedFrameRateCache;
        private long frameEpsilon;
        private StepResult.Metadata latestMetadata;
        private StepCache cachedSample;
        public void Open(string path)
        {
            estimatedFrameRateCache = 0;
            frameEpsilon = 2000;

            SourceReaderSupported = true;
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
            IMFAttributes attributes = MFCreateAttributes(1);
            IMFMediaType outType = MFCreateMediaType();
            attributes.Set(SourceReaderAttributeKeys.EnableVideoProcessing, 1);
            outType.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
            outType.Set(MediaTypeAttributeKeys.Subtype, VideoFormatGuids.Rgb32);
            try
            {
                reader = MFCreateSourceReaderFromURL(path, attributes);
                reader.SetStreamSelection(SourceReaderIndex.AllStreams, false);
                reader.SetStreamSelection(SourceReaderIndex.FirstVideoStream, true);
                reader.SetCurrentMediaType(SourceReaderIndex.FirstVideoStream, outType);
                latestMetadata = new(reader);
                reader.SetCurrentPosition(0);
            }
            catch
            {
                SourceReaderSupported = false;
            }
            finally
            {
                attributes.Dispose();
                outType.Dispose();
            }
        }
        public void OnInformationFetch(ProbeMediaInfo info)
        {
            estimatedFrameRateCache = info.EstimatedFrameRate;
            frameEpsilon = (long)Math.Min((10_000_000.0 / info.EstimatedFrameRate) / 4.0, 500); //at longest, one frame at 20,000fps
        }
        private double GetFramesBetween(StepResult to, StepResult from) => GetFramesBetween(to.timestamp, from.timestamp);
        private double GetFramesBetween(StepResult to, long from) => GetFramesBetween(to.timestamp, from);
        private double GetFramesBetween(long to, StepResult from) => GetFramesBetween(to, from.timestamp);
        private double GetFramesBetween(long to, long from) => StepResult.TimestampToSeconds(to - from) * estimatedFrameRateCache;
        /// <returns>If false, don't try to step again. True does not garuantee <paramref name="result"/> to have a value, just that the stepping can continue.</returns>
        private bool TryStepReader(out StepResult result)
        {
            bool success = false;
            result = default;
            try
            {
                success = StepResult.StepReader(reader, ref latestMetadata, out result);
            }
            catch
            {
                //Not only should the reader not try to step again this "seek", it should flat out be considered unsupported until the media changes.
                SourceReaderSupported = false;
                result.Dispose();
                return false;
            }
            return success;
        }
        private struct SampleComparison
        {
            public long Delta;
            public bool IsEqual, IsBefore, IsAfter;
            public enum Result
            {
                UseNothing, CacheAndKeepSearching, UseThis, UseLast
            }
            public Result Evaluate(StepFindMode mode)
            {
                if (IsEqual)
                {
                    if (mode.Now())
                    {
                        return Result.UseThis;
                    }
                    if (mode.Previous())
                    {
                        return Result.UseLast;
                    }

                    return Result.CacheAndKeepSearching;
                }

                if (IsBefore)
                {
                    return Result.CacheAndKeepSearching;
                }

                //IsAfter = true
                if (mode.Next())
                {
                    return Result.UseThis;
                }
                else if (mode.Previous() || mode.Now())
                {
                    return Result.UseLast;
                }

                return Result.UseNothing;
            }
            public SampleComparison(long sampleTime, long target, long epsilon)
            {
                Delta = sampleTime - target;
                IsEqual = Math.Abs(Delta) < epsilon;
                IsBefore = Delta < -epsilon;
                IsAfter = Delta > epsilon;
            }
            public SampleComparison(long sampleTime, long target, long epsilon, long forwardThreshold)
            {
                IsBefore = sampleTime < target - forwardThreshold;
                IsAfter = sampleTime > target + epsilon;
                IsEqual = !IsBefore && !IsAfter;
            }
        }
        private StepResult BestSample(StepFindMode targetMode, long targetTimestamp)
        {
            const int MAX_SAMPLES = 1000; //If we've tried 1000 samples and still haven't hit it, we should just give up. Increase if 1000 is not enough.
            const double MAX_FORWARD_SEEK = 5; //If we're trying to jump ahead more than 5 seconds, we should jump ahead. 

            //Program.TryOutput($"--TARGETTING {targetMode} RELATIVE TO {targetTimestamp}--");

            void JumpTo(long timestamp) //We want to avoid using this function as much as possible.
            {
                //Program.TryOutput("Jumping");
                cachedSample.Dispose();
                reader.Flush(SourceReaderIndex.AllStreams);
                reader.SetCurrentPosition(timestamp);
            }

            StepResult best = StepResult.Empty; //previous frame cache when appropriate

            StepResult? EvaluateNext(StepResult sample) //If returns null, keep going. If HasValue, return whatever the value is (even if the value is an empty struct)
            {
                if (!sample.Exists) return null;

                SampleComparison compare = new(sample.timestamp, targetTimestamp, frameEpsilon);
                SampleComparison.Result evaluation = compare.Evaluate(targetMode);
                //Program.TryOutput($"Sample: {evaluation} for {sample.timestamp}");
                switch (evaluation)
                {
                    case SampleComparison.Result.UseNothing:
                    default:
                        Program.TryOutput($"Error: Tried to seek to {targetTimestamp} but next available frame was at {sample.timestamp}.");
                        best.Dispose();
                        sample.Dispose();
                        cachedSample.Dispose();
                        return default;
                    case SampleComparison.Result.CacheAndKeepSearching:
                        best.DisposeAndOverwrite(sample);
                        cachedSample.NewStep(best.timestamp);
                        return null;
                    case SampleComparison.Result.UseLast:
                        cachedSample.End(sample);
                        if (!best.Exists)
                        {
                            //We already returned the "best sample" last time.
                            return StepResult.Empty;
                        }
                        return best;
                    case SampleComparison.Result.UseThis:
                        best.Dispose();
                        cachedSample.NewStep(sample.timestamp);
                        return sample;
                }
            }

            if (Math.Abs(targetTimestamp - cachedSample.timestampNow) > StepResult.SecondsToTimestamp(MAX_FORWARD_SEEK))
            {
                JumpTo(targetTimestamp);
            }

            if (cachedSample.Exists)
            {
                //Do a check on the "current frame"
                SampleComparison compare = new(cachedSample.timestampNow,
                        targetTimestamp,
                        frameEpsilon,
                        cachedSample.GetNextTimeThreshold(frameEpsilon));

                SampleComparison.Result evaluation = compare.Evaluate(targetMode);
                string future = cachedSample.forwardCache.Exists ? cachedSample.forwardCache.timestamp.ToString() : "empty";
                //Program.TryOutput($"Cached Sample: {evaluation} for {cachedSample.timestampNow} with future {future}");
                switch (evaluation)
                {
                    case SampleComparison.Result.UseNothing:
                    case SampleComparison.Result.UseLast: //The JumpTo for this condition specifically could be somewhat reduced by caching the previous frame as well. The conditions where this actually saves any work, however, are probably few and far between.
                        JumpTo(targetTimestamp);
                        cachedSample.Dispose();
                        break;
                    case SampleComparison.Result.UseThis:
                        //We already returned the "best sample" last time.
                        return default;
                    case SampleComparison.Result.CacheAndKeepSearching:
                        if (cachedSample.forwardCache.Exists)
                        {
                            StepResult? result = EvaluateNext(cachedSample.PopCache());
                            if (result.HasValue) return result.Value;
                        }
                        break;
                }
            }
            else
            {
                JumpTo(targetTimestamp);
            }

            for (int i = 0; i < MAX_SAMPLES; i++)
            {
                if (TryStepReader(out StepResult sample))
                {
                    StepResult? result = EvaluateNext(sample);
                    if (result.HasValue) return result.Value;
                }
                else return StepResult.Empty;
            }
            Program.TryOutput($"Warning: stepped more than {MAX_SAMPLES} times but failed to retrieve a valid video frame after the current playback position. Returning video frame at {best.GetSeconds()}s.");
            return best;
        }
        /// <param name="find">Relative to <paramref name="targetTimestamp"/>, finds the sample before, after, or at the current timestamp.</param>
        /// <returns>True when a sample was found matching the criteria, or false if the most recently read sample was that sample.</returns>
        public bool StepSourceReader(double targetSeconds, [MaybeNullWhen(false)] out StepResult result, StepFindMode find = StepFindMode.Next)
        {
            return StepSourceReader(StepResult.SecondsToTimestamp(targetSeconds), out result, find);
        }
        /// <param name="find">Relative to <paramref name="targetTimestamp"/>, finds the sample before, after, or at the current timestamp.</param>
        /// <returns>True when a sample was found matching the criteria, or false if the most recently read sample was that sample.</returns>
        public bool StepSourceReader(long targetTimestamp, [MaybeNullWhen(false)] out StepResult result, StepFindMode find = StepFindMode.Next)
        {
            if (!SourceReaderSupported)
            {
                result = StepResult.Empty;
                return false;
            }

            result = BestSample(find, targetTimestamp);
            return result.Exists;
        }
        
        public void Dispose()
        {
            reader?.Dispose();
            cachedSample.Dispose();
        }
    }
}
