namespace SokobanCompact
{
    partial class formShowInfo
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
            this.textInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textInfo
            // 
            this.textInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textInfo.Location = new System.Drawing.Point(0, 0);
            this.textInfo.Multiline = true;
            this.textInfo.Name = "textInfo";
            this.textInfo.ReadOnly = true;
            this.textInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textInfo.Size = new System.Drawing.Size(240, 268);
            this.textInfo.TabIndex = 0;
            this.textInfo.Text = "(info)";
            this.textInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // formShowInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.textInfo);
            this.Menu = this.mainMenu1;
            this.Name = "formShowInfo";
            this.Text = "Info";
            this.Deactivate += new System.EventHandler(this.formShowInfo_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textInfo;

    }
}