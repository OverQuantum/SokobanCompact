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
    /// <summary>Form for selecting levelset from list</summary>
    public partial class ChooseLevelSet : Form
    {
        /// <summary>List of levelsets</summary>
        private SokobanLevelSetList uList;

        /// <summary>Index of selected levelset</summary>
        public int iSelectedSet;

        /// <summary>Filename of selected levelset</summary>
        public string sSelectedSet;

        /// <summary>Void constructor</summary>
        public ChooseLevelSet()
        {
            InitializeComponent();
        }

        /// <summary>Prepare listbox with list of levelsets</summary>
        private void Enlist()
        {
            listLevelSets.Items.Clear();//Clean listbox

            for (int i = 0; i < uList.iListUsed ; i++)//Iterate thru all levelsets
            {
                listLevelSets.Items.Add(uList.uLevelSets[i]);//Add levelset description to listbox
                if (iSelectedSet == i)
                    //CHG 2010.02.12, dont know why it was via IndexOf
                    //listLevelSets.SelectedIndex = listLevelSets.Items.IndexOf(uList.uLevelSets[i]);//Highlight selected levelset in list
                    listLevelSets.SelectedIndex = i;//Highlight selected levelset in list
            }
        }

        private void UpdateSelected()
        {
            uList.FindLevelSet(uList.GetCurrentLevelSet());
        }

        /// <summary>Activate form for selecting levelset (list of levelsets)</summary>
        public DialogResult SelectLevelSet(SokobanLevelSetList uLevelSetList)
        {
            uList = uLevelSetList;//Store handle to levelset list
            iSelectedSet = uList.GetCurrentLevelSetIndex();//Get currently loaded levelset

            Enlist();//Fill listbox for user to choose

            if (Environment.OSVersion.Platform==PlatformID.WinCE)
                Visible = true;//Activate form (used only for WinCE, because of crush on Win32)

            if (uList.iListUsed == 0)
            {   //Empty levelset list - automatically fill
                //MessageBox.Show("LevelSet List are empty, press Update button to search for LevelSets", "LevelSet List");
                ActionUpdateList();
            }

            DialogResult = DialogResult.Cancel;//Default result of choosing
            sSelectedSet = "";

            DialogResult bRv = ShowDialog();//Activate choosing in modal mode

            uList.SaveList();//Save of levelsets (for cases, then it was updated, or sorted, or number of solved levels changed)

            Dispose();//Release resources of dialog

            return bRv;
        }

        private void ActionUpdateList()
        {
            Cursor.Current = Cursors.WaitCursor;//Indication of "please wait" (hourglass or something)
            uList.UpdateList();//Update list
            uList.SaveList();//Save updated list
            UpdateSelected();
            iSelectedSet = uList.GetCurrentLevelSetIndex();//Get currently loaded levelset
            Enlist();//Update listbox with new list
            Cursor.Current = Cursors.Default;//Restore original cursor
            return;
        }

        /// <summary>Get index and filename of selected levelset from listbox</summary>
        private void GetSelected()
        {
            iSelectedSet = listLevelSets.SelectedIndex;//Get index of selected item in listbox
            if (iSelectedSet < 0 || iSelectedSet >= listLevelSets.Items.Count)
            {   //Outside of listbox range
                sSelectedSet = "";//No levelset is selected
                iSelectedSet = -1;
                return;
            }
            sSelectedSet = ((LevelSetDescr)(listLevelSets.Items[iSelectedSet])).sFileName;//Get filename from listbox description
        }

        private void ActionSelect()
        {
            GetSelected();
            if (iSelectedSet >= 0)
            {   //If something selected
                if (uList.uLevelSets[iSelectedSet].sFileName.Length < 1) //check for delimiter
                {
                    MessageBox.Show("This is delimiter, not levelset", "Selecting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);//warn user
                    return;
                }
                //close dialog with success
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        private void ActionSelectAndLevel()
        {
            GetSelected();
            if (iSelectedSet >= 0)
            {   //If something selected
                if (uList.uLevelSets[iSelectedSet].sFileName.Length < 1) //check for delimiter
                {
                    MessageBox.Show("This is delimiter, not levelset", "Selecting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);//warn user
                    return;
                }
                //close dialog with indiction to open level selection dialog
                DialogResult = DialogResult.Retry;
                Close();
            }
        }

        /// <summary>Button on toolbar was pressed</summary>
        private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button == toolBarButtonOK)
            {   //OK button - levelset selected
                ActionSelect();
            }
            else if (e.Button == toolBarButtonAndLevel)
            {   //OK-with-choosing level - levelset selected, but user want to select level afterward
                ActionSelectAndLevel();
            }
            else if (e.Button == toolBarButtonCancel)
            {   //Cancel - user refuse selecting
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }
            else if (e.Button == toolBarButtonUpdate)
            {   //Update - need to refresh list of levelsets
                ActionUpdateList();
                return;
            }
            else if (e.Button == toolBarButtonMoveTop)
            {   //Move current levelset to top of list
                GetSelected();
                if (iSelectedSet > 0)
                {   //If something selected and it is not first element
                    LevelSetDescr uTemp = uList.uLevelSets[iSelectedSet];//Temp. description - store selected levelset
                    for (int i = iSelectedSet;i>0;i--)
                        uList.uLevelSets[i] = uList.uLevelSets[i-1]; //Move down all element from first to current
                    uList.uLevelSets[0] = uTemp;//Put selected levelset to top
                    iSelectedSet=0;
                    UpdateSelected();
                    Enlist();//Update listbox with new list
                }
                return;
            }
            else if (e.Button == toolBarButtonMoveBottom)
            {   //Move current levelset to bottom of list
                GetSelected();
                if (iSelectedSet >= 0 && iSelectedSet < (uList.iListUsed - 1))
                {   //If something selected and it is not last element
                    LevelSetDescr uTemp = uList.uLevelSets[iSelectedSet];//Temp. description - store selected levelset
                    for (int i = iSelectedSet; i < (uList.iListUsed-1); i++)
                        uList.uLevelSets[i] = uList.uLevelSets[i + 1];//Move up all element from current to last
                    uList.uLevelSets[uList.iListUsed - 1] = uTemp;//Put selected levelset to bottom
                    iSelectedSet = uList.iListUsed - 1;
                    UpdateSelected();
                    Enlist();//Update listbox with new list
                }
                return;
            }
            else if (e.Button == toolBarButtonMoveUp)
            {   //Move current levelset up one step
                GetSelected();
                if (iSelectedSet > 0)
                {   //If something selected and it is not first element

                    //Swap selected levelset and previous
                    LevelSetDescr uTemp = uList.uLevelSets[iSelectedSet];
                    uList.uLevelSets[iSelectedSet] = uList.uLevelSets[iSelectedSet - 1];
                    uList.uLevelSets[iSelectedSet - 1] = uTemp;
                    
                    iSelectedSet--;
                    UpdateSelected();
                    Enlist();//Update listbox with new list
                }
                return;
            }
            else if (e.Button == toolBarButtonMoveDown)
            {   //Move current levelset down one step
                GetSelected();
                if (iSelectedSet >= 0 && iSelectedSet<(uList.iListUsed-1))
                {   //If something selected and it is not last element

                    //Swap selected levelset and next
                    LevelSetDescr uTemp = uList.uLevelSets[iSelectedSet];
                    uList.uLevelSets[iSelectedSet] = uList.uLevelSets[iSelectedSet + 1];
                    uList.uLevelSets[iSelectedSet + 1] = uTemp;
                    
                    iSelectedSet++;
                    UpdateSelected();
                    Enlist();//Update listbox with new list
                }
                return;
            }
            /*else if (e.Button == toolBarButtonComment)
            {   //Comment - change user comment for levelset
                GetSelected();
                if (iSelectedSet >= 0)
                {   //If something selected
                    InputBox uBox = new InputBox();//Use input box
                    if (uBox.AskUser(uList.uLevelSets[iSelectedSet].sComment, "LevelSet comment", "Your comment:") == DialogResult.OK) //Ask user about new comment
                    {   //If new comment confirmed
                        uList.uLevelSets[iSelectedSet].sComment = uBox.GetResult();//Update comment
                        uList.UpdateDisplayed(iSelectedSet);//Update displayed text (as comment changed)
                        Enlist();//Update listbox
                    }
                }
                return;
            }
            else if (e.Button == toolBarButtonDelete)
            {   //Delete - remove levelset from list
                GetSelected();
                if (iSelectedSet >= 0)
                {   //If something selected

                    if (MessageBox.Show("This will remove LevelSet " + " from list. Are you sure?", "Removing LevelSet", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel) //Ask user, may he misclick
                        return;

                    uList.iListUsed--;
                    for (int i = iSelectedSet; i < uList.iListUsed; i++)
                        uList.uLevelSets[i] = uList.uLevelSets[i + 1];//Move up all element from current to last

                    iSelectedSet = -1;
                    UpdateSelected();
                    Enlist();//Update listbox
                }
                return;
            }*/
        }

        private void menuUpdateList_Click(object sender, EventArgs e)
        {
            ActionUpdateList();
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            //Delete - remove levelset from list
            GetSelected();
            if (iSelectedSet >= 0)
            {   //If something selected

                if (MessageBox.Show("Are you sure?", "Removing", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel) //Ask user, may he misclick
                    return;

                uList.iListUsed--;
                for (int i = iSelectedSet; i < uList.iListUsed; i++)
                    uList.uLevelSets[i] = uList.uLevelSets[i + 1];//Move up all element from current to last

                iSelectedSet = -1;
                UpdateSelected();
                Enlist();//Update listbox
            }
            return;
        }

        private void menuComment_Click(object sender, EventArgs e)
        {
            GetSelected();
            if (iSelectedSet >= 0)
            {   //If something selected
                InputBox uBox = new InputBox();//Use input box
                if (uBox.AskUser(uList.uLevelSets[iSelectedSet].sComment, "LevelSet comment", "Your comment:") == DialogResult.OK) //Ask user about new comment
                {   //If new comment confirmed
                    uList.uLevelSets[iSelectedSet].sComment = uBox.GetResult();//Update comment
                    uList.UpdateDisplayed(iSelectedSet);//Update displayed text (as comment changed)
                    Enlist();//Update listbox
                }
            }
            return;
        }

        private void menuSelect_Click(object sender, EventArgs e)
        {
            ActionSelect();
        }

        private void menuSelectAndChoose_Click(object sender, EventArgs e)
        {
            ActionSelectAndLevel();
        }

        private void menuTotalLevels_Click(object sender, EventArgs e)
        {
            int iLevels = 0, iSolved = 0;
            for (int i = 0; i < uList.iListUsed; i++)
            {
                iLevels += uList.uLevelSets[i].iLevelsTotal;
                iSolved += uList.uLevelSets[i].iLevelsSolved;
            }

            MessageBox.Show("Totally: "+uList.iListUsed.ToString() + " levelsets,\r\n" +iLevels.ToString()+ " levels,\r\n"+ iSolved.ToString()+ " levels solved");
        }

        private void menuAddDelimiter_Click(object sender, EventArgs e)
        {
            GetSelected();
            InputBox uBox = new InputBox();//Use input box
            if (uBox.AskUser("-", "Delimiter text", "Your text:") == DialogResult.OK) //Ask user about text
            {
                uList.AddDelimiter(uBox.GetResult(), iSelectedSet);
                Enlist();//Update listbox
            }
        }
    }
}