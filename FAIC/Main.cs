using FAIC.Types;
using FAIC.Types.Cuts;
using FAIC.Types.Formats;
using FAIC.Types.Forms;
using System.IO;
using static Vortice.MediaFoundation.MediaFactory;

namespace FAIC
{
    //TODO & nice-to-haves
    //- Add right click>copy crop/paste crop
    //- Playback plays at the speed that the speed slider is set (maybe update "Play" to have in brackets the speed)
    //- More robust playback (the playhead drag is extremely bad, basically doesn't work)

    public partial class Main : Form, IConsole
    {
        public string InputPath
        {
            get => inputPath;
            set
            {
                if (string.IsNullOrEmpty(value)) return;

                Program.TryOutput(ConsoleMessageType.System, $"Opening file at \"{value}\"");

                if (Path.Exists(value))
                {
                    //These will be reenabled once ffprobe can read the media
                    convertButton.Enabled = false;

                    inputPath = value;
                    if (videoPreview != null)
                    {
                        videoPreview.Open(inputPath, OnNewVideoInfo);
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
        private ProbeMediaInfo latestInfo;
        private CutCollection cuts;
        public Main(string inputPath)
        {
            Program.UpdateConsole(this);

            if (MFStartup().Failure)
                throw new InvalidOperationException("Failed to start Vortice.MediaFoundation.");

            InitializeComponent();
            DragDrop += OnDragDrop;
            DragEnter += OnDragEnter;
            videoPreview = new();
            videoPreviewHost.Child = videoPreview;
            videoPreview.OnNewTime += VideoPreview_OnNewTime;
            videoPreview.OnStateChange += VideoPreview_OnStateChanged;
            videoPreview.OnSupportChange += VideoPreview_OnSupportChange;
            videoPreview.OnCropRectChanged += VideoPreview_OnCropRectChanged;

            UpdatePlayPanelState();

            InputPath = inputPath;
            if (string.IsNullOrEmpty(inputPath))
            {
                Program.TryOutput(ConsoleMessageType.Tip, "Drag a media file onto the window to convert it!");
            }

            fpsSetting.SelectedIndex = 0;
            repeatValue.Value = -1;

            //TODO listen to size visual update
            relativeSizeInput.OnSizeValueUpdate += RelativeSizeInput_OnSizeValueUpdate;

            cuts = new();
            cuts.OnNewSelectionOrSize += RefreshSelectedCutUI;
            cuts.OnNewSelectedValues += RefreshSelectedCutUI;

            cutsControl.Cuts = cuts;
        }
        private void OnNewVideoInfo(ProbeMediaInfo info)
        {
            latestInfo = info;
            settingsGroupBox.Enabled = true;
            convertButton.Enabled = true;
            try
            {
                bool hadInfo = !(info?.IsEmpty() ?? true);
                if (!hadInfo) throw new System.NullReferenceException("Could not retrieve info from the input file.");
                if (info.BestLength <= 0) throw new System.ArgumentOutOfRangeException("Cannot use media with zero length.");

                if (info.Width.ReadData && info.Height.ReadData)
                {
                    relativeSizeInput.SourceWidth = info.Width;
                    relativeSizeInput.SourceHeight = info.Height;
                }
                relativeSizeInput.CropWidthRatio = 1m;
                relativeSizeInput.CropHeightRatio = 1m;
                relativeSizeInput.Ratio = 1m;

                transparentCheckbox.Text = InputCodecUtils.GetTarget(info.Codec).SupportsTransparency() ? "Transparent" : "Transparent Bars";
                hdrCheckbox.Enabled = info.SupportedTransfer.TransferIsHDR();
                hdrCheckbox.Checked = hdrCheckbox.Enabled;

                refreshingCutUI = true;
                TimeNumericUpDown.IMode targetTrimMode = latestInfo.IsConcat
                    ? new TimeNumericUpDown.FramesMode((int)(info.BestLength))
                    : new TimeNumericUpDown.SecondsMode((decimal)info.BestLength);
                Program.NormalizedMinimumCutLength = targetTrimMode.GetMinimumCut() / (decimal)(info.BestLength);

                beginningInput.SetMode(targetTrimMode, true);
                endInput.SetMode(targetTrimMode, false);
                refreshingCutUI = false;


                playhead.Value = 0;
                playhead.Maximum = latestInfo.IsConcat
                    ? (int)(info.BestLength - 1)
                    : (int)(1000 * info.BestLength);

                playhead.TickFrequency = latestInfo.IsConcat ? 1 : 1000; playhead.SmallChange = playhead.TickFrequency;
                playhead.LargeChange = latestInfo.IsConcat ? 10 : 10000;


                fpsSetting.Items.Clear();
                fpsSetting.Items.AddRange(latestInfo.IsConcat ? ["Custom"] : new string[] { "Same", "Nearest", "Blended" });
                fpsSetting.SelectedIndex = 0;
                fpsSetting.Enabled = !latestInfo.IsConcat;

                speedSlider.Enabled = !latestInfo.IsConcat;

                if (latestInfo.IsConcat)
                {
                    fpsValue.Enabled = true;
                    fpsValue.Minimum = 0.001m;
                    fpsValue.Maximum = 10000;
                    fpsValue.Value = 30;

                    speedSlider.Value = DEFAULT_SPEED;
                }
                else
                {
                    automaticallyChangedFrameRateModeToNearest = false;
                    frameRateLastModeBuffer = 0;
                    MatchFrameRateToMedia();
                    fpsSetting_SelectedIndexChanged(this, default);
                }

                repeatValue.Value = -1;

                cuts.Reset();

                Program.TryOutput(ConsoleMessageType.System, $"Detected media: {info}");
            }
            catch (Exception ex)
            {
                Program.TryOutput(ConsoleMessageType.Error, $"Unable to read media: {ex.Message}");
#if DEBUG
                Program.TryOutput(ConsoleMessageType.Error, ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Program.TryOutput(ConsoleMessageType.Error, ex.InnerException.Message);
                    Program.TryOutput(ConsoleMessageType.Error, ex.InnerException.StackTrace);
                }
#endif
                AppendLog("");
                settingsGroupBox.Enabled = false;
                convertButton.Enabled = false;
                return;
            }
        }
        #region Video preview panel
        #region Playhead management
        private void VideoPreview_OnNewTime(double time)
        {
            int target;
            if (latestInfo.IsConcat)
            {
                target = (int)Math.Round(time);
            }
            else
            {
                target = (int)(time * 1000);
            }
            target = Math.Clamp(target, playhead.Minimum, playhead.Maximum);
            playhead.Value = target;
            decimal latestTimeNormalized = (decimal)time / (decimal)latestInfo.BestLength;
            cuts.OnPlayheadMoved(latestTimeNormalized);
            cutsControl.PlayheadPosition = latestTimeNormalized; //seems redundant
        }
        private void playhead_Scroll(object sender, EventArgs e)
        {
            double t;
            if (latestInfo.IsConcat)
            {
                t = playhead.Value;
            }
            else
            {
                t = playhead.Value / 1000.0;
            }
            videoPreview.Seek(t);
        }
        #endregion
        private void playButton_Click(object sender, EventArgs e)
        {
            if (videoPreview.IsPlaying)
                videoPreview.Pause();
            else
                videoPreview.Play();

            UpdatePlayPanelState();
        }
        private void seekButton_Click(object sender, EventArgs e)
        {
            videoPreview?.Step();
            UpdatePlayPanelState();
        }
        private void reverseSeekButton_Click(object sender, EventArgs e)
        {
            videoPreview?.ReverseStep();
            UpdatePlayPanelState();
        }
        private void UpdatePlayPanelState()
        {
            mainSplit.Panel1Collapsed = !videoPreview.CanReadMedia;
            playhead.Enabled = videoPreview.CanReadMedia && !videoPreview.IsPlaying;
            playButton.Enabled = videoPreview.MediaPlayerSupported;
            bool seekEnabled = videoPreview.SourceReaderSupported || videoPreview.ImageSequenceSupported;
            seekButton.Enabled = seekEnabled;
            reverseSeekButton.Enabled = seekEnabled;
            if (videoPreview.MediaPlayerSupported)
            {
                if (videoPreview.IsPlaying)
                {
                    playButton.Text = "Pause";
                }
                else
                {
                    playButton.Text = "Play";
                }
            }
            else
            {
                playButton.Text = "Play/Pause Unsupported";
            }
            if (seekEnabled)
            {
                seekButton.Text = "Next Frame";
                reverseSeekButton.Text = "Previous Frame";
            }
            else
            {
                seekButton.Text = "Next Frame Unsupported";
                reverseSeekButton.Text = "Previous Frame Unsupported";
            }
        }
        private void VideoPreview_OnStateChanged()
        {
            UpdatePlayPanelState();
        }
        private void VideoPreview_OnSupportChange()
        {
            UpdatePlayPanelState();
        }
        #endregion
        #region Settings and console panel
        #region Quality setting
        private void qualitySlider_Scroll(object sender, EventArgs e)
        {
            UpdateQualityLabel();
        }
        private void UpdateQualityLabel()
        {
            qualityLabel.Text = $"Quality ({qualitySlider.Value}%)";
        }
        #endregion
        #region Size setting
        private bool isResizeUpdate = false;
        private void resizeSlider_Scroll(object sender, EventArgs e)
        {
            if (isResizeUpdate) return;

            relativeSizeInput.Ratio = resizeSlider.Value / 100m;
        }
        private void RelativeSizeInput_OnSizeValueUpdate()
        {
            if (isResizeUpdate) return;

            isResizeUpdate = true;

            resizeSlider.Value = (int)Math.Clamp(
                relativeSizeInput.Ratio * 100,
                resizeSlider.Minimum,
                resizeSlider.Maximum
                );

            decimal percentageSize = Math.Max(relativeSizeInput.Ratio * 100, 0.01m);
            string formattedPercentage = percentageSize switch
            {
                < 10 => percentageSize.ToString("0.00"),
                < 100 => percentageSize.ToString("0.0"),
                < 1000 => percentageSize.ToString("0"),
                _ => ">999"
            };
            resizeLabel.Text = $"Resize ({formattedPercentage}%)";

            isResizeUpdate = false;
        }
        #endregion
        #region Speed setting
        public static readonly double[] Speeds =
        [
            0.1, 0.25, 0.33333333333333333, 0.5, 0.75, 1, 1.25, 1.5, 2, 3, 4, 5, 6, 8, 12, 16, 32
        ];
        private const int DEFAULT_SPEED = 5;
        private void speedSlider_Scroll(object sender, EventArgs e)
        {
            if (!latestInfo.IsConcat
                && (!automaticallyChangedFrameRateModeToNearest
                && fpsSetting.SelectedIndex == 0))
            {
                automaticallyChangedFrameRateModeToNearest = true;
                fpsSetting.SelectedIndex = 1;
                MatchFrameRateToMedia(factorSpeed: false);
            }
            speedLabel.Text = $"Speed ({Math.Round(Speeds[speedSlider.Value], 2)}x)";
            if (!latestInfo.IsConcat && fpsSetting.SelectedIndex == 0)
            {
                MatchFrameRateToMedia();
            }
        }
        #endregion
        #region Cuts setting
        private void cutNumberInput_ValueChanged(object sender, EventArgs e)
        {
            if (refreshingCutUI) return;

            int zeroBasedIndex = (int)cutNumberInput.Value - 1;

            if (cuts.Selected != zeroBasedIndex)
                cuts.OnNewSelectedValue(zeroBasedIndex);
        }
        private void addCutButton_Click(object sender, EventArgs e)
        {
            decimal normalisedTime = latestInfo.IsConcat
                ? playhead.Value / (decimal)latestInfo.BestLength
                : (playhead.Value - playhead.Minimum) / (decimal)(playhead.Maximum - playhead.Minimum);

            cuts.AddButton(normalisedTime);
        }
        private void removeCutButton_Click(object sender, EventArgs e)
        {
            cuts.RemoveButton();
        }
        private void beginningInput_ValueChanged(object sender, EventArgs e)
        {
            if (refreshingCutUI) return;

            Cut current = cuts[cuts.Selected];
            current.Start = beginningInput.NormalizedValue ?? current.Start;
            cuts[cuts.Selected] = current;
        }
        private void endInput_ValueChanged(object sender, EventArgs e)
        {
            if (refreshingCutUI) return;

            Cut current = cuts[cuts.Selected];
            current.End = endInput.NormalizedValue ?? current.End;
            cuts[cuts.Selected] = current;
        }
        #endregion
        #region Frame rate setting
        public const int FRAME_RATE_PRECISION = 3;
        private decimal GetSourceMediaFrameRate() => videoPreview.Latest != null ? (decimal)videoPreview.Latest.EstimatedFrameRate : 0;
        private void MatchFrameRateToMedia(bool factorSpeed = true)
        {
            if (latestInfo.IsConcat) return;

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
        #region Console
        const int MAX_LINES = 2000;
        public void AppendLog(string msg, System.Drawing.Color? color = null, bool bold = false)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => AppendLog(msg, color, bold));
                return;
            }

            try
            {
                Font prevSelectionFont = commandLineOutput.SelectionFont;
                System.Drawing.Color prevColorBuffer = commandLineOutput.SelectionColor;
                if (color.HasValue) commandLineOutput.SelectionColor = color.Value;
                if (bold) commandLineOutput.SelectionFont = new Font(commandLineOutput.Font, FontStyle.Bold);

                commandLineOutput.AppendText(msg + Environment.NewLine);

                if (bold) commandLineOutput.SelectionFont = prevSelectionFont;
                if (color.HasValue) commandLineOutput.SelectionColor = prevColorBuffer;

                if (commandLineOutput.Lines.Length > MAX_LINES)
                {
                    var lines = commandLineOutput.Lines.Skip(commandLineOutput.Lines.Length - MAX_LINES).ToArray();
                    commandLineOutput.Lines = lines;
                }
            }
            catch (ObjectDisposedException)
            {
                //Ignore - application shutting down
            }
        }
        #endregion
        #endregion
        #region Cuts management
        private bool refreshingCutUI;
        private void RefreshSelectedCutUI()
        {
            Cut cut = cuts[cuts.Selected];
            decimal timelineLength = (decimal)latestInfo.BestLength;

            refreshingCutUI = true;
            try
            {
                cutNumberInput.Value = 1;
                cutNumberInput.Minimum = 1m;
                cutNumberInput.Maximum = Math.Max(1, cuts.Total);

                decimal displayedCutNumber = cuts.Selected + 1m;
                if (cutNumberInput.Value != displayedCutNumber)
                {
                    cutNumberInput.Value = displayedCutNumber;
                }

                //Maximum change won't update the text display, but text includes maximum
                cutNumberInput.ForceUpdateEditText();

                decimal displayedStart = cut.Start * timelineLength;
                decimal displayedEnd = cut.End * timelineLength;

                if (beginningInput.IsSetup)
                    beginningInput.SetNew(displayedStart, true, displayedEnd);
                if (endInput.IsSetup)
                    endInput.SetNew(displayedEnd, false, displayedStart);

                videoPreview.NormalizedCropRect = cut.NormalizedCrop;
                videoPreview.SelectedCutIndex = cuts.Selected;
            }
            finally
            {
                refreshingCutUI = false;
            }

            string addMethod = cuts.ShouldSelectedSplitNotAdd() ? "Split" : "After";
            addCutButton.Text = $"Add ({addMethod})";
            removeCutButton.Enabled = cuts.Total > 1;
        }
        private (double maxWidthCrop, double maxHeightCrop) GetMaxCrops()
        {
            double maxWidthCrop = 0, maxHeightCrop = 0;
            for (int i = 0; i < cuts.Total; i++)
            {
                maxWidthCrop = Math.Max(maxWidthCrop, cuts[i].NormalizedCrop.Width);
                maxHeightCrop = Math.Max(maxHeightCrop, cuts[i].NormalizedCrop.Height);
            }
            return (maxWidthCrop, maxHeightCrop);
        }
        private void VideoPreview_OnCropRectChanged(System.Windows.Rect crop)
        {
            if (refreshingCutUI) return;

            Cut current = cuts[cuts.Selected];
            current.NormalizedCrop = crop;
            cuts[cuts.Selected] = current;

            var crops = GetMaxCrops();
            relativeSizeInput.CropWidthRatio = (decimal)crops.maxWidthCrop;
            relativeSizeInput.CropHeightRatio = (decimal)crops.maxHeightCrop;
        }
        #endregion
        #region Encoding
        private void convertButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputPath))
            {
                MessageBox.Show($"Input file path was null or empty.", "Encode Failed");
            }

