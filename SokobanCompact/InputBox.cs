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
    /// <summary>Form InputBox - for promting user to input something</summary>
    public partial class InputBox : Form
    {
        /// <summary>Typed value, may be initialized with default value</summary>
        private string sText;

        /// <summary>Void constructor</summary>
        public InputBox()
        {
            InitializeComponent();
        }

        /// <summary>Activate form for inputing (default text, title of form, promt/description)</summary>
        public DialogResult AskUser(string sDefText, string sTitle, string sPromt)
        {
            sText = sDefText;//Copy parameters into internal variables and interface
            textInputBox.Text = sText;
            labelPromt.Text = sPromt;
            Text = sTitle;
            textInputBox.Top = labelPromt.Height;

            DialogResult = DialogResult.Cancel;//Default dialog result

            DialogResult bRv = ShowDialog();//Activate form in modal mode

            Dispose();//Release resources of dialog

            return bRv;//Return result status of dialog
        }

        /// <summary>Get typed text</summary>
        public string GetResult()
        {
            return sText;
        }

        /// <summary>Menu "OK" clicked</summary>
        private void menuOK_Click(object sender, EventArgs e)
        {
            //Copy typed text from interface and close dialog with success
            sText = textInputBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>Menu "Cancel" clicked</summary>
        private void menuCancel_Click(object sender, EventArgs e)
        {
            //Close dialog with cancel
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}