using FAIC.Types;
using FAIC.Types.Forms;
using System.IO;
using static Vortice.MediaFoundation.MediaFactory;

namespace FAIC
{
    public partial class Main : Form, IConsole
    {
        public string InputPath
        {
            get => inputPath;
            set
            {
                if (Path.Exists(value))
                {
                    inputPath = value;
                    if (videoPreview != null)
                    {
                        videoPreview.Open(inputPath, AppendLog, OnNewVideoInfo);
                    }
                }
                else
                {
                    inputPath = "";
                }
            }
        }
        public string inputPath;
        /// <summary>
        /// By default, if the user changes the speed while the frame rate mode is still "same",
        /// we automatically change it to "nearest" and set the output frame rate to be the same.
        /// This may be annoying for users who want the frame rate to automatically change with the speed, though,
        /// so we track if the user has overwritten the mode since the file was imported,
        /// and do not automatically change the mode if they have.
        /// </summary>
        private bool automaticallyChangedFrameRateModeToNearest;
        /// <summary>
        /// For <see cref="automaticallyChangedFrameRateModeToNearest"/> to work, we also need to track what the previous <see cref="fpsSetting"/> selected index was.
        /// </summary>
        private int frameRateLastModeBuffer = 0;
        private VideoPreview videoPreview;
        public Main(string inputPath)
        {
            Program.UpdateConsole(this);

            if (MFStartup().Failure)
                throw new InvalidOperationException("Failed to start Vortice.MediaFoundation.");

            InitializeComponent();
            videoPreview = new();
            videoPreviewHost.Child = videoPreview;
            videoPreview.OnNewTime += VideoPreview_OnNewTime;
            videoPreview.OnStateChange += VideoPreview_OnStateChanged;
            InputPath = inputPath;

            DragDrop += OnDragDrop;
            DragEnter += OnDragEnter;

            if (string.IsNullOrEmpty(inputPath))
            {
                AppendLog("Drag a media file onto the window to convert it!");
            }

            fpsSetting.SelectedIndex = 0;
            repeatValue.Value = -1;
        }

        const int ABOUT_SYSMENU_ID = 0x1FFF; // any ID > 0xF000
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            IntPtr sysMenu = Native.GetSystemMenu(Handle, false);

            Native.AppendMenu(sysMenu, Native.MF_SEPARATOR, 0, "");
            Native.AppendMenu(sysMenu, Native.MF_STRING, ABOUT_SYSMENU_ID, "About\tCtrl+F1");

