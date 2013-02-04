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
using System.Threading;

namespace SokobanCompact
{
    ///<summary>Return values for functions</summary>
    public enum FunctionResult : int
    {
        ///<summary>Successfull execution</summary>
        OK = 0,

        ///<summary>Error: file not found or reading error</summary>
        FailedToOpenFile,

        ///<summary>Error: file not found or can not be written</summary>
        ErrorOnWritingFile,

        ///<summary>Error: file not found or some problems with reading or decoding</summary>
        ErrorOnReadingFile,

        ///<summary>Error: some file system operation fails</summary>
        FileSystemError,

        ///<summary>Error: file not found</summary>
        FileNotExist,

        ///<summary>(Not error) Function is meaningless in current situation</summary>
        NothingToDo,

        ///<summary>Error: move tree can not be build, because its root location is blocked</summary>
        StartIsBlocked,

        ///<summary>Error: specified coordinates is outside level boundaries</summary>
        OutOfLevel,

        ///<summary>Error: specified level index is outside list of levels in current levelset</summary>
        OutOfLevelSet,

        ///<summary>Error: no levels found in levelset file</summary>
        NoLevelsFound,

        ///<summary>(Not error) Path to desired location is found, further execution not required</summary>
        PathFound,

        ///<summary>(Not error) User decide to cancel operation</summary>
        Canceled,

        ///<summary>(Not error) Game position is game-deadlock</summary>
        GameDeadlock,

    }

    ///<summary>Integer coordinates</summary>
    public struct Coordinates
    {
        ///<summary>X, usually horizontal offset from leftmost boundary</summary>
        public int x;

        ///<summary>Y, usually vertical offset from topmost boundary</summary>
        public int y;
    }

    ///<summary>Statistics of level</summary>
    public struct LevelStats
    {
        ///<summary>Number of cell with Wall</summary>
        public int iNumWalls;

        ///<summary>Number of empty cell (not including background)</summary>
        public int iNumEmpty;

        ///<summary>Number of boxes</summary>
        public int iNumBoxes;

        ///<summary>Number of targets</summary>
        public int iNumTargets;

        ///<summary>Number of players (currently should be 1)</summary>
        public int iNumPlayers;

        ///<summary>Number of background cells</summary>
        public int iNumBackground;

        ///<summary>Number of boxes, already placed on targets</summary>
        public int iNumBoxesOnTargets;
    }

    ///<summary>Statistics of position or solution</summary>
    public struct PositionStats
    {
        ///<summary>Total number of moves</summary>
        public int iMoves;

        ///<summary>Number of pushes</summary>
        public int iPushes;

        ///<summary>Number of linear moves (one linear move - one or more sequental steps in single direction)</summary>
        public int iLinearMoves;

        ///<summary>Number of linear pushes (one linear push - one or more sequental pushes in single direction)</summary>
        public int iLinearPushes;

        ///<summary>Number of pushing sessions (one pushing session - one or more sequental pushes)</summary>
        public int iPushSessions;

        ///<summary>Number of box changes (box change occurs then player pushes not the same box, as on previous push)</summary>
        public int iBoxChanges;

        /*
         * 
         *  1 M  = 1 move  - u \ d \ l \ r \ U \ D \ L \ R)
         *  1 P  = 1 push  - U \ D \ L \ R )
         *  1 LM = 1 line move (pusher line)  - uUUU \ ddddD \ lllll \ rrRRR 
         *  1 L = 1 linear (box line)  - (UUUU \ DD \ L \ RRRRRR ...)
         *  1 S = 1 session/phases  - (.rUURDDLD. \ .dULDR. ...)
         *  1 C = 1 change (box change)
         * 
         * 1 session/phases - 1 or more linears and contain 0 or move change
         * 1 line move - can end with 1 linear or can not end with it
         * M >= P >= L >= S > 0, P > C >= 0, M >= LM >= L 
         * 
         * 
         */

        ///<summary>Name of solution or position, usully specified just before saving</summary>
        public string sName;

        ///<summary>Initialize with maximal values - for comparing during search best solution</summary>
        public void InitMax()
        {
            iMoves = int.MaxValue;
            iPushes = int.MaxValue;
            iLinearMoves = int.MaxValue;
            iLinearPushes = int.MaxValue;
            iPushSessions = int.MaxValue;
            iBoxChanges = int.MaxValue;
            sName = null;
        }

