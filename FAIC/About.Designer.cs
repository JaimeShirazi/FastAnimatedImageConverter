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
            aboutLayoutPanel.Name = "aboutLayoutPanel";
            aboutLayoutPanel.RowCount = 1;
            aboutLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            aboutLayoutPanel.Size = new Size(778, 344);
            aboutLayoutPanel.TabIndex = 0;
            // 
            // logo
            // 
            logo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            logo.Image = Properties.Resources.Logo_512;
            logo.Location = new Point(32, 32);
            logo.Margin = new Padding(32);
            logo.MinimumSize = new Size(192, 192);
            logo.Name = "logo";
            logo.Size = new Size(246, 280);
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
            informationPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            informationPanel.Controls.Add(versionLabel, 0, 1);
            informationPanel.Controls.Add(aboutText, 0, 0);
            informationPanel.Dock = DockStyle.Fill;
            informationPanel.Location = new Point(313, 3);
            informationPanel.Name = "informationPanel";
            informationPanel.RowCount = 2;
            informationPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            informationPanel.RowStyles.Add(new RowStyle());
            informationPanel.Size = new Size(462, 338);
            informationPanel.TabIndex = 1;
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.Dock = DockStyle.Right;
            versionLabel.Location = new Point(346, 313);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(113, 25);
            versionLabel.TabIndex = 1;
            versionLabel.Text = "Version Error";
            versionLabel.TextAlign = ContentAlignment.TopRight;
            // 
            // aboutText
            // 
            aboutText.Dock = DockStyle.Fill;
            aboutText.Location = new Point(3, 3);
            aboutText.Name = "aboutText";
            aboutText.ReadOnly = true;
            aboutText.Size = new Size(456, 307);
            aboutText.TabIndex = 1;
            aboutText.Text = "Error loading About text.";
            // 
            // About
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(778, 344);
            Controls.Add(aboutLayoutPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(600, 300);
            Name = "About";
            ShowInTaskbar = false;
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