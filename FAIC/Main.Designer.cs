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
            settingsTable = new TableLayoutPanel();
            resizeLabel = new Label();
            resizeSlider = new TrackBar();
            resizeDimensionLabel = new Label();
            resizeDimensionContainer = new Panel();
            resizeDimensionValue = new NumericUpDown();
            samplingLabel = new Label();
            samplingLayoutPanel = new TableLayoutPanel();
            sampleFastRadio = new RadioButton();
            sampleBestRadio = new RadioButton();
            settingsDivider1 = new Panel();
            trimColumnLayoutPanel = new TableLayoutPanel();
            trimLabel = new Label();
            firstFrame = new Label();
            firstFrameContainer = new Panel();
            firstFrameInput = new NumericUpDown();
            lastFrameLabel = new Label();
            lastFrameContainer = new Panel();
            lastFrameInput = new NumericUpDown();
            settingsDivider2 = new Panel();
            speedLabel = new Label();
            speedSlider = new TrackBar();
            fpsLabel = new Label();
            fpsSetting = new ComboBox();
            fpsSpacerPanel = new Panel();
            fpsContainer = new Panel();
            fpsValue = new NumericUpDown();
            repeatsLabel = new Label();
            repeatContainer = new Panel();
            repeatValue = new FAIC.Types.Forms.InfinityNumericUpDown();
            settingsDivider3 = new Panel();
            transparentLabel = new Label();
            transparentCheckbox = new CheckBox();
            settingsDivider4 = new Panel();
            advancedLabel = new Label();
            editArgumentsCheckbox = new CheckBox();
            mainSplit = new SplitContainer();
            videoPanelTable = new TableLayoutPanel();
            videoContainingPanel = new Panel();
            videoPreviewHost = new ElementHost();
            previewControlsPanel = new TableLayoutPanel();
            trimStartHereButton = new Button();
            playhead = new TrackBar();
            trimEndHereButton = new Button();
            playbackConsoleTable = new TableLayoutPanel();
            reverseSeekButton = new Button();
            playButton = new Button();
            seekButton = new Button();
            conversionPanel = new TableLayoutPanel();
            commandLinePaddingPanel = new Panel();
            actionsPanel = new Panel();
            tooltips = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)qualitySlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)settingsBindingSource).BeginInit();
            settingsGroupBox.SuspendLayout();
            settingsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resizeSlider).BeginInit();
            resizeDimensionContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resizeDimensionValue).BeginInit();
            samplingLayoutPanel.SuspendLayout();
            trimColumnLayoutPanel.SuspendLayout();
            firstFrameContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)firstFrameInput).BeginInit();
            lastFrameContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)lastFrameInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).BeginInit();
            fpsContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fpsValue).BeginInit();
            repeatContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)repeatValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)mainSplit).BeginInit();
            mainSplit.Panel1.SuspendLayout();
            mainSplit.Panel2.SuspendLayout();
            mainSplit.SuspendLayout();
            videoPanelTable.SuspendLayout();
            videoContainingPanel.SuspendLayout();
            previewControlsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)playhead).BeginInit();
            playbackConsoleTable.SuspendLayout();
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
            qualitySlider.Location = new Point(100, 2);
            qualitySlider.Margin = new Padding(2);
            qualitySlider.Maximum = 100;
            qualitySlider.Name = "qualitySlider";
            qualitySlider.Size = new Size(216, 19);
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
            convertButton.Location = new Point(2, 2);
            convertButton.Margin = new Padding(2);
            convertButton.Name = "convertButton";
            convertButton.Size = new Size(706, 29);
            convertButton.TabIndex = 0;
            convertButton.Text = "Convert To...";
            tooltips.SetToolTip(convertButton, "Begin conversion with current settings. Select desired format in the \"Save To\" dialogue after pressing this button.");
            convertButton.UseVisualStyleBackColor = true;
            convertButton.Click += convertButton_Click;
            // 
            // qualityLabel
            // 
            qualityLabel.AutoSize = true;
            qualityLabel.Dock = DockStyle.Fill;
            qualityLabel.Location = new Point(2, 0);
            qualityLabel.Margin = new Padding(2, 0, 2, 0);
            qualityLabel.Name = "qualityLabel";
            qualityLabel.Size = new Size(94, 23);
            qualityLabel.TabIndex = 0;
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
            cancelButton.Location = new Point(712, 2);
            cancelButton.Margin = new Padding(2);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(70, 29);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancel";
            tooltips.SetToolTip(cancelButton, "Cancel the current conversion.");
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // commandLineOutput
            // 
            commandLineOutput.BackColor = SystemColors.Window;
            commandLineOutput.BorderStyle = BorderStyle.None;
            commandLineOutput.Dock = DockStyle.Fill;
            commandLineOutput.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            commandLineOutput.Location = new Point(6, 6);
            commandLineOutput.Margin = new Padding(2);
            commandLineOutput.Name = "commandLineOutput";
            commandLineOutput.ReadOnly = true;
            commandLineOutput.Size = new Size(310, 120);
            commandLineOutput.TabIndex = 0;
            commandLineOutput.Text = "";
            tooltips.SetToolTip(commandLineOutput, "Console output from the converter tools.");
            commandLineOutput.WordWrap = false;
            // 
            // settingsGroupBox
            // 
            settingsGroupBox.AutoSize = true;
            settingsGroupBox.Controls.Add(settingsTable);
            settingsGroupBox.Dock = DockStyle.Top;
            settingsGroupBox.Enabled = false;
            settingsGroupBox.Location = new Point(2, 2);
            settingsGroupBox.Margin = new Padding(2, 2, 2, 0);
            settingsGroupBox.Name = "settingsGroupBox";
            settingsGroupBox.Padding = new Padding(2);
            settingsGroupBox.Size = new Size(322, 380);
            settingsGroupBox.TabIndex = 0;
            settingsGroupBox.TabStop = false;
            settingsGroupBox.Text = "Settings";
            // 
            // settingsTable
            // 
            settingsTable.AutoSize = true;
            settingsTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            settingsTable.ColumnCount = 2;
            settingsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 98F));
            settingsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            settingsTable.Controls.Add(qualityLabel, 0, 0);
            settingsTable.Controls.Add(qualitySlider, 1, 0);
            settingsTable.Controls.Add(resizeLabel, 0, 1);
            settingsTable.Controls.Add(resizeSlider, 1, 1);
            settingsTable.Controls.Add(resizeDimensionLabel, 0, 2);
            settingsTable.Controls.Add(resizeDimensionContainer, 1, 2);
            settingsTable.Controls.Add(samplingLabel, 0, 3);
            settingsTable.Controls.Add(samplingLayoutPanel, 1, 3);
            settingsTable.Controls.Add(settingsDivider1, 0, 4);
            settingsTable.Controls.Add(trimColumnLayoutPanel, 0, 5);
            settingsTable.Controls.Add(firstFrameContainer, 1, 5);
            settingsTable.Controls.Add(lastFrameLabel, 0, 6);
            settingsTable.Controls.Add(lastFrameContainer, 1, 6);
            settingsTable.Controls.Add(settingsDivider2, 0, 7);
            settingsTable.Controls.Add(speedLabel, 0, 8);
            settingsTable.Controls.Add(speedSlider, 1, 8);
            settingsTable.Controls.Add(fpsLabel, 0, 9);
            settingsTable.Controls.Add(fpsSetting, 1, 9);
            settingsTable.Controls.Add(fpsSpacerPanel, 0, 10);
            settingsTable.Controls.Add(fpsContainer, 1, 10);
            settingsTable.Controls.Add(repeatsLabel, 0, 11);
            settingsTable.Controls.Add(repeatContainer, 1, 11);
            settingsTable.Controls.Add(settingsDivider3, 0, 12);
            settingsTable.Controls.Add(transparentLabel, 0, 13);
            settingsTable.Controls.Add(transparentCheckbox, 1, 13);
            settingsTable.Controls.Add(settingsDivider4, 0, 14);
            settingsTable.Controls.Add(advancedLabel, 0, 15);
            settingsTable.Controls.Add(editArgumentsCheckbox, 1, 15);
            settingsTable.Dock = DockStyle.Fill;
            settingsTable.Location = new Point(2, 18);
            settingsTable.Margin = new Padding(2, 2, 2, 0);
            settingsTable.Name = "settingsTable";
            settingsTable.RowCount = 16;
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.RowStyles.Add(new RowStyle());
            settingsTable.Size = new Size(318, 360);
            settingsTable.TabIndex = 0;
            // 
            // resizeLabel
            // 
            resizeLabel.AutoSize = true;
            resizeLabel.Dock = DockStyle.Fill;
            resizeLabel.Location = new Point(2, 23);
            resizeLabel.Margin = new Padding(2, 0, 2, 0);
            resizeLabel.Name = "resizeLabel";
            resizeLabel.Size = new Size(94, 23);
            resizeLabel.TabIndex = 2;
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
            resizeSlider.Location = new Point(100, 25);
            resizeSlider.Margin = new Padding(2);
            resizeSlider.Maximum = 100;
            resizeSlider.Name = "resizeSlider";
            resizeSlider.Size = new Size(216, 19);
            resizeSlider.TabIndex = 3;
            resizeSlider.TickFrequency = 10;
            tooltips.SetToolTip(resizeSlider, "The percentage to decrease the output resolution by. Lower values will help improve performance and decrease file size.\r\n");
            resizeSlider.Value = 100;
            resizeSlider.Scroll += resizeSlider_Scroll;
            // 
            // resizeDimensionLabel
            // 
            resizeDimensionLabel.Dock = DockStyle.Fill;
            resizeDimensionLabel.Location = new Point(2, 46);
            resizeDimensionLabel.Margin = new Padding(2, 0, 2, 0);
            resizeDimensionLabel.MinimumSize = new Size(45, 0);
            resizeDimensionLabel.Name = "resizeDimensionLabel";
            resizeDimensionLabel.Size = new Size(94, 27);
            resizeDimensionLabel.TabIndex = 4;
            resizeDimensionLabel.Text = "Height";
            resizeDimensionLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // resizeDimensionContainer
            // 
            resizeDimensionContainer.AutoSize = true;
            resizeDimensionContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            resizeDimensionContainer.Controls.Add(resizeDimensionValue);
            resizeDimensionContainer.Dock = DockStyle.Fill;
            resizeDimensionContainer.Location = new Point(101, 49);
            resizeDimensionContainer.Name = "resizeDimensionContainer";
            resizeDimensionContainer.Size = new Size(214, 21);
            resizeDimensionContainer.TabIndex = 5;
            // 
            // resizeDimensionValue
            // 
            resizeDimensionValue.AccessibleName = "Resize Input";
            resizeDimensionValue.Dock = DockStyle.Fill;
            resizeDimensionValue.Location = new Point(0, 0);
            resizeDimensionValue.Margin = new Padding(2);
            resizeDimensionValue.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
            resizeDimensionValue.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            resizeDimensionValue.Name = "resizeDimensionValue";
            resizeDimensionValue.Size = new Size(214, 23);
            resizeDimensionValue.TabIndex = 0;
            tooltips.SetToolTip(resizeDimensionValue, "The desired output resolution in pixels of the largest dimension. The other dimension will be rescaled as well to preserve the aspect ratio.");
            resizeDimensionValue.Value = new decimal(new int[] { 1, 0, 0, 0 });
            resizeDimensionValue.ValueChanged += resizeDimensionValue_ValueChanged;
            // 
            // samplingLabel
            // 
            samplingLabel.AutoSize = true;
            samplingLabel.Dock = DockStyle.Fill;
            samplingLabel.Location = new Point(2, 73);
            samplingLabel.Margin = new Padding(2, 0, 2, 0);
            samplingLabel.MinimumSize = new Size(45, 0);
            samplingLabel.Name = "samplingLabel";
            samplingLabel.Size = new Size(94, 27);
            samplingLabel.TabIndex = 6;
            samplingLabel.Text = "Sampling";
            samplingLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // samplingLayoutPanel
            // 
            samplingLayoutPanel.AutoSize = true;
            samplingLayoutPanel.ColumnCount = 2;
            samplingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            samplingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            samplingLayoutPanel.Controls.Add(sampleFastRadio, 0, 0);
            samplingLayoutPanel.Controls.Add(sampleBestRadio, 1, 0);
            samplingLayoutPanel.Dock = DockStyle.Fill;
            samplingLayoutPanel.Enabled = false;
            samplingLayoutPanel.Location = new Point(100, 75);
            samplingLayoutPanel.Margin = new Padding(2);
            samplingLayoutPanel.Name = "samplingLayoutPanel";
            samplingLayoutPanel.RowCount = 1;
            samplingLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            samplingLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            samplingLayoutPanel.Size = new Size(216, 23);
            samplingLayoutPanel.TabIndex = 7;
            // 
            // sampleFastRadio
            // 
            sampleFastRadio.AccessibleName = "Fast resampling algorithm button";
            sampleFastRadio.AutoSize = true;
            sampleFastRadio.Checked = true;
            sampleFastRadio.Location = new Point(2, 2);
            sampleFastRadio.Margin = new Padding(2);
            sampleFastRadio.Name = "sampleFastRadio";
            sampleFastRadio.Size = new Size(46, 19);
            sampleFastRadio.TabIndex = 0;
            sampleFastRadio.TabStop = true;
            sampleFastRadio.Text = "Fast";
            tooltips.SetToolTip(sampleFastRadio, "Use billinear for resizing. Fast, but can lead to aliasing and shimmer in motion at lower resolutions, and worse contrast at higher resolutions.");
            sampleFastRadio.UseVisualStyleBackColor = true;
            // 
            // sampleBestRadio
            // 
            sampleBestRadio.AccessibleName = "Best resampling algorithm button";
            sampleBestRadio.AutoSize = true;
            sampleBestRadio.Location = new Point(110, 2);
            sampleBestRadio.Margin = new Padding(2);
            sampleBestRadio.Name = "sampleBestRadio";
            sampleBestRadio.Size = new Size(47, 19);
            sampleBestRadio.TabIndex = 1;
            sampleBestRadio.Text = "Best";
            tooltips.SetToolTip(sampleBestRadio, "Use Lanczos for resizing to smaller, and Spline36 for resizing to larger. Best quality, but slower.");
            sampleBestRadio.UseVisualStyleBackColor = true;
            // 
            // settingsDivider1
            // 
            settingsDivider1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            settingsTable.SetColumnSpan(settingsDivider1, 2);
            settingsDivider1.Location = new Point(2, 102);
            settingsDivider1.Margin = new Padding(2);
            settingsDivider1.MaximumSize = new Size(0, 8);
            settingsDivider1.MinimumSize = new Size(0, 8);
            settingsDivider1.Name = "settingsDivider1";
            settingsDivider1.Size = new Size(314, 8);
            settingsDivider1.TabIndex = 8;
            // 
            // trimColumnLayoutPanel
            // 
            trimColumnLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trimColumnLayoutPanel.AutoSize = true;
            trimColumnLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            trimColumnLayoutPanel.ColumnCount = 2;
            trimColumnLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            trimColumnLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            trimColumnLayoutPanel.Controls.Add(trimLabel, 0, 0);
            trimColumnLayoutPanel.Controls.Add(firstFrame, 1, 0);
            trimColumnLayoutPanel.Location = new Point(0, 112);
            trimColumnLayoutPanel.Margin = new Padding(0);
            trimColumnLayoutPanel.Name = "trimColumnLayoutPanel";
            trimColumnLayoutPanel.RowCount = 1;
            trimColumnLayoutPanel.RowStyles.Add(new RowStyle());
            trimColumnLayoutPanel.Size = new Size(98, 29);
            trimColumnLayoutPanel.TabIndex = 9;
            // 
            // trimLabel
            // 
            trimLabel.AutoSize = true;
            trimLabel.Dock = DockStyle.Fill;
            trimLabel.Location = new Point(2, 0);
            trimLabel.Margin = new Padding(2, 0, 2, 0);
            trimLabel.Name = "trimLabel";
            trimLabel.Size = new Size(31, 29);
            trimLabel.TabIndex = 6;
            trimLabel.Text = "Trim";
            trimLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // firstFrame
            // 
            firstFrame.AutoSize = true;
            firstFrame.Dock = DockStyle.Fill;
            firstFrame.Location = new Point(37, 0);
            firstFrame.Margin = new Padding(2, 0, 2, 0);
            firstFrame.MinimumSize = new Size(45, 0);
            firstFrame.Name = "firstFrame";
            firstFrame.Size = new Size(59, 29);
            firstFrame.TabIndex = 7;
            firstFrame.Text = "Start";
            firstFrame.TextAlign = ContentAlignment.MiddleRight;
            // 
            // firstFrameContainer
            // 
            firstFrameContainer.AutoSize = true;
            firstFrameContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            firstFrameContainer.Controls.Add(firstFrameInput);
            firstFrameContainer.Dock = DockStyle.Fill;
            firstFrameContainer.Location = new Point(101, 115);
            firstFrameContainer.Name = "firstFrameContainer";
            firstFrameContainer.Size = new Size(214, 23);
            firstFrameContainer.TabIndex = 10;
            // 
            // firstFrameInput
            // 
            firstFrameInput.AccessibleName = "Trim start of the video to X seconds input";
            firstFrameInput.AutoSize = true;
            firstFrameInput.DecimalPlaces = 3;
            firstFrameInput.Dock = DockStyle.Fill;
            firstFrameInput.Location = new Point(0, 0);
            firstFrameInput.Margin = new Padding(2);
            firstFrameInput.Name = "firstFrameInput";
            firstFrameInput.Size = new Size(214, 23);
            firstFrameInput.TabIndex = 0;
            tooltips.SetToolTip(firstFrameInput, "Where to start the converted output relative to the source media in seconds.\r\n\r\n");
            firstFrameInput.ValueChanged += firstFrameInput_ValueChanged;
            // 
            // lastFrameLabel
            // 
            lastFrameLabel.AutoSize = true;
            lastFrameLabel.Dock = DockStyle.Fill;
            lastFrameLabel.Location = new Point(2, 141);
            lastFrameLabel.Margin = new Padding(2, 0, 2, 0);
            lastFrameLabel.MinimumSize = new Size(45, 0);
            lastFrameLabel.Name = "lastFrameLabel";
            lastFrameLabel.Size = new Size(94, 29);
            lastFrameLabel.TabIndex = 11;
            lastFrameLabel.Text = "End";
            lastFrameLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lastFrameContainer
            // 
            lastFrameContainer.AutoSize = true;
            lastFrameContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            lastFrameContainer.Controls.Add(lastFrameInput);
            lastFrameContainer.Dock = DockStyle.Fill;
            lastFrameContainer.Location = new Point(101, 144);
            lastFrameContainer.Name = "lastFrameContainer";
            lastFrameContainer.Size = new Size(214, 23);
            lastFrameContainer.TabIndex = 12;
            // 
            // lastFrameInput
            // 
            lastFrameInput.AccessibleName = "Trim end of the video to X seconds input";
            lastFrameInput.AutoSize = true;
            lastFrameInput.DecimalPlaces = 3;
            lastFrameInput.Dock = DockStyle.Fill;
            lastFrameInput.Location = new Point(0, 0);
            lastFrameInput.Margin = new Padding(2);
            lastFrameInput.Name = "lastFrameInput";
            lastFrameInput.Size = new Size(214, 23);
            lastFrameInput.TabIndex = 0;
            tooltips.SetToolTip(lastFrameInput, "Where to end the converted output relative to the source media in seconds.\r\n");
            // 
            // settingsDivider2
            // 
            settingsDivider2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            settingsTable.SetColumnSpan(settingsDivider2, 2);
            settingsDivider2.Location = new Point(2, 172);
            settingsDivider2.Margin = new Padding(2);
            settingsDivider2.MaximumSize = new Size(0, 8);
            settingsDivider2.MinimumSize = new Size(0, 8);
            settingsDivider2.Name = "settingsDivider2";
            settingsDivider2.Size = new Size(314, 8);
            settingsDivider2.TabIndex = 13;
            // 
            // speedLabel
            // 
            speedLabel.AutoSize = true;
            speedLabel.Dock = DockStyle.Fill;
            speedLabel.Location = new Point(2, 182);
            speedLabel.Margin = new Padding(2, 0, 2, 0);
            speedLabel.Name = "speedLabel";
            speedLabel.Size = new Size(94, 23);
            speedLabel.TabIndex = 14;
            speedLabel.Text = "Speed (1.00x)";
            speedLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // speedSlider
            // 
            speedSlider.AccessibleName = "Speed Slider";
            speedSlider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            speedSlider.AutoSize = false;
            speedSlider.DataBindings.Add(new Binding("DataContext", settingsBindingSource, "Quality", true));
            speedSlider.DataBindings.Add(new Binding("Value", settingsBindingSource, "Quality", true, DataSourceUpdateMode.OnPropertyChanged));
            speedSlider.LargeChange = 1;
            speedSlider.Location = new Point(100, 184);
            speedSlider.Margin = new Padding(2);
            speedSlider.Maximum = 16;
            speedSlider.Name = "speedSlider";
            speedSlider.Size = new Size(216, 19);
            speedSlider.TabIndex = 15;
            tooltips.SetToolTip(speedSlider, "The percentage to decrease the output resolution by. Lower values will help improve performance and decrease file size.\r\n");
            speedSlider.Value = 5;
            speedSlider.Scroll += speedSlider_Scroll;
            // 
            // fpsLabel
            // 
            fpsLabel.AutoSize = true;
            fpsLabel.Dock = DockStyle.Fill;
            fpsLabel.Location = new Point(2, 205);
            fpsLabel.Margin = new Padding(2, 0, 2, 0);
            fpsLabel.Name = "fpsLabel";
            fpsLabel.Size = new Size(94, 27);
            fpsLabel.TabIndex = 16;
            fpsLabel.Text = "Frame Rate";
            fpsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // fpsSetting
            // 
            fpsSetting.AccessibleName = "Frame rate mode dropdown";
            fpsSetting.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fpsSetting.FormattingEnabled = true;
            fpsSetting.Items.AddRange(new object[] { "Same", "Nearest", "Blended" });
            fpsSetting.Location = new Point(100, 207);
            fpsSetting.Margin = new Padding(2);
            fpsSetting.MaxDropDownItems = 3;
            fpsSetting.Name = "fpsSetting";
            fpsSetting.Size = new Size(216, 23);
            fpsSetting.TabIndex = 17;
            tooltips.SetToolTip(fpsSetting, resources.GetString("fpsSetting.ToolTip"));
            fpsSetting.SelectedIndexChanged += fpsSetting_SelectedIndexChanged;
            // 
            // fpsSpacerPanel
            // 
            fpsSpacerPanel.AutoSize = true;
            fpsSpacerPanel.Dock = DockStyle.Fill;
            fpsSpacerPanel.Location = new Point(2, 234);
            fpsSpacerPanel.Margin = new Padding(2);
            fpsSpacerPanel.Name = "fpsSpacerPanel";
            fpsSpacerPanel.Size = new Size(94, 25);
            fpsSpacerPanel.TabIndex = 18;
            // 
            // fpsContainer
            // 
            fpsContainer.AutoSize = true;
            fpsContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            fpsContainer.Controls.Add(fpsValue);
            fpsContainer.Dock = DockStyle.Fill;
            fpsContainer.Location = new Point(101, 235);
            fpsContainer.Name = "fpsContainer";
            fpsContainer.Size = new Size(214, 23);
            fpsContainer.TabIndex = 19;
            // 
            // fpsValue
            // 
            fpsValue.AccessibleName = "Target frame rate input";
            fpsValue.AutoSize = true;
            fpsValue.DecimalPlaces = 3;
            fpsValue.Dock = DockStyle.Fill;
            fpsValue.Enabled = false;
            fpsValue.Location = new Point(0, 0);
            fpsValue.Margin = new Padding(2);
            fpsValue.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            fpsValue.Name = "fpsValue";
            fpsValue.Size = new Size(214, 23);
            fpsValue.TabIndex = 0;
            tooltips.SetToolTip(fpsValue, "Target frame rate of the converted output.");
            // 
            // repeatsLabel
            // 
            repeatsLabel.AutoSize = true;
            repeatsLabel.Dock = DockStyle.Fill;
            repeatsLabel.Location = new Point(2, 261);
            repeatsLabel.Margin = new Padding(2, 0, 2, 0);
            repeatsLabel.Name = "repeatsLabel";
            repeatsLabel.Size = new Size(94, 29);
            repeatsLabel.TabIndex = 20;
            repeatsLabel.Text = "Repeats";
            repeatsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // repeatContainer
            // 
            repeatContainer.AutoSize = true;
            repeatContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            repeatContainer.Controls.Add(repeatValue);
            repeatContainer.Dock = DockStyle.Fill;
            repeatContainer.Location = new Point(101, 264);
            repeatContainer.Name = "repeatContainer";
            repeatContainer.Size = new Size(214, 23);
            repeatContainer.TabIndex = 21;
            // 
            // repeatValue
            // 
            repeatValue.AccessibleName = "Repeat count input";
            repeatValue.AutoSize = true;
            repeatValue.Dock = DockStyle.Fill;
            repeatValue.Location = new Point(0, 0);
            repeatValue.Margin = new Padding(2);
            repeatValue.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            repeatValue.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            repeatValue.Name = "repeatValue";
            repeatValue.Size = new Size(214, 23);
            repeatValue.TabIndex = 0;
            tooltips.SetToolTip(repeatValue, resources.GetString("repeatValue.ToolTip"));
            // 
            // settingsDivider3
            // 
            settingsDivider3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            settingsTable.SetColumnSpan(settingsDivider3, 2);
            settingsDivider3.Location = new Point(2, 292);
            settingsDivider3.Margin = new Padding(2);
            settingsDivider3.MaximumSize = new Size(0, 8);
            settingsDivider3.MinimumSize = new Size(0, 8);
            settingsDivider3.Name = "settingsDivider3";
            settingsDivider3.Size = new Size(314, 8);
            settingsDivider3.TabIndex = 22;
            // 
            // transparentLabel
            // 
            transparentLabel.AutoSize = true;
            transparentLabel.Dock = DockStyle.Fill;
            transparentLabel.Location = new Point(2, 302);
            transparentLabel.Margin = new Padding(2, 0, 2, 0);
            transparentLabel.MinimumSize = new Size(45, 0);
            transparentLabel.Name = "transparentLabel";
            transparentLabel.Size = new Size(94, 23);
            transparentLabel.TabIndex = 23;
            transparentLabel.Text = "Format";
            transparentLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // transparentCheckbox
            // 
            transparentCheckbox.AutoSize = true;
            transparentCheckbox.Dock = DockStyle.Fill;
            transparentCheckbox.Location = new Point(100, 304);
            transparentCheckbox.Margin = new Padding(2);
            transparentCheckbox.Name = "transparentCheckbox";
            transparentCheckbox.Size = new Size(216, 19);
            transparentCheckbox.TabIndex = 24;
            transparentCheckbox.Text = "Transparent";
            transparentCheckbox.UseVisualStyleBackColor = true;
            // 
            // settingsDivider4
            // 
            settingsDivider4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            settingsTable.SetColumnSpan(settingsDivider4, 2);
            settingsDivider4.Location = new Point(2, 327);
            settingsDivider4.Margin = new Padding(2);
            settingsDivider4.MaximumSize = new Size(0, 8);
            settingsDivider4.MinimumSize = new Size(0, 8);
            settingsDivider4.Name = "settingsDivider4";
            settingsDivider4.Size = new Size(314, 8);
            settingsDivider4.TabIndex = 25;
            // 
            // advancedLabel
            // 
            advancedLabel.AutoSize = true;
            advancedLabel.Dock = DockStyle.Fill;
            advancedLabel.Location = new Point(2, 337);
            advancedLabel.Margin = new Padding(2, 0, 2, 0);
            advancedLabel.MinimumSize = new Size(45, 0);
            advancedLabel.Name = "advancedLabel";
            advancedLabel.Size = new Size(94, 23);
            advancedLabel.TabIndex = 26;
            advancedLabel.Text = "Advanced";
            advancedLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // editArgumentsCheckbox
            // 
            editArgumentsCheckbox.AutoSize = true;
            editArgumentsCheckbox.Dock = DockStyle.Fill;
            editArgumentsCheckbox.Location = new Point(100, 339);
            editArgumentsCheckbox.Margin = new Padding(2);
            editArgumentsCheckbox.Name = "editArgumentsCheckbox";
            editArgumentsCheckbox.Size = new Size(216, 19);
            editArgumentsCheckbox.TabIndex = 27;
            editArgumentsCheckbox.Text = "Edit Arguments";
            editArgumentsCheckbox.UseVisualStyleBackColor = true;
            // 
            // mainSplit
            // 
            mainSplit.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mainSplit.Location = new Point(8, 7);
            mainSplit.Margin = new Padding(2);
            mainSplit.Name = "mainSplit";
            // 
            // mainSplit.Panel1
            // 
            mainSplit.Panel1.Controls.Add(videoPanelTable);
            mainSplit.Panel1MinSize = 200;
            // 
            // mainSplit.Panel2
            // 
            mainSplit.Panel2.Controls.Add(conversionPanel);
            mainSplit.Panel2MinSize = 320;
            mainSplit.Size = new Size(767, 518);
            mainSplit.SplitterDistance = 438;
            mainSplit.SplitterWidth = 3;
            mainSplit.TabIndex = 0;
            mainSplit.SplitterMoved += mainSplit_SplitterMoved;
            // 
            // videoPanelTable
            // 
            videoPanelTable.ColumnCount = 1;
            videoPanelTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            videoPanelTable.Controls.Add(videoContainingPanel, 0, 0);
            videoPanelTable.Controls.Add(previewControlsPanel, 0, 1);
            videoPanelTable.Dock = DockStyle.Fill;
            videoPanelTable.Location = new Point(0, 0);
            videoPanelTable.Name = "videoPanelTable";
            videoPanelTable.RowCount = 2;
            videoPanelTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            videoPanelTable.RowStyles.Add(new RowStyle());
            videoPanelTable.Size = new Size(438, 518);
            videoPanelTable.TabIndex = 0;
            // 
            // videoContainingPanel
            // 
            videoContainingPanel.Controls.Add(videoPreviewHost);
            videoContainingPanel.Dock = DockStyle.Fill;
            videoContainingPanel.Location = new Point(2, 2);
            videoContainingPanel.Margin = new Padding(2);
            videoContainingPanel.Name = "videoContainingPanel";
            videoContainingPanel.Size = new Size(434, 451);
            videoContainingPanel.TabIndex = 0;
            // 
            // videoPreviewHost
            // 
            videoPreviewHost.Dock = DockStyle.Fill;
            videoPreviewHost.Location = new Point(0, 0);
            videoPreviewHost.Margin = new Padding(2);
            videoPreviewHost.Name = "videoPreview";
            videoPreviewHost.Size = new Size(434, 451);
            videoPreviewHost.TabIndex = 0;
            // 
            // previewControlsPanel
            // 
            previewControlsPanel.AutoSize = true;
            previewControlsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            previewControlsPanel.ColumnCount = 3;
            previewControlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            previewControlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            previewControlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            previewControlsPanel.Controls.Add(trimStartHereButton, 0, 0);
            previewControlsPanel.Controls.Add(playhead, 1, 0);
            previewControlsPanel.Controls.Add(trimEndHereButton, 2, 0);
            previewControlsPanel.Controls.Add(playbackConsoleTable, 1, 1);
            previewControlsPanel.Dock = DockStyle.Bottom;
            previewControlsPanel.Location = new Point(3, 458);
            previewControlsPanel.Name = "previewControlsPanel";
            previewControlsPanel.RowCount = 2;
            previewControlsPanel.RowStyles.Add(new RowStyle());
            previewControlsPanel.RowStyles.Add(new RowStyle());
            previewControlsPanel.Size = new Size(432, 57);
            previewControlsPanel.TabIndex = 1;
            // 
            // trimStartHereButton
            // 
            trimStartHereButton.AccessibleName = "Trim start of output to current playhead";
            trimStartHereButton.AutoSize = true;
            trimStartHereButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            trimStartHereButton.Dock = DockStyle.Fill;
            trimStartHereButton.Location = new Point(2, 2);
            trimStartHereButton.Margin = new Padding(2);
            trimStartHereButton.Name = "trimStartHereButton";
            previewControlsPanel.SetRowSpan(trimStartHereButton, 2);
            trimStartHereButton.Size = new Size(26, 53);
            trimStartHereButton.TabIndex = 0;
            trimStartHereButton.Text = "[";
            tooltips.SetToolTip(trimStartHereButton, "Set \"Start Time\" to current playback position.\r\n");
            trimStartHereButton.UseVisualStyleBackColor = true;
            trimStartHereButton.Click += trimStartHereButton_Click;
            // 
            // playhead
            // 
            playhead.AccessibleName = "Playhead/timeline";
            playhead.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            playhead.AutoSize = false;
            playhead.BackColor = SystemColors.Control;
            playhead.LargeChange = 10000;
            playhead.Location = new Point(32, 2);
            playhead.Margin = new Padding(2);
            playhead.Maximum = 1000;
            playhead.Name = "playhead";
            playhead.Size = new Size(368, 24);
            playhead.SmallChange = 1000;
            playhead.TabIndex = 1;
            playhead.TickFrequency = 1000;
            tooltips.SetToolTip(playhead, "Timeline with playhead to scrub through the media. Playhead freezes during playback.");
            playhead.Scroll += playhead_Scroll;
            // 
            // trimEndHereButton
            // 
            trimEndHereButton.AccessibleName = "Trim end of output to current playhead";
            trimEndHereButton.AutoSize = true;
            trimEndHereButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            trimEndHereButton.Dock = DockStyle.Fill;
            trimEndHereButton.Location = new Point(404, 2);
            trimEndHereButton.Margin = new Padding(2);
            trimEndHereButton.Name = "trimEndHereButton";
            previewControlsPanel.SetRowSpan(trimEndHereButton, 2);
            trimEndHereButton.Size = new Size(26, 53);
            trimEndHereButton.TabIndex = 2;
            trimEndHereButton.Text = "]";
            tooltips.SetToolTip(trimEndHereButton, "Set \"End Time\" to current playback position.");
            trimEndHereButton.UseVisualStyleBackColor = true;
            trimEndHereButton.Click += trimEndHereButton_Click;
            // 
            // playbackConsoleTable
            // 
            playbackConsoleTable.AutoSize = true;
            playbackConsoleTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            playbackConsoleTable.ColumnCount = 3;
            playbackConsoleTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3290024F));
            playbackConsoleTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.332F));
            playbackConsoleTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.339F));
            playbackConsoleTable.Controls.Add(reverseSeekButton, 0, 0);
            playbackConsoleTable.Controls.Add(playButton, 1, 0);
            playbackConsoleTable.Controls.Add(seekButton, 2, 0);
            playbackConsoleTable.Dock = DockStyle.Fill;
            playbackConsoleTable.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
            playbackConsoleTable.Location = new Point(30, 30);
            playbackConsoleTable.Margin = new Padding(0, 2, 0, 2);
            playbackConsoleTable.Name = "playbackConsoleTable";
            playbackConsoleTable.RowCount = 1;
            playbackConsoleTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            playbackConsoleTable.Size = new Size(372, 25);
            playbackConsoleTable.TabIndex = 3;
            // 
            // reverseSeekButton
            // 
            reverseSeekButton.AccessibleName = "Step backwards 1 frame button";
            reverseSeekButton.AutoSize = true;
            reverseSeekButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            reverseSeekButton.Dock = DockStyle.Fill;
            reverseSeekButton.Location = new Point(2, 0);
            reverseSeekButton.Margin = new Padding(2, 0, 2, 0);
            reverseSeekButton.Name = "reverseSeekButton";
            reverseSeekButton.Size = new Size(119, 25);
            reverseSeekButton.TabIndex = 0;
            reverseSeekButton.Text = "Previous Frame";
            tooltips.SetToolTip(reverseSeekButton, "Go to the previous frame in the media. This may cause the program to freeze for a bit.");
            reverseSeekButton.UseVisualStyleBackColor = true;
            reverseSeekButton.Click += reverseSeekButton_Click;
            // 
            // playButton
            // 
            playButton.AccessibleName = "Begin playback button";
            playButton.AutoSize = true;
            playButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            playButton.Dock = DockStyle.Fill;
            playButton.Location = new Point(125, 0);
            playButton.Margin = new Padding(2, 0, 2, 0);
            playButton.Name = "playButton";
            playButton.Size = new Size(119, 25);
            playButton.TabIndex = 1;
            playButton.Text = "Play";
            tooltips.SetToolTip(playButton, "Start playing the media from the current playhead position.");
            playButton.UseVisualStyleBackColor = true;
            playButton.Click += playButton_Click;
            // 
            // seekButton
            // 
            seekButton.AccessibleName = "Step forwards 1 frame button";
            seekButton.AutoSize = true;
            seekButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            seekButton.Dock = DockStyle.Fill;
            seekButton.Location = new Point(248, 0);
            seekButton.Margin = new Padding(2, 0, 2, 0);
            seekButton.Name = "seekButton";
            seekButton.Size = new Size(122, 25);
            seekButton.TabIndex = 2;
            seekButton.Text = "Next Frame";
            tooltips.SetToolTip(seekButton, resources.GetString("seekButton.ToolTip"));
            seekButton.UseVisualStyleBackColor = true;
            seekButton.Click += seekButton_Click;
            // 
            // conversionPanel
            // 
            conversionPanel.ColumnCount = 1;
            conversionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            conversionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 14F));
            conversionPanel.Controls.Add(settingsGroupBox, 0, 0);
            conversionPanel.Controls.Add(commandLinePaddingPanel, 0, 1);
            conversionPanel.Dock = DockStyle.Fill;
            conversionPanel.Location = new Point(0, 0);
            conversionPanel.Margin = new Padding(2);
            conversionPanel.Name = "conversionPanel";
            conversionPanel.RowCount = 2;
            conversionPanel.RowStyles.Add(new RowStyle());
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
            conversionPanel.Size = new Size(326, 518);
            conversionPanel.TabIndex = 0;
            // 
            // commandLinePaddingPanel
            // 
            commandLinePaddingPanel.BackColor = SystemColors.Window;
            commandLinePaddingPanel.Controls.Add(commandLineOutput);
            commandLinePaddingPanel.Dock = DockStyle.Fill;
            commandLinePaddingPanel.Location = new Point(2, 384);
            commandLinePaddingPanel.Margin = new Padding(2);
            commandLinePaddingPanel.Name = "commandLinePaddingPanel";
            commandLinePaddingPanel.Padding = new Padding(6);
            commandLinePaddingPanel.Size = new Size(322, 132);
            commandLinePaddingPanel.TabIndex = 1;
            // 
            // actionsPanel
            // 
            actionsPanel.Controls.Add(cancelButton);
            actionsPanel.Controls.Add(convertButton);
            actionsPanel.Dock = DockStyle.Bottom;
            actionsPanel.Location = new Point(0, 528);
            actionsPanel.Margin = new Padding(2);
            actionsPanel.Name = "actionsPanel";
            actionsPanel.Size = new Size(784, 33);
            actionsPanel.TabIndex = 1;
            // 
            // Main
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(mainSplit);
            Controls.Add(actionsPanel);
            HelpButton = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MinimumSize = new Size(512, 512);
            Name = "Main";
            Text = "Fast Animated Image Converter";
            Load += Main_Load;
            ((System.ComponentModel.ISupportInitialize)qualitySlider).EndInit();
            ((System.ComponentModel.ISupportInitialize)settingsBindingSource).EndInit();
            settingsGroupBox.ResumeLayout(false);
            settingsGroupBox.PerformLayout();
            settingsTable.ResumeLayout(false);
            settingsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)resizeSlider).EndInit();
            resizeDimensionContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)resizeDimensionValue).EndInit();
            samplingLayoutPanel.ResumeLayout(false);
            samplingLayoutPanel.PerformLayout();
            trimColumnLayoutPanel.ResumeLayout(false);
            trimColumnLayoutPanel.PerformLayout();
            firstFrameContainer.ResumeLayout(false);
            firstFrameContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)firstFrameInput).EndInit();
            lastFrameContainer.ResumeLayout(false);
            lastFrameContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)lastFrameInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).EndInit();
            fpsContainer.ResumeLayout(false);
            fpsContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)fpsValue).EndInit();
            repeatContainer.ResumeLayout(false);
            repeatContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)repeatValue).EndInit();
            mainSplit.Panel1.ResumeLayout(false);
            mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplit).EndInit();
            mainSplit.ResumeLayout(false);
            videoPanelTable.ResumeLayout(false);
            videoPanelTable.PerformLayout();
            videoContainingPanel.ResumeLayout(false);
            previewControlsPanel.ResumeLayout(false);
            previewControlsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)playhead).EndInit();
            playbackConsoleTable.ResumeLayout(false);
            playbackConsoleTable.PerformLayout();
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
        private BindingSource settingsBindingSource;
        private Label fpsLabel;
        private NumericUpDown fpsValue;
        private ComboBox fpsSetting;
        private TrackBar resizeSlider;
        private Label resizeLabel;
        private Label resizeDimensionLabel;
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
        private Label samplingLabel;
        private TableLayoutPanel samplingLayoutPanel;
        private RadioButton sampleBestRadio;
        private RadioButton sampleFastRadio;
        private Panel commandLinePaddingPanel;
        private Button reverseSeekButton;
        private CheckBox transparentCheckbox;
        private Label transparentLabel;
        private Panel settingsDivider2;
        private Panel settingsDivider1;
        private Panel settingsDivider3;
        private TableLayoutPanel previewControlsPanel;
        private TableLayoutPanel videoPanelTable;
        private NumericUpDown resizeDimensionValue;
        private Panel resizeDimensionContainer;
        private Panel firstFrameContainer;
        private Panel lastFrameContainer;
        private Panel fpsContainer;
        private Panel repeatContainer;
        private Label advancedLabel;
        private CheckBox editArgumentsCheckbox;
        private Panel settingsDivider4;
    }
}
