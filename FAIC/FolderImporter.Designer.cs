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
            fpsLabel = new Label();
            fpsValueContainer = new Panel();
            fpsValue = new NumericUpDown();
            subfoldersLabel = new Label();
            subfoldersCheckbox = new CheckBox();
            importButton = new Button();
            spacer1 = new Panel();
            importProgress = new ProgressBar();
            importLog = new RichTextBox();
            saveConcatDialogue = new SaveFileDialog();
            verticalLayout.SuspendLayout();
            settings.SuspendLayout();
            fpsValueContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fpsValue).BeginInit();
            SuspendLayout();
            // 
            // verticalLayout
            // 
            verticalLayout.ColumnCount = 1;
            verticalLayout.ColumnStyles.Add(new ColumnStyle());
            verticalLayout.Controls.Add(settings, 0, 0);
            verticalLayout.Controls.Add(importButton, 0, 1);
            verticalLayout.Controls.Add(spacer1, 0, 2);
            verticalLayout.Controls.Add(importProgress, 0, 3);
            verticalLayout.Controls.Add(importLog, 0, 4);
            verticalLayout.Dock = DockStyle.Fill;
            verticalLayout.Location = new Point(0, 0);
            verticalLayout.Name = "verticalLayout";
            verticalLayout.RowCount = 5;
            verticalLayout.RowStyles.Add(new RowStyle());
            verticalLayout.RowStyles.Add(new RowStyle());
            verticalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            verticalLayout.RowStyles.Add(new RowStyle());
            verticalLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            verticalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            verticalLayout.Size = new Size(368, 217);
            verticalLayout.TabIndex = 0;
            // 
            // settings
            // 
            settings.AutoSize = true;
            settings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            settings.ColumnCount = 2;
            settings.ColumnStyles.Add(new ColumnStyle());
            settings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            settings.Controls.Add(fpsLabel, 0, 0);
            settings.Controls.Add(fpsValueContainer, 1, 0);
            settings.Controls.Add(subfoldersLabel, 0, 1);
            settings.Controls.Add(subfoldersCheckbox, 1, 1);
            settings.Dock = DockStyle.Fill;
            settings.Location = new Point(3, 3);
            settings.Name = "settings";
            settings.RowCount = 2;
            settings.RowStyles.Add(new RowStyle());
            settings.RowStyles.Add(new RowStyle());
            settings.Size = new Size(362, 54);
            settings.TabIndex = 6;
            // 
            // fpsLabel
            // 
            fpsLabel.AutoSize = true;
            fpsLabel.Dock = DockStyle.Fill;
            fpsLabel.Location = new Point(3, 0);
            fpsLabel.Name = "fpsLabel";
            fpsLabel.Size = new Size(107, 29);
            fpsLabel.TabIndex = 2;
            fpsLabel.Text = "Frames Per Second";
            fpsLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // fpsValueContainer
            // 
            fpsValueContainer.AutoSize = true;
            fpsValueContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            fpsValueContainer.Controls.Add(fpsValue);
            fpsValueContainer.Dock = DockStyle.Fill;
            fpsValueContainer.Location = new Point(116, 3);
            fpsValueContainer.Name = "fpsValueContainer";
            fpsValueContainer.Size = new Size(243, 23);
            fpsValueContainer.TabIndex = 3;
            // 
            // fpsValue
            // 
            fpsValue.AutoSize = true;
            fpsValue.Dock = DockStyle.Fill;
            fpsValue.Location = new Point(0, 0);
            fpsValue.Name = "fpsValue";
            fpsValue.Size = new Size(243, 23);
            fpsValue.TabIndex = 1;
            fpsValue.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // subfoldersLabel
            // 
            subfoldersLabel.AutoSize = true;
            subfoldersLabel.Dock = DockStyle.Fill;
            subfoldersLabel.Location = new Point(3, 29);
            subfoldersLabel.Name = "subfoldersLabel";
            subfoldersLabel.Size = new Size(107, 25);
            subfoldersLabel.TabIndex = 2;
            subfoldersLabel.Text = "Scan";
            subfoldersLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // subfoldersCheckbox
            // 
            subfoldersCheckbox.Dock = DockStyle.Fill;
            subfoldersCheckbox.Location = new Point(116, 32);
            subfoldersCheckbox.Name = "subfoldersCheckbox";
            subfoldersCheckbox.Size = new Size(243, 19);
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
            importButton.Location = new Point(3, 63);
            importButton.Name = "importButton";
            importButton.Size = new Size(362, 25);
            importButton.TabIndex = 0;
            importButton.Text = "Import";
            importButton.UseVisualStyleBackColor = true;
            importButton.Click += importButton_Click;
            // 
            // spacer1
            // 
            spacer1.Dock = DockStyle.Fill;
            spacer1.Location = new Point(3, 94);
            spacer1.Name = "spacer1";
            spacer1.Size = new Size(362, 14);
            spacer1.TabIndex = 8;
            // 
            // importProgress
            // 
            verticalLayout.SetColumnSpan(importProgress, 2);
            importProgress.Dock = DockStyle.Fill;
            importProgress.Location = new Point(3, 114);
            importProgress.Name = "importProgress";
            importProgress.Size = new Size(362, 23);
            importProgress.TabIndex = 3;
            // 
            // importLog
            // 
            importLog.Dock = DockStyle.Fill;
            importLog.Location = new Point(3, 143);
            importLog.Name = "importLog";
            importLog.Size = new Size(362, 71);
            importLog.TabIndex = 7;
            importLog.Text = "";
            // 
            // saveConcatDialogue
            // 
            saveConcatDialogue.DefaultExt = "txt";
            saveConcatDialogue.Filter = "Virtual concatenation script|*.txt|All files|*.*";
            // 
            // FolderImporter
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 217);
            Controls.Add(verticalLayout);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(256, 128);
            Name = "FolderImporter";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Import Multiple";
            verticalLayout.ResumeLayout(false);
            verticalLayout.PerformLayout();
            settings.ResumeLayout(false);
            settings.PerformLayout();
            fpsValueContainer.ResumeLayout(false);
            fpsValueContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)fpsValue).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel verticalLayout;
        private Button importButton;
        private NumericUpDown fpsValue;
        private Label fpsLabel;
        private ProgressBar importProgress;
        private TableLayoutPanel settings;
        private Panel fpsValueContainer;
        private RichTextBox importLog;
        private Panel spacer1;
        private Label subfoldersLabel;
        private CheckBox subfoldersCheckbox;
        private SaveFileDialog saveConcatDialogue;
    }
}