        ///<summary>Initialize with zero values - for counting statistics</summary>
        public void InitZero()
        {
            iMoves = 0;
            iPushes = 0;
            iLinearMoves = 0;
            iLinearPushes = 0;
            iPushSessions = 0;
            iBoxChanges = 0;
            sName = null;
        }

        ///<summary>Return short form of statistics - six values delimited by "/" </summary>
        override public string ToString()
        {
            return
                iMoves.ToString() + @"/" +
                iPushes.ToString() + @"/" +
                iLinearMoves.ToString() + @"/" +
                iLinearPushes.ToString() + @"/" +
                iPushSessions.ToString() + @"/" +
                iBoxChanges.ToString();
        }
    }

    ///<summary>Solution achivements</summary>
    [Flags]
    public enum SolutionFlags : int
    {
        ///<summary>No achivments</summary>
        Nothing = 0x0,

        ///<summary>First solution of level</summary>
        FirstSolution = 0x1,

        ///<summary>New best moves record</summary>
        BestMoves = 0x2,

        ///<summary>New best pushes record</summary>
        BestPushes = 0x4,
    }


    ///<summary>Content of a cell</summary>
    [Flags]
    public enum SokoCell : byte
    {
        ///<summary>Empty - floor, there boxes can be moved</summary>
        Empty = 0x0,

        ///<summary>Background - not drawing anything, physically it is also wall</summary>
        Background = 0x01,

        ///<summary>Wall - obstacle for player and boxes</summary>
        Wall = 0x02,

        ///<summary>Box - should be pushed, one at a time, and placed to targets</summary>
        Box = 0x04,

        ///<summary>Target - mark on the floor, boxes should be placed here</summary>
        Target = 0x08,

        ///<summary>Box placed on Target</summary>
        BoxOnTarget = 0x0C,

        ///<summary>Player - can push boxes and move around</summary>
        Player = 0x10,

        ///<summary>Player, staying on Target</summary>
        PlayerOnTarget = 0x18,

        ///<summary>Cell-deadlock - push box here and you will not be able to push it to any target</summary>
        CellDeadlock = 0x20,

        ///<summary>!MASK ONLY! Some obstacle - box or wall or background</summary>
        MaskObstacle = 0xC7,

        ///<summary>!MASK ONLY! For analyze level for normal objects</summary>
        MaskAnalyze = 0x1F,

        ///<summary>!MASK ONLY! Permanent obstacle - wall or background</summary>
        MaskPermanentObstacle = 0xC3,

        ///<summary>!MASK ONLY! Will remove box and player from cell</summary>
        FilterForCalcCellDeadlocks = 0xEB,

        ///<summary>!MASK ONLY! Will remove cell-deadlocks</summary>
        FilterSkipCellDeadlocks = 0xDF,
    }

    ///<summary>Player movement</summary>
    [Flags]
    public enum SokoMove : byte
    {
        ///<summary>Move to up (y-1)</summary>
        Up = 0x0,
        ///<summary>Move to down (y+1)</summary>
        Down = 0x01,
        ///<summary>Move to left (x-1)</summary>
        Left = 0x02,
        ///<summary>Move to right (x+1)</summary>
        Right = 0x03,

        ///<summary>Move with push</summary>
        Push = 0x04,

        ///<summary>Marker of first move in group-action</summary>
        Marker = 0x08,

        //Combinations:

        ///<summary>Push to up</summary>
        PushUp = 0x04,
        ///<summary>Push to down</summary>
        PushDown = 0x05,
        ///<summary>Push to left</summary>
        PushLeft = 0x06,
        ///<summary>Push to right</summary>
        PushRight = 0x07,

        //Masks:

        ///<summary>Mask of directions</summary>
        Direction = 0x03,
        ///<summary>Mask of directions and push</summary>
        DirectionPush = 0x07,
    
    }

    ///<summary>Result of a move</summary>
    public enum MoveResult : int
    {
        ///<summary>Player moved, but do not push boxes</summary>
        Moved = 0,

        ///<summary>Player moved and push box to empty cell</summary>
        MovedAndPushBox,

        ///<summary>Player moved and push box to target</summary>
        MovedAndPushBoxToTarget,

        ///<summary>Player cannot move that way</summary>
        WayBlocked,
    }