            UpdateQualityLabel();
        }
        protected override bool ProcessCmdKey(
            ref Message msg,
            Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F1))
            {
                using (var about = new About())
                {
                    about.ShowDialog(this);
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Native.WM_SYSCOMMAND)
            {
                if ((int)m.WParam == ABOUT_SYSMENU_ID)
                {
                    using (var about = new About())
                    {
                        about.ShowDialog(this);
                    }
                    return;
                }
            }

            base.WndProc(ref m);
        }
        #region Console
        public void Write(string text) => AppendLog(text);
        const int MAX_LINES = 2000;
        void AppendLog(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => AppendLog(msg));
                return;
            }

            commandLineOutput.AppendText(msg + Environment.NewLine);

            if (commandLineOutput.Lines.Length > MAX_LINES)
            {
                var lines = commandLineOutput.Lines.Skip(commandLineOutput.Lines.Length - MAX_LINES).ToArray();
                commandLineOutput.Lines = lines;
            }
        }
        #endregion
        // Handle the DragDrop event
        private void OnDragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the dropped data as an array of strings (file paths)
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            InputPath = files[0];
            AppendLog($"Set input file to file at path \"{InputPath}\"");
        }
        // Handle the DragEnter event
        private void OnDragEnter(object sender, DragEventArgs e)
        {
            // Check if the data being dragged is in the FileDrop format
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // If it is, show the 'copy' cursor (or another effect)
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                // Otherwise, show no effect, indicating drop is not allowed
                e.Effect = DragDropEffects.None;
            }
        }
        #region Playhead management
        private void OnNewVideoInfo(ProbeMediaInfo info)
        {
            settingsGroupBox.Enabled = !info.IsEmpty();
            if (info.IsEmpty())
            {
                AppendLog("Unable to read media.");
                return;
            }
            else
            {
                AppendLog($"Detected media: {info}");
            }
            if (info.Width.ReadData && info.Height.ReadData)
            {
                resizeDimensionLabel.Text = info.Width > info.Height ? "Width" : (info.Width == info.Height ? "Size" : "Height");
                resizeDimensionValue.Value = Math.Max(info.Width, info.Height);
            }
            samplingLayoutPanel.Enabled = false;
            resizeSlider.Value = 100;
            playhead.Value = 0;
            playhead.Maximum = (int)(1000 * info.Length);
            lastFrameInput.Maximum = Math.Round((decimal)info.Length, 3);
            firstFrameInput.Maximum = lastFrameInput.Maximum;
            lastFrameInput.Value = lastFrameInput.Maximum;

            fpsSetting.SelectedIndex = 0;
            automaticallyChangedFrameRateModeToNearest = false;
            frameRateLastModeBuffer = 0;
            MatchFrameRateToMedia();
            fpsSetting_SelectedIndexChanged(this, default);

            repeatValue.Value = -1;
        }
        private void VideoPreview_OnNewTime(double time)
        {
            playhead.Value = (int)(time * 1000);
        }
        private void playhead_Scroll(object sender, EventArgs e)
        {
            double t = playhead.Value / 1000.0;
            videoPreview.Seek(t);
        }
        #endregion
        private void qualitySlider_Scroll(object sender, EventArgs e)
        {
            UpdateQualityLabel();
        }
        private void UpdateQualityLabel()
        {
            qualityLabel.Text = $"Quality ({qualitySlider.Value}%)";
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void firstFrameInput_ValueChanged(object sender, EventArgs e)
        {

        }

        private void mainSplit_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
        private void playButton_Click(object sender, EventArgs e)
        {
            if (videoPreview.IsPlaying)
                videoPreview.Pause();
            else
                videoPreview.Play();

            if (videoPreview.IsPlaying)
            {
                playButton.Text = "Pause";
                playhead.Enabled = false;
            }
            else
            {
                playButton.Text = "Play";
                playhead.Enabled = true;
            }
        }
        private void seekButton_Click(object sender, EventArgs e)
        {
            videoPreview?.Step();
        }
        private void VideoPreview_OnStateChanged()
        {
            //I don't remember why I made this. Playhead sync? TODO double check
        }

        private void trimStartHereButton_Click(object sender, EventArgs e)
        {
            firstFrameInput.Value = Math.Round((decimal)videoPreview.Time, 3);
        }

        private void trimEndHereButton_Click(object sender, EventArgs e)
        {
            lastFrameInput.Value = Math.Round((decimal)videoPreview.Time, 3);
        }

        #region Encoding
        private CancellationTokenSource? _encodeCTS;
        private Task? _encodeTask;
        private void convertButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputPath))
            {
                MessageBox.Show($"Input file path was null or empty.", "Encode Failed");
            }

            if (firstFrameInput.Value > lastFrameInput.Value)
            {
                AppendLog("End time is before start time, swapping values.");
                decimal lastFrameTimeCurrent = lastFrameInput.Value;
                lastFrameInput.Value = firstFrameInput.Value;
                firstFrameInput.Value = lastFrameTimeCurrent;
            }

            if (saveFileDialogue.ShowDialog() == DialogResult.OK)
            {
                if (_encodeTask is { IsCompleted: false })
                    return; // already running

                _encodeCTS = new CancellationTokenSource();

                _encodeTask = StartEncodeAsync(saveFileDialogue.FileName, _encodeCTS.Token);
            }
        }
        private async Task StartEncodeAsync(string outputPath, CancellationToken token)
        {
            commandLineOutput.Clear();

            string extension = Path.GetExtension(outputPath).TrimStart('.').ToLowerInvariant();

            try
            {
                convertButton.Enabled = false;
                cancelButton.Enabled = true;

                switch (extension)
                {
                    case "gif":
                        if (fpsValue.Value > 100)
                        {
                            AppendLog("GIF does not support >100fps. Clamping to 100.");
                            if (GetSourceMediaFrameRate() > 100 && fpsSetting.SelectedIndex == 0)
                            {
                                fpsSetting.SelectedIndex = 1;
                            }
                            fpsValue.Value = 100;
                        }
                        break;
                    default:
                        break;
                }

                int smallestDimension = (int)Math.Round(GetMediaSmallestDimension() * (resizeDimensionValue.Value / GetMediaLargestDimension()));
                bool widthLarger = IsMediaWidthLarger();
                int width = widthLarger ? (int)resizeDimensionValue.Value : smallestDimension;
                int height = widthLarger ? smallestDimension : (int)resizeDimensionValue.Value;

                EncodeSettings.ResampleSetting resampleSetting;
                if ((int)resizeDimensionValue.Value == GetMediaLargestDimension())
                {
                    resampleSetting = EncodeSettings.ResampleSetting.None;
                }
                else if (sampleFastRadio.Checked)
                {
                    resampleSetting = EncodeSettings.ResampleSetting.Bilinear;
                }
                else if (sampleBestRadio.Checked)
                {
                    if ((int)resizeDimensionValue.Value < GetMediaLargestDimension())
                    {
                        resampleSetting = EncodeSettings.ResampleSetting.Lanczos;
                    }
                    else
                    {
                        resampleSetting = EncodeSettings.ResampleSetting.Spline36;
                    }
                }
                else
                {
                    AppendLog("Failed to determine resample filter mode. Defaulting to bilinear.");
                    resampleSetting = EncodeSettings.ResampleSetting.Bilinear;
                }

                EncodeSettings.InterpolateSetting interpolateSetting;
                switch (fpsSetting.SelectedIndex)
                {
                    case 0:
                        interpolateSetting = EncodeSettings.InterpolateSetting.None;
                        break;
                    case 1:
                        interpolateSetting = EncodeSettings.InterpolateSetting.Nearest;
                        break;
                    case 2:
                        interpolateSetting = EncodeSettings.InterpolateSetting.Blended;
                        break;
                    default:
                        AppendLog("Unknown frame rate mode. Defaulting to Same.");
                        interpolateSetting = EncodeSettings.InterpolateSetting.None;
                        break;
                }

                EncodeSettings settings = new EncodeSettings()
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Quality = qualitySlider.Value,
                    Start = firstFrameInput.Value,
                    End = lastFrameInput.Value,
                    Width = width,
                    Height = height,
                    Resample = resampleSetting,
                    Speed = Speeds[speedSlider.Value],
                    TargetFrameRate = fpsValue.Value,
                    Interpolate = interpolateSetting,
                    Repeats = (int)repeatValue.Value < 0 ? -1 : (int)repeatValue.Value
                };

                switch (extension)
                {
                    case "avif":
                    default:
                        await Program.EncodeAVIF(settings, token);
                        break;
                    case "gif":
                        await Program.EncodeGIF(settings, token);
                        break;
                    case "jxl":
                        await Program.EncodeJXL(settings, token);
                        break;
                    case "apng": case "png":
                        await Program.EncodeAPNG(settings, token);
                        break;
                    case "webp":
                        await Program.EncodeWebP(settings, token);
                        break;
                }

            }
            catch (OperationCanceledException)
            {
                // user cancelled — fine
            }
            catch (Exception ex)
            {
                AppendLog($"Encode Failed: {ex.ToString}");
            }
            finally
            {
                convertButton.Enabled = true;
                cancelButton.Enabled = false;
            }
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            _encodeCTS?.Cancel();
            cancelButton.Enabled = false;
        }
        #endregion
        #region Frame rate
        public const int FRAME_RATE_PRECISION = 3;
        private decimal GetSourceMediaFrameRate() => videoPreview.Latest != null ? (decimal)videoPreview.Latest.EstimatedFrameRate : 0;
        private void MatchFrameRateToMedia(bool factorSpeed = true)
        {
            decimal roundedFPS = Math.Round(GetSourceMediaFrameRate() * (factorSpeed ? (decimal)Speeds[speedSlider.Value] : 1), FRAME_RATE_PRECISION);
            if (fpsValue.Enabled)
            {
                fpsValue.Maximum = Math.Max(roundedFPS, 1000);
                fpsValue.Value = Math.Min(roundedFPS, 1000);
            }
            else
            {
                fpsValue.Maximum = roundedFPS;
                fpsValue.Value = roundedFPS;
            }
        }
        private void fpsSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (frameRateLastModeBuffer != fpsSetting.SelectedIndex)
            {
                frameRateLastModeBuffer = fpsSetting.SelectedIndex;
                automaticallyChangedFrameRateModeToNearest = true; //Override this behaviour if the user touched this control
            }
            bool wasEnabled = fpsValue.Enabled;
            fpsValue.Enabled = fpsSetting.SelectedIndex != 0;
            if (fpsValue.Enabled != wasEnabled)
            {
                MatchFrameRateToMedia();
            }
        }
        #endregion
        private bool isResizeUpdate = false;
        private bool IsMediaWidthLarger() => videoPreview.Latest != null ? videoPreview.Latest.Width > videoPreview.Latest.Height : true;
        private int GetMediaLargestDimension() => videoPreview.Latest != null ? Math.Max(videoPreview.Latest.Width, videoPreview.Latest.Height) : 0;
        private int GetMediaSmallestDimension() => videoPreview.Latest != null ? Math.Min(videoPreview.Latest.Width, videoPreview.Latest.Height) : 0;
        private void resizeSlider_Scroll(object sender, EventArgs e)
        {
            if (isResizeUpdate) return; isResizeUpdate = true;

            int largestDimension = GetMediaLargestDimension();
            resizeDimensionValue.Value = Math.Clamp(
                Math.Round(largestDimension * (resizeSlider.Value / (decimal)100)),
                resizeDimensionValue.Minimum,
                resizeDimensionValue.Maximum
                );

            OnResizeChange();

            isResizeUpdate = false;
        }
        private void resizeDimensionValue_ValueChanged(object sender, EventArgs e)
        {
            if (isResizeUpdate) return; isResizeUpdate = true;

            resizeSlider.Value = (int)Math.Clamp(
                Math.Round((resizeDimensionValue.Value / GetMediaLargestDimension()) * 100),
                resizeSlider.Minimum,
                resizeSlider.Maximum
                );

            OnResizeChange();

            isResizeUpdate = false;
        }
        private void OnResizeChange()
        {
            decimal percentageSize = Math.Max(
                (resizeDimensionValue.Value / GetMediaLargestDimension()) * (decimal)100.0,
                (decimal)0.01
                );

            samplingLayoutPanel.Enabled = percentageSize != 100;

            string formattedPercentage = percentageSize switch
            {
                < 10 => percentageSize.ToString("0.00"),
                < 100 => percentageSize.ToString("0.0"),
                < 1000 => percentageSize.ToString("0"),
                _ => ">999"
            };

            resizeLabel.Text = $"Resize ({formattedPercentage}%)";
        }

        public static readonly double[] Speeds =
        [
            0.1, 0.25, 0.33333333333333333, 0.5, 0.75, 1, 1.25, 1.5, 2, 3, 4, 5, 6, 8, 12, 16
        ];
        private void speedSlider_Scroll(object sender, EventArgs e)
        {
            if (!automaticallyChangedFrameRateModeToNearest
                && fpsSetting.SelectedIndex == 0)
            {
                automaticallyChangedFrameRateModeToNearest = true;
                fpsSetting.SelectedIndex = 1;
                MatchFrameRateToMedia(factorSpeed: false);
            }
            speedLabel.Text = $"Speed ({Math.Round(Speeds[speedSlider.Value], 2)}x)";
            if (fpsSetting.SelectedIndex == 0)
            {
                MatchFrameRateToMedia();
            }
        }
        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            _encodeCTS?.Cancel();

            if (_encodeTask != null)
                await _encodeTask;

            if (videoPreview != null)
            {
                await videoPreview.End();
            }

            Properties.Settings.Default.Save();

            base.OnFormClosing(e);

            MFShutdown().CheckError();
        }
    }
}
