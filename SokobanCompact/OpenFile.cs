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
    /// <summary>Form OpenFile - simple file choosing dialog</summary>
    public partial class OpenFile : Form
    {
        /// <summary>Name of selected file, may be preinitialized for highlighting file</summary>
        public string sFileName;

        /// <summary>Path from where list of files are loaded</summary>
        public string sFolder;

        /// <summary>Title of dialog</summary>
        public string sTitle;

        /// <summary>Filter for files in wildcards form</summary>
        public string sFilter;

        /// <summary>Default constructor</summary>
        public OpenFile()
        {
            sTitle = "Open File";//Default values, could be changed externally
            sFilter = "*.*";
            sFileName = "";
            sFolder = "";

            InitializeComponent();
        }

        /// <summary>Activate form for choosing file</summary>
        public DialogResult SelectFileForLoad()
        {
            string sItem;

            string[] sList = System.IO.Directory.GetFiles(sFolder, sFilter);//Get list of files from specified folder with using filter
            Array.Sort(sList);//Sort files alphabetically
            listFileList.Items.Clear();//Clean list box

            for (int i = 0; i < sList.Length; i++)
            {
                sItem = System.IO.Path.GetFileName(sList[i]).ToLower();//String description - filename
                listFileList.Items.Add(sItem);//Add item to list
                if (sItem == sFileName)//Found file, that was specified externally
                    listFileList.SelectedIndex = listFileList.Items.Count - 1;//Highlight item with selection
            }

            Text = sTitle;//Copy title of dialog

            DialogResult = DialogResult.Cancel;//Default dialog result

            DialogResult bRv = ShowDialog();//Activate form in modal mode

            Dispose();//Release resources of dialog

            return bRv;
        }

        /// <summary>Selection changed</summary>
        private void listFileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            sFileName = listFileList.Items[listFileList.SelectedIndex].ToString();
        }

        /// <summary>Menu "Open" clicked</summary>
        private void menuOpen_Click(object sender, EventArgs e)
        {
            if (listFileList.SelectedIndex >= 0 && listFileList.SelectedIndex < listFileList.Items.Count)
            {   //If some item is actually selected - close dialog with success
                DialogResult = DialogResult.OK;
                Close();
            }
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