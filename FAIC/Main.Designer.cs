using System.Windows.Forms.Integration;

namespace FAIC
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            qualitySlider = new TrackBar();
            settingsBindingSource = new BindingSource(components);
            convertButton = new Button();
            qualityLabel = new Label();
            saveFileDialogue = new SaveFileDialog();
            cancelButton = new Button();
            commandLineOutput = new RichTextBox();
            settingsGroupBox = new GroupBox();
            settingsContainerPanel = new Panel();
            settingsTable = new TableLayoutPanel();
            lastFrameLabel = new Label();
            resizeDimensionLabel = new Label();
            label1 = new Label();
            resizeLabel = new Label();
            resizeSlider = new TrackBar();
            fpsValue = new NumericUpDown();
            repeatValue = new FAIC.Types.Forms.InfinityNumericUpDown();
            fpsSetting = new ComboBox();
            speedSlider = new TrackBar();
            repeatsLabel = new Label();
            fpsLabel = new Label();
            speedLabel = new Label();
            lastFrameInput = new NumericUpDown();
            trimColumnLayoutPanel = new TableLayoutPanel();
            trimLabel = new Label();
            firstFrame = new Label();
            firstFrameInput = new NumericUpDown();
            resizeDimensionValue = new NumericUpDown();
            fpsSpacerPanel = new Panel();
            samplingLayoutPanel = new TableLayoutPanel();
            sampleBestRadio = new RadioButton();
            sampleFastRadio = new RadioButton();
            mainSplit = new SplitContainer();
            previewPanel = new Panel();
            playbackConsoleTable = new TableLayoutPanel();
            seekButton = new Button();
            playButton = new Button();
            reverseSeekButton = new Button();
            trimStartHereButton = new Button();
            trimEndHereButton = new Button();
            playhead = new TrackBar();
            videoContainingPanel = new Panel();
            videoPreviewHost = new ElementHost();
            conversionPanel = new TableLayoutPanel();
            commandLinePaddingPanel = new Panel();
            actionsPanel = new Panel();
            tooltips = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)qualitySlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)settingsBindingSource).BeginInit();
            settingsGroupBox.SuspendLayout();
            settingsContainerPanel.SuspendLayout();
            settingsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resizeSlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)fpsValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)repeatValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lastFrameInput).BeginInit();
            trimColumnLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)firstFrameInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)resizeDimensionValue).BeginInit();
            samplingLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplit).BeginInit();
            mainSplit.Panel1.SuspendLayout();
            mainSplit.Panel2.SuspendLayout();
            mainSplit.SuspendLayout();
            previewPanel.SuspendLayout();
            playbackConsoleTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)playhead).BeginInit();
            videoContainingPanel.SuspendLayout();
            conversionPanel.SuspendLayout();
            commandLinePaddingPanel.SuspendLayout();
            actionsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // qualitySlider
            // 
            qualitySlider.AccessibleName = "Quality Slider";
            qualitySlider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            qualitySlider.AutoSize = false;
            qualitySlider.DataBindings.Add(new Binding("DataContext", settingsBindingSource, "Quality", true));
            qualitySlider.DataBindings.Add(new Binding("Value", settingsBindingSource, "Quality", true, DataSourceUpdateMode.OnPropertyChanged));
            qualitySlider.Location = new Point(143, 3);
            qualitySlider.Maximum = 100;
            qualitySlider.Name = "qualitySlider";
            qualitySlider.Size = new Size(254, 31);
            qualitySlider.TabIndex = 1;
            qualitySlider.TickFrequency = 10;
            tooltips.SetToolTip(qualitySlider, resources.GetString("qualitySlider.ToolTip"));
            qualitySlider.Value = 50;
            qualitySlider.Scroll += qualitySlider_Scroll;
            // 
            // settingsBindingSource
            // 
            settingsBindingSource.DataSource = typeof(Properties.Settings);
            // 
            // convertButton
            // 
            convertButton.AccessibleDescription = "";
            convertButton.AccessibleName = "Begin conversion button";
            convertButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            convertButton.Location = new Point(3, 3);
            convertButton.Name = "convertButton";
            convertButton.Size = new Size(890, 48);
            convertButton.TabIndex = 17;
            convertButton.Text = "Convert To...";
            tooltips.SetToolTip(convertButton, "Begin conversion with current settings. Select desired format in the \"Save To\" dialogue after pressing this button.");
            convertButton.UseVisualStyleBackColor = true;
            convertButton.Click += convertButton_Click;
            // 
            // qualityLabel
            // 
            qualityLabel.AutoSize = true;
            qualityLabel.Dock = DockStyle.Fill;
            qualityLabel.Location = new Point(3, 0);
            qualityLabel.Name = "qualityLabel";
            qualityLabel.Size = new Size(134, 37);
            qualityLabel.TabIndex = 2;
            qualityLabel.Text = "Quality (100%)";
            qualityLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // saveFileDialogue
            // 
            saveFileDialogue.DefaultExt = "avif";
            saveFileDialogue.Filter = "AV1 Image File Format (*.avif)|*.avif|JPEG XL (*.jxl)|*.jxl|WebP (*.webp)|*.webp|Animated Portable Network Graphics (*.png, *.apng)|*.png;*.apng|Graphics Interchange Format (*.gif)|*.gif";
            // 
            // cancelButton
            // 
            cancelButton.AccessibleName = "Cancel current conversion button";
            cancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Enabled = false;
            cancelButton.Location = new Point(899, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(100, 48);
            cancelButton.TabIndex = 18;
            cancelButton.Text = "Cancel";
            tooltips.SetToolTip(cancelButton, "Cancel the current conversion.");
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // commandLineOutput
            // 
            commandLineOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            commandLineOutput.BackColor = SystemColors.Window;
            commandLineOutput.BorderStyle = BorderStyle.None;
            commandLineOutput.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            commandLineOutput.Location = new Point(3, 3);
            commandLineOutput.Name = "commandLineOutput";
            commandLineOutput.ReadOnly = true;
            commandLineOutput.Size = new Size(400, 179);
            commandLineOutput.TabIndex = 5;
            commandLineOutput.Text = "";
            tooltips.SetToolTip(commandLineOutput, "Console output from the converter tools.");
            commandLineOutput.WordWrap = false;
            // 
            // settingsGroupBox
            // 
            settingsGroupBox.AutoSize = true;
            settingsGroupBox.Controls.Add(settingsContainerPanel);
            settingsGroupBox.Dock = DockStyle.Top;
            settingsGroupBox.Enabled = false;
            settingsGroupBox.Location = new Point(3, 3);
            settingsGroupBox.Margin = new Padding(3, 3, 3, 0);
            settingsGroupBox.Name = "settingsGroupBox";
            settingsGroupBox.Size = new Size(406, 446);
            settingsGroupBox.TabIndex = 6;
            settingsGroupBox.TabStop = false;
            settingsGroupBox.Text = "Settings";
            // 
            // settingsContainerPanel
            // 
            settingsContainerPanel.AutoSize = true;
            settingsContainerPanel.Controls.Add(settingsTable);
            settingsContainerPanel.Dock = DockStyle.Fill;
            settingsContainerPanel.Location = new Point(3, 27);
            settingsContainerPanel.Name = "settingsContainerPanel";
            settingsContainerPanel.Size = new Size(400, 416);
            settingsContainerPanel.TabIndex = 9;
            // 
            // settingsTable
            // 
            settingsTable.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            settingsTable.AutoSize = true;
            settingsTable.ColumnCount = 2;
            settingsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            settingsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            settingsTable.Controls.Add(lastFrameLabel, 0, 6);
            settingsTable.Controls.Add(resizeDimensionLabel, 0, 2);
            settingsTable.Controls.Add(label1, 0, 3);
            settingsTable.Controls.Add(resizeLabel, 0, 1);
            settingsTable.Controls.Add(resizeSlider, 1, 1);
            settingsTable.Controls.Add(qualitySlider, 1, 0);
            settingsTable.Controls.Add(qualityLabel, 0, 0);
            settingsTable.Controls.Add(fpsValue, 1, 10);
            settingsTable.Controls.Add(repeatValue, 1, 11);
            settingsTable.Controls.Add(fpsSetting, 1, 9);
            settingsTable.Controls.Add(speedSlider, 1, 8);
            settingsTable.Controls.Add(repeatsLabel, 0, 11);
            settingsTable.Controls.Add(fpsLabel, 0, 9);
            settingsTable.Controls.Add(speedLabel, 0, 8);
            settingsTable.Controls.Add(lastFrameInput, 1, 6);
            settingsTable.Controls.Add(trimColumnLayoutPanel, 0, 5);
            settingsTable.Controls.Add(firstFrameInput, 1, 5);
            settingsTable.Controls.Add(resizeDimensionValue, 1, 2);
            settingsTable.Controls.Add(fpsSpacerPanel, 0, 10);
            settingsTable.Controls.Add(samplingLayoutPanel, 1, 3);
            settingsTable.Location = new Point(0, 0);
            settingsTable.Margin = new Padding(3, 3, 3, 0);
            settingsTable.Name = "settingsTable";
            settingsTable.RowCount = 12;
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.Size = new Size(400, 416);
            settingsTable.TabIndex = 8;
            // 
            // lastFrameLabel
            // 
            lastFrameLabel.AutoSize = true;
            lastFrameLabel.Dock = DockStyle.Fill;
            lastFrameLabel.Location = new Point(3, 209);
            lastFrameLabel.MinimumSize = new Size(65, 0);
            lastFrameLabel.Name = "lastFrameLabel";
            lastFrameLabel.Size = new Size(134, 37);
            lastFrameLabel.TabIndex = 7;
            lastFrameLabel.Text = "End";
            lastFrameLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // resizeDimensionLabel
            // 
            resizeDimensionLabel.AutoSize = true;
            resizeDimensionLabel.Dock = DockStyle.Fill;
            resizeDimensionLabel.Location = new Point(3, 74);
            resizeDimensionLabel.MinimumSize = new Size(65, 0);
            resizeDimensionLabel.Name = "resizeDimensionLabel";
            resizeDimensionLabel.Size = new Size(134, 37);
            resizeDimensionLabel.TabIndex = 0;
            resizeDimensionLabel.Text = "Height";
            resizeDimensionLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(3, 111);
            label1.MinimumSize = new Size(65, 0);
            label1.Name = "label1";
            label1.Size = new Size(134, 41);
            label1.TabIndex = 19;
            label1.Text = "Sampling";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // resizeLabel
            // 
            resizeLabel.AutoSize = true;
            resizeLabel.Dock = DockStyle.Fill;
            resizeLabel.Location = new Point(3, 37);
            resizeLabel.Name = "resizeLabel";
            resizeLabel.Size = new Size(134, 37);
            resizeLabel.TabIndex = 9;
            resizeLabel.Text = "Size (100%)";
            resizeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // resizeSlider
            // 
            resizeSlider.AccessibleName = "Resize Slider";
            resizeSlider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            resizeSlider.AutoSize = false;
            resizeSlider.DataBindings.Add(new Binding("DataContext", settingsBindingSource, "Quality", true));
            resizeSlider.DataBindings.Add(new Binding("Value", settingsBindingSource, "Quality", true, DataSourceUpdateMode.OnPropertyChanged));
            resizeSlider.Location = new Point(143, 40);
            resizeSlider.Maximum = 100;
            resizeSlider.Name = "resizeSlider";
            resizeSlider.Size = new Size(254, 31);
            resizeSlider.TabIndex = 2;
            resizeSlider.TickFrequency = 10;
            tooltips.SetToolTip(resizeSlider, "The percentage to decrease the output resolution by. Lower values will help improve performance and decrease file size.\r\n");
            resizeSlider.Value = 100;
            resizeSlider.Scroll += resizeSlider_Scroll;
            // 
            // fpsValue
            // 
            fpsValue.AccessibleName = "Target frame rate input";
            fpsValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fpsValue.DecimalPlaces = 3;
            fpsValue.Enabled = false;
            fpsValue.Location = new Point(143, 345);
            fpsValue.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            fpsValue.Name = "fpsValue";
            fpsValue.Size = new Size(254, 31);
            fpsValue.TabIndex = 10;
            tooltips.SetToolTip(fpsValue, "Target frame rate of the converted output.");
            // 
            // repeatValue
            // 
            repeatValue.AccessibleName = "Repeat count input";
            repeatValue.Dock = DockStyle.Fill;
            repeatValue.Location = new Point(143, 382);
            repeatValue.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            repeatValue.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            repeatValue.Name = "repeatValue";
            repeatValue.Size = new Size(254, 31);
            repeatValue.TabIndex = 11;
            tooltips.SetToolTip(repeatValue, resources.GetString("repeatValue.ToolTip"));
            // 
            // fpsSetting
            // 
            fpsSetting.AccessibleName = "Frame rate mode dropdown";
            fpsSetting.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fpsSetting.FormattingEnabled = true;
            fpsSetting.Items.AddRange(new object[] { "Same", "Nearest", "Blended" });
            fpsSetting.Location = new Point(143, 306);
            fpsSetting.MaxDropDownItems = 3;
            fpsSetting.Name = "fpsSetting";
            fpsSetting.Size = new Size(254, 33);
            fpsSetting.TabIndex = 9;
            tooltips.SetToolTip(fpsSetting, resources.GetString("fpsSetting.ToolTip"));
            fpsSetting.SelectedIndexChanged += fpsSetting_SelectedIndexChanged;
            // 
            // speedSlider
            // 
            speedSlider.AccessibleName = "Speed Slider";
            speedSlider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            speedSlider.AutoSize = false;
            speedSlider.DataBindings.Add(new Binding("DataContext", settingsBindingSource, "Quality", true));
            speedSlider.DataBindings.Add(new Binding("Value", settingsBindingSource, "Quality", true, DataSourceUpdateMode.OnPropertyChanged));
            speedSlider.LargeChange = 1;
            speedSlider.Location = new Point(143, 269);
            speedSlider.Maximum = 15;
            speedSlider.Name = "speedSlider";
            speedSlider.Size = new Size(254, 31);
            speedSlider.TabIndex = 8;
            tooltips.SetToolTip(speedSlider, "The percentage to decrease the output resolution by. Lower values will help improve performance and decrease file size.\r\n");
            speedSlider.Value = 5;
            speedSlider.Scroll += speedSlider_Scroll;
            // 
            // repeatsLabel
            // 
            repeatsLabel.AutoSize = true;
            repeatsLabel.Dock = DockStyle.Fill;
            repeatsLabel.Location = new Point(3, 379);
            repeatsLabel.Name = "repeatsLabel";
            repeatsLabel.Size = new Size(134, 37);
            repeatsLabel.TabIndex = 10;
            repeatsLabel.Text = "Repeats";
            repeatsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // fpsLabel
            // 
            fpsLabel.AutoSize = true;
            fpsLabel.Dock = DockStyle.Fill;
            fpsLabel.Location = new Point(3, 303);
            fpsLabel.Name = "fpsLabel";
            fpsLabel.Size = new Size(134, 39);
            fpsLabel.TabIndex = 9;
            fpsLabel.Text = "Frame Rate";
            fpsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // speedLabel
            // 
            speedLabel.AutoSize = true;
            speedLabel.Dock = DockStyle.Fill;
            speedLabel.Location = new Point(3, 266);
            speedLabel.Name = "speedLabel";
            speedLabel.Size = new Size(134, 37);
            speedLabel.TabIndex = 13;
            speedLabel.Text = "Speed (1.00x)";
            speedLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lastFrameInput
            // 
            lastFrameInput.AccessibleName = "Trim end of the video to X seconds input";
            lastFrameInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lastFrameInput.DecimalPlaces = 3;
            lastFrameInput.Location = new Point(143, 212);
            lastFrameInput.Name = "lastFrameInput";
            lastFrameInput.Size = new Size(254, 31);
            lastFrameInput.TabIndex = 7;
            tooltips.SetToolTip(lastFrameInput, "Where to end the converted output relative to the source media in seconds.\r\n");
            // 
            // trimColumnLayoutPanel
            // 
            trimColumnLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trimColumnLayoutPanel.AutoSize = true;
            trimColumnLayoutPanel.ColumnCount = 2;
            trimColumnLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            trimColumnLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            trimColumnLayoutPanel.Controls.Add(trimLabel, 0, 0);
            trimColumnLayoutPanel.Controls.Add(firstFrame, 1, 0);
            trimColumnLayoutPanel.Location = new Point(0, 172);
            trimColumnLayoutPanel.Margin = new Padding(0);
            trimColumnLayoutPanel.Name = "trimColumnLayoutPanel";
            trimColumnLayoutPanel.RowCount = 1;
            trimColumnLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            trimColumnLayoutPanel.Size = new Size(140, 37);
            trimColumnLayoutPanel.TabIndex = 16;
            // 
            // trimLabel
            // 
            trimLabel.AutoSize = true;
            trimLabel.Dock = DockStyle.Fill;
            trimLabel.Location = new Point(3, 0);
            trimLabel.Name = "trimLabel";
            trimLabel.Size = new Size(45, 37);
            trimLabel.TabIndex = 6;
            trimLabel.Text = "Trim";
            trimLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // firstFrame
            // 
            firstFrame.Dock = DockStyle.Fill;
            firstFrame.Location = new Point(54, 0);
            firstFrame.MinimumSize = new Size(65, 0);
            firstFrame.Name = "firstFrame";
            firstFrame.Size = new Size(83, 37);
            firstFrame.TabIndex = 7;
            firstFrame.Text = "Start";
            firstFrame.TextAlign = ContentAlignment.MiddleRight;
            // 
            // firstFrameInput
            // 
            firstFrameInput.AccessibleName = "Trim start of the video to X seconds input";
            firstFrameInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            firstFrameInput.DecimalPlaces = 3;
            firstFrameInput.Location = new Point(143, 175);
            firstFrameInput.Name = "firstFrameInput";
            firstFrameInput.Size = new Size(254, 31);
            firstFrameInput.TabIndex = 6;
            tooltips.SetToolTip(firstFrameInput, "Where to start the converted output relative to the source media in seconds.\r\n\r\n");
            firstFrameInput.ValueChanged += firstFrameInput_ValueChanged;
            // 
            // resizeDimensionValue
            // 
            resizeDimensionValue.AccessibleName = "Resize Input";
            resizeDimensionValue.Dock = DockStyle.Fill;
            resizeDimensionValue.Location = new Point(143, 77);
            resizeDimensionValue.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
            resizeDimensionValue.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            resizeDimensionValue.Name = "resizeDimensionValue";
            resizeDimensionValue.Size = new Size(254, 31);
            resizeDimensionValue.TabIndex = 3;
            tooltips.SetToolTip(resizeDimensionValue, "The desired output resolution in pixels of the largest dimension. The other dimension will be rescaled as well to preserve the aspect ratio.");
            resizeDimensionValue.Value = new decimal(new int[] { 1, 0, 0, 0 });
            resizeDimensionValue.ValueChanged += resizeDimensionValue_ValueChanged;
            // 
            // fpsSpacerPanel
            // 
            fpsSpacerPanel.AutoSize = true;
            fpsSpacerPanel.Dock = DockStyle.Fill;
            fpsSpacerPanel.Location = new Point(3, 345);
            fpsSpacerPanel.Name = "fpsSpacerPanel";
            fpsSpacerPanel.Size = new Size(134, 31);
            fpsSpacerPanel.TabIndex = 17;
            // 
            // samplingLayoutPanel
            // 
            samplingLayoutPanel.AutoSize = true;
            samplingLayoutPanel.ColumnCount = 2;
            samplingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            samplingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            samplingLayoutPanel.Controls.Add(sampleBestRadio, 1, 0);
            samplingLayoutPanel.Controls.Add(sampleFastRadio, 0, 0);
            samplingLayoutPanel.Dock = DockStyle.Fill;
            samplingLayoutPanel.Enabled = false;
            samplingLayoutPanel.Location = new Point(143, 114);
            samplingLayoutPanel.Name = "samplingLayoutPanel";
            samplingLayoutPanel.RowCount = 1;
            samplingLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            samplingLayoutPanel.Size = new Size(254, 35);
            samplingLayoutPanel.TabIndex = 18;
            // 
            // sampleBestRadio
            // 
            sampleBestRadio.AccessibleName = "Best resampling algorithm button";
            sampleBestRadio.AutoSize = true;
            sampleBestRadio.Location = new Point(130, 3);
            sampleBestRadio.Name = "sampleBestRadio";
            sampleBestRadio.Size = new Size(70, 29);
            sampleBestRadio.TabIndex = 5;
            sampleBestRadio.Text = "Best";
            tooltips.SetToolTip(sampleBestRadio, "Use Lanczos for resizing to smaller, and Spline36 for resizing to larger. Best quality, but slower.");
            sampleBestRadio.UseVisualStyleBackColor = true;
            // 
            // sampleFastRadio
            // 
            sampleFastRadio.AccessibleName = "Fast resampling algorithm button";
            sampleFastRadio.AutoSize = true;
            sampleFastRadio.Checked = true;
            sampleFastRadio.Location = new Point(3, 3);
            sampleFastRadio.Name = "sampleFastRadio";
            sampleFastRadio.Size = new Size(68, 29);
            sampleFastRadio.TabIndex = 4;
            sampleFastRadio.TabStop = true;
            sampleFastRadio.Text = "Fast";
            tooltips.SetToolTip(sampleFastRadio, "Use billinear for resizing. Fast, but can lead to aliasing and shimmer in motion at lower resolutions, and worse contrast at higher resolutions.");
            sampleFastRadio.UseVisualStyleBackColor = true;
            // 
            // mainSplit
            // 
            mainSplit.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mainSplit.Location = new Point(12, 12);
            mainSplit.Name = "mainSplit";
            // 
            // mainSplit.Panel1
            // 
            mainSplit.Panel1.Controls.Add(previewPanel);
            mainSplit.Panel1.Controls.Add(videoContainingPanel);
            mainSplit.Panel1MinSize = 200;
            // 
            // mainSplit.Panel2
            // 
            mainSplit.Panel2.Controls.Add(conversionPanel);
            mainSplit.Panel2MinSize = 320;
            mainSplit.Size = new Size(978, 640);
            mainSplit.SplitterDistance = 562;
            mainSplit.TabIndex = 0;
            mainSplit.SplitterMoved += mainSplit_SplitterMoved;
            // 
            // previewPanel
            // 
            previewPanel.Controls.Add(playbackConsoleTable);
            previewPanel.Controls.Add(trimStartHereButton);
            previewPanel.Controls.Add(trimEndHereButton);
            previewPanel.Controls.Add(playhead);
            previewPanel.Dock = DockStyle.Bottom;
            previewPanel.Location = new Point(0, 553);
            previewPanel.Name = "previewPanel";
            previewPanel.Size = new Size(562, 87);
            previewPanel.TabIndex = 1;
            // 
            // playbackConsoleTable
            // 
            playbackConsoleTable.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            playbackConsoleTable.ColumnCount = 3;
            playbackConsoleTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3290024F));
            playbackConsoleTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3319969F));
            playbackConsoleTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3389969F));
            playbackConsoleTable.Controls.Add(seekButton, 2, 0);
            playbackConsoleTable.Controls.Add(playButton, 1, 0);
            playbackConsoleTable.Controls.Add(reverseSeekButton, 0, 0);
            playbackConsoleTable.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
            playbackConsoleTable.Location = new Point(41, 49);
            playbackConsoleTable.Margin = new Padding(0, 3, 0, 3);
            playbackConsoleTable.Name = "playbackConsoleTable";
            playbackConsoleTable.RowCount = 1;
            playbackConsoleTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            playbackConsoleTable.Size = new Size(477, 35);
            playbackConsoleTable.TabIndex = 5;
            // 
            // seekButton
            // 
            seekButton.AccessibleName = "Step forwards 1 frame button";
            seekButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            seekButton.Location = new Point(319, 0);
            seekButton.Margin = new Padding(3, 0, 3, 0);
            seekButton.Name = "seekButton";
            seekButton.Size = new Size(155, 35);
            seekButton.TabIndex = 16;
            seekButton.Text = "Next Frame";
            tooltips.SetToolTip(seekButton, resources.GetString("seekButton.ToolTip"));
            seekButton.UseVisualStyleBackColor = true;
            seekButton.Click += seekButton_Click;
            // 
            // playButton
            // 
            playButton.AccessibleName = "Begin playback button";
            playButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            playButton.Location = new Point(161, 0);
            playButton.Margin = new Padding(3, 0, 3, 0);
            playButton.Name = "playButton";
            playButton.Size = new Size(152, 35);
            playButton.TabIndex = 15;
            playButton.Text = "Play";
            tooltips.SetToolTip(playButton, "Start playing the media from the current playhead position.");
            playButton.UseVisualStyleBackColor = true;
            playButton.Click += playButton_Click;
            // 
            // reverseSeekButton
            // 
            reverseSeekButton.AccessibleName = "Step backwards 1 frame button";
            reverseSeekButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            reverseSeekButton.Location = new Point(3, 0);
            reverseSeekButton.Margin = new Padding(3, 0, 3, 0);
            reverseSeekButton.Name = "reverseSeekButton";
            reverseSeekButton.Size = new Size(152, 35);
            reverseSeekButton.TabIndex = 17;
            reverseSeekButton.Text = "Previous Frame";
            tooltips.SetToolTip(reverseSeekButton, "Go to the previous frame in the media. This may cause the program to freeze for a bit.");
            reverseSeekButton.UseVisualStyleBackColor = true;
            reverseSeekButton.Click += reverseSeekButton_Click;
            // 
            // trimStartHereButton
            // 
            trimStartHereButton.AccessibleName = "Trim start of output to current playhead";
            trimStartHereButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            trimStartHereButton.Location = new Point(3, 3);
            trimStartHereButton.Name = "trimStartHereButton";
            trimStartHereButton.Size = new Size(35, 81);
            trimStartHereButton.TabIndex = 12;
            trimStartHereButton.Text = "[";
            tooltips.SetToolTip(trimStartHereButton, "Set \"Start Time\" to current playback position.\r\n");
            trimStartHereButton.UseVisualStyleBackColor = true;
            trimStartHereButton.Click += trimStartHereButton_Click;
            // 
            // trimEndHereButton
            // 
            trimEndHereButton.AccessibleName = "Trim end of output to current playhead";
            trimEndHereButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            trimEndHereButton.Location = new Point(521, 3);
            trimEndHereButton.Name = "trimEndHereButton";
            trimEndHereButton.Size = new Size(35, 81);
            trimEndHereButton.TabIndex = 14;
            trimEndHereButton.Text = "]";
            tooltips.SetToolTip(trimEndHereButton, "Set \"End Time\" to current playback position.");
            trimEndHereButton.UseVisualStyleBackColor = true;
            trimEndHereButton.Click += trimEndHereButton_Click;
            // 
            // playhead
            // 
            playhead.AccessibleName = "Playhead/timeline";
            playhead.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            playhead.AutoSize = false;
            playhead.LargeChange = 10000;
            playhead.Location = new Point(44, 3);
            playhead.Maximum = 1000;
            playhead.Name = "playhead";
            playhead.Size = new Size(474, 40);
            playhead.SmallChange = 1000;
            playhead.TabIndex = 13;
            playhead.TickFrequency = 1000;
            tooltips.SetToolTip(playhead, "Timeline with playhead to scrub through the media. Playhead freezes during playback.");
            playhead.Scroll += playhead_Scroll;
            // 
            // videoContainingPanel
            // 
            videoContainingPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            videoContainingPanel.Controls.Add(videoPreviewHost);
            videoContainingPanel.Location = new Point(3, 3);
            videoContainingPanel.Name = "videoContainingPanel";
            videoContainingPanel.Size = new Size(553, 544);
            videoContainingPanel.TabIndex = 0;
            // 
            // videoPreviewHost
            // 
            videoPreviewHost.Dock = DockStyle.Fill;
            videoPreviewHost.Location = new Point(0, 0);
            videoPreviewHost.Name = "videoPreview";
            videoPreviewHost.Size = new Size(553, 544);
            videoPreviewHost.TabIndex = 7;
            // 
            // conversionPanel
            // 
            conversionPanel.ColumnCount = 1;
            conversionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            conversionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            conversionPanel.Controls.Add(settingsGroupBox, 0, 0);
            conversionPanel.Controls.Add(commandLinePaddingPanel, 0, 1);
            conversionPanel.Dock = DockStyle.Fill;
            conversionPanel.Location = new Point(0, 0);
            conversionPanel.Name = "conversionPanel";
            conversionPanel.RowCount = 2;
            conversionPanel.RowStyles.Add(new RowStyle());
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            conversionPanel.Size = new Size(412, 640);
            conversionPanel.TabIndex = 7;
            // 
            // commandLinePaddingPanel
            // 
            commandLinePaddingPanel.BackColor = SystemColors.Window;
            commandLinePaddingPanel.Controls.Add(commandLineOutput);
            commandLinePaddingPanel.Dock = DockStyle.Fill;
            commandLinePaddingPanel.Location = new Point(3, 452);
            commandLinePaddingPanel.Name = "commandLinePaddingPanel";
            commandLinePaddingPanel.Size = new Size(406, 185);
            commandLinePaddingPanel.TabIndex = 9;
            // 
            // actionsPanel
            // 
            actionsPanel.Controls.Add(cancelButton);
            actionsPanel.Controls.Add(convertButton);
            actionsPanel.Dock = DockStyle.Bottom;
            actionsPanel.Location = new Point(0, 658);
            actionsPanel.Name = "actionsPanel";
            actionsPanel.Size = new Size(1002, 54);
            actionsPanel.TabIndex = 8;
            // 
            // Main
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1002, 712);
            Controls.Add(actionsPanel);
            Controls.Add(mainSplit);
            HelpButton = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(740, 700);
            Name = "Main";
            Text = "Fast Animated Image Converter";
            Load += Main_Load;
            ((System.ComponentModel.ISupportInitialize)qualitySlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)settingsBindingSource).EndInit();
            settingsGroupBox.ResumeLayout(false);
            settingsGroupBox.PerformLayout();
            settingsContainerPanel.ResumeLayout(false);
            settingsContainerPanel.PerformLayout();
            settingsTable.ResumeLayout(false);
            settingsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)resizeSlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)fpsValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)repeatValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)lastFrameInput).EndInit();
            trimColumnLayoutPanel.ResumeLayout(false);
            trimColumnLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)firstFrameInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)resizeDimensionValue).EndInit();
            samplingLayoutPanel.ResumeLayout(false);
            samplingLayoutPanel.PerformLayout();
            mainSplit.Panel1.ResumeLayout(false);
            mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplit).EndInit();
            mainSplit.ResumeLayout(false);
            previewPanel.ResumeLayout(false);
            playbackConsoleTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)playhead).EndInit();
            videoContainingPanel.ResumeLayout(false);
            conversionPanel.ResumeLayout(false);
            conversionPanel.PerformLayout();
            commandLinePaddingPanel.ResumeLayout(false);
            actionsPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TrackBar qualitySlider;
        private Button convertButton;
        private Label qualityLabel;
        private SaveFileDialog saveFileDialogue;
        private Button cancelButton;
        private RichTextBox commandLineOutput;
        private GroupBox settingsGroupBox;
        private SplitContainer mainSplit;
        private Panel previewPanel;
        private Panel videoContainingPanel;
        private Panel actionsPanel;
        private TrackBar playhead;
        private ElementHost videoPreviewHost;
        private NumericUpDown firstFrameInput;
        private Button trimStartHereButton;
        private Button trimEndHereButton;
        private Label trimLabel;
        private Button playButton;
        private Button seekButton;
        private TableLayoutPanel playbackConsoleTable;
        private TableLayoutPanel settingsTable;
        private TableLayoutPanel conversionPanel;
        private Panel settingsContainerPanel;
        private BindingSource settingsBindingSource;
        private Label fpsLabel;
        private NumericUpDown fpsValue;
        private ComboBox fpsSetting;
        private TrackBar resizeSlider;
        private Label resizeLabel;
        private Label resizeDimensionLabel;
        private NumericUpDown resizeDimensionValue;
        private ToolTip tooltips;
        private Label repeatsLabel;
        private Types.Forms.InfinityNumericUpDown repeatValue;
        private TrackBar speedSlider;
        private Label speedLabel;
        private Label lastFrameLabel;
        private NumericUpDown lastFrameInput;
        private Label firstFrame;
        private TableLayoutPanel trimColumnLayoutPanel;
        private Panel fpsSpacerPanel;
        private Label label1;
        private TableLayoutPanel samplingLayoutPanel;
        private RadioButton sampleBestRadio;
        private RadioButton sampleFastRadio;
        private Panel commandLinePaddingPanel;
        private Button reverseSeekButton;
    }
}
