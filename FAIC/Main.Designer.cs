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
            commandLineOutput = new RichTextBox();
            settingsGroupBox = new GroupBox();
            settingsTable = new TableLayoutPanel();
            resizeLabel = new Label();
            resizeSlider = new TrackBar();
            resizeDimensionLabel = new Label();
            resizeDimensionContainer = new Panel();
            resizeDimensionValue = new NumericUpDown();
            settingsDivider1 = new Panel();
            trimColumnLayoutPanel = new TableLayoutPanel();
            trimLabel = new Label();
            firstFrame = new Label();
            firstFrameContainer = new Panel();
            beginningInput = new FAIC.Types.Forms.TimeNumericUpDown();
            lastFrameLabel = new Label();
            lastFrameContainer = new Panel();
            endInput = new FAIC.Types.Forms.TimeNumericUpDown();
            settingsDivider2 = new Panel();
            speedLabel = new Label();
            speedSlider = new TrackBar();
            fpsColumnLayoutPanel = new TableLayoutPanel();
            fpsLabel = new Label();
            fpsModeLabel = new Label();
            fpsSetting = new ComboBox();
            fpsTargetLabel = new Label();
            fpsContainer = new Panel();
            fpsValue = new NumericUpDown();
            settingsDivider3 = new Panel();
            formatLabel = new Label();
            transparentCheckbox = new CheckBox();
            repeatsLabel = new Label();
            repeatContainer = new Panel();
            repeatValue = new FAIC.Types.Forms.InfinityNumericUpDown();
            settingsDivider4 = new Panel();
            advancedLabel = new Label();
            editArgumentsCheckbox = new CheckBox();
            processingModeLabel = new Label();
            processingLayoutPanel = new TableLayoutPanel();
            processingFastRadio = new RadioButton();
            processingBestRadio = new RadioButton();
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
            masterLayout = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)qualitySlider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)settingsBindingSource).BeginInit();
            settingsGroupBox.SuspendLayout();
            settingsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resizeSlider).BeginInit();
            resizeDimensionContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resizeDimensionValue).BeginInit();
            trimColumnLayoutPanel.SuspendLayout();
            firstFrameContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)beginningInput).BeginInit();
            lastFrameContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)endInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).BeginInit();
            fpsColumnLayoutPanel.SuspendLayout();
            fpsContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fpsValue).BeginInit();
            repeatContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)repeatValue).BeginInit();
            processingLayoutPanel.SuspendLayout();
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
            masterLayout.SuspendLayout();
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
            qualitySlider.Size = new Size(218, 19);
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
            convertButton.AutoSize = true;
            convertButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            convertButton.Dock = DockStyle.Fill;
            convertButton.Location = new Point(0, 0);
            convertButton.Margin = new Padding(4);
            convertButton.Name = "convertButton";
            convertButton.Size = new Size(776, 25);
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
            commandLineOutput.Size = new Size(312, 122);
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
            settingsGroupBox.Location = new Point(4, 4);
            settingsGroupBox.Margin = new Padding(2, 2, 2, 0);
            settingsGroupBox.Name = "settingsGroupBox";
            settingsGroupBox.Padding = new Padding(2);
            settingsGroupBox.Size = new Size(324, 380);
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
            settingsTable.Controls.Add(settingsDivider1, 0, 3);
            settingsTable.Controls.Add(trimColumnLayoutPanel, 0, 4);
            settingsTable.Controls.Add(firstFrameContainer, 1, 4);
            settingsTable.Controls.Add(lastFrameLabel, 0, 5);
            settingsTable.Controls.Add(lastFrameContainer, 1, 5);
            settingsTable.Controls.Add(settingsDivider2, 0, 6);
            settingsTable.Controls.Add(speedLabel, 0, 7);
            settingsTable.Controls.Add(speedSlider, 1, 7);
            settingsTable.Controls.Add(fpsColumnLayoutPanel, 0, 8);
            settingsTable.Controls.Add(fpsSetting, 1, 8);
            settingsTable.Controls.Add(fpsTargetLabel, 0, 9);
            settingsTable.Controls.Add(fpsContainer, 1, 9);
            settingsTable.Controls.Add(settingsDivider3, 0, 10);
            settingsTable.Controls.Add(formatLabel, 0, 11);
            settingsTable.Controls.Add(transparentCheckbox, 1, 11);
            settingsTable.Controls.Add(repeatsLabel, 0, 12);
            settingsTable.Controls.Add(repeatContainer, 1, 12);
            settingsTable.Controls.Add(settingsDivider4, 0, 13);
            settingsTable.Controls.Add(advancedLabel, 0, 14);
            settingsTable.Controls.Add(editArgumentsCheckbox, 1, 14);
            settingsTable.Controls.Add(processingModeLabel, 0, 15);
            settingsTable.Controls.Add(processingLayoutPanel, 1, 15);
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
            settingsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            settingsTable.Size = new Size(320, 360);
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
            resizeSlider.Size = new Size(218, 19);
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
            resizeDimensionContainer.Size = new Size(216, 21);
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
            resizeDimensionValue.Size = new Size(216, 23);
            resizeDimensionValue.TabIndex = 0;
            tooltips.SetToolTip(resizeDimensionValue, "The desired output resolution in pixels of the largest dimension. The other dimension will be rescaled as well to preserve the aspect ratio.");
            resizeDimensionValue.Value = new decimal(new int[] { 1, 0, 0, 0 });
            resizeDimensionValue.ValueChanged += resizeDimensionValue_ValueChanged;
            // 
            // settingsDivider1
            // 
            settingsDivider1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            settingsTable.SetColumnSpan(settingsDivider1, 2);
            settingsDivider1.Location = new Point(2, 75);
            settingsDivider1.Margin = new Padding(2);
            settingsDivider1.MaximumSize = new Size(0, 8);
            settingsDivider1.MinimumSize = new Size(0, 8);
            settingsDivider1.Name = "settingsDivider1";
            settingsDivider1.Size = new Size(316, 8);
            settingsDivider1.TabIndex = 6;
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
            trimColumnLayoutPanel.Location = new Point(0, 85);
            trimColumnLayoutPanel.Margin = new Padding(0);
            trimColumnLayoutPanel.Name = "trimColumnLayoutPanel";
            trimColumnLayoutPanel.RowCount = 1;
            trimColumnLayoutPanel.RowStyles.Add(new RowStyle());
            trimColumnLayoutPanel.Size = new Size(98, 29);
            trimColumnLayoutPanel.TabIndex = 7;
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
            firstFrameContainer.Controls.Add(beginningInput);
            firstFrameContainer.Dock = DockStyle.Fill;
            firstFrameContainer.Location = new Point(101, 88);
            firstFrameContainer.Name = "firstFrameContainer";
            firstFrameContainer.Size = new Size(216, 23);
            firstFrameContainer.TabIndex = 8;
            // 
            // beginningInput
            // 
            beginningInput.AccessibleName = "Trim start of the video input";
            beginningInput.AutoSize = true;
            beginningInput.DecimalPlaces = 3;
            beginningInput.Dock = DockStyle.Fill;
            beginningInput.Location = new Point(0, 0);
            beginningInput.Margin = new Padding(2);
            beginningInput.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            beginningInput.Name = "beginningInput";
            beginningInput.Size = new Size(216, 23);
            beginningInput.TabIndex = 0;
            tooltips.SetToolTip(beginningInput, "Where to start the converted output relative to the source media.");
            // 
            // lastFrameLabel
            // 
            lastFrameLabel.AutoSize = true;
            lastFrameLabel.Dock = DockStyle.Fill;
            lastFrameLabel.Location = new Point(2, 114);
            lastFrameLabel.Margin = new Padding(2, 0, 2, 0);
            lastFrameLabel.MinimumSize = new Size(45, 0);
            lastFrameLabel.Name = "lastFrameLabel";
            lastFrameLabel.Size = new Size(94, 29);
            lastFrameLabel.TabIndex = 9;
            lastFrameLabel.Text = "End";
            lastFrameLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lastFrameContainer
            // 
            lastFrameContainer.AutoSize = true;
            lastFrameContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            lastFrameContainer.Controls.Add(endInput);
            lastFrameContainer.Dock = DockStyle.Fill;
            lastFrameContainer.Location = new Point(101, 117);
            lastFrameContainer.Name = "lastFrameContainer";
            lastFrameContainer.Size = new Size(216, 23);
            lastFrameContainer.TabIndex = 10;
            // 
            // endInput
            // 
            endInput.AccessibleName = "Trim end of the video input";
            endInput.AutoSize = true;
            endInput.DecimalPlaces = 3;
            endInput.Dock = DockStyle.Fill;
            endInput.Location = new Point(0, 0);
            endInput.Margin = new Padding(2);
            endInput.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            endInput.Name = "endInput";
            endInput.Size = new Size(216, 23);
            endInput.TabIndex = 0;
            tooltips.SetToolTip(endInput, "Where to end the converted output relative to the source media.");
            // 
            // settingsDivider2
            // 
            settingsDivider2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            settingsTable.SetColumnSpan(settingsDivider2, 2);
            settingsDivider2.Location = new Point(2, 145);
            settingsDivider2.Margin = new Padding(2);
            settingsDivider2.MaximumSize = new Size(0, 8);
            settingsDivider2.MinimumSize = new Size(0, 8);
            settingsDivider2.Name = "settingsDivider2";
            settingsDivider2.Size = new Size(316, 8);
            settingsDivider2.TabIndex = 13;
            // 
            // speedLabel
            // 
            speedLabel.AutoSize = true;
            speedLabel.Dock = DockStyle.Fill;
            speedLabel.Location = new Point(2, 155);
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
            speedSlider.Location = new Point(100, 157);
            speedSlider.Margin = new Padding(2);
            speedSlider.Maximum = 16;
            speedSlider.Name = "speedSlider";
            speedSlider.Size = new Size(218, 19);
            speedSlider.TabIndex = 15;
            tooltips.SetToolTip(speedSlider, "The percentage to decrease the output resolution by. Lower values will help improve performance and decrease file size.\r\n");
            speedSlider.Value = 5;
            speedSlider.Scroll += speedSlider_Scroll;
            // 
            // fpsColumnLayoutPanel
            // 
            fpsColumnLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            fpsColumnLayoutPanel.AutoSize = true;
            fpsColumnLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            fpsColumnLayoutPanel.ColumnCount = 2;
            fpsColumnLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            fpsColumnLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            fpsColumnLayoutPanel.Controls.Add(fpsLabel, 0, 0);
            fpsColumnLayoutPanel.Controls.Add(fpsModeLabel, 1, 0);
            fpsColumnLayoutPanel.Location = new Point(0, 178);
            fpsColumnLayoutPanel.Margin = new Padding(0);
            fpsColumnLayoutPanel.Name = "fpsColumnLayoutPanel";
            fpsColumnLayoutPanel.RowCount = 1;
            fpsColumnLayoutPanel.RowStyles.Add(new RowStyle());
            fpsColumnLayoutPanel.Size = new Size(98, 27);
            fpsColumnLayoutPanel.TabIndex = 29;
            // 
            // fpsLabel
            // 
            fpsLabel.AutoSize = true;
            fpsLabel.Dock = DockStyle.Fill;
            fpsLabel.Location = new Point(2, 0);
            fpsLabel.Margin = new Padding(2, 0, 2, 0);
            fpsLabel.Name = "fpsLabel";
            fpsLabel.Size = new Size(26, 27);
            fpsLabel.TabIndex = 6;
            fpsLabel.Text = "FPS";
            fpsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // fpsModeLabel
            // 
            fpsModeLabel.AutoSize = true;
            fpsModeLabel.Dock = DockStyle.Fill;
            fpsModeLabel.Location = new Point(32, 0);
            fpsModeLabel.Margin = new Padding(2, 0, 2, 0);
            fpsModeLabel.MinimumSize = new Size(45, 0);
            fpsModeLabel.Name = "fpsModeLabel";
            fpsModeLabel.Size = new Size(64, 27);
            fpsModeLabel.TabIndex = 7;
            fpsModeLabel.Text = "Mode";
            fpsModeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // fpsSetting
            // 
            fpsSetting.AccessibleName = "Frame rate mode dropdown";
            fpsSetting.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fpsSetting.FormattingEnabled = true;
            fpsSetting.Items.AddRange(new object[] { "Same", "Nearest", "Blended" });
            fpsSetting.Location = new Point(100, 180);
            fpsSetting.Margin = new Padding(2);
            fpsSetting.MaxDropDownItems = 3;
            fpsSetting.Name = "fpsSetting";
            fpsSetting.Size = new Size(218, 23);
            fpsSetting.TabIndex = 17;
            tooltips.SetToolTip(fpsSetting, resources.GetString("fpsSetting.ToolTip"));
            fpsSetting.SelectedIndexChanged += fpsSetting_SelectedIndexChanged;
            // 
            // fpsTargetLabel
            // 
            fpsTargetLabel.AutoSize = true;
            fpsTargetLabel.Dock = DockStyle.Fill;
            fpsTargetLabel.Location = new Point(2, 205);
            fpsTargetLabel.Margin = new Padding(2, 0, 2, 0);
            fpsTargetLabel.Name = "fpsTargetLabel";
            fpsTargetLabel.Size = new Size(94, 29);
            fpsTargetLabel.TabIndex = 28;
            fpsTargetLabel.Text = "Target";
            fpsTargetLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // fpsContainer
            // 
            fpsContainer.AutoSize = true;
            fpsContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            fpsContainer.Controls.Add(fpsValue);
            fpsContainer.Dock = DockStyle.Fill;
            fpsContainer.Location = new Point(101, 208);
            fpsContainer.Name = "fpsContainer";
            fpsContainer.Size = new Size(216, 23);
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
            fpsValue.Size = new Size(216, 23);
            fpsValue.TabIndex = 0;
            tooltips.SetToolTip(fpsValue, "Target frame rate of the converted output.");
            // 
            // settingsDivider3
            // 
            settingsDivider3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            settingsTable.SetColumnSpan(settingsDivider3, 2);
            settingsDivider3.Location = new Point(2, 236);
            settingsDivider3.Margin = new Padding(2);
            settingsDivider3.MaximumSize = new Size(0, 8);
            settingsDivider3.MinimumSize = new Size(0, 8);
            settingsDivider3.Name = "settingsDivider3";
            settingsDivider3.Size = new Size(316, 8);
            settingsDivider3.TabIndex = 22;
            // 
            // formatLabel
            // 
            formatLabel.AutoSize = true;
            formatLabel.Dock = DockStyle.Fill;
            formatLabel.Location = new Point(2, 246);
            formatLabel.Margin = new Padding(2, 0, 2, 0);
            formatLabel.MinimumSize = new Size(45, 0);
            formatLabel.Name = "formatLabel";
            formatLabel.Size = new Size(94, 23);
            formatLabel.TabIndex = 23;
            formatLabel.Text = "Format";
            formatLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // transparentCheckbox
            // 
            transparentCheckbox.AutoSize = true;
            transparentCheckbox.Dock = DockStyle.Fill;
            transparentCheckbox.Location = new Point(100, 248);
            transparentCheckbox.Margin = new Padding(2);
            transparentCheckbox.Name = "transparentCheckbox";
            transparentCheckbox.Size = new Size(218, 19);
            transparentCheckbox.TabIndex = 24;
            transparentCheckbox.Text = "Transparent";
            transparentCheckbox.UseVisualStyleBackColor = true;
            // 
            // repeatsLabel
            // 
            repeatsLabel.AutoSize = true;
            repeatsLabel.Dock = DockStyle.Fill;
            repeatsLabel.Location = new Point(2, 269);
            repeatsLabel.Margin = new Padding(2, 0, 2, 0);
            repeatsLabel.Name = "repeatsLabel";
            repeatsLabel.Size = new Size(94, 29);
            repeatsLabel.TabIndex = 20;
            repeatsLabel.Text = "Repeats";
            repeatsLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // repeatContainer
            // 
            repeatContainer.AutoSize = true;
            repeatContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            repeatContainer.Controls.Add(repeatValue);
            repeatContainer.Dock = DockStyle.Fill;
            repeatContainer.Location = new Point(101, 272);
            repeatContainer.Name = "repeatContainer";
            repeatContainer.Size = new Size(216, 23);
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
            repeatValue.Size = new Size(216, 23);
            repeatValue.TabIndex = 0;
            tooltips.SetToolTip(repeatValue, resources.GetString("repeatValue.ToolTip"));
            // 
            // settingsDivider4
            // 
            settingsDivider4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            settingsTable.SetColumnSpan(settingsDivider4, 2);
            settingsDivider4.Location = new Point(2, 300);
            settingsDivider4.Margin = new Padding(2);
            settingsDivider4.MaximumSize = new Size(0, 8);
            settingsDivider4.MinimumSize = new Size(0, 8);
            settingsDivider4.Name = "settingsDivider4";
            settingsDivider4.Size = new Size(316, 8);
            settingsDivider4.TabIndex = 25;
            // 
            // advancedLabel
            // 
            advancedLabel.AutoSize = true;
            advancedLabel.Dock = DockStyle.Fill;
            advancedLabel.Location = new Point(2, 310);
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
            editArgumentsCheckbox.Location = new Point(100, 312);
            editArgumentsCheckbox.Margin = new Padding(2);
            editArgumentsCheckbox.Name = "editArgumentsCheckbox";
            editArgumentsCheckbox.Size = new Size(218, 19);
            editArgumentsCheckbox.TabIndex = 27;
            editArgumentsCheckbox.Text = "Edit Arguments";
            editArgumentsCheckbox.UseVisualStyleBackColor = true;
            // 
            // processingModeLabel
            // 
            processingModeLabel.AutoSize = true;
            processingModeLabel.Dock = DockStyle.Fill;
            processingModeLabel.Location = new Point(2, 333);
            processingModeLabel.Margin = new Padding(2, 0, 2, 0);
            processingModeLabel.MinimumSize = new Size(45, 0);
            processingModeLabel.Name = "processingModeLabel";
            processingModeLabel.Size = new Size(94, 27);
            processingModeLabel.TabIndex = 6;
            processingModeLabel.Text = "Tuning";
            processingModeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // processingLayoutPanel
            // 
            processingLayoutPanel.AutoSize = true;
            processingLayoutPanel.ColumnCount = 2;
            processingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            processingLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            processingLayoutPanel.Controls.Add(processingFastRadio, 0, 0);
            processingLayoutPanel.Controls.Add(processingBestRadio, 1, 0);
            processingLayoutPanel.Dock = DockStyle.Fill;
            processingLayoutPanel.Location = new Point(100, 335);
            processingLayoutPanel.Margin = new Padding(2);
            processingLayoutPanel.Name = "processingLayoutPanel";
            processingLayoutPanel.RowCount = 1;
            processingLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            processingLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            processingLayoutPanel.Size = new Size(218, 23);
            processingLayoutPanel.TabIndex = 7;
            // 
            // processingFastRadio
            // 
            processingFastRadio.AccessibleName = "Fast resampling algorithm button";
            processingFastRadio.AutoSize = true;
            processingFastRadio.Checked = true;
            processingFastRadio.Location = new Point(2, 2);
            processingFastRadio.Margin = new Padding(2);
            processingFastRadio.Name = "processingFastRadio";
            processingFastRadio.Size = new Size(46, 19);
            processingFastRadio.TabIndex = 0;
            processingFastRadio.TabStop = true;
            processingFastRadio.Text = "Fast";
            tooltips.SetToolTip(processingFastRadio, "Preset for prioritise conversion speed.");
            processingFastRadio.UseVisualStyleBackColor = true;
            // 
            // processingBestRadio
            // 
            processingBestRadio.AccessibleName = "Best resampling algorithm button";
            processingBestRadio.AutoSize = true;
            processingBestRadio.Location = new Point(111, 2);
            processingBestRadio.Margin = new Padding(2);
            processingBestRadio.Name = "processingBestRadio";
            processingBestRadio.Size = new Size(47, 19);
            processingBestRadio.TabIndex = 1;
            processingBestRadio.Text = "Best";
            tooltips.SetToolTip(processingBestRadio, "Preset for prioritising output quality.");
            processingBestRadio.UseVisualStyleBackColor = true;
            // 
            // mainSplit
            // 
            mainSplit.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mainSplit.Location = new Point(2, 2);
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
            mainSplit.Size = new Size(780, 524);
            mainSplit.SplitterDistance = 445;
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
            videoPanelTable.Size = new Size(445, 524);
            videoPanelTable.TabIndex = 0;
            // 
            // videoContainingPanel
            // 
            videoContainingPanel.Controls.Add(videoPreviewHost);
            videoContainingPanel.Dock = DockStyle.Fill;
            videoContainingPanel.Location = new Point(2, 2);
            videoContainingPanel.Margin = new Padding(2);
            videoContainingPanel.Name = "videoContainingPanel";
            videoContainingPanel.Padding = new Padding(2);
            videoContainingPanel.Size = new Size(441, 463);
            videoContainingPanel.TabIndex = 0;
            // 
            // videoPreviewHost
            // 
            videoPreviewHost.Dock = DockStyle.Fill;
            videoPreviewHost.Location = new Point(2, 2);
            videoPreviewHost.Margin = new Padding(2);
            videoPreviewHost.Name = "videoPreview";
            videoPreviewHost.Size = new Size(437, 459);
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
            previewControlsPanel.Location = new Point(0, 467);
            previewControlsPanel.Margin = new Padding(0);
            previewControlsPanel.Name = "previewControlsPanel";
            previewControlsPanel.RowCount = 2;
            previewControlsPanel.RowStyles.Add(new RowStyle());
            previewControlsPanel.RowStyles.Add(new RowStyle());
            previewControlsPanel.Size = new Size(445, 57);
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
            playhead.Size = new Size(381, 24);
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
            trimEndHereButton.Location = new Point(417, 2);
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
            playbackConsoleTable.Size = new Size(385, 25);
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
            reverseSeekButton.Size = new Size(124, 25);
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
            playButton.Location = new Point(130, 0);
            playButton.Margin = new Padding(2, 0, 2, 0);
            playButton.Name = "playButton";
            playButton.Size = new Size(124, 25);
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
            seekButton.Location = new Point(258, 0);
            seekButton.Margin = new Padding(2, 0, 2, 0);
            seekButton.Name = "seekButton";
            seekButton.Size = new Size(125, 25);
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
            conversionPanel.Padding = new Padding(2);
            conversionPanel.RowCount = 2;
            conversionPanel.RowStyles.Add(new RowStyle());
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
            conversionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
            conversionPanel.Size = new Size(332, 524);
            conversionPanel.TabIndex = 0;
            // 
            // commandLinePaddingPanel
            // 
            commandLinePaddingPanel.BackColor = SystemColors.Window;
            commandLinePaddingPanel.Controls.Add(commandLineOutput);
            commandLinePaddingPanel.Dock = DockStyle.Fill;
            commandLinePaddingPanel.Location = new Point(4, 386);
            commandLinePaddingPanel.Margin = new Padding(2);
            commandLinePaddingPanel.Name = "commandLinePaddingPanel";
            commandLinePaddingPanel.Padding = new Padding(6);
            commandLinePaddingPanel.Size = new Size(324, 134);
            commandLinePaddingPanel.TabIndex = 1;
            // 
            // actionsPanel
            // 
            actionsPanel.AutoSize = true;
            actionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            actionsPanel.Controls.Add(convertButton);
            actionsPanel.Dock = DockStyle.Fill;
            actionsPanel.Location = new Point(4, 532);
            actionsPanel.Margin = new Padding(4);
            actionsPanel.Name = "actionsPanel";
            actionsPanel.Size = new Size(776, 25);
            actionsPanel.TabIndex = 1;
            // 
            // masterLayout
            // 
            masterLayout.ColumnCount = 1;
            masterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            masterLayout.Controls.Add(mainSplit, 0, 0);
            masterLayout.Controls.Add(actionsPanel, 0, 1);
            masterLayout.Dock = DockStyle.Fill;
            masterLayout.Location = new Point(0, 0);
            masterLayout.Name = "masterLayout";
            masterLayout.RowCount = 2;
            masterLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            masterLayout.RowStyles.Add(new RowStyle());
            masterLayout.Size = new Size(784, 561);
            masterLayout.TabIndex = 2;
            // 
            // Main
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(masterLayout);
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
            trimColumnLayoutPanel.ResumeLayout(false);
            trimColumnLayoutPanel.PerformLayout();
            firstFrameContainer.ResumeLayout(false);
            firstFrameContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)beginningInput).EndInit();
            lastFrameContainer.ResumeLayout(false);
            lastFrameContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)endInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)speedSlider).EndInit();
            fpsColumnLayoutPanel.ResumeLayout(false);
            fpsColumnLayoutPanel.PerformLayout();
            fpsContainer.ResumeLayout(false);
            fpsContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)fpsValue).EndInit();
            repeatContainer.ResumeLayout(false);
            repeatContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)repeatValue).EndInit();
            processingLayoutPanel.ResumeLayout(false);
            processingLayoutPanel.PerformLayout();
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
            actionsPanel.PerformLayout();
            masterLayout.ResumeLayout(false);
            masterLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private TrackBar qualitySlider;
        private Button convertButton;
        private Label qualityLabel;
        private SaveFileDialog saveFileDialogue;
        private RichTextBox commandLineOutput;
        private GroupBox settingsGroupBox;
        private SplitContainer mainSplit;
        private Panel videoContainingPanel;
        private Panel actionsPanel;
        private TrackBar playhead;
        private ElementHost videoPreviewHost;
        private Button trimStartHereButton;
        private Button trimEndHereButton;
        private Label trimLabel;
        private Button playButton;
        private Button seekButton;
        private TableLayoutPanel playbackConsoleTable;
        private TableLayoutPanel settingsTable;
        private TableLayoutPanel conversionPanel;
        private BindingSource settingsBindingSource;
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
        private Label firstFrame;
        private TableLayoutPanel trimColumnLayoutPanel;
        private Label processingModeLabel;
        private TableLayoutPanel processingLayoutPanel;
        private RadioButton processingBestRadio;
        private RadioButton processingFastRadio;
        private Panel commandLinePaddingPanel;
        private Button reverseSeekButton;
        private CheckBox transparentCheckbox;
        private Label formatLabel;
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
        private Label fpsTargetLabel;
        private TableLayoutPanel fpsColumnLayoutPanel;
        private Label fpsLabel;
        private Label fpsModeLabel;
        private TableLayoutPanel masterLayout;
        private Types.Forms.TimeNumericUpDown beginningInput;
        private Types.Forms.TimeNumericUpDown endInput;
    }
}
