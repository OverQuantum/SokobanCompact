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
    /// <summary>Form ShowInfo - displaying large text information</summary>
    public partial class formShowInfo : Form
    {
        /// <summary>Void constructor</summary>
        public formShowInfo()
        {
            InitializeComponent();
        }

        /// <summary>Activate form with displaying text (text, title of form)</summary>
        public void ShowInfoText(string sText, string sTitle)
        {
            textInfo.Text = sText;//Copy text to interface
            Text = sTitle;//Copy title
            ShowDialog();//Activate form in modal mode
            Dispose();//Release resources of dialog
        }

        /// <summary>Something other activated, "X" pressed or else</summary>
        private void formShowInfo_Deactivate(object sender, EventArgs e)
        {
            Close();//Close form
        }

    }
}