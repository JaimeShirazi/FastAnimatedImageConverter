namespace FAIC
{
    partial class CustomArguments
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomArguments));
            arguments = new RichTextBox();
            confirm = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            title = new Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // arguments
            // 
            arguments.DetectUrls = false;
            arguments.Dock = DockStyle.Fill;
            arguments.Location = new Point(6, 26);
            arguments.Name = "arguments";
            arguments.ScrollBars = RichTextBoxScrollBars.Vertical;
            arguments.Size = new Size(484, 156);
            arguments.TabIndex = 1;
            arguments.Text = "";
            // 
            // confirm
            // 
            confirm.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            confirm.Location = new Point(6, 188);
            confirm.Name = "confirm";
            confirm.Size = new Size(484, 23);
            confirm.TabIndex = 2;
            confirm.Text = "Confirm";
            confirm.UseVisualStyleBackColor = true;
            confirm.Click += confirm_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(title, 0, 0);
            tableLayoutPanel1.Controls.Add(arguments, 0, 1);
            tableLayoutPanel1.Controls.Add(confirm, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(3);
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(496, 217);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // title
            // 
            title.AutoSize = true;
            title.Dock = DockStyle.Fill;
            title.Location = new Point(6, 3);
            title.Name = "title";
            title.Size = new Size(484, 20);
            title.TabIndex = 0;
            title.Text = "Edit the ffmpeg arguments in the text box below.";
            title.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CustomArguments
            // 
            AcceptButton = confirm;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(496, 217);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CustomArguments";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Advanced Arguments Editor";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox arguments;
        private Button confirm;
        private TableLayoutPanel tableLayoutPanel1;
        private Label title;
    }
}