namespace SokobanCompact
{
    partial class ChooseLevel
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
            this.menuSelect = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuShowUnsolved = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.listLevels = new System.Windows.Forms.ListBox();
            this.menuLevelSet = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuSelect);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuSelect
            // 
            this.menuSelect.Text = "Select";
            this.menuSelect.Click += new System.EventHandler(this.menuSelect_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.MenuItems.Add(this.menuShowUnsolved);
            this.menuItem2.MenuItems.Add(this.menuItem6);
            this.menuItem2.MenuItems.Add(this.menuLevelSet);
            this.menuItem2.MenuItems.Add(this.menuCancel);
            this.menuItem2.Text = "Menu";
            // 
            // menuShowUnsolved
            // 
            this.menuShowUnsolved.Text = "Show unsolved";
            this.menuShowUnsolved.Click += new System.EventHandler(this.menuShowUnsolved_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Text = "-";
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // listLevels
            // 
            this.listLevels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLevels.Location = new System.Drawing.Point(0, 0);
            this.listLevels.Name = "listLevels";
            this.listLevels.Size = new System.Drawing.Size(240, 268);
            this.listLevels.TabIndex = 0;
            this.listLevels.SelectedIndexChanged += new System.EventHandler(this.listLevels_SelectedIndexChanged);
            // 
            // menuLevelSet
            // 
            this.menuLevelSet.Text = "Change LevelSet";
            this.menuLevelSet.Click += new System.EventHandler(this.menuLevelSet_Click);
            // 
            // ChooseLevel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.listLevels);
            this.Menu = this.mainMenu1;
            this.Name = "ChooseLevel";
            this.Text = "Choose Level";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listLevels;
        private System.Windows.Forms.MenuItem menuSelect;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuShowUnsolved;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.MenuItem menuLevelSet;
    }
}