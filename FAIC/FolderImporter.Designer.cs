namespace FAIC
{
    partial class FolderImporter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderImporter));
            verticalLayout = new TableLayoutPanel();
            settings = new TableLayoutPanel();
            subfoldersLabel = new Label();
            subfoldersCheckbox = new CheckBox();
            importButton = new Button();
            spacer1 = new Panel();
            importLog = new RichTextBox();
            saveConcat = new SaveFileDialog();
            verticalLayout.SuspendLayout();
            settings.SuspendLayout();
            SuspendLayout();
            // 
            // verticalLayout
            // 
            verticalLayout.ColumnCount = 1;
            verticalLayout.ColumnStyles.Add(new ColumnStyle());
            verticalLayout.Controls.Add(settings, 0, 0);
            verticalLayout.Controls.Add(importButton, 0, 1);
            verticalLayout.Controls.Add(spacer1, 0, 2);
            verticalLayout.Controls.Add(importLog, 0, 3);
            verticalLayout.Dock = DockStyle.Fill;
            verticalLayout.Location = new Point(0, 0);
            verticalLayout.Name = "verticalLayout";
            verticalLayout.RowCount = 4;
            verticalLayout.RowStyles.Add(new RowStyle());
            verticalLayout.RowStyles.Add(new RowStyle());
            verticalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            verticalLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            verticalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            verticalLayout.Size = new Size(384, 175);
            verticalLayout.TabIndex = 0;
            // 
            // settings
            // 
            settings.AutoSize = true;
            settings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            settings.ColumnCount = 2;
            settings.ColumnStyles.Add(new ColumnStyle());
            settings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            settings.Controls.Add(subfoldersLabel, 0, 0);
            settings.Controls.Add(subfoldersCheckbox, 1, 0);
            settings.Dock = DockStyle.Fill;
            settings.Location = new Point(3, 3);
            settings.Name = "settings";
            settings.RowCount = 1;
            settings.RowStyles.Add(new RowStyle());
            settings.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            settings.Size = new Size(378, 25);
            settings.TabIndex = 6;
            // 
            // subfoldersLabel
            // 
            subfoldersLabel.AutoSize = true;
            subfoldersLabel.Dock = DockStyle.Fill;
            subfoldersLabel.Location = new Point(3, 0);
            subfoldersLabel.Name = "subfoldersLabel";
            subfoldersLabel.Size = new Size(32, 25);
            subfoldersLabel.TabIndex = 2;
            subfoldersLabel.Text = "Scan";
            subfoldersLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // subfoldersCheckbox
            // 
            subfoldersCheckbox.Dock = DockStyle.Fill;
            subfoldersCheckbox.Location = new Point(41, 3);
            subfoldersCheckbox.Name = "subfoldersCheckbox";
            subfoldersCheckbox.Size = new Size(334, 19);
            subfoldersCheckbox.TabIndex = 4;
            subfoldersCheckbox.Text = "Include Subfolders";
            subfoldersCheckbox.UseVisualStyleBackColor = true;
            // 
            // importButton
            // 
            importButton.AutoSize = true;
            importButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            verticalLayout.SetColumnSpan(importButton, 2);
            importButton.Dock = DockStyle.Fill;
            importButton.Location = new Point(3, 34);
            importButton.Name = "importButton";
            importButton.Size = new Size(378, 25);
            importButton.TabIndex = 0;
            importButton.Text = "Import";
            importButton.UseVisualStyleBackColor = true;
            importButton.Click += importButton_Click;
            // 
            // spacer1
            // 
            spacer1.Dock = DockStyle.Fill;
            spacer1.Location = new Point(3, 65);
            spacer1.Name = "spacer1";
            spacer1.Size = new Size(378, 14);
            spacer1.TabIndex = 8;
            // 
            // importLog
            // 
            importLog.Dock = DockStyle.Fill;
            importLog.Location = new Point(3, 85);
            importLog.Name = "importLog";
            importLog.Size = new Size(378, 87);
            importLog.TabIndex = 7;
            importLog.Text = "This dialogue creates a ffconcat file from the images in your folder.\nThis file is used as input for the animated image converter.\n";
            // 
            // saveConcat
            // 
            saveConcat.DefaultExt = "ffcat";
            saveConcat.FileName = "index.ffcat";
            saveConcat.Filter = "ffconcat files|*.ffcat;*.ffconcat|Text files|*.txt|All files|*.*";
            saveConcat.Title = "Imported File Save Dialogue";
            // 
            // FolderImporter
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 175);
            Controls.Add(verticalLayout);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(400, 128);
            Name = "FolderImporter";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Import Folder";
            verticalLayout.ResumeLayout(false);
            verticalLayout.PerformLayout();
            settings.ResumeLayout(false);
            settings.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel verticalLayout;
        private Button importButton;
        private TableLayoutPanel settings;
        private Panel spacer1;
        private CheckBox subfoldersCheckbox;
        private Label subfoldersLabel;
        private RichTextBox importLog;
        private SaveFileDialog saveConcat;
    }
}