using SharpGen.Runtime;
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
        SourceReaderManager sourceReader = new();

        public bool CanReadMedia => mediaPlayerSupported || sourceReader.SourceReaderSupported;
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
        private bool mediaPlayerSupported;
        

        DrawingGroup drawingGroup = new();
        VideoDrawing videoDrawing = new();

        private CancellationTokenSource getInfoCTS;
        private Task getInfoTask;

        System.Drawing.Size videoSize;

        private WriteableBitmap? readerFrame = null;

        public VideoPreview()
        {
            SnapsToDevicePixels = true;

            videoDrawing.Player = player;
            drawingGroup.Children.Add(videoDrawing);

            player.ScrubbingEnabled = true;

            sourceReader.OnSupportChange += OnSourceReaderSupportChange;
        }
        void OnSourceReaderSupportChange()
        {
            OnSupportChange?.Invoke();
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
                    dc.DrawImage(readerFrame, GetOutputRect(sourceReader?.LatestWidth ?? 0, sourceReader?.LatestHeight ?? 0));
                }
            }
        }
        
        public void Open(string path, Action<ProbeMediaInfo> onVideoInfoRead)
        {
            CachedTime = 0;

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

            sourceReader.Open(path);

            getInfoCTS = new CancellationTokenSource();
            getInfoTask = Program.GetMediaInfo(path, getInfoCTS.Token, (info) =>
            {
                if (info == null)
                {
                    MediaPlayerSupported = false;
                    sourceReader.Open("");
                    onVideoInfoRead.Invoke(null);
                    return;
                }
                latest = info;
                sourceReader.OnInformationFetch(info);
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
                if (sourceReader.StepSourceReader(seconds, out SourceReaderManager.StepResult result, find: StepFindMode.Equal))
                {
                    if (result.TryBlit(ref readerFrame))
                    {
                        InvalidateVisual();
                    }
                    result.Dispose();
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
        public void Step() => StepSource(StepFindMode.Next);
        public void ReverseStep() => StepSource(StepFindMode.Previous);
        private void StepSource(StepFindMode findMode)
        {
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
            sourceReader?.Dispose();
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
