namespace SokobanCompact
{
    partial class ChooseLevelSet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseLevelSet));
            this.listLevelSets = new System.Windows.Forms.ListBox();
            this.contextMenuList = new System.Windows.Forms.ContextMenu();
            this.menuSelect = new System.Windows.Forms.MenuItem();
            this.menuSelectAndChoose = new System.Windows.Forms.MenuItem();
            this.menuComment = new System.Windows.Forms.MenuItem();
            this.menuDelete = new System.Windows.Forms.MenuItem();
            this.menuTotalLevels = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuUpdateList = new System.Windows.Forms.MenuItem();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButtonOK = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonAndLevel = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonUpdate = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonMoveTop = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonMoveUp = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonMoveDown = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonMoveBottom = new System.Windows.Forms.ToolBarButton();
            this.toolBarButtonCancel = new System.Windows.Forms.ToolBarButton();
            this.imageList1 = new System.Windows.Forms.ImageList();
            this.menuAddDelimiter = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // listLevelSets
            // 
            this.listLevelSets.ContextMenu = this.contextMenuList;
            this.listLevelSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLevelSets.Location = new System.Drawing.Point(0, 0);
            this.listLevelSets.Name = "listLevelSets";
            this.listLevelSets.Size = new System.Drawing.Size(240, 268);
            this.listLevelSets.TabIndex = 1;
            // 
            // contextMenuList
            // 
            this.contextMenuList.MenuItems.Add(this.menuSelect);
            this.contextMenuList.MenuItems.Add(this.menuSelectAndChoose);
            this.contextMenuList.MenuItems.Add(this.menuComment);
            this.contextMenuList.MenuItems.Add(this.menuAddDelimiter);
            this.contextMenuList.MenuItems.Add(this.menuDelete);
            this.contextMenuList.MenuItems.Add(this.menuTotalLevels);
            this.contextMenuList.MenuItems.Add(this.menuItem5);
            this.contextMenuList.MenuItems.Add(this.menuUpdateList);
            // 
            // menuSelect
            // 
            this.menuSelect.Text = "Select LevelSet";
            this.menuSelect.Click += new System.EventHandler(this.menuSelect_Click);
            // 
            // menuSelectAndChoose
            // 
            this.menuSelectAndChoose.Text = "Select and Choose Level";
            this.menuSelectAndChoose.Click += new System.EventHandler(this.menuSelectAndChoose_Click);
            // 
            // menuComment
            // 
            this.menuComment.Text = "Set Comment";
            this.menuComment.Click += new System.EventHandler(this.menuComment_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Text = "Remove from List";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // menuTotalLevels
            // 
            this.menuTotalLevels.Text = "Total Number of Levels";
            this.menuTotalLevels.Click += new System.EventHandler(this.menuTotalLevels_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Text = "-";
            // 
            // menuUpdateList
            // 
            this.menuUpdateList.Text = "Update List";
            this.menuUpdateList.Click += new System.EventHandler(this.menuUpdateList_Click);
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.Add(this.toolBarButtonOK);
            this.toolBar1.Buttons.Add(this.toolBarButtonAndLevel);
            this.toolBar1.Buttons.Add(this.toolBarButtonUpdate);
            this.toolBar1.Buttons.Add(this.toolBarButtonMoveTop);
            this.toolBar1.Buttons.Add(this.toolBarButtonMoveUp);
            this.toolBar1.Buttons.Add(this.toolBarButtonMoveDown);
            this.toolBar1.Buttons.Add(this.toolBarButtonMoveBottom);
            this.toolBar1.Buttons.Add(this.toolBarButtonCancel);
            this.toolBar1.ImageList = this.imageList1;
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // toolBarButtonOK
            // 
            this.toolBarButtonOK.ImageIndex = 0;
            this.toolBarButtonOK.ToolTipText = "Select";
            // 
            // toolBarButtonAndLevel
            // 
            this.toolBarButtonAndLevel.ImageIndex = 7;
            this.toolBarButtonAndLevel.ToolTipText = "Select and Choose Level";
            // 
            // toolBarButtonUpdate
            // 
            this.toolBarButtonUpdate.ImageIndex = 2;
            this.toolBarButtonUpdate.ToolTipText = "Update list";
            // 
            // toolBarButtonMoveTop
            // 
            this.toolBarButtonMoveTop.ImageIndex = 6;
            this.toolBarButtonMoveTop.ToolTipText = "Move Top";
            // 
            // toolBarButtonMoveUp
            // 
            this.toolBarButtonMoveUp.ImageIndex = 5;
            this.toolBarButtonMoveUp.ToolTipText = "Move Up";
            // 
            // toolBarButtonMoveDown
            // 
            this.toolBarButtonMoveDown.ImageIndex = 4;
            this.toolBarButtonMoveDown.ToolTipText = "Move Down";
            // 
            // toolBarButtonMoveBottom
            // 
            this.toolBarButtonMoveBottom.ImageIndex = 3;
            this.toolBarButtonMoveBottom.ToolTipText = "Move Bottom";
            // 
            // toolBarButtonCancel
            // 
            this.toolBarButtonCancel.ImageIndex = 1;
            this.toolBarButtonCancel.ToolTipText = "Cancel";
            this.imageList1.Images.Clear();
            this.imageList1.Images.Add(((System.Drawing.Icon)(resources.GetObject("resource"))));
            this.imageList1.Images.Add(((System.Drawing.Icon)(resources.GetObject("resource1"))));
            this.imageList1.Images.Add(((System.Drawing.Icon)(resources.GetObject("resource2"))));
            this.imageList1.Images.Add(((System.Drawing.Icon)(resources.GetObject("resource3"))));
            this.imageList1.Images.Add(((System.Drawing.Icon)(resources.GetObject("resource4"))));
            this.imageList1.Images.Add(((System.Drawing.Icon)(resources.GetObject("resource5"))));
            this.imageList1.Images.Add(((System.Drawing.Icon)(resources.GetObject("resource6"))));
            this.imageList1.Images.Add(((System.Drawing.Icon)(resources.GetObject("resource7"))));
            // 
            // menuAddDelimiter
            // 
            this.menuAddDelimiter.Text = "Add Delimiter";
            this.menuAddDelimiter.Click += new System.EventHandler(this.menuAddDelimiter_Click);
            // 
            // ChooseLevelSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.toolBar1);
            this.Controls.Add(this.listLevelSets);
            this.KeyPreview = true;
            this.Name = "ChooseLevelSet";
            this.Text = "ChooseLevelSet";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listLevelSets;
        private System.Windows.Forms.ToolBar toolBar1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolBarButton toolBarButtonMoveTop;
        private System.Windows.Forms.ToolBarButton toolBarButtonMoveUp;
        private System.Windows.Forms.ToolBarButton toolBarButtonMoveDown;
        private System.Windows.Forms.ToolBarButton toolBarButtonMoveBottom;
        private System.Windows.Forms.ToolBarButton toolBarButtonCancel;
        private System.Windows.Forms.ToolBarButton toolBarButtonOK;
        private System.Windows.Forms.ToolBarButton toolBarButtonAndLevel;
        private System.Windows.Forms.ContextMenu contextMenuList;
        private System.Windows.Forms.MenuItem menuSelect;
        private System.Windows.Forms.MenuItem menuSelectAndChoose;
        private System.Windows.Forms.MenuItem menuComment;
        private System.Windows.Forms.MenuItem menuDelete;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuUpdateList;
        private System.Windows.Forms.ToolBarButton toolBarButtonUpdate;
        private System.Windows.Forms.MenuItem menuTotalLevels;
        private System.Windows.Forms.MenuItem menuAddDelimiter;
    }
}