    ///<summary>Queue of coordinates, FIFO</summary>
    public class CoordQueue
    {
        //Implementation is very simple, no cycling are used, so memory consumed for total ammount of added items, even if live items are few

        ///<summary>Array of coordinates</summary>
        private Coordinates[] uValues;

        ///<summary>Allocated len of coordinates array</summary>
        private int iAllocated;

        ///<summary>Top of queue - where new items should be places</summary>
        private int iTop;

        ///<summary>Bottom of queue - where items should be extracted</summary>
        private int iBottom;

        ///<summary>Constructor, 500 elements allocated</summary>
        public CoordQueue()
        {
            iAllocated = 500;
            uValues = new Coordinates[iAllocated];
            iTop = 0;
            iBottom = 0;
        }

        ///<summary>Add new item to the top of queue (x, y coordinates)</summary>
        public void AddNext(int x, int y)
        {
            uValues[iTop].x = x;//Copy values
            uValues[iTop].y = y;
            iTop++;//Move top
            CheckQueue();//Check for reallocation
        }

        ///<summary>Remove item from bottom of queue and return it (x,y coordinates)</summary>
        public bool Get(ref int x, ref int y)
        {
            if (iBottom == iTop) return false;//Queue is empty
            x = uValues[iBottom].x;//Copy values
            y = uValues[iBottom].y;
            iBottom++;//Move bottom
            return true;//Ok, value removed and returned
        }

        ///<summary>Reset queue to initial state</summary>
        public void Reset()
        {
            iBottom = 0;//After this memory is released
            iTop = 0;
        }

        ///<summary>Check queue for reallocation</summary>
        private void CheckQueue()
        {
            if (iTop >= iAllocated)
            {   //Reallocation needed
                iAllocated *= 2;
                Coordinates[] uNew = new Coordinates[iAllocated];
                uValues.CopyTo(uNew, 0);
                uValues = uNew;
            }
        }
    }

    ///<summary>Description of levelset</summary>
    public struct LevelSetDescr
    {
        ///<summary>File name of levelset</summary>
        public string sFileName;

        ///<summary>Displayed name of levelset</summary>
        public string sDisplayedName;

        ///<summary>User comment for levelset</summary>
        public string sComment;

        ///<summary>Number of levels in levelset</summary>
        public int iLevelsTotal;

        ///<summary>Number of solved levels in levelset</summary>
        public int iLevelsSolved;

        ///<summary>Returns displayed name</summary>
        override public string ToString()
        {
            return sDisplayedName;
        }
    }

    ///<summary>List of skins</summary>
    public class SkinSet
    {
        //public string bAnimateTravel;
        private string[] sSkins;
        private int[] iSizes;
        private int iSkinUsed;

        //private string sPath;

        ///<summary>Void constructor</summary>
        public SkinSet()
        {
            return;
        }

        ///<summary>Load set of skins from file (file with path)</summary>
        public FunctionResult Load(string sFileName)
        {
            /*
             * List of levelsets stored in 4 lines of ini files, started from LevelSet = ...
             */
            int i;
            int j=0;
            IniHold.IniFile uList = new IniHold.IniFile();//Use ini engine
            uList.LoadIni(sFileName);//Load file as ini file

            if (uList.iNumIniItems == 0)//No ini-items found
                return FunctionResult.NothingToDo;

            /*
            try
            {
                sPath = System.IO.Path.GetDirectoryName(sFileName);
            }
            catch 
            {
                sPath = "";
            }*/

            iSkinUsed = uList.iNumIniItems;
            sSkins = new string[iSkinUsed];
            iSizes = new int[iSkinUsed];

            for (i = 0; i < uList.iNumIniItems; i++)//Iterate thru all items, founded by ini engine
            {
                iSizes[j] = OQConvertTools.string2int(uList.uIniItems[i].sID);//Convert ID (size) into int
                if (iSizes[j]>0)
                {   //If size have meaning
                    sSkins[j] = /*sPath + "\\" +*/ uList.uIniItems[i].sValue;//Store skin name
                    j++;
                }
            }
            iSkinUsed = j;

            if (iSkinUsed==0)
                return FunctionResult.NothingToDo;//No skin found

            return FunctionResult.OK;
        }


        ///<summary>Get skin of specific size (size)</summary>
        public string GetSkin(int iSize)
        {
            for (int i = 0; i < iSkinUsed; i++)
            {
                if (iSizes[i]==iSize)
                {   //Find skin with equal size
                    return sSkins[i];
                }
            }
            return null;
        }