            if (beginningInput.Value > endInput.Value)
            {
                Program.TryOutput(ConsoleMessageType.Warning, "End time is before start time, swapping values.");
                decimal lastFrameTimeCurrent = endInput.Value;
                endInput.Value = beginningInput.Value;
                beginningInput.Value = lastFrameTimeCurrent;
            }
            saveFileDialogue.InitialDirectory = Path.GetDirectoryName(inputPath);
            saveFileDialogue.FileName = Path.GetFileNameWithoutExtension(inputPath) + "_Converted";
            if (saveFileDialogue.ShowDialog(this) == DialogResult.OK)
            {
                StartEncode(saveFileDialogue.FileName);
            }
        }
        public ArgumentsWindowOutputs DoArgumentsWindow(ArgumentsWindowInputs inputs)
        {
            ArgumentsWindowOutputs outputs = new()
            {
                confirmed = false
            };
            inputs.onConfirm = (data) =>
            {
                outputs = data;
            };

            using (var args = new CustomArguments(inputs))
            {
                args.ShowDialog(this);
                return outputs;
            }
        }
        private void StartEncode(string outputPath)
        {
            commandLineOutput.Clear();

            string extension = Path.GetExtension(outputPath).TrimStart('.').ToLowerInvariant();

            switch (extension)
            {
                case "gif":
                    if (fpsValue.Value > 100)
                    {
                        Program.TryOutput(ConsoleMessageType.Warning, "GIF does not support >100fps. Clamping to 100.");
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

            OutputCodec outputCodec = OutputCodecUtils.GetTarget(outputPath);

            bool isHDR = latestInfo.SupportedTransfer.TransferIsHDR();
            bool isHighBits = latestInfo.SupportedTransfer.TransferIsHighBit();
            ColorHandlingMode colorMode =
                (isHDR && outputCodec.SupportsHDR() && hdrCheckbox.Checked) ? ColorHandlingMode.HighDynamicRange
                : ((isHighBits && outputCodec.SupportsHighBits()) ? ColorHandlingMode.HighBits : ColorHandlingMode.NormalBits);

            try
            {
                EncodeSettings settings = new EncodeSettings(cuts, latestInfo, outputCodec,
                    relativeSizeInput.EffectiveWidth, relativeSizeInput.EffectiveHeight,
                    processingFastRadio.Checked ? EncodeSettings.TuningSetting.Fast : EncodeSettings.TuningSetting.Best,
                    transparentCheckbox.Checked, colorMode,
                    Speeds[speedSlider.Value], fpsValue.Value, fpsSetting.SelectedIndex switch
                    {
                        0 => latestInfo.IsConcat ? EncodeSettings.InterpolateSetting.Nearest : EncodeSettings.InterpolateSetting.None,
                        1 => EncodeSettings.InterpolateSetting.Nearest,
                        2 => EncodeSettings.InterpolateSetting.Blended,
                        _ => throw new System.NotImplementedException($"Unrecognised FPS setting target option number ({fpsSetting.SelectedIndex})")
                    })
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    Quality = qualitySlider.Value,
                    Repeats = (int)repeatValue.Value < 0 ? -1 : (int)repeatValue.Value,
                    onBeforeArguments = editArgumentsCheckbox.Checked ? DoArgumentsWindow : null
                };

                ConversionWindow conversion = new ConversionWindow(settings);
                conversion.Show(this);
            }
            catch (Exception ex)
            {
                Program.TryOutput(ConsoleMessageType.Error, ex.Message);
            }
        }
        #endregion

        #region Window lifetime management
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
        private void OnDragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            FileAttributes attr = File.GetAttributes(files[0]);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                using (var importer = new FolderImporter(files[0], (input) => { InputPath = input; }))
                {
                    importer.ShowDialog(this);
                }
            }
            else
            {
                InputPath = files[0];
            }
        }
        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            if (videoPreview != null)
            {
                await videoPreview.End();
            }

            Properties.Settings.Default.Save();

            base.OnFormClosing(e);

            MFShutdown().CheckError();
        }

        #endregion
    }
}
