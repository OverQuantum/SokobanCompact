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

namespace SokobanCompact
{
    /// <summary>Contain list of levelsets</summary>
    public class SokobanLevelSetList
    {
        /// <summary>Array of levelset descriptions</summary>
        public LevelSetDescr[] uLevelSets;

        /// <summary>Len of levelset list</summary>
        public int iListUsed;
        
        /// <summary>Allocated len of levelset list array</summary>
        private int iListAlloc;

        /// <summary>File, that store list of levelsets</summary>
        private readonly string sFileName;

        ///<summary>Folder with levelsets</summary>
        private readonly string sLevelsPath;

        ///<summary>Folder with records</summary>
        private readonly string sSolutionsPath;

        /// <summary>Name of currently loaded levelset</summary>
        private string sCurrentLevelSet;

        /// <summary>Index of currently loaded levelset in list</summary>
        private int iCurrentLevelSet;

        /// <summary>Get index of currently loaded levelset in list</summary>
        public int GetCurrentLevelSetIndex()
        {
            return iCurrentLevelSet;
        }

        /// <summary>Get name of currently loaded levelset</summary>
        public string GetCurrentLevelSet()
        {
            return sCurrentLevelSet;
        }

        /// <summary>Reallocate list if needed</summary>
        private void EnlargeArray()
        {
            if (iListUsed >= iListAlloc)
            {
                iListAlloc *= 2;
                LevelSetDescr[] sLevelSetsNew = new LevelSetDescr[iListAlloc];
                uLevelSets.CopyTo(sLevelSetsNew, 0);
                uLevelSets = sLevelSetsNew;
            }
        }

        /// <summary>Update list of levelsets</summary>
        public FunctionResult UpdateList()
        {
            string[] sFilesList;
            SokobanLevelSet uTempLevelSet = new SokobanLevelSet();
            try
            {
                sFilesList = System.IO.Directory.GetFiles(sLevelsPath);//Get list of files in folder with levelsets
            }
            catch
            {
                return FunctionResult.FileSystemError;//List of files not received - error
            }

            int i,j;
            string sNewFile;

            Array.Sort(sFilesList);//Sort filenames alphabetically

            for (i = 0; i < sFilesList.Length; i++)//Iterate thru all detected files
            {
                sNewFile = System.IO.Path.GetFileName(sFilesList[i]).ToLower();//Get lowercased filename
                
                //Search this filename thru list of levelsets
                for (j = 0; j < iListUsed; j++)
                {
                    if (uLevelSets[j].sFileName == sNewFile)
                        goto lFound;
                }

                //Not found - add to list
                j = iListUsed;//New item
                iListUsed++;//Update number of levelsets
                EnlargeArray();//Realloc array if needed
                uLevelSets[j].sComment = "";//Initial comment - empty
lFound:
                uLevelSets[j].sFileName = sNewFile;//Update filename in list
                uTempLevelSet.LoadTxtLevelSet(sFilesList[i],true);//Load levelset with simplified method - to get only number of levels
                uTempLevelSet.AssignRecordFile(sSolutionsPath + SokobanLevelSet.FileName2sID(uLevelSets[j].sFileName) + ".rec");//Load record file - to get number of solved levels
                uLevelSets[j].iLevelsTotal = uTempLevelSet.GetLevelsNum();//Get total number of levels
                uLevelSets[j].iLevelsSolved = uTempLevelSet.GetNumOfSolved();//Get number of solved levels
                UpdateDisplayed(j);//Update displayed text for levelset

            }
            return FunctionResult.OK;//Updated
        }