        ///<summary>Get smallest size</summary>
        public int GetSmallest()
        {
            int i;
            int iReturn = int.MaxValue;
            for (i = 0; i < iSkinUsed; i++)
            {
                if (iSizes[i] < iReturn)
                {   //Find smallest size
                    iReturn = iSizes[i];
                }
            }
            if (iReturn == int.MaxValue)
                iReturn = 0;
            return iReturn;
        }

        ///<summary>Get largest size</summary>
        public int GetLargest()
        {
            int i;
            int iReturn = 0;
            for (i = 0; i < iSkinUsed; i++)
            {
                if (iSizes[i] > iReturn)
                {   //Find largest size
                    iReturn = iSizes[i];
                }
            }
            return iReturn;
        }


        ///<summary>Get smallest size, that larger than specified (size)</summary>
        public int GetNextLarger(int iSize)
        {
            int i;
            int iReturn = int.MaxValue;
            for (i = 0; i < iSkinUsed; i++)
            {
                if (iSizes[i] > iSize && iSizes[i] < iReturn)
                {   //Find smallest size, that larger than specified
                    iReturn = iSizes[i];
                }
            }
            if (iReturn == int.MaxValue)
                iReturn = 0;
            return iReturn;
        }

        ///<summary>Get largest size, that smaller than specified (size)</summary>
        public int GetNextSmaller(int iSize)
        {
            int i;
            int iReturn = 0;
            for (i = 0; i < iSkinUsed; i++)
            {
                if (iSizes[i] < iSize && iSizes[i] > iReturn)
                {   //Find largest size, that smaller than specified
                    iReturn = iSizes[i];
                }
            }
            return iReturn;
        }

        ///<summary>Get nearest size to the specified (size)</summary>
        public int GetNearestSize(int iSize)
        {
            int i;
            int iReturn=0;
            for (i = 0; i < iSkinUsed; i++)
            {
                if (iSizes[i] <= iSize && iSizes[i] > iReturn)
                {   //Find largest size, that less or equal to specified
                    iReturn = iSizes[i];
                }
            }
            if (iReturn==0)
            {   //Noting? Find smallest size
                for (i = 0; i < iSkinUsed; i++)
                {
                    if (iReturn==0 || iSizes[i] < iReturn)
                    {   //Find smallest size
                        iReturn = iSizes[i];
                    }
                }
            }
            return iReturn;
        }
    }

    ///<summary>Identificators for entries of log file</summary>
    public enum ActionID : int
    {
        ///<summary>Program started</summary>
        Start = 0, //+
        ///<summary>Program exit</summary>
        Exit = 1, //+

        ///<summary>Level loaded</summary>
        LoadLevel = 10, //+
        ///<summary>Level restarted by player</summary>
        RestartLevel = 11, //+

        ///<summary>Position loaded</summary>
        LoadPosition = 20, //+
        ///<summary>Position saved</summary>
        SavePosition = 21, //+
        ///<summary>Position loaded</summary>
        QuickLoadPosition = 22, //n/a
        ///<summary>Position saved</summary>
        QuickSavePosition = 23, //n/a

        ///<summary>Level clicked somewhere without result</summary>
        Click = 30, //+
        ///<summary>Box highlighted (by first click)</summary>
        ClickBox = 31, //+
        ///<summary>Box pushed to new location (by second click)</summary>
        ClickPushBox = 32, //+
        ///<summary>Player traveled to new location (by single click)</summary>
        ClickTravel = 33, //+

        ///<summary>Toolbar switched</summary>
        SwitchToolbar = 38, //+

        ///<summary>Button Up pressed</summary>
        ButtonUp = 40, //+
        ///<summary>Button Down pressed</summary>
        ButtonDown = 41, //+
        ///<summary>Button Left pressed</summary>
        ButtonLeft = 42, //+
        ///<summary>Button Right pressed</summary>
        ButtonRight = 43, //+
        ///<summary>Button Undo pressed</summary>
        ButtonUndo = 44, //+
        ///<summary>Other button pressed</summary>
        ButtonOther = 45, //+

