namespace SokobanCompact
{
    partial class formSettings
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
            this.menuItemOK = new System.Windows.Forms.MenuItem();
            this.menuItemCancel = new System.Windows.Forms.MenuItem();
            this.textPlayerName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkAskRecordName = new System.Windows.Forms.CheckBox();
            this.checkAdditionalMessages = new System.Windows.Forms.CheckBox();
            this.checkAnimateTravel = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkAnimateBoxPushing = new System.Windows.Forms.CheckBox();
            this.checkAnimateMassUndoRedo = new System.Windows.Forms.CheckBox();
            this.checkAskSavingFirstSolution = new System.Windows.Forms.CheckBox();
            this.checkAutocalcDeadlocks = new System.Windows.Forms.CheckBox();
            this.checkDeadlockLimitsAutopush = new System.Windows.Forms.CheckBox();
            this.numericMinAutosize = new System.Windows.Forms.NumericUpDown();
            this.checkAutoSize = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericAntiSuspend = new System.Windows.Forms.NumericUpDown();
            this.numericAnimationTravelDelay = new System.Windows.Forms.NumericUpDown();
            this.numericAnimationBoxPushingDelay = new System.Windows.Forms.NumericUpDown();
            this.numericAnimationMassUndoRedoDelay = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numericDragMinMove = new System.Windows.Forms.NumericUpDown();
            this.checkAutosizeUseful = new System.Windows.Forms.CheckBox();
            this.labelScreenKeyboardPlaceholder_DoNotDelete = new System.Windows.Forms.Label();
            this.checkLogActions = new System.Windows.Forms.CheckBox();
            this.numericBackgroundAutoDeadlocks = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItemOK);
            this.mainMenu1.MenuItems.Add(this.menuItemCancel);
            // 
            // menuItemOK
            // 
            this.menuItemOK.Text = "OK";
            this.menuItemOK.Click += new System.EventHandler(this.menuItemOK_Click);
            // 
            // menuItemCancel
            // 
            this.menuItemCancel.Text = "Cancel";
            this.menuItemCancel.Click += new System.EventHandler(this.menuItemCancel_Click);
            // 
            // textPlayerName
            // 
            this.textPlayerName.Location = new System.Drawing.Point(3, 20);
            this.textPlayerName.Name = "textPlayerName";
            this.textPlayerName.Size = new System.Drawing.Size(224, 21);
            this.textPlayerName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 17);
            this.label1.Text = "Player name:";
            // 
            // checkAskRecordName
            // 
            this.checkAskRecordName.Location = new System.Drawing.Point(3, 47);
            this.checkAskRecordName.Name = "checkAskRecordName";
            this.checkAskRecordName.Size = new System.Drawing.Size(224, 20);
            this.checkAskRecordName.TabIndex = 1;
            this.checkAskRecordName.Text = "Ask name of each record";
            // 
            // checkAdditionalMessages
            // 
            this.checkAdditionalMessages.Location = new System.Drawing.Point(3, 99);
            this.checkAdditionalMessages.Name = "checkAdditionalMessages";
            this.checkAdditionalMessages.Size = new System.Drawing.Size(224, 20);
            this.checkAdditionalMessages.TabIndex = 2;
            this.checkAdditionalMessages.Text = "Display verbose messages";
            // 
            // checkAnimateTravel
            // 
            this.checkAnimateTravel.Location = new System.Drawing.Point(17, 348);
            this.checkAnimateTravel.Name = "checkAnimateTravel";
            this.checkAnimateTravel.Size = new System.Drawing.Size(145, 20);
            this.checkAnimateTravel.TabIndex = 3;
            this.checkAnimateTravel.Text = "Player traveling";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(0, 328);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 17);
            this.label2.Text = "Animation:";
            // 
            // checkAnimateBoxPushing
            // 
            this.checkAnimateBoxPushing.Location = new System.Drawing.Point(17, 374);
            this.checkAnimateBoxPushing.Name = "checkAnimateBoxPushing";
            this.checkAnimateBoxPushing.Size = new System.Drawing.Size(145, 20);
            this.checkAnimateBoxPushing.TabIndex = 4;
            this.checkAnimateBoxPushing.Text = "Box pushing";
            // 
            // checkAnimateMassUndoRedo
            // 
            this.checkAnimateMassUndoRedo.Location = new System.Drawing.Point(17, 400);
            this.checkAnimateMassUndoRedo.Name = "checkAnimateMassUndoRedo";
            this.checkAnimateMassUndoRedo.Size = new System.Drawing.Size(145, 20);
            this.checkAnimateMassUndoRedo.TabIndex = 5;
            this.checkAnimateMassUndoRedo.Text = "Undo and Redo";
            // 
            // checkAskSavingFirstSolution
            // 
            this.checkAskSavingFirstSolution.Location = new System.Drawing.Point(3, 73);
            this.checkAskSavingFirstSolution.Name = "checkAskSavingFirstSolution";
            this.checkAskSavingFirstSolution.Size = new System.Drawing.Size(224, 20);
            this.checkAskSavingFirstSolution.TabIndex = 8;
            this.checkAskSavingFirstSolution.Text = "Ask about saving first solution";
            // 
            // checkAutocalcDeadlocks
            // 
            this.checkAutocalcDeadlocks.Location = new System.Drawing.Point(3, 125);
            this.checkAutocalcDeadlocks.Name = "checkAutocalcDeadlocks";
            this.checkAutocalcDeadlocks.Size = new System.Drawing.Size(224, 20);
            this.checkAutocalcDeadlocks.TabIndex = 11;
            this.checkAutocalcDeadlocks.Text = "Autocalculate deadlocks";
            // 
            // checkDeadlockLimitsAutopush
            // 
            this.checkDeadlockLimitsAutopush.Location = new System.Drawing.Point(3, 176);
            this.checkDeadlockLimitsAutopush.Name = "checkDeadlockLimitsAutopush";
            this.checkDeadlockLimitsAutopush.Size = new System.Drawing.Size(224, 20);
            this.checkDeadlockLimitsAutopush.TabIndex = 12;
            this.checkDeadlockLimitsAutopush.Text = "Deadlocks limits autopushing";
            // 
            // numericMinAutosize
            // 
            this.numericMinAutosize.Location = new System.Drawing.Point(121, 225);
            this.numericMinAutosize.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericMinAutosize.Name = "numericMinAutosize";
            this.numericMinAutosize.Size = new System.Drawing.Size(75, 22);
            this.numericMinAutosize.TabIndex = 15;
            this.numericMinAutosize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // checkAutoSize
            // 
            this.checkAutoSize.Location = new System.Drawing.Point(3, 202);
            this.checkAutoSize.Name = "checkAutoSize";
            this.checkAutoSize.Size = new System.Drawing.Size(224, 20);
            this.checkAutoSize.TabIndex = 16;
            this.checkAutoSize.Text = "Autosize skin on level change";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 228);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 19);
            this.label3.Text = "Minimum size:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(3, 280);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(159, 19);
            this.label4.Text = "Delay device suspend, min:";
            // 
            // numericAntiSuspend
            // 
            this.numericAntiSuspend.Location = new System.Drawing.Point(168, 277);
            this.numericAntiSuspend.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericAntiSuspend.Name = "numericAntiSuspend";
            this.numericAntiSuspend.Size = new System.Drawing.Size(56, 22);
            this.numericAntiSuspend.TabIndex = 20;
            this.numericAntiSuspend.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numericAnimationTravelDelay
            // 
            this.numericAnimationTravelDelay.Location = new System.Drawing.Point(168, 348);
            this.numericAnimationTravelDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericAnimationTravelDelay.Name = "numericAnimationTravelDelay";
            this.numericAnimationTravelDelay.Size = new System.Drawing.Size(56, 22);
            this.numericAnimationTravelDelay.TabIndex = 25;
            this.numericAnimationTravelDelay.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numericAnimationBoxPushingDelay
            // 
            this.numericAnimationBoxPushingDelay.Location = new System.Drawing.Point(168, 372);
            this.numericAnimationBoxPushingDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericAnimationBoxPushingDelay.Name = "numericAnimationBoxPushingDelay";
            this.numericAnimationBoxPushingDelay.Size = new System.Drawing.Size(56, 22);
            this.numericAnimationBoxPushingDelay.TabIndex = 26;
            this.numericAnimationBoxPushingDelay.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numericAnimationMassUndoRedoDelay
            // 
            this.numericAnimationMassUndoRedoDelay.Location = new System.Drawing.Point(168, 395);
            this.numericAnimationMassUndoRedoDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericAnimationMassUndoRedoDelay.Name = "numericAnimationMassUndoRedoDelay";
            this.numericAnimationMassUndoRedoDelay.Size = new System.Drawing.Size(56, 22);
            this.numericAnimationMassUndoRedoDelay.TabIndex = 27;
            this.numericAnimationMassUndoRedoDelay.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(107, 328);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 17);
            this.label5.Text = "Speed, ms/frame:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(3, 305);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(159, 19);
            this.label6.Text = "Drag-n-Drop sensivity, pix:";
            // 
            // numericDragMinMove
            // 
            this.numericDragMinMove.Location = new System.Drawing.Point(168, 302);
            this.numericDragMinMove.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericDragMinMove.Name = "numericDragMinMove";
            this.numericDragMinMove.Size = new System.Drawing.Size(56, 22);
            this.numericDragMinMove.TabIndex = 34;
            this.numericDragMinMove.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // checkAutosizeUseful
            // 
            this.checkAutosizeUseful.Location = new System.Drawing.Point(3, 253);
            this.checkAutosizeUseful.Name = "checkAutosizeUseful";
            this.checkAutosizeUseful.Size = new System.Drawing.Size(221, 20);
            this.checkAutosizeUseful.TabIndex = 41;
            this.checkAutosizeUseful.Text = "Autosize/recenter by useful cells";
            // 
            // labelScreenKeyboardPlaceholder_DoNotDelete
            // 
            this.labelScreenKeyboardPlaceholder_DoNotDelete.Location = new System.Drawing.Point(0, 449);
            this.labelScreenKeyboardPlaceholder_DoNotDelete.Name = "labelScreenKeyboardPlaceholder_DoNotDelete";
            this.labelScreenKeyboardPlaceholder_DoNotDelete.Size = new System.Drawing.Size(224, 81);
            this.labelScreenKeyboardPlaceholder_DoNotDelete.Text = " ";
            this.labelScreenKeyboardPlaceholder_DoNotDelete.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // checkLogActions
            // 
            this.checkLogActions.Location = new System.Drawing.Point(3, 426);
            this.checkLogActions.Name = "checkLogActions";
            this.checkLogActions.Size = new System.Drawing.Size(221, 20);
            this.checkLogActions.TabIndex = 48;
            this.checkLogActions.Text = "Log all actions";
            // 
            // numericBackgroundAutoDeadlocks
            // 
            this.numericBackgroundAutoDeadlocks.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericBackgroundAutoDeadlocks.Location = new System.Drawing.Point(151, 151);
            this.numericBackgroundAutoDeadlocks.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericBackgroundAutoDeadlocks.Name = "numericBackgroundAutoDeadlocks";
            this.numericBackgroundAutoDeadlocks.Size = new System.Drawing.Size(73, 22);
            this.numericBackgroundAutoDeadlocks.TabIndex = 56;
            this.numericBackgroundAutoDeadlocks.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(3, 154);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 19);
            this.label7.Text = "in backgr. if level larger";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // formSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.ControlBox = false;
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numericBackgroundAutoDeadlocks);
            this.Controls.Add(this.checkLogActions);
            this.Controls.Add(this.labelScreenKeyboardPlaceholder_DoNotDelete);
            this.Controls.Add(this.checkAutosizeUseful);
            this.Controls.Add(this.numericDragMinMove);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericAnimationMassUndoRedoDelay);
            this.Controls.Add(this.numericAnimationBoxPushingDelay);
            this.Controls.Add(this.numericAnimationTravelDelay);
            this.Controls.Add(this.numericAntiSuspend);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkAutoSize);
            this.Controls.Add(this.numericMinAutosize);
            this.Controls.Add(this.checkDeadlockLimitsAutopush);
            this.Controls.Add(this.checkAutocalcDeadlocks);
            this.Controls.Add(this.checkAskSavingFirstSolution);
            this.Controls.Add(this.checkAnimateMassUndoRedo);
            this.Controls.Add(this.checkAnimateBoxPushing);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkAnimateTravel);
            this.Controls.Add(this.checkAdditionalMessages);
            this.Controls.Add(this.checkAskRecordName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textPlayerName);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "formSettings";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.formSettings_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItemOK;
        private System.Windows.Forms.MenuItem menuItemCancel;
        private System.Windows.Forms.TextBox textPlayerName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkAskRecordName;
        private System.Windows.Forms.CheckBox checkAdditionalMessages;
        private System.Windows.Forms.CheckBox checkAnimateTravel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkAnimateBoxPushing;
        private System.Windows.Forms.CheckBox checkAnimateMassUndoRedo;
        private System.Windows.Forms.CheckBox checkAskSavingFirstSolution;
        private System.Windows.Forms.CheckBox checkAutocalcDeadlocks;
        private System.Windows.Forms.CheckBox checkDeadlockLimitsAutopush;
        private System.Windows.Forms.NumericUpDown numericMinAutosize;
        private System.Windows.Forms.CheckBox checkAutoSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericAntiSuspend;
        private System.Windows.Forms.NumericUpDown numericAnimationTravelDelay;
        private System.Windows.Forms.NumericUpDown numericAnimationBoxPushingDelay;
        private System.Windows.Forms.NumericUpDown numericAnimationMassUndoRedoDelay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericDragMinMove;
        private System.Windows.Forms.CheckBox checkAutosizeUseful;
        private System.Windows.Forms.Label labelScreenKeyboardPlaceholder_DoNotDelete;
        private System.Windows.Forms.CheckBox checkLogActions;
        private System.Windows.Forms.NumericUpDown numericBackgroundAutoDeadlocks;
        private System.Windows.Forms.Label label7;
    }
}