        /// <summary>Save list of levelsets into file</summary>
        public FunctionResult SaveList()
        {
            try
            {
                int i;
                IniHold.IniFile uList = new IniHold.IniFile();//Use ini engine
                System.IO.StreamWriter hWrite = new System.IO.StreamWriter(sFileName, false);//Open file for overwriting
                uList.SetWriter(hWrite);//Transmit file-writer into ini engine

                for (i = 0; i < iListUsed; i++)//Iterate thru whole list
                {
                    uList.SaveItem("LevelSet", uLevelSets[i].sFileName);//Save filename
                    uList.SaveItem("LevelsTotal", uLevelSets[i].iLevelsTotal.ToString());//Save total number of levels
                    uList.SaveItem("LevelsSolved", uLevelSets[i].iLevelsSolved.ToString());//Save number of solved levels
                    uList.SaveItem("Comment", uLevelSets[i].sComment);//Save user comment for levelset
                }
                hWrite.Close();//Close file
            }
            catch
            {   //On any error - exit
                return FunctionResult.ErrorOnWritingFile;
            }
            return FunctionResult.OK;//Saved successfully
        }

        /// <summary>Load list of levelsets from file</summary>
        public FunctionResult LoadList()
        {
            /*
             * List of levelsets stored in 4 lines of ini files, started from LevelSet = ...
             */
            int i;
            IniHold.IniFile uList = new IniHold.IniFile();//Use ini engine
            uList.LoadIni(sFileName);//Load file as ini file

            iListUsed = -1;//We are before first "LevelSet"
            for (i = 0; i < uList.iNumIniItems; i++)//Iterate thru all items, founded by ini engine
            {
                if (uList.uIniItems[i].sID == "LevelSet")
                {   //"LevelSet" found - new item in list
                    iListUsed++;
                    EnlargeArray();//Realloc array if needed
                    uLevelSets[iListUsed].sFileName = uList.uIniItems[i].sValue.ToLower();//Convert to lowercase for comparations
                }
                else if (iListUsed < 0)
                {   //Before first "LevelSet" - skip all
                    continue;
                }
                else if (uList.uIniItems[i].sID == "LevelsTotal")
                {   //LevelsTotal - number of levels 
                    uLevelSets[iListUsed].iLevelsTotal = OQConvertTools.string2int(uList.uIniItems[i].sValue);
                }
                else if (uList.uIniItems[i].sID == "LevelsSolved")
                {   //LevelsSolved - number of solved levels 
                    uLevelSets[iListUsed].iLevelsSolved = OQConvertTools.string2int(uList.uIniItems[i].sValue);
                }
                else if (uList.uIniItems[i].sID == "Comment")
                {   //Comment - user comment
                    uLevelSets[iListUsed].sComment = uList.uIniItems[i].sValue;
                }
            }
            iListUsed++;//Finish last item

            //Update displayed text for all loaded items
            for (i = 0; i < iListUsed; i++)
                UpdateDisplayed(i);

            return FunctionResult.OK;
        }

        /// <summary>Combine parameters of levelset into displayed text (index of levelsed in list)</summary>
        public FunctionResult UpdateDisplayed(int iIndex)
        {
            if (iIndex < 0 || iIndex >= iListUsed)
                return FunctionResult.OutOfLevelSet;//Specified index is not valid

            if (uLevelSets[iIndex].sFileName.Length > 0)
            {
                uLevelSets[iIndex].sDisplayedName = System.IO.Path.GetFileNameWithoutExtension(uLevelSets[iIndex].sFileName);//First part - filename of levelset file without extension
                uLevelSets[iIndex].sDisplayedName += " [" + uLevelSets[iIndex].iLevelsSolved.ToString() + "/" + uLevelSets[iIndex].iLevelsTotal.ToString() + "]"; //Second part - number of solved / total levels
                if (uLevelSets[iIndex].sComment.Length > 0)
                    uLevelSets[iIndex].sDisplayedName += " - " + uLevelSets[iIndex].sComment;//Last part - user comment, if present
            }
            else
            {
                uLevelSets[iIndex].sDisplayedName = "--- " + uLevelSets[iIndex].sComment;
            }
            return FunctionResult.OK;
        }

        /// <summary>Update index and name of current levelset by specified filename (filename)</summary>
        public FunctionResult FindLevelSet(string sFindFileName)
        {
            //Function used after loading levelset without using levelset list
            
            int i;
            iCurrentLevelSet = -1;//Current levelset not (yet) found in list
            sCurrentLevelSet = sFindFileName;//Name of searched levelset
            for (i = 0; i < iListUsed; i++)//Iterate thru all levelsets
            {
                if (uLevelSets[i].sFileName == sCurrentLevelSet)//Filename found
                {
                    iCurrentLevelSet = i;//Update index
                }
            }
            return FunctionResult.OK;
        }

