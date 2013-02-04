namespace SokobanCompact
{
    partial class OpenFile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.menuOpen = new System.Windows.Forms.MenuItem();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.listFileList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuOpen);
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            // 
            // menuOpen
            // 
            this.menuOpen.Text = "Open";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // listFileList
            // 
            this.listFileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listFileList.Location = new System.Drawing.Point(0, 0);
            this.listFileList.Name = "listFileList";
            this.listFileList.Size = new System.Drawing.Size(240, 268);
            this.listFileList.TabIndex = 0;
            this.listFileList.SelectedIndexChanged += new System.EventHandler(this.listFileList_SelectedIndexChanged);
            // 
            // OpenFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.listFileList);
            this.Menu = this.mainMenu1;
            this.Name = "OpenFile";
            this.Text = "OpenFile";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listFileList;
        private System.Windows.Forms.MenuItem menuOpen;
        private System.Windows.Forms.MenuItem menuCancel;
    }
}