        ///<summary>Undo 1 move (by menu, not button)</summary>
        Undo1 = 50, //+
        ///<summary>Undo group of moves</summary>
        UndoGroup = 51, //+
        ///<summary>Full undo</summary>
        UndoFull = 52, //+
        ///<summary>Redo 1 move (by menu, not button)</summary>
        Redo1 = 53, //+
        ///<summary>Redo group of moves</summary>
        RedoGroup = 54, //+
        ///<summary>Full redo</summary>
        RedoFull = 55, //+

        ///<summary>Calculate deadlocks called manually</summary>
        CalculateDeadlocks = 60,//+

        ///<summary>Skin changed</summary>
        LoadSkin = 70,//+
        ///<summary>Skin changed</summary>
        LoadSkinSet = 71,//+
        ///<summary>Skin size changed</summary>
        SkinSizeChanged = 72,//+
        ///<summary>Background changed</summary>
        ChangedBackground = 73,//+

        ///<summary>Level scrolled by drag-n-drop</summary>
        DragLevel = 80,//+
        ///<summary>Level recentered</summary>
        RecenterLevel = 81,//+

        ///<summary>Dialog box displayed over main window (it do not cover all window, so thinking can proceed)</summary>
        ShowDialog = 90, //+
        ///<summary>Main Menu activated</summary>
        ShowMenuMain = 91, //+
        ///<summary>Screen Menu activated</summary>
        ShowMenuScreen = 92, //+

        ///<summary>New form shown over main window (it cover all window, so it is hard to continue thinking)</summary>
        ShowForm = 100, //+

        ///<summary>Main window activated</summary>
        WindowActivated = 110,//+
        ///<summary>Main window got focus</summary>
        WindowGotFocus = 111,//+
        ///<summary>Main window resized</summary>
        WindowResized = 112,//+
        ///<summary>Main window maximized</summary>
        WindowMaximized = 113,//+

        ///<summary>Main window normalized, i.e. maximize turned off</summary>
        WindowNormalized = 114,//+

        ///<summary>User minimize main window by 'x' or menu</summary>
        WindowMinimizedManually = 115, //+
        ///<summary>Main window deactivated</summary>
        WindowDeactivated = 116,//+
        ///<summary>Main window lost focus</summary>
        WindowLostFocus = 117,//+

        ///<summary>Antisuspend exceed timeout and do not prevent device anymore</summary>
        AntisuspendFinished = 120,//+

        ///<summary>Level solved</summary>
        LevelSolved = 200,

        ///<summary>Game-deadlock detected</summary>
        GameDeadlock = 210,

        ///<summary>Some user-generated action failed</summary>
        ActionFailed = 220,


    }


    ///<summary>Log file to log all actions</summary>
    public class LogFile
    {
        ///<summary>Stream to write file</summary>
        private System.IO.StreamWriter uLogFileStream;

        ///<summary>Start log file</summary>
        public FunctionResult Start(string sLogFileName)
        {
            try //Protection from file operations errors
            {
                uLogFileStream = new System.IO.StreamWriter(sLogFileName, true);//Open file with append
                return FunctionResult.OK;
            }
            catch
            {
                return FunctionResult.FailedToOpenFile;
            }
        }

        ///<summary>Append a line into log</summary>
        public FunctionResult LogString(ActionID ActionCode, string sString)
        {
            try //Protection from file operations errors
            {
                //DateTime uNow = DateTime.Now;
                string s = Environment.TickCount.ToString("0000");
                uLogFileStream.Write(DateTime.Now.ToString("yyyyMMdd-HHmmss.") + s.Substring(s.Length-4));
                uLogFileStream.Write("; ");
                uLogFileStream.Write(((int)ActionCode).ToString());
                uLogFileStream.Write("; ");
                uLogFileStream.WriteLine(sString);
                return FunctionResult.OK;
            }
            catch
            {
                return FunctionResult.ErrorOnWritingFile;
            }
        }

        ///<summary>Finish log and close</summary>
        public FunctionResult End()
        {
            try //Protection from file operations errors
            {
                uLogFileStream.Close();//Close file
                return FunctionResult.OK;
            }
            catch
            {
                return FunctionResult.FileSystemError;
            }
        }

    }

    ///<summary>Thread for background calculations</summary>
    public class BackgroundThread
    {
        private BackgroundFinishedCallback hFuncCallback;
        //private SokobanGame uMainGame;
        private SokobanGame uTempGame;

