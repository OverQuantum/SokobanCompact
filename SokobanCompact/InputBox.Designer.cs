namespace SokobanCompact
{
    partial class InputBox
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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuOK = new System.Windows.Forms.MenuItem();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.textInputBox = new System.Windows.Forms.TextBox();
            this.labelPromt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuOK);
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            // 
            // menuOK
            // 
            this.menuOK.Text = "OK";
            this.menuOK.Click += new System.EventHandler(this.menuOK_Click);
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // textInputBox
            // 
            this.textInputBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.textInputBox.Location = new System.Drawing.Point(0, 44);
            this.textInputBox.Multiline = true;
            this.textInputBox.Name = "textInputBox";
            this.textInputBox.Size = new System.Drawing.Size(240, 55);
            this.textInputBox.TabIndex = 0;
            // 
            // labelPromt
            // 
            this.labelPromt.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPromt.Location = new System.Drawing.Point(0, 0);
            this.labelPromt.Name = "labelPromt";
            this.labelPromt.Size = new System.Drawing.Size(240, 44);
            // 
            // InputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.textInputBox);
            this.Controls.Add(this.labelPromt);
            this.Menu = this.mainMenu1;
            this.Name = "InputBox";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuOK;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.TextBox textInputBox;
        private System.Windows.Forms.Label labelPromt;
    }
}