        /// <summary>Update number of solved levels in current levelset (new number)</summary>
        public FunctionResult UpdateSolved(int iNewSolved)
        {
            if (iCurrentLevelSet >= 0)
            {   //Only if levelset is in list
                uLevelSets[iCurrentLevelSet].iLevelsSolved = iNewSolved;//Update number
                UpdateDisplayed(iCurrentLevelSet);//Update displayed text for levelset
                return FunctionResult.OK;
            }
            return FunctionResult.NothingToDo;//Levelset is unknown, no update
        }

        /// <summary>Load levelset by index (destination levelset object, index in list)</summary>
        public FunctionResult LoadLevelSet(SokobanLevelSet uLevelSet, int iIndex)
        {
            if (iIndex>=0 && iIndex<iListUsed)
            {   //Index is valid
                iCurrentLevelSet = iIndex;//Update index of current levelset
                return LoadLevelSet(uLevelSet, uLevelSets[iIndex].sFileName);//Load levelset by file
            }
            return FunctionResult.NothingToDo;//Index is out of list
        }

        ///<summary>Generate null level set - 1 random level (levelset)</summary>
        public FunctionResult GenNullLevelSet(SokobanLevelSet uLevelSet)
        {
            uLevelSet.GenNullLevelSet();//Generate levelset with 1 random level
            iCurrentLevelSet = -1;//This levelset can not be in list
            sCurrentLevelSet = uLevelSet.sFileName;
            return FunctionResult.OK;
        }

        /// <summary>Load levelset by file (destination levelset object, filename)</summary>
        public FunctionResult LoadLevelSet(SokobanLevelSet uLevelSet, string sLoadFileName)
        {
            FunctionResult uRV = uLevelSet.LoadTxtLevelSet(sLevelsPath + sLoadFileName,false);//Load levelset file
            if (uRV != FunctionResult.OK)
            {
                return uRV;//Something wrong with loading - exit
            }
            uLevelSet.sID = SokobanLevelSet.FileName2sID(sLoadFileName);//Convert filename into ID of levelset
            uLevelSet.sFileName = sLoadFileName;
            uLevelSet.AssignRecordFile(sSolutionsPath + uLevelSet.sID + ".rec");//Load record file
            FindLevelSet(sLoadFileName);//Update current levelset in list
            return uRV;
        }

        /// <summary>Constructor (list filename, path to levelsets, path to records)</summary>
        public SokobanLevelSetList(string sListFileName, string sLevelsPath, string sSolutionsPath)
        {
            iListUsed = 0;
            iListAlloc = 16;
            uLevelSets = new LevelSetDescr[iListAlloc];//Allocate new array for list

            iCurrentLevelSet = -1;//No current levelset
            sCurrentLevelSet = "";

            sFileName = sListFileName;//Copy external parameters into private fields
            this.sLevelsPath = sLevelsPath;
            this.sSolutionsPath = sSolutionsPath;
        }

        /// <summary>Add text delimiter(text, index of element to insert after)</summary>
        public FunctionResult AddDelimiter(string sText, int iBeforeElement)
        {
            int i, j;
            if (iBeforeElement == -1 || iBeforeElement > iListUsed)
                iBeforeElement = iListUsed;
            j = iListUsed;//New item
            iListUsed++;//Update number of levelsets
            EnlargeArray();//Realloc array if needed
            for (i = j; i > iBeforeElement; i--)
                uLevelSets[i] = uLevelSets[i-1];
            uLevelSets[iBeforeElement].sComment = sText;
            uLevelSets[iBeforeElement].sFileName = "";
            uLevelSets[iBeforeElement].iLevelsTotal = 0;
            uLevelSets[iBeforeElement].iLevelsSolved = 0;
            UpdateDisplayed(iBeforeElement);//Update displayed text for levelset
            return FunctionResult.OK;
        }
    }

}
