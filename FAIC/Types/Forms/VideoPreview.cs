using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Vortice.MediaFoundation;
using static Vortice.MediaFoundation.MediaFactory;

namespace FAIC.Types.Forms
{
    public class VideoPreview : FrameworkElement
    {
        public event Action<double> OnNewTime = (_) => { };
        public event Action OnStateChange = () => { };
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

        DrawingGroup drawingGroup = new();
        VideoDrawing videoDrawing = new();

        private CancellationTokenSource getInfoCTS;
        private Task getInfoTask;

        System.Drawing.Size videoSize;

        private BitmapSource? readerFrame = null;
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

            int drawSourceWidth = mode == PreviewMode.SourceReader
                ? readerFrameData.Width
                : videoSize.Width;

            int drawSourceHeight = mode == PreviewMode.SourceReader
                ? readerFrameData.Height
                : videoSize.Height;

            if (drawSourceWidth <= 0 || drawSourceHeight <= 0)
                return;

            double sx = RenderSize.Width / drawSourceWidth;
            double sy = RenderSize.Height / drawSourceHeight;
            double scale = Math.Min(sx, sy);

            double drawW = drawSourceWidth * scale;
            double drawH = drawSourceHeight * scale;

            double offsetX = (RenderSize.Width - drawW) / 2;
            double offsetY = (RenderSize.Height - drawH) / 2;

            //Treating it like an update loop here
            if (mode == PreviewMode.MediaPlayerPlaying)
            {
                CachedTime = player.Position.TotalSeconds;
            }

            switch (mode)
            {
                case PreviewMode.MediaPlayerPaused:
                case PreviewMode.MediaPlayerPlaying:
                    videoDrawing.Rect = new Rect(offsetX, offsetY, drawW, drawH);
                    dc.DrawDrawing(videoDrawing);
                    break;
                case PreviewMode.SourceReader:
                    if (readerFrame != null)
                    {
                        dc.DrawImage(readerFrame, new Rect(offsetX, offsetY, drawW, drawH));
                    }
                    break;
                default:
                    break;
            }
        }
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
        public void Open(string path, Action<string> appendLog, Action<ProbeMediaInfo> onVideoInfoRead)
        {
            if (getInfoTask is { IsCompleted: false })
            {
                getInfoCTS?.Cancel();
            }

            player.Open(new Uri(path));

            player.MediaOpened += (_, __) =>
            {
                videoSize = new System.Drawing.Size(
                    player.NaturalVideoWidth,
                    player.NaturalVideoHeight
                );

                player.Play();
                player.Pause();
                Mode = PreviewMode.MediaPlayerPaused;
                InvalidateVisual();
            };

            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
            IMFAttributes attributes = MFCreateAttributes(1);
            attributes.Set(SourceReaderAttributeKeys.EnableVideoProcessing, 1);
            reader = MFCreateSourceReaderFromURL(path, attributes);
            reader.SetStreamSelection(SourceReaderIndex.AllStreams, false);
            reader.SetStreamSelection(SourceReaderIndex.FirstVideoStream, true);
            IMFMediaType outType = MFCreateMediaType();
            outType.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
            outType.Set(MediaTypeAttributeKeys.Subtype, VideoFormatGuids.Rgb32);
            reader.SetCurrentMediaType(SourceReaderIndex.FirstVideoStream, outType);
            readerFrameData = new(reader);

            attributes.Dispose();
            outType.Dispose();

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
                Program.TryOutput("Switched to MediaPlayerPaused");
            }
        }
        public void Seek(double seconds)
        {
            EnsureMediaPlayer(); //Scrubbing will be done with MediaPlayer

            player.Position = TimeSpan.FromSeconds(seconds);
            drawingGroup.Dispatcher.Invoke(() => { },
                System.Windows.Threading.DispatcherPriority.Render);

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
            if (sample == null) return;

            Mode = PreviewMode.SourceReader;

            CachedTime = timestamp / 10_000_000.0;

            IMFMediaBuffer buffer = sample.ConvertToContiguousBuffer();

            buffer.Lock(out nint ptr, out int maxLength, out int currentLength);

            readerFrame = BitmapSource.Create(
                readerFrameData.Width,
                readerFrameData.Height,
                96,
                96,
                PixelFormats.Bgr32,
                null,
                ptr,
                currentLength,
                readerFrameData.Stride);

            buffer.Unlock();
            buffer.Dispose();
            sample.Dispose();

            InvalidateVisual();
        }
        private IMFSample? StepSourceReader(long afterTimestamp, out long timestamp)
        {
            const int MAX_SAMPLES = 1000; //If we've tried 1000 samples and still haven't hit it, we should just give up. Increase if 1000 is not enough.

            timestamp = 0;
            IMFSample closest = null;

            for (int i = 0; i < MAX_SAMPLES; i++)
            {
                IMFSample sample = reader.ReadSample(
                    SourceReaderIndex.FirstVideoStream,
                    SourceReaderControlFlag.None,
                    out int actualStreamIndex,
                    out SourceReaderFlag flags,
                    out timestamp);

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
                    closest = sample;
                    if (timestamp > afterTimestamp) return closest;
                }
            }
            Program.TryOutput($"Warning: stepped more than {MAX_SAMPLES} times but failed to retrieve a valid video frame after the current playback position. Returning video frame at {timestamp / 10_000_000.0}s.");
            return closest;
        }
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
