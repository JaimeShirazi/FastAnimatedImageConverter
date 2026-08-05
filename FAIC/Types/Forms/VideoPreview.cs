using SharpGen.Runtime;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vortice.MediaFoundation;

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
                if (cachedTime == value) return;

                cachedTime = value;
                OnNewTime.Invoke(cachedTime);
            }
        }
        private double cachedTime;
        
        
        public bool IsPlaying => mode == PreviewMode.MediaPlayerPlaying;
        private enum PreviewMode
        {
            None, MediaPlayerPaused, MediaPlayerPlaying, SourceReader, ImageSequence
        }
        private PreviewMode Mode
        {
            set
            {
                if (mode == value) return;

                mode = value;
                OnStateChange.Invoke();
            }
        }
        private PreviewMode mode = PreviewMode.None;
        MediaPlayer player = new();
        SourceReaderManager sourceReader = new();
        ImageSequenceManager? imageSequence;

        public bool CanReadMedia => MediaPlayerSupported || SourceReaderSupported || ImageSequenceSupported;
        private bool mediaPlayerSupported;
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
        public bool SourceReaderSupported => sourceReader?.SourceReaderSupported ?? false;
        public bool ImageSequenceSupported => imageSequence != null && imageSequence.FrameCount > 0;

        DrawingGroup drawingGroup = new();
        VideoDrawing videoDrawing = new();

        private CancellationTokenSource getInfoCTS;
        private Task getInfoTask;

        private System.Drawing.Size videoSize;
        private WriteableBitmap? readerFrame = null;

        public VideoPreview()
        {
            SnapsToDevicePixels = true;

            videoDrawing.Player = player;
            drawingGroup.Children.Add(videoDrawing);
            player.ScrubbingEnabled = true;
            player.MediaOpened += OnMediaOpened;

            sourceReader.OnSupportChange += OnSourceReaderSupportChange;
        }
        private void OnMediaOpened(object? sender, EventArgs e)
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
        }
        void OnSourceReaderSupportChange() => OnSupportChange.Invoke();
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            //Treating it like an update loop here
            if (mode == PreviewMode.MediaPlayerPlaying)
            {
                CachedTime = player.Position.TotalSeconds;
            }

            Rect displayedSource = Rect.Empty;
            switch (mode)
            {
                case PreviewMode.MediaPlayerPaused:
                case PreviewMode.MediaPlayerPlaying:
                    if (MediaPlayerSupported)
                    {
                        //TODO: fix player still struggling to recognise the black padding in certain videos
                        VideoFrameLayout layout = GetCurrentVideoLayout();
                        displayedSource = GetOutputRect(layout.DisplayWidth, layout.DisplayHeight);
                        DrawMediaPlayer(dc, displayedSource, layout);
                    }
                    break;
                case PreviewMode.SourceReader:
                    if (SourceReaderSupported && readerFrame != null)
                    {
                        VideoFrameLayout layout = sourceReader.LatestFrameLayout;
                        double width = layout.IsValid ? layout.DisplayWidth : readerFrame.PixelWidth;
                        double height = layout.IsValid ? layout.DisplayHeight : readerFrame.PixelHeight;
                        displayedSource = GetOutputRect(width, height);
                        dc.DrawImage(readerFrame, displayedSource);
                    }
                    break;
                case PreviewMode.ImageSequence:
                    if (ImageSequenceSupported && imageSequence != null)
                    {
                        VideoFrameLayout layout = imageSequence.FrameLayout;
                        displayedSource = GetOutputRect(layout.DisplayWidth, layout.DisplayHeight);
                        dc.DrawImage(imageSequence.CurrentFrame, displayedSource);
                    }
                    break;
            }
        }
        private Rect GetOutputRect(double sourceWidth, double sourceHeight)
        {
            if (sourceWidth <= 0 || sourceHeight <= 0 || RenderSize.Width <= 0 || RenderSize.Height <= 0)
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
        private VideoFrameLayout GetCurrentVideoLayout()
        {
            if (sourceReader.LatestFrameLayout.IsValid)
                return sourceReader.LatestFrameLayout;
            return VideoFrameLayout.FullFrame(videoSize.Width, videoSize.Height);
        }
        private void DrawMediaPlayer(DrawingContext dc, Rect outputRect, VideoFrameLayout layout)
        {
            if (outputRect.IsEmpty || videoSize.Width <= 0 || videoSize.Height <= 0)
                return;

            Rect visiblePixels = layout.GetMediaPlayerVisiblePixels(videoSize.Width, videoSize.Height);
            if (visiblePixels.IsEmpty)
                visiblePixels = new Rect(0, 0, videoSize.Width, videoSize.Height);

            double scaleX = outputRect.Width / visiblePixels.Width;
            double scaleY = outputRect.Height / visiblePixels.Height;
            videoDrawing.Rect = new Rect(
                outputRect.X - (visiblePixels.X * scaleX),
                outputRect.Y - (visiblePixels.Y * scaleY),
                videoSize.Width * scaleX,
                videoSize.Height * scaleY);

            dc.PushClip(new RectangleGeometry(outputRect));
            dc.DrawDrawing(drawingGroup);
            dc.Pop();
        }
        public void Open(string path, Action<ProbeMediaInfo> onVideoInfoRead)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("VideoPreview recieved an empty path!", nameof(path));
            if (onVideoInfoRead == null)
                throw new ArgumentNullException(nameof(onVideoInfoRead));

            if (getInfoTask is { IsCompleted: false })
            {
                getInfoCTS?.Cancel();
            }

            latest = null;
            CachedTime = 0;
            Mode = PreviewMode.None;
            readerFrame = null;

            MediaPlayerSupported = false;
            player.Stop();
            player.Close();
            imageSequence?.Dispose();
            imageSequence = null;

            player.Open(new Uri(path));
            sourceReader.Open(path);

            //Clear the frame so we don't have the previous video left over
            if (readerFrame != null)
            {
                readerFrame.Lock();
                try
                {
                    Int32Rect rect = new Int32Rect(0, 0, readerFrame.PixelWidth, readerFrame.PixelHeight);
                    int bytesPerPixel = readerFrame.Format.BitsPerPixel / 8;
                    byte[] empty = new byte[rect.Width * rect.Height * bytesPerPixel];
                    int emptyStride = rect.Width * bytesPerPixel;
                    readerFrame.WritePixels(rect, empty, emptyStride, 0);
                }
                finally
                {
                    readerFrame.Unlock();
                }
            }

            getInfoCTS = new CancellationTokenSource();
            getInfoTask = Program.GetMediaInfo(path, getInfoCTS.Token, (info) =>
            {
                if (info == null)
                {
                    MediaPlayerSupported = false;
                    sourceReader.Open("");
                }
                else
                {
                    latest = info;
                    if (info.Format.Value.Equals("concat", StringComparison.OrdinalIgnoreCase))
                    {
                        MediaPlayerSupported = false;
                        sourceReader.Open("");
                        //Start the image sequence

                        imageSequence = new(info.ConcatData.orderedFrames);
                        Mode = PreviewMode.ImageSequence;
                        InvalidateVisual();
                    }
                    else
                    {
                        sourceReader.OnInformationFetch(info);
                    }
                }
                onVideoInfoRead.Invoke(info);
            });
        }
        private void EnsureMediaPlayer()
        {
            switch (mode)
            {
                case PreviewMode.MediaPlayerPaused:
                case PreviewMode.MediaPlayerPlaying:
                    return;
                case PreviewMode.SourceReader:
                    player.Position = TimeSpan.FromSeconds(CachedTime);
                    Mode = PreviewMode.MediaPlayerPaused;
                    InvalidateVisual();
                    break;
                case PreviewMode.ImageSequence:
                    System.Windows.Forms.MessageBox.Show("Ensure media player support not implemented for ImageSequence!", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
        public void Seek(double seconds)
        {
            if (double.IsNaN(seconds) || double.IsInfinity(seconds))
                throw new ArgumentOutOfRangeException(nameof(seconds), "Seek time must be finite.");
            seconds = Math.Max(0, seconds);

            if (mode == PreviewMode.ImageSequence && imageSequence != null)
            {
                seconds = Math.Round(seconds);
                int frame = (int)seconds; //each frame is treated as 1 second
                try
                {
                    imageSequence.SetFrame(frame);
                    CachedTime = frame;
                    InvalidateVisual();
                }
                catch (Exception ex)
                {
                    Program.TryOutput($"Failed to seek image sequence: {ex.Message}");
                }
                return;
            }

            if (MediaPlayerSupported)
            {
                EnsureMediaPlayer(); //Scrubbing will be done with MediaPlayer
                player.Position = TimeSpan.FromSeconds(seconds);
                CachedTime = seconds;

                drawingGroup.Dispatcher.Invoke(() => { },
                    System.Windows.Threading.DispatcherPriority.Render);
                return;
            }

            if (SourceReaderSupported
                && sourceReader.StepSourceReader(seconds, out SourceReaderManager.StepResult result, find: StepFindMode.Equal))
            {
                if (result.TryBlit(ref readerFrame))
                {
                    Mode = PreviewMode.SourceReader;
                    CachedTime = result.GetSeconds();
                    InvalidateVisual();
                }
                result.Dispose();
            }

            CachedTime = seconds;
        }
        public void Play()
        {
            if (mode == PreviewMode.ImageSequence) return;

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
        public void Step() => StepSource(StepFindMode.Next);
        public void ReverseStep() => StepSource(StepFindMode.Previous);
        private void StepSource(StepFindMode findMode)
        {
            if (mode == PreviewMode.ImageSequence && imageSequence != null)
            {
                try
                {
                    int direction = findMode.Previous() ? -1 : 1;
                    imageSequence.Step(direction);
                    CachedTime = imageSequence.CurrentFrameIndex;
                    InvalidateVisual();
                }
                catch (Exception ex)
                {
                    Program.TryOutput($"Failed to step image sequence: {ex.Message}");
                }
                return;
            }

            if (mode == PreviewMode.MediaPlayerPlaying)
            {
                Pause();
            }
            if (!SourceReaderSupported)
            {
                Program.TryOutput("Seeking is not supported for the current media type.");
                return;
            }
            if (sourceReader.StepSourceReader(CachedTime, out SourceReaderManager.StepResult sample, findMode))
            {
                if (sample.TryBlit(ref readerFrame))
                {
                    Mode = PreviewMode.SourceReader;
                    CachedTime = sample.GetSeconds();
                    InvalidateVisual();
                }
                sample.Dispose();
            }
        }
        public async Task End()
        {
            player.Stop();
            player.Close();
            MediaPlayerSupported = false;
            sourceReader?.Dispose();
            imageSequence?.Dispose();
            readerFrame = null;
            latest = null;
            Mode = PreviewMode.None;
            await EndGetInfoTask();
        }
        private async Task EndGetInfoTask()
        {
            getInfoCTS?.Cancel();

            if (getInfoTask != null)
            {
                try
                {
                    await getInfoTask;
                }
                catch (OperationCanceledException)
                {
                }
            }
            getInfoCTS?.Dispose();
            getInfoCTS = null;
            getInfoTask = null;
        }
    }
}
