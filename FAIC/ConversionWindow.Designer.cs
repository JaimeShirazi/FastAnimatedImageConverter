namespace FAIC
{
    partial class ConversionWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConversionWindow));
            encodeProgressBar = new ProgressBar();
            tableLayoutPanel1 = new TableLayoutPanel();
            statsPanel = new TableLayoutPanel();
            frameStatsLabel = new Label();
            sizeStatsLabel = new Label();
            actionPanel = new TableLayoutPanel();
            closeButton = new Button();
            closeAndShowButton = new Button();
            cancelButton = new Button();
            tableLayoutPanel1.SuspendLayout();
            statsPanel.SuspendLayout();
            actionPanel.SuspendLayout();
            SuspendLayout();
            // 
            // encodeProgressBar
            // 
            encodeProgressBar.Dock = DockStyle.Fill;
            encodeProgressBar.Location = new Point(3, 32);
            encodeProgressBar.Maximum = 100000;
            encodeProgressBar.Name = "encodeProgressBar";
            encodeProgressBar.Size = new Size(398, 23);
            encodeProgressBar.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(statsPanel, 0, 0);
            tableLayoutPanel1.Controls.Add(encodeProgressBar, 0, 1);
            tableLayoutPanel1.Controls.Add(actionPanel, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(404, 95);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // statsPanel
            // 
            statsPanel.AutoSize = true;
            statsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            statsPanel.ColumnCount = 2;
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            statsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            statsPanel.Controls.Add(frameStatsLabel, 0, 1);
            statsPanel.Controls.Add(sizeStatsLabel, 2, 1);
            statsPanel.Dock = DockStyle.Fill;
            statsPanel.Location = new Point(3, 3);
            statsPanel.Name = "statsPanel";
            statsPanel.RowCount = 1;
            statsPanel.RowStyles.Add(new RowStyle());
            statsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            statsPanel.Size = new Size(398, 23);
            statsPanel.TabIndex = 1;
            // 
            // frameStatsLabel
            // 
            frameStatsLabel.AutoSize = true;
            frameStatsLabel.Dock = DockStyle.Fill;
            frameStatsLabel.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            frameStatsLabel.Location = new Point(3, 0);
            frameStatsLabel.Name = "frameStatsLabel";
            frameStatsLabel.Size = new Size(193, 23);
            frameStatsLabel.TabIndex = 0;
            frameStatsLabel.Text = "0 frames at 1.0x (0fps)";
            frameStatsLabel.TextAlign = ContentAlignment.BottomLeft;
            // 
            // sizeStatsLabel
            // 
            sizeStatsLabel.AutoSize = true;
            sizeStatsLabel.Dock = DockStyle.Fill;
            sizeStatsLabel.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sizeStatsLabel.Location = new Point(202, 0);
            sizeStatsLabel.Name = "sizeStatsLabel";
            sizeStatsLabel.Size = new Size(193, 23);
            sizeStatsLabel.TabIndex = 2;
            sizeStatsLabel.Text = "0kB";
            sizeStatsLabel.TextAlign = ContentAlignment.BottomRight;
            // 
            // actionPanel
            // 
            actionPanel.AutoSize = true;
            actionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            actionPanel.ColumnCount = 3;
            actionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            actionPanel.ColumnStyles.Add(new ColumnStyle());
            actionPanel.ColumnStyles.Add(new ColumnStyle());
            actionPanel.Controls.Add(closeButton, 0, 0);
            actionPanel.Controls.Add(closeAndShowButton, 1, 0);
            actionPanel.Controls.Add(cancelButton, 2, 0);
            actionPanel.Dock = DockStyle.Fill;
            actionPanel.Location = new Point(3, 61);
            actionPanel.Name = "actionPanel";
            actionPanel.RowCount = 1;
            actionPanel.RowStyles.Add(new RowStyle());
            actionPanel.Size = new Size(398, 31);
            actionPanel.TabIndex = 2;
            // 
            // closeButton
            // 
            closeButton.Dock = DockStyle.Fill;
            closeButton.Enabled = false;
            closeButton.Location = new Point(3, 3);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(168, 25);
            closeButton.TabIndex = 0;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += closeButton_Click;
            // 
            // closeAndShowButton
            // 
            closeAndShowButton.AutoSize = true;
            closeAndShowButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            closeAndShowButton.Dock = DockStyle.Fill;
            closeAndShowButton.Enabled = false;
            closeAndShowButton.Location = new Point(177, 3);
            closeAndShowButton.Name = "closeAndShowButton";
            closeAndShowButton.Size = new Size(159, 25);
            closeAndShowButton.TabIndex = 1;
            closeAndShowButton.Text = "Close and Show in Explorer";
            closeAndShowButton.UseVisualStyleBackColor = true;
            closeAndShowButton.Click += closeAndShowButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.AutoSize = true;
            cancelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cancelButton.Dock = DockStyle.Fill;
            cancelButton.Location = new Point(342, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(53, 25);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // ConversionWindow
            // 
            AcceptButton = closeButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = closeButton;
            ClientSize = new Size(404, 95);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(1600, 134);
            MinimumSize = new Size(420, 134);
            Name = "ConversionWindow";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Conversion Progress (busy 0s)";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            statsPanel.ResumeLayout(false);
            statsPanel.PerformLayout();
            actionPanel.ResumeLayout(false);
            actionPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar encodeProgressBar;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel statsPanel;
        private Label frameStatsLabel;
        private Label sizeStatsLabel;
        private TableLayoutPanel actionPanel;
        private Button closeButton;
        private Button closeAndShowButton;
        private Button cancelButton;
    }
}