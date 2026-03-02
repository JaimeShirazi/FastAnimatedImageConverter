using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vortice.MediaFoundation;
using static Vortice.MediaFoundation.MediaFactory;

namespace FAIC.Types.Forms
{
    public class VideoPreview : FrameworkElement
    {
        public event Action<double> OnNewTime = (_) => { };
        public event Action OnStateChange = () => { };
        public event Action OnSupportChange = () => { };
        public double Time => mode == PreviewMode.MediaPlayerPlaying ? player.Position.TotalSeconds : CachedTime;
        public ProbeMediaInfo Latest => latest;
        private ProbeMediaInfo latest;
        private double CachedTime
        {
            get => cachedTime;
            set
            {
                if (cachedTime != value)
                {
                    cachedTime = value;
                    OnNewTime.Invoke(cachedTime);
                }
            }
        }
        private double cachedTime;
        public bool IsPlaying => mode == PreviewMode.MediaPlayerPlaying;
        private enum PreviewMode
        {
            None, MediaPlayerPaused, MediaPlayerPlaying, SourceReader
        }
        private PreviewMode Mode
        {
            set
            {
                if (mode != value)
                {
                    mode = value;
                    OnStateChange.Invoke();
                }
            }
        }
        private PreviewMode mode = PreviewMode.None;


        MediaPlayer player = new();
        IMFSourceReader reader;
        public bool CanReadMedia => mediaPlayerSupported || sourceReaderSupported;
        public bool MediaPlayerSupported
        {
            get => mediaPlayerSupported;
            private set
            {
                if (mediaPlayerSupported != value)
                {
                    mediaPlayerSupported = value;
                    OnSupportChange.Invoke();
                }
            }
        }
        private bool mediaPlayerSupported;
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
        private bool sourceReaderSupported;

        DrawingGroup drawingGroup = new();
        VideoDrawing videoDrawing = new();

        private CancellationTokenSource getInfoCTS;
        private Task getInfoTask;

        System.Drawing.Size videoSize;

        private WriteableBitmap? readerFrame = null;
        private SourceReaderSampleData readerFrameData;

        public VideoPreview()
        {
            SnapsToDevicePixels = true;

            videoDrawing.Player = player;
            drawingGroup.Children.Add(videoDrawing);

            player.ScrubbingEnabled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            Rect GetOutputRect(int sourceWidth, int sourceHeight)
            {
                if (sourceWidth <= 0 || sourceHeight <= 0)
                    return Rect.Empty;

                double sx = RenderSize.Width / sourceWidth;
                double sy = RenderSize.Height / sourceHeight;
                double scale = Math.Min(sx, sy);

                double drawW = sourceWidth * scale;
                double drawH = sourceHeight * scale;

                double offsetX = (RenderSize.Width - drawW) / 2;
                double offsetY = (RenderSize.Height - drawH) / 2;

                return new Rect(offsetX, offsetY, drawW, drawH);
            }

            //Treating it like an update loop here
            if (mode == PreviewMode.MediaPlayerPlaying)
            {
                CachedTime = player.Position.TotalSeconds;
            }

            if (mode == PreviewMode.MediaPlayerPaused || mode == PreviewMode.MediaPlayerPlaying
                && MediaPlayerSupported)
            {
                videoDrawing.Rect = GetOutputRect(videoSize.Width, videoSize.Height);
                dc.DrawDrawing(videoDrawing);
            }
            else if (SourceReaderSupported && mode != PreviewMode.None)
            {
                if (readerFrame != null)
                {
                    dc.DrawImage(readerFrame, GetOutputRect(readerFrameData.Width, readerFrameData.Height));
                }
            }
        }
        
        public void Open(string path, Action<ProbeMediaInfo> onVideoInfoRead)
        {
            if (getInfoTask is { IsCompleted: false })
            {
                getInfoCTS?.Cancel();
            }

            MediaPlayerSupported = false;
            player.Open(new Uri(path));
            player.MediaOpened += (_, __) =>
            {
                MediaPlayerSupported = true;

                videoSize = new System.Drawing.Size(
                    player.NaturalVideoWidth,
                    player.NaturalVideoHeight
                );

                player.Play();
                player.Pause();
                Mode = PreviewMode.MediaPlayerPaused;
                InvalidateVisual();
            };

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
                readerFrameData = new(reader);
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

            getInfoCTS = new CancellationTokenSource();
            getInfoTask = Program.GetMediaInfo(path, getInfoCTS.Token, (info) =>
            {
                latest = info;
                onVideoInfoRead.Invoke(info);
            });
        }
        private void EnsureMediaPlayer()
        {
            if (mode == PreviewMode.SourceReader) 
            {
                player.Position = TimeSpan.FromSeconds(CachedTime);
                Mode = PreviewMode.MediaPlayerPaused;
                InvalidateVisual();
            }
        }
        public void Seek(double seconds)
        {
            player.Position = TimeSpan.FromSeconds(seconds);

            if (MediaPlayerSupported)
            {
                EnsureMediaPlayer(); //Scrubbing will be done with MediaPlayer

                drawingGroup.Dispatcher.Invoke(() => { },
                    System.Windows.Threading.DispatcherPriority.Render);
            }
            else if (SourceReaderSupported)
            {
                long sourceReaderTime = (long)(seconds * 10_000_000.0);
                //TODO: Figure out how to cut out the need to SetCurrentPosition, because it allocates so much memory
                /*bool setPosition = false;
                if (seconds < CachedTime)
                {
                    setPosition = true;*/
                    reader.SetCurrentPosition(sourceReaderTime);
                //}

                IMFSample? sample = StepSourceReader(sourceReaderTime, out long timestamp, getClosestEarlier: true);
                /*if (sample == null && !setPosition)
                {
                    setPosition = true;
                    reader.SetCurrentPosition(sourceReaderTime);
                }
                sample = StepSourceReader(sourceReaderTime, out timestamp, getClosestEarlier: true);*/

                if (TryBufferSourceReaderSample(sample))
                {
                    InvalidateVisual();
                }
            }

            CachedTime = seconds;
        }
        public void Play()
        {
            EnsureMediaPlayer();
            if (mode == PreviewMode.MediaPlayerPaused)
            {
                player.Play();
                Mode = PreviewMode.MediaPlayerPlaying;
            }
        }
        public void Pause()
        {
            if (mode == PreviewMode.MediaPlayerPlaying)
            {
                player.Pause();
                CachedTime = player.Position.TotalSeconds;
                Mode = PreviewMode.MediaPlayerPaused;
            }
        }
        public void Step()
        {
            if (mode == PreviewMode.MediaPlayerPlaying)
            {
                Pause();
            }
            if (mode == PreviewMode.MediaPlayerPaused)
            {
                reader.SetCurrentPosition((long)(CachedTime * 10_000_000.0)); //Sync the sourcereader with the player
            }
            IMFSample? sample = StepSourceReader((long)(CachedTime * 10_000_000.0), out long timestamp);
            if (!SourceReaderSupported)
            {
                Program.TryOutput("Seeking is not supported for the current media type.");
                return;
            }
            if (TryBufferSourceReaderSample(sample))
            {
                Mode = PreviewMode.SourceReader;
                CachedTime = timestamp / 10_000_000.0;
                InvalidateVisual();
            }
        }
        #region Source reader reading and blitting logic
        struct SourceReaderSampleData
        {
            public int Width => HasAperture ? ApertureWidth : CodedWidth;
            public int Height => HasAperture ? ApertureHeight : CodedHeight;
            public bool HasAperture;
            public int CodedWidth, CodedHeight;
            public int Stride;
            public short OffsetX, OffsetXFrac, OffsetY, OffsetYFrac; //Not actually used - would require some changes to properly support
            public int ApertureWidth, ApertureHeight;
            public SourceReaderSampleData(IMFSourceReader reader)
            {
                IMFMediaType newType = reader.GetCurrentMediaType(SourceReaderIndex.FirstVideoStream);

                // Get packed frame size (UINT64)
                ulong packedSize = newType.GetUInt64(MediaTypeAttributeKeys.FrameSize);

                // Extract width/height
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
        /// <param name="getClosestEarlier">If true, returns the closest sample just before (or at) <paramref name="afterTimestamp"/>, rather than the sample after.</param>
        private IMFSample? StepSourceReader(long afterTimestamp, out long timestamp, bool getClosestEarlier = false)
        {
            const int MAX_SAMPLES = 1000; //If we've tried 1000 samples and still haven't hit it, we should just give up. Increase if 1000 is not enough.

            timestamp = 0;
            IMFSample closest = null;

            if (!SourceReaderSupported) return null;

            for (int i = 0; i < MAX_SAMPLES; i++)
            {
                IMFSample sample = null;
                SourceReaderFlag flags = SourceReaderFlag.None;

                try
                {
                    sample = reader.ReadSample(
                        SourceReaderIndex.FirstVideoStream,
                        SourceReaderControlFlag.None,
                        out int actualStreamIndex,
                        out flags,
                        out timestamp);
                }
                catch
                {
                    SourceReaderSupported = false;
                    return null;
                }

                //End of stream
                if ((flags & SourceReaderFlag.EndOfStream) != 0) return null;

                if ((flags & SourceReaderFlag.CurrentMediaTypeChanged) != 0)
                {
                    readerFrameData = new(reader);
                    continue;
                }

                if ((flags & SourceReaderFlag.StreamTick) != 0) continue; //Gap in stream

                if (sample != null)
                {
                    bool isNextSample = timestamp > afterTimestamp;

                    if (isNextSample && getClosestEarlier && closest != null) return closest;

                    if (closest != null)
                    {
                        closest.Dispose();
                    }
                    closest = sample;

                    if (isNextSample) return closest;
                }
            }
            Program.TryOutput($"Warning: stepped more than {MAX_SAMPLES} times but failed to retrieve a valid video frame after the current playback position. Returning video frame at {timestamp / 10_000_000.0}s.");
            return closest;
        }
        private bool TryBufferSourceReaderSample(IMFSample? sample)
        {
            if (sample == null) return false;

            nint ptr = 0;
            int pitch = 0;
            int length = 0;
            IMF2DBuffer buffer2D = null;
            IMFMediaBuffer buffer = null;
            if (sample.BufferCount == 1)
            {
                buffer = sample.GetBufferByIndex(0);

                buffer2D = buffer.QueryInterfaceOrNull<IMF2DBuffer>();
                if (buffer2D != null)
                {
                    buffer2D.Lock2D(out ptr, out pitch);
                    length = pitch * readerFrameData.Height;
                }
            }
            if (buffer2D == null)
            {
                if (buffer == null)
                {
                    buffer = sample.ConvertToContiguousBuffer();
                }
                buffer.Lock(out ptr, out int maxLength, out length);
                pitch = readerFrameData.Stride;
            }

            if (readerFrame == null
                || readerFrame.PixelWidth != readerFrameData.Width
                || readerFrame.PixelHeight != readerFrameData.Height)
            {
                readerFrame = new WriteableBitmap(
                    readerFrameData.Width,
                    readerFrameData.Height,
                    96,
                    96,
                    PixelFormats.Bgr32,
                    null);
            }

            if (readerFrame.TryLock(new Duration(new TimeSpan(0))))
            {
                readerFrame.WritePixels(new Int32Rect(0, 0, readerFrameData.Width, readerFrameData.Height), ptr, length, pitch);
                readerFrame.Unlock();
            }
            else
            {
                Program.TryOutput("Failed to preview from source media.");
            }

            if (buffer2D != null)
            {
                buffer2D.Unlock2D();
                buffer2D.Dispose();
            }
            else
            {
                buffer.Unlock();
                buffer.Dispose();
            }
            sample.Dispose();

            return true;
        }
        #endregion
        public async Task End()
        {
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
            await EndGetInfoTask();
        }
        private async Task EndGetInfoTask()
        {
            getInfoCTS?.Cancel();

            if (getInfoTask != null)
            {
                await getInfoTask;
            }
        }
    }
}
