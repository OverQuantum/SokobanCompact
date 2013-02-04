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
using System.Collections;

namespace SokobanCompact
{
    ///<summary>Levelset - sequence of levels</summary>
    public class SokobanLevelSet
    {
        ///<summary>Name for null (random) levelset</summary>
        public const string sNullName = "__null";

        ///<summary>Text ID of levelset, used for naming files</summary>
        public string sID;

        ///<summary>Filename, from where levelset was loaded</summary>
        public string sFileName;

        ///<summary>Title or name of levelset, provided by author</summary>
        public string sTitle;

        ///<summary>Author, as he specify itself in levelset</summary>
        public string sAuthor;

        ///<summary>Copyright information</summary>
        public string sCopyright;

        ///<summary>Comment, provided by author</summary>
        public string sComment;

        ///<summary>Filename of file with records (minimum moves/pushes)</summary>
        private string sRecordFile;

        ///<summary>Array of levels</summary>
        SokobanLevel[] uLevels;

        ///<summary>Allocated len of array</summary>
        private int iLevelsAllocated;

        ///<summary>Number of levels in levelset</summary>
        private int iLevelsNum;

        ///<summary>Currently played level</summary>
        private int iCurrentLevel;

        ///<summary>Basic constructor</summary>
        public SokobanLevelSet()
        {
            Reset();//Create empty levelset
        }

        ///<summary>Clean all</summary>
        public void Reset()
        {
            //Clean all, if something exist
            iLevelsNum = 0;
            iLevelsAllocated = 0;
            iCurrentLevel = -1;
            sID = "";
            sTitle = "[empty levelset]";
            sAuthor = "";
            sCopyright = "";
            sComment = "";
            sRecordFile = "";
            uLevels = null;// new SokobanLevel[iLevelsAllocated];
        }

        ///<summary>Get number of levels</summary>
        public int GetLevelsNum()
        {
            return iLevelsNum;
        }

        ///<summary>Get currently played level</summary>
        public int GetCurrentLevel()
        {
            return iCurrentLevel;
        }

        ///<summary>Generate null level set - 1 random level</summary>
        public FunctionResult GenNullLevelSet()
        {
            int i, j,k;
            int iXsize, iYsize;
            Random uRandom = new Random(Environment.TickCount);
            Reset();//Reset levelset

            sAuthor = "SokobanCompact";
            sComment = "Randomly generated";
            sCopyright = "";
            sTitle = "null levelset";
            sFileName = sNullName;
            sID = sNullName;

            sRecordFile = null;

            iLevelsNum = 1;//1 level
            iLevelsAllocated = iLevelsNum;
            uLevels = new SokobanLevel[iLevelsAllocated];//Allocate array of levels
            iXsize = 10;
            iYsize = 10;
            uLevels[0] = new SokobanLevel(iXsize, iYsize);
            uLevels[0].sTitle = "null level";
            uLevels[0].sAuthor = "SokobanCompact";
            uLevels[0].sComment = "Randomly generated";
            for (i = 0; i < iXsize; i++)
                for (j = 0; j < iYsize; j++)
                {
                    if (i==0 || j==0 || i==(iXsize-1) || j==(iYsize-1))
                        uLevels[0].SetCell(i, j, SokoCell.Wall);
                    else
                        uLevels[0].SetCell(i, j, SokoCell.Empty);
                }
            uLevels[0].SetCell(1, 1, SokoCell.Player);

            k = 0;
            do
            {
                i = uRandom.Next(2, iXsize - 2);
                j = uRandom.Next(2, iYsize - 2);
                if (uLevels[0].GetCell(i, j)!=SokoCell.Empty) continue;
                uLevels[0].SetCell(i, j, SokoCell.Box);
                k++;
            } while (k < 10);

            k = 0;
            do
            {
                i = uRandom.Next(2, iXsize - 2);
                j = uRandom.Next(2, iYsize - 2);
                if (uLevels[0].GetCell(i, j)!=SokoCell.Empty) continue;
                uLevels[0].SetCell(i, j, SokoCell.Target);

                k++;
            } while (k < 10);
            return FunctionResult.OK;
        }

        ///<summary>Loader of txt levelset (filename, simplified loading for fast update)</summary>
        public FunctionResult LoadTxtLevelSet(string sFileName, bool bSimplified)
        {
            System.IO.StreamReader hReader;//For reading file
            try
            {
                hReader = new System.IO.StreamReader(sFileName);//Open file
            }
            catch
            {
                return FunctionResult.FailedToOpenFile;//Unable
            }

            Reset();//Reset levelset

            string sLine, sTrim;
            string sLastNotEmpty="";
            char[] sChars;
            int i,j;
            int iPos, iEnd;
            int iCommentPhase;//Comment phase: 0 - before comments block found, 1 - inside, 2 - after
            SokoCell uNewCell;

            try
            {
                sTitle = System.IO.Path.GetFileNameWithoutExtension(sFileName);//Assing initial title (would be replaced by title from file, if specified)
            }
            catch
            {
                return FunctionResult.FileSystemError;//Something very wrong with filename
            }

            ArrayList uXSizeList = new ArrayList(20);
            ArrayList uYSizeList = new ArrayList(20);
            ArrayList uPosList = new ArrayList(20);
            ArrayList uFileLines = new ArrayList(100);

            //Stag 1. Estimate level number and their sizes

            int iPhase = 0;
            iLevelsNum = 0;
            iCommentPhase = 0;
            int iX = 0; int iY = 0;

            while (!hReader.EndOfStream)  //Read till end of file
            {
                try
                {
                    sLine = hReader.ReadLine(); //Read one line of file
                }
                catch
                {
                    return FunctionResult.ErrorOnReadingFile;//Something wrong with file
                }
                uFileLines.Add(sLine);//Store line into array
                sTrim = sLine.Trim();
                if (sTrim.StartsWith("#"))
                {//All lines, that starts with # are lines of level board
                    if (iPhase != 2)
                    {//Level was not yet started
                        iLevelsNum++;//Start level
                        uPosList.Add(uFileLines.Count-1);//Remeber line, where level starts
                        iPhase = 2;//Now we loading level
                        iX = 0;//Initial sizes
                        iY = 0;
                    }
                    iY++;//New line for level
                    if (iX < sLine.Length) iX = sLine.Length;//Update width of level, if needed
                }
                else
                {//All lines, that not starts with # are delimits levels from each other
                    if (iPhase == 0)
                    {   //Header of levelset
                        try
                        {   //Try to get values from comments (comparisons should be in increment-len order, otherwise exception may occurs earlier, than line will be found)
                            if (sTrim.Substring(0, 6) == "Name: ")
                            {
                                sTitle = sTrim.Substring(6);
                                sTrim = "";
                            }
                            else if (sTrim.Substring(0, 7) == "Title: ")
                            {
                                sTitle = sTrim.Substring(7);
                                sTrim = "";
                            }
                            else if (sTrim.Substring(0, 8) == "Author: ")
                            {
                                sAuthor = sTrim.Substring(8);
                                sTrim = "";
                            }
                            else if (sTrim.Substring(0, 8) == "Comment:")
                            {
                                sComment = "";
                                if (sTrim.Length > 8)
                                    sComment = sTrim.Substring(8).Trim();//Line contain text after "Comment:", so it is single-line comment
                                else
                                    iCommentPhase = 1;//Line do not contain other text, so it is multi-line comment
                                sTrim = "";
                            }
                            else if (sTrim.Substring(0, 11) == "Copyright: ")
                            {
                                sCopyright = sTrim.Substring(11);
                                sTrim = "";
                            }
                            else if (sTrim.Substring(0, 11) == "Comment-End")
                            {//End of multi-line comment
                                iCommentPhase = 2;
                                sTrim = "";
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            //If line is very short, substring will throw exception. We bury such exceptions here :)
                        }
                        if (iCommentPhase == 1)
                        {   //Multiline comment continuing
                            if (sComment.Length > 0)
                                sComment += "\r\n";//Not first line, add line delimiter
                            sComment += sTrim;
                            sTrim = "";
                        }
                        if (sTrim.Length > 0)
                        {   //Line is still not identified
                            if (iCommentPhase == 0)
                            {   //All lines before comment block could be comment
                                if (sComment.Length > 0)
                                    sComment += "\r\n";//Not first line, add line delimiter
                                sComment += sTrim;
                            }
                            sLastNotEmpty = sTrim;//Store line, it could be title for first level
                        }
                    }

                    if (iPhase == 2)
                    {   //Level was ended
                        uXSizeList.Add(iX);//Add sizes of level into arrays
                        uYSizeList.Add(iY);
                        iPhase = 3;//Now we waiting next level
                    }
                }
            }
            if (iPhase == 2)
            {   //Last level was not fully ended, end it here
                uXSizeList.Add(iX);
                uYSizeList.Add(iY);
            }
            hReader.Close();//Close file

            if (iLevelsNum == 0)
                return FunctionResult.NoLevelsFound;//No levels found in file

            //Stage 2. Loading all detected levels

            iLevelsAllocated = iLevelsNum;//Now we know exact number of levels
            uLevels = new SokobanLevel[iLevelsAllocated];//Allocate array of levels

            for (i = 0; i < iLevelsNum; i++) //Iterate for all levels
            {
                //Create new level with sizes, calculated in stage 1
                uLevels[i] = new SokobanLevel((int)uXSizeList[i], (int)uYSizeList[i]);

                if (bSimplified) continue;//Simplified loading - skip decoding levels (for fast updating list of levelsets, level contructor above required to load record file and calculate number of solved levels)

                //uLevels[i].sTitle = "unnamed level " + (i + 1).ToString();//Initial level title
                uLevels[i].sTitle = ""; //void level title 

                iPos = (int)uPosList[i];//Restore line number, where level starts
                
                //Choose where we should end loading level
                if (i<(iLevelsNum-1))
                    iEnd = (int)uPosList[i + 1]; //Not last level - up to first line of previous level
                else
                    iEnd = uFileLines.Count; //Last level - up to last line of level
                
                iPhase = 1;//Initial phase - comment before level
                iCommentPhase = 0;
                iY = 0;
                //uNewCell = SokoCell.Background;

                while (iPos < iEnd)//Iterate thru all lines of level board
                {
                    sLine = (string)uFileLines[iPos]; iPos++;//Restore line from array
                    sTrim = sLine.Trim();//Trim whitespaces
                    if (sTrim.StartsWith("#"))
                    {   //All lines that starts with # are lines of level
                        if (iPhase == 1)
                        {   //Get title from last unidentified line
                            uLevels[i].sTitle = sLastNotEmpty;
                        }
                        if (iPhase == 3)
                        {   //Level board already was loaded, it cannot start second time
                            break;//Exit from loading this level
                        }
                        iPhase = 2;//Now we loading level
                        sChars = sLine.ToCharArray();//Get chars of string
                        iX = 0;//Start new line
                        for (j = 0; j < sChars.Length; j++)//Iterate thru all chars
                        {
                            switch (sChars[j])//Decore char into cell of level
                            {
                                case '#': uNewCell = SokoCell.Wall; break;
                                case ' ': uNewCell = SokoCell.Empty; break;
                                case '.': uNewCell = SokoCell.Target; break;
                                case '$': uNewCell = SokoCell.Box; break;
                                case '@': uNewCell = SokoCell.Player; break;
                                case '*': uNewCell = SokoCell.BoxOnTarget; break;
                                case '+': uNewCell = SokoCell.PlayerOnTarget; break;
                                default: continue;//Unknown - skipped
                            }
                            uLevels[i].SetCell(iX, iY, uNewCell);//Put cell into level
                            iX++;//Advance to next cell
                        }
                        iY++;//Advance to next line
                    }
                    else
                    {   //All lines, that not starts with # are delimits levels from each other

                        if (iPhase == 2)//If level just ends
                            sLastNotEmpty = "";//Forgot all previous unidentified lines

                        iPhase = 3;//Now we are after level board
                        try
                        {   //Try to get values from comments (comparisons should be in increment-len order, otherwise exception may occurs earlier, than line will be found)
                            if (sTrim.Substring(0, 7) == "Title: ")
                            {
                                uLevels[i].sTitle = sTrim.Substring(7);
                                sTrim = "";
                            }
                            else if (sTrim.Substring(0, 8) == "Author: ")
                            {
                                uLevels[i].sAuthor = sTrim.Substring(8);
                                sTrim = "";
                            }
                            else if (sTrim.Substring(0, 8) == "Comment:")
                            {
                                if (sTrim.Length > 8)
                                    uLevels[i].sComment = sTrim.Substring(8).Trim();//Line contain text after "Comment:", so it is single-line comment
                                else
                                    iCommentPhase = 1;//Line do not contain other text, so it is multi-line comment
                                sTrim = "";
                            }
                            else if (sTrim.Substring(0, 11) == "Comment-End")
                            {   //Multi-line comment ends
                                iCommentPhase = 0;
                                sTrim = "";
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            //If line is very short, substring will throw exception. We bury such exceptions here :)
                        }
                        if (iCommentPhase == 1)
                        {   //Multiline comment continuing
                            if (uLevels[i].sComment.Length > 0)
                                uLevels[i].sComment += "\r\n";//Not first line, add line delimiter
                            uLevels[i].sComment += sTrim;
                            sTrim = "";
                        }
                        if (sTrim.Length > 0)
                        {   //Line is still not identified
                            sLastNotEmpty = sTrim;//Store line, it could be title for next level
                        }

                    }
                }
                if (uLevels[i].sTitle.Length <= 0)
                {
                    uLevels[i].sTitle = "level " + (i+1).ToString();//Initial level title
                }
                uLevels[i].CleanUp();//Distinguish backgrounds from empty cells (both specified as spaces in file)
            }

            return FunctionResult.OK;//Level set successfully loaded
        }

        ///<summary>Load record file for levelset</summary>
        public FunctionResult AssignRecordFile(string sFileName)
        {
            /*
             * Records are stored in fixed-width text files, one record in one line
             * Format:
             * NNNNN, MMMMM, PPPPP, 'Name of record'
             * where NNNNN - level number, MMMMM - number of moves, PPPPP - number of pushes
             */

            sRecordFile = sFileName;//Store record filename

            if (!System.IO.File.Exists(sFileName)) //(Do this really needed?)
                return FunctionResult.FileNotExist;
            System.IO.StreamReader hReader;//For reading file

            try
            {
                hReader = new System.IO.StreamReader(sFileName);//Open file
            }
            catch
            {
                return FunctionResult.FailedToOpenFile;//Unable...
            }

            string s;
            PositionStats uReadedStats = new PositionStats();//Temprorary position statistics, to compare with records
            int iLevelNum;

            while (!hReader.EndOfStream)//Till end of file
            {
                try
                {
                    s = hReader.ReadLine(); //Read one line
                }
                catch
                {
                    return FunctionResult.ErrorOnReadingFile;
                }

                if (s.Length > 22) //Normal record line should be at least 23 chars long
                {
                    uReadedStats.InitMax();//Reset record
                    iLevelNum = int.Parse(s.Substring(0, 5));//Load level number
                    if (iLevelNum >= 0 && iLevelNum < iLevelsNum)//If such level exist in current levelset
                    {
                        try
                        {
                            uReadedStats.iMoves = int.Parse(s.Substring(7, 5));//Load number of moves
                            uReadedStats.iPushes = int.Parse(s.Substring(14, 5));//Load number of pushes
                            uReadedStats.sName = s.Substring(22, s.Length - 23);//Load record name
                            uLevels[iLevelNum].CheckSolutionForRecord(uReadedStats);//Test loaded record against records of level
                        }
                        catch 
                        { } //We do not interested in exceptions, caused by wrong formats of something else
                    }
                }
            }
            hReader.Close();//Close file
            return FunctionResult.OK;//Records successfully loaded
        }

        ///<summary>Load level to play it (game, level number)</summary>
        public FunctionResult LoadLevel(SokobanGame uDstLevel, int iLevelNumber)
        {
            if (iLevelNumber < 0 || iLevelNumber >= iLevelsNum) return FunctionResult.OutOfLevelSet; //Incorrent number specified
            

            //if (!bKeepPosition)
            uDstLevel.FlushPosition();//Reset position (undo stack etc.)
            uDstLevel.InvalidateBoxMoveTree();//Reset BMT
            uDstLevel.CopyFrom(uLevels[iLevelNumber]);//Copy level content into game
            uDstLevel.ReAnalyze();//Update level info (box/target counters etc.)
            iCurrentLevel = iLevelNumber;//Store number of loaded level
            return FunctionResult.OK;//Successfully
        }

        ///<summary>Save new record into file</summary>
        public FunctionResult SaveNewRecord(SokobanGame uSolution)
        {
            /*
             * All solutions of level are sequentaly save into file (currently)
             * This is kinda log of solving levels
             * On loading all solutions are checked for best ones
             */
            uLevels[iCurrentLevel].CheckSolutionForRecord(uSolution.uStats);//Update records of current level

            //Prepare string for writing into file
            string sSolutionString = iCurrentLevel.ToString("00000");
            sSolutionString += ", ";
            sSolutionString += uSolution.uStats.iMoves.ToString("00000");
            sSolutionString += ", ";
            sSolutionString += uSolution.uStats.iPushes.ToString("00000");
            sSolutionString += ", ";
            sSolutionString += "'" + uSolution.uStats.sName+ "'";

            System.IO.StreamWriter hAppend;//For writing file
            try
            {
                hAppend = new System.IO.StreamWriter(sRecordFile, true);//Open file for append
                hAppend.WriteLine(sSolutionString);//Write string
                hAppend.Close();//Close file
            }
            catch
            {
                return FunctionResult.ErrorOnWritingFile;//Unable to open or write file
            }

            return FunctionResult.OK;//Successfully
        }

        ///<summary>Return description of specific level - title, sizes and number of boxes</summary>
        public string GetLevelDescription(int iLevelNumber)
        {
            if (iLevelNumber < 0 || iLevelNumber >= iLevelsNum)
                return "";
            return uLevels[iLevelNumber].GetLevelDescription();
        }

        ///<summary>Is specific level is solved?</summary>
        public bool IsSolved(int iLevelNumber)
        {
            if (iLevelNumber < 0 || iLevelNumber >= iLevelsNum)
                return false;
            return uLevels[iLevelNumber].IsSolved();
        }

        ///<summary>Get index of next unsolved level, or first, or -1 if all levelset if solved</summary>
        public int GetNextUnsolved()
        {
            int i;
            //Check levels from current till last in levelset
            for (i = iCurrentLevel+1; i < iLevelsNum; i++)
            {
                if (!uLevels[i].IsSolved())
                    return i;//Found
            }

            //Check levels from first till current
            for (i = 0; i <= iCurrentLevel; i++)
            {
                if (!uLevels[i].IsSolved())
                    return i;//Found
            }
            return -1;//Not found - all levels are solved
        }
        
        ///<summary>Get index of previous unsolved level, or last, or -1 if all levelset if solved</summary>
        public int GetPrevUnsolved()
        {
            int i;
            //Check levels from current till first in levelset
            for (i = iCurrentLevel - 1; i >= 0; i--)
            {
                if (!uLevels[i].IsSolved())
                    return i;//Found
            }

            //Check levels from last till current
            for (i = iLevelsNum - 1; i >= iCurrentLevel; i--)
            {
                if (!uLevels[i].IsSolved())
                    return i;//Found
            }
            return -1;//Not found - all levels are solved
        }

        ///<summary>Get index of random unsolved level</summary>
        public int GetRandUnsolved()
        {
            int i,iNum=0;
            //Count unsolved levels
            for (i = 0;i<iLevelsNum; i++)
            {
                if (!uLevels[i].IsSolved())
                    iNum++;//Found
            }
            if (iNum==0)
                return -1;//All levels are solved

            Random uR = new Random();
            iNum = uR.Next(iNum);//Get random

            //Walk thru all unsolved levels and select iNum-th unsolved
            for (i = 0; i < iLevelsNum; i++)
            {
                if (!uLevels[i].IsSolved())
                {
                    if (iNum == 0)
                        return i;//Ok, this is random unsolved
                    iNum--;
                }
            }
            return -1;//Unable (should not happens, but still)
        }

        ///<summary>Return number of unsolved levels</summary>
        public int GetNumOfUnsolved()
        {
            //Just count and return
            int iNum = 0;
            for (int i = 0; i < iLevelsNum; i++)
                if (!uLevels[i].IsSolved())
                    iNum++ ;
            return iNum;
        }

        ///<summary>Return number of solved levels</summary>
        public int GetNumOfSolved()
        {
            //Just count and return
            int iNum = 0;
            for (int i = 0; i < iLevelsNum; i++)
                if (uLevels[i].IsSolved())
                    iNum++;
            return iNum;
        }

        ///<summary>Convert filename into ID, suitable as comparable text identifier</summary>
        public static string FileName2sID(string sFileName)
        {
            return sFileName.ToLower().Replace(".", "_"); //Convert to lowercase and replace extension delimiter into underscore "_"
        }

    }
}