        ///<summary>Constructor (callback function to call on finish)</summary>
        public BackgroundThread(BackgroundFinishedCallback hCallback) 
        {
            //uMainGame = uSourceGame;
            hFuncCallback = hCallback;
        }

        ///<summary>Calculate cell deadlocks</summary>
        public void CalcDeadlocks()
        {
            //Thread.Sleep(2000);
            uTempGame.ReAnalyze();
            uTempGame.iCountDebug = 0;
            int iTime = Environment.TickCount;
            uTempGame.CalcDeadlocks();

            hFuncCallback(Environment.TickCount-iTime);
        }

        ///<summary>Provide info about game(source game - to copy)</summary>
        public void PleaseGetGame(SokobanGame uSourceGame)
        {
            uTempGame = new SokobanGame(uSourceGame);
        }

        ///<summary>Result retrieving</summary>
        public void GetResults(SokobanGame uDestinationGame)
        {
            uDestinationGame.DownloadDeadlocks(uTempGame);
            //uMainGame.Xdeadlocks
        }
    }

    ///<summary>Callback function, called by background thread after finish</summary>
    public delegate void BackgroundFinishedCallback(int iInt);

    ///<summary>Global game settings</summary>
    public class Settings
    {
        ///<summary>Redraw level for all phases of player travel to clicked location (configured in FormSettings form)</summary>
        public bool bAnimateTravel;

        ///<summary>Redraw level for all phases of pushing box to clicked location (configured in FormSettings form)</summary>
        public bool bAnimateBoxPushing;

        ///<summary>Redraw level for all phases of mass undo/redo (configured in FormSettings form)</summary>
        public bool bAnimateMassUndoRedo;

        ///<summary>On selecting level show only unsolved (configured in ChooseLevel form)</summary>
        public bool bShowOnlyUnsolvedLevels;

        ///<summary>Display addtional debug messages during play (configured in FormSettings form)</summary>
        public bool bAdditionalMessages;

        ///<summary>Ask name of record each time (configured in FormSettings form)</summary>
        public bool bAskRecordName;

        ///<summary>Ask about saving first solution of level (configured in FormSettings form)</summary>
        public bool bAskSavingFirstSolution;

        ///<summary>Automatically calculate cell-deadlocks after loading level (configured in FormSettings form)</summary>
        public bool bAutocalcDeadlocks;

        ///<summary>Box move tree stops on cell-deadlocks (configured in FormSettings form)</summary>
        public bool bDeadlockLimitsAutopush;

        ///<summary>Player name, used for records (configured in FormSettings form)</summary>
        public string sPlayerName;

        ///<summary>File name of current skin (configured by selecting skin)</summary>
        public string sSkin;

        ///<summary>Last played levelset, used on startup (configured by selecting levelset)</summary>
        public string sLastLevelSet;

        ///<summary>Last played level index, used on startup (configured by selecting level)</summary>
        public int iLastLevelPlayed;

        ///<summary>How long device will not suspend to allow user to think (configured in FormSettings form)</summary>
        public int iKeepAliveMinutes;

        ///<summary>Perform autosize after loading level (configured in FormSettings form)</summary>
        public bool bAutosize;

        ///<summary>Autosize of skin use only useful cells (configured in FormSettings form)</summary>
        public bool bAutosizeUseful;

        ///<summary>Smallest skin size allowed for autosize (configured in FormSettings form)</summary>
        public int iAutosizeLowerLimit;

        ///<summary>Last loaded skinset (configured by by selecting skinset)</summary>
        public string sSkinSet;

        ///<summary>Color to draw single-color background (configured by typing background color RRGGBB)</summary>
        public int iBackgroundColor;

        ///<summary>Texture to braw textured background (configured by loading backgroundimage)</summary>
        public string sBackgroundImageFile;

        ///<summary>Pause between "frames" of player travel to clicked location (configured in FormSettings form)</summary>
        public int iAnimationDelayTravel;

        ///<summary>Pause between "frames" of pushing box to clicked location (configured in FormSettings form)</summary>
        public int iAnimationDelayBoxPushing;

        ///<summary>Pause between "frames" of mass undo/redo (configured in FormSettings form)</summary>
        public int iAnimationDelayMassUndoRedo;

        ///<summary>Drag-n-drop sensivity, if user drag less than this value, it is considered as click (configured in FormSettings)</summary>
        public int iDragMinMove;

        ///<summary>Lock level scrolling (configured by main menu)</summary>
        public bool bScrollLock;

        ///<summary>Log all actions to file (configured by in FormSettings)</summary>
        public bool bLogActions;

        ///<summary>Minimum size of level (in total cells) when cell-deadlocks will be calculated in background</summary>
        public int iBackgroundAutoDeadlocksLimit;

        /// <summary>Default settings</summary>
        public Settings()
        {
            bAnimateTravel = true;//Full animation
            bAnimateBoxPushing = true;
            bAnimateMassUndoRedo = true;
            bShowOnlyUnsolvedLevels = false;//All levels
            bAdditionalMessages = false;//No debug message
            sPlayerName = "Player";//Default player
            sLastLevelSet = "";//No levelset and level
            iLastLevelPlayed = 0;
            sSkin = ""; //((TODO)) No skin
            bAskRecordName = false;//Not annoying
            bAskSavingFirstSolution = true;//But as for first solution
            bAutocalcDeadlocks = true;//Cell-deadlocks calculation (may be timeconsuming)
            bDeadlockLimitsAutopush = true;//This is convinient
            iKeepAliveMinutes = 5;//More or less convinient
            bAutosize = true;//This is convinient
            bAutosizeUseful = true;//This is convinient
            iAutosizeLowerLimit = 10;//TODO: estimate
            sSkinSet = "";//((TODO)) No skin set
            iBackgroundColor = 0x75759B;//Purple (TODO?)
            sBackgroundImageFile = "";//No textured background;
            iAnimationDelayTravel = 10;//TODO!
            iAnimationDelayBoxPushing = 10;
            iAnimationDelayMassUndoRedo = 10;
            iDragMinMove = 5;//Default
            bScrollLock = false;//Default
            bLogActions = false;//Default - for most users
            iBackgroundAutoDeadlocksLimit = 500;
        }

        ///<summary>Load settings from ini file (filename)</summary>
        public FunctionResult Load(string sFileName)
        {
            IniHold.IniFile uIni = new IniHold.IniFile();//Ini-file parser
            uIni.LoadIni(sFileName);//Load settings file

            //Load all settings
            sPlayerName = uIni.GetItemValue("PlayerName","Player");
            sSkin = uIni.GetItemValue("Skin","oq_16.bmp");//((TODO))
            sLastLevelSet = uIni.GetItemValue("LevelSet", "original.txt").ToLower();
            iLastLevelPlayed = OQConvertTools.string2int(uIni.GetItemValue("Level", "0"));
            bAnimateTravel = OQConvertTools.string2bool(uIni.GetItemValue("AnimateTravel", "true"));
            bAnimateBoxPushing = OQConvertTools.string2bool(uIni.GetItemValue("AnimateBoxPushing", "true"));
            bAnimateMassUndoRedo = OQConvertTools.string2bool(uIni.GetItemValue("AnimateMassUndoRedo", "true"));
            bShowOnlyUnsolvedLevels = OQConvertTools.string2bool(uIni.GetItemValue("ShowOnlyUnsolvedLevels", "false"));
            bAdditionalMessages = OQConvertTools.string2bool(uIni.GetItemValue("AdditionalMessages", "false"));
            bAskRecordName = OQConvertTools.string2bool(uIni.GetItemValue("AskRecordName", "false"));
            bAskSavingFirstSolution = OQConvertTools.string2bool(uIni.GetItemValue("AskSavingFirstSolution", "true"));
            bAutocalcDeadlocks = OQConvertTools.string2bool(uIni.GetItemValue("AutocalcDeadlocks", "true"));
            bDeadlockLimitsAutopush = OQConvertTools.string2bool(uIni.GetItemValue("DeadlockLimitsAutopush", "true"));
            iKeepAliveMinutes = OQConvertTools.string2int(uIni.GetItemValue("KeepAliveMinutes", "5"));
            bAutosize = OQConvertTools.string2bool(uIni.GetItemValue("Autosize", "true"));
            bAutosizeUseful = OQConvertTools.string2bool(uIni.GetItemValue("AutosizeUseful", "true"));
            iAutosizeLowerLimit = OQConvertTools.string2int(uIni.GetItemValue("AutosizeLowerLimit", "10"));
            sSkinSet = uIni.GetItemValue("SkinSet", "oq.sks");//((TODO))
            iBackgroundColor = OQConvertTools.hex2int(uIni.GetItemValue("BackgroundColor", "75759B"));
            if (iBackgroundColor == -1)
                iBackgroundColor = 0;//OQConvertTools.hex2int return -1 on errors
            sBackgroundImageFile = uIni.GetItemValue("BackgroundImageFile", "");

            iAnimationDelayTravel = OQConvertTools.string2int(uIni.GetItemValue("AnimationDelayTravel", "10"));
            iAnimationDelayBoxPushing = OQConvertTools.string2int(uIni.GetItemValue("AnimationDelayBoxPushing", "10"));
            iAnimationDelayMassUndoRedo = OQConvertTools.string2int(uIni.GetItemValue("AnimationDelayMassUndoRedo", "10"));
            iDragMinMove = OQConvertTools.string2int(uIni.GetItemValue("DragMinMove", "5"));
            bScrollLock = OQConvertTools.string2bool(uIni.GetItemValue("ScrollLock", "false"));
            bLogActions = OQConvertTools.string2bool(uIni.GetItemValue("LogActions", "false"));
            iBackgroundAutoDeadlocksLimit = OQConvertTools.string2int(uIni.GetItemValue("BackgroundAutoDeadlocksLimit", "500"));

            return FunctionResult.OK;
        }

        ///<summary>Save settings to ini file (filename)</summary>
        public FunctionResult Save(string sFileName)
        {
            IniHold.IniFile uIni = new IniHold.IniFile();//Ini-file parser
            System.IO.StreamWriter hAppend;//For writing files
            try
            {
                hAppend = new System.IO.StreamWriter(sFileName, false);//Open file for overwriting
                uIni.SetWriter(hAppend);//Transmit file to ini-file parser

                //Save all settings
                uIni.SaveItem("PlayerName",sPlayerName);
                uIni.SaveItem("Skin",sSkin);
                uIni.SaveItem("LevelSet",sLastLevelSet);
                uIni.SaveItem("Level",iLastLevelPlayed.ToString());
                uIni.SaveItem("AnimateTravel",bAnimateTravel.ToString());
                uIni.SaveItem("AnimateBoxPushing", bAnimateBoxPushing.ToString());
                uIni.SaveItem("AnimateMassUndoRedo", bAnimateMassUndoRedo.ToString());
                uIni.SaveItem("ShowOnlyUnsolvedLevels", bShowOnlyUnsolvedLevels.ToString());
                uIni.SaveItem("AdditionalMessages", bAdditionalMessages.ToString());
                uIni.SaveItem("AskRecordName", bAskRecordName.ToString());
                uIni.SaveItem("AskSavingFirstSolution", bAskSavingFirstSolution.ToString());
                uIni.SaveItem("AutocalcDeadlocks", bAutocalcDeadlocks.ToString());
                uIni.SaveItem("DeadlockLimitsAutopush", bDeadlockLimitsAutopush.ToString());
                uIni.SaveItem("KeepAliveMinutes", iKeepAliveMinutes.ToString());
                uIni.SaveItem("Autosize", bAutosize.ToString());
                uIni.SaveItem("AutosizeUseful", bAutosizeUseful.ToString());
                uIni.SaveItem("AutosizeLowerLimit", iAutosizeLowerLimit.ToString());
                uIni.SaveItem("SkinSet", sSkinSet.ToString());
                uIni.SaveItem("BackgroundColor", iBackgroundColor.ToString("X").PadLeft(6,'0'));
                uIni.SaveItem("BackgroundImageFile", sBackgroundImageFile.ToString());
                uIni.SaveItem("AnimationDelayTravel", iAnimationDelayTravel.ToString());
                uIni.SaveItem("AnimationDelayBoxPushing", iAnimationDelayBoxPushing.ToString());
                uIni.SaveItem("AnimationDelayMassUndoRedo", iAnimationDelayMassUndoRedo.ToString());
                uIni.SaveItem("DragMinMove", iDragMinMove.ToString());
                uIni.SaveItem("ScrollLock", bScrollLock.ToString());
                uIni.SaveItem("LogActions", bLogActions.ToString());
                uIni.SaveItem("BackgroundAutoDeadlocksLimit", iBackgroundAutoDeadlocksLimit.ToString());
                hAppend.Close();//Close file

                return FunctionResult.OK;//Successfull
            }
            catch
            {
                return FunctionResult.ErrorOnWritingFile;//Some problems with writing file
            }

        }
    }


}