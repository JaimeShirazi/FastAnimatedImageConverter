namespace FAIC
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            aboutLayoutPanel = new TableLayoutPanel();
            logo = new PictureBox();
            informationPanel = new TableLayoutPanel();
            versionLabel = new Label();
            aboutText = new RichTextBox();
            aboutLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logo).BeginInit();
            informationPanel.SuspendLayout();
            SuspendLayout();
            // 
            // aboutLayoutPanel
            // 
            aboutLayoutPanel.ColumnCount = 2;
            aboutLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            aboutLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            aboutLayoutPanel.Controls.Add(logo, 0, 0);
            aboutLayoutPanel.Controls.Add(informationPanel, 1, 0);
            aboutLayoutPanel.Dock = DockStyle.Fill;
            aboutLayoutPanel.Location = new Point(0, 0);
            aboutLayoutPanel.Margin = new Padding(2, 2, 2, 2);
            aboutLayoutPanel.Name = "aboutLayoutPanel";
            aboutLayoutPanel.RowCount = 1;
            aboutLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            aboutLayoutPanel.Size = new Size(584, 361);
            aboutLayoutPanel.TabIndex = 0;
            // 
            // logo
            // 
            logo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            logo.Image = Properties.Resources.Logo_512;
            logo.Location = new Point(22, 19);
            logo.Margin = new Padding(22, 19, 22, 19);
            logo.MinimumSize = new Size(134, 115);
            logo.Name = "logo";
            logo.Size = new Size(172, 323);
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            logo.TabIndex = 0;
            logo.TabStop = false;
            // 
            // informationPanel
            // 
            informationPanel.AutoSize = true;
            informationPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            informationPanel.ColumnCount = 1;
            informationPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            informationPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 14F));
            informationPanel.Controls.Add(versionLabel, 0, 1);
            informationPanel.Controls.Add(aboutText, 0, 0);
            informationPanel.Dock = DockStyle.Fill;
            informationPanel.Location = new Point(218, 2);
            informationPanel.Margin = new Padding(2, 2, 2, 2);
            informationPanel.Name = "informationPanel";
            informationPanel.RowCount = 2;
            informationPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            informationPanel.RowStyles.Add(new RowStyle());
            informationPanel.Size = new Size(364, 357);
            informationPanel.TabIndex = 1;
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.Dock = DockStyle.Right;
            versionLabel.Location = new Point(289, 342);
            versionLabel.Margin = new Padding(2, 0, 2, 0);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(73, 15);
            versionLabel.TabIndex = 1;
            versionLabel.Text = "Version Error";
            versionLabel.TextAlign = ContentAlignment.TopRight;
            // 
            // aboutText
            // 
            aboutText.Dock = DockStyle.Fill;
            aboutText.Location = new Point(2, 2);
            aboutText.Margin = new Padding(2, 2, 2, 2);
            aboutText.Name = "aboutText";
            aboutText.ReadOnly = true;
            aboutText.Size = new Size(360, 338);
            aboutText.TabIndex = 1;
            aboutText.Text = "Error loading About text.";
            // 
            // About
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 361);
            Controls.Add(aboutLayoutPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2, 2, 2, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(512, 200);
            Name = "About";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About";
            TopMost = true;
            aboutLayoutPanel.ResumeLayout(false);
            aboutLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logo).EndInit();
            informationPanel.ResumeLayout(false);
            informationPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel aboutLayoutPanel;
        private PictureBox logo;
        private RichTextBox aboutText;
        private TableLayoutPanel informationPanel;
        private Label versionLabel;
    }
}