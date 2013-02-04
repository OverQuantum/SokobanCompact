/*
 * This file is a part of SokobanCompact
 * 
 * Copyright © 2008-2013 OverQuantum
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * Author contacts:
 * http://overquantum.livejournal.com
 * https://github.com/OverQuantum
 * 
 * Project homepage:
 * https://github.com/OverQuantum/SokobanCompact
 * 
 */

using System;
using System.Windows.Forms;

namespace SokobanCompact
{
    /// <summary>Form Settings for change game global settings</summary>
    public partial class formSettings : Form
    {
        /// <summary>Handle to global settings</summary>
        private Settings uSettings;

        /// <summary>Void constructor</summary>
        public formSettings()
        {
            InitializeComponent();
        }

        /// <summary>Activate form for changing settings (glonal setting object)</summary>
        public DialogResult ChangeSettings(Settings uSokoSettings)
        {
            uSettings = uSokoSettings;//Copy handle to global settings

            //Copy settings into controls of form
            textPlayerName.Text = uSettings.sPlayerName;
            checkAdditionalMessages.Checked = uSettings.bAdditionalMessages;
            checkAnimateBoxPushing.Checked = uSettings.bAnimateBoxPushing;
            checkAnimateMassUndoRedo.Checked = uSettings.bAnimateMassUndoRedo;
            checkAnimateTravel.Checked = uSettings.bAnimateTravel;
            checkAskRecordName.Checked = uSettings.bAskRecordName;
            checkAskSavingFirstSolution.Checked = uSettings.bAskSavingFirstSolution;
            checkAutocalcDeadlocks.Checked = uSettings.bAutocalcDeadlocks;
            checkDeadlockLimitsAutopush.Checked = uSettings.bDeadlockLimitsAutopush;
            checkAutoSize.Checked = uSettings.bAutosize;
            checkAutosizeUseful.Checked = uSettings.bAutosizeUseful;
            checkLogActions.Checked = uSettings.bLogActions;
            numericAntiSuspend.Value = uSettings.iKeepAliveMinutes;
            numericMinAutosize.Value = uSettings.iAutosizeLowerLimit;
            numericAnimationTravelDelay.Value = uSettings.iAnimationDelayTravel;
            numericAnimationBoxPushingDelay.Value = uSettings.iAnimationDelayBoxPushing;
            numericAnimationMassUndoRedoDelay.Value = uSettings.iAnimationDelayMassUndoRedo;
            numericDragMinMove.Value = uSettings.iDragMinMove;
            numericBackgroundAutoDeadlocks.Value = uSettings.iBackgroundAutoDeadlocksLimit;
            //textBackColor.Text = uSettings.iBackgroundColor.ToString("X").PadLeft(6, '0');
            
            DialogResult uRV = ShowDialog();//Activate form in modal mode

            Dispose();//Release resources of dialog
            return uRV;
        }

        /// <summary>Menu "OK" clicked</summary>
        private void menuItemOK_Click(object sender, EventArgs e)
        {
            //Copy all settings from controls into global settings object
            uSettings.sPlayerName = textPlayerName.Text;
            uSettings.bAdditionalMessages = checkAdditionalMessages.Checked;
            uSettings.bAnimateBoxPushing = checkAnimateBoxPushing.Checked;
            uSettings.bAnimateMassUndoRedo = checkAnimateMassUndoRedo.Checked;
            uSettings.bAnimateTravel = checkAnimateTravel.Checked;
            uSettings.bAskRecordName = checkAskRecordName.Checked;
            uSettings.bAskSavingFirstSolution = checkAskSavingFirstSolution.Checked;
            uSettings.bAutocalcDeadlocks = checkAutocalcDeadlocks.Checked;
            uSettings.bDeadlockLimitsAutopush = checkDeadlockLimitsAutopush.Checked;
            uSettings.bAutosize = checkAutoSize.Checked;
            uSettings.bAutosizeUseful = checkAutosizeUseful.Checked;
            uSettings.iKeepAliveMinutes = (int)numericAntiSuspend.Value;
            uSettings.iAutosizeLowerLimit = (int)numericMinAutosize.Value;
            uSettings.iAnimationDelayTravel = (int)numericAnimationTravelDelay.Value;
            uSettings.iAnimationDelayBoxPushing = (int)numericAnimationBoxPushingDelay.Value;
            uSettings.iAnimationDelayMassUndoRedo = (int)numericAnimationMassUndoRedoDelay.Value;
            uSettings.iDragMinMove = (int)numericDragMinMove.Value;
            uSettings.iBackgroundAutoDeadlocksLimit = (int)numericBackgroundAutoDeadlocks.Value;
            uSettings.bLogActions = checkLogActions.Checked;

            /*iBackgroundColor_bk = uSettings.iBackgroundColor;
            uSettings.iBackgroundColor = OQConvertTools.hex2int(textBackColor.Text);
            if (uSettings.iBackgroundColor == -1)
                uSettings.iBackgroundColor = 0;//OQConvertTools.hex2int return -1 on errors

            if (iBackgroundColor_bk != uSettings.iBackgroundColor)
                bBackColorChanged = true;*/

            //Close form with success
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>Menu "Cancel" clicked</summary>
        private void menuItemCancel_Click(object sender, EventArgs e)
        {
            //Close form with cancel
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void formSettings_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                // Up
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                // Down
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // Left
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // Right
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                // Enter
            }

        }
    }
}