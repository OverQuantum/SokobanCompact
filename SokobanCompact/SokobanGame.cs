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
using System.Drawing;

namespace SokobanCompact
{

    ///<summary>Level and position</summary>
    public class SokobanGame : SokobanLevel
    {
        /// <summary>Player to up from box</summary>
        private const int PLAYER_TO_UP = 0;
        /// <summary>Player to down from box</summary>
        private const int PLAYER_TO_DOWN = 1;
        /// <summary>Player to left from box</summary>
        private const int PLAYER_TO_LEFT = 2;
        /// <summary>Player to right from box</summary>
        private const int PLAYER_TO_RIGHT = 3;
        /// <summary>Flags of this cell</summary>
        private const int BMT_FLAGS = 4;

        /// <summary>Player can move around box from up to down</summary>
        private const short BMTF_UP_TO_DOWN = 0x1;
        /// <summary>Player can move around box from up to left</summary>
        private const short BMTF_UP_TO_LEFT = 0x2;
        /// <summary>Player can move around box from up to right</summary>
        private const short BMTF_UP_TO_RIGHT = 0x4;
        /// <summary>Player can move around box from down to left</summary>
        private const short BMTF_DOWN_TO_LEFT = 0x8;
        /// <summary>Player can move around box from down to right</summary>
        private const short BMTF_DOWN_TO_RIGHT = 0x10;
        /// <summary>Player can move around box from left to right</summary>
        private const short BMTF_LEFT_TO_RIGHT = 0x20;
        /// <summary>Cell must be checked</summary>
        private const short BMTF_TO_CHECK = 0x40;
        //private const short BMTF_TO_CHECK_ODD = 0x80;
        /// <summary>Cell is achived by box moving tree</summary>
        private const short BMTF_ACHIVED = 0x100;
        /// <summary>Logical and with this mask will remove TO_CHECK flags and keep all other</summary>
        private const short BMTF_MASK_CLEAR_TOCHECK = unchecked((short)0xFF3F);
        //private const short BMTF_MASK_TOCHECK = (short)0x00C0;

        ///<summary>Player position</summary>
        public int iPlayerX, iPlayerY;
        ///<summary>Player look into...</summary>
        public SokoMove uPlayerDir;

        ///<summary>Counter of boxes, that not on targets. If zero - level is completed</summary>
        private int iNumFreeBoxes;
        ///<summary>Counter of targets, without boxes. If zero - level is completed</summary>
        private int iNumFreeTargets;
        ///<summary>Counter of boxes, that exceed above targets but not yet in deadlocks</summary>
        public int iNumRemainExceedBoxes;

        ///<summary>Is game in game-deadlock? (updated by checking method)</summary>
        private bool bDeadlock;

        ///<summary>Number of move, that set game into game-deadlock, -1 if game is not in game-deadlock</summary>
        private int iFirstDeadlockMove; //If undo returned earlier, then game is not in game-deadlock

        ///<summary>Position statistic - moves, pushes and so on</summary>
        public PositionStats uStats;

        /// <summary>Last move stored for method RedrawAround</summary>
        public SokoMove uLastMove;

        ///<summary>Whole undo-redo stack</summary>
        private SokoMove[] bMoves;
        ///<summary>Allocated length of undo-redo stack</summary>
        private int iMovesAlloc;
        ///<summary>Used length of undo-redo stack</summary>
        private int iMovesNum;
        ///<summary>Current position in undo-redo stack</summary>
        private int iPosition;

        ///<summary>Reverted route - for calculating moving and pushing</summary>
        private SokoMove[] uReversRoute;
        ///<summary>Allocated length of reverted route</summary>
        private int iReversAlloc;
        ///<summary>Used length of reverted route</summary>
        private int iReversLen;

        ///<summary>Player move tree - distances in moves from root location</summary>
        private short[,] iMoveTree;
        ///<summary>Player move tree root location, -1 - not exist</summary>
        private int iMoveTreeFromX, iMoveTreeFromY;

        ///<summary>Box move tree - distances in moves from root location, 3rd dimension is player pos near box</summary>
        private short[, ,] iBoxMoveTree;
        ///<summary>Box move tree root location, -1 - not exist</summary>
        private int iBoxMoveTreeFromX, iBoxMoveTreeFromY;
        ///<summary>Box move tree root directions - where (relative to box) player can reach from it's current location</summary>
        private bool[] bBoxMoveTreeRootDirections;

        ///<summary>Rectangle of useful cells of level</summary>
        public Rectangle uUsefulCellsRect;

        ///<summary>Default constructor</summary>
        public SokobanGame()
        {
            BaseConstructor();
        }

        ///<summary>Copy-constructor for creating temprorary levels (source level)</summary>
        public SokobanGame(SokobanLevel uLevelToCopy)
        {
            iMovesAlloc = 0;
            iReversAlloc = 0;
            CopyFrom(uLevelToCopy);//Copy all data from source

            //Flush player and box move trees
            iMoveTreeFromX = -1; iMoveTreeFromY = -1;
            iBoxMoveTreeFromX = -1; iBoxMoveTreeFromY = -1;
        }

        ///<summary>Just allocating stack and reverted route</summary>
        private void BaseConstructor()
        {
            iMovesAlloc = 100;
            iMovesNum = 0;
            iPosition = 0;
            bMoves = new SokoMove[iMovesAlloc];

            iReversAlloc = 100;
            iReversLen = 0;
            uReversRoute = new SokoMove[iReversAlloc];

            bBoxMoveTreeRootDirections = new bool[4];

            uStats.InitZero();
        }

        ///<summary>Remove all undo/redo</summary>
        public void FlushPosition()
        {
            iMovesNum = 0;
            iPosition = 0;
            uStats.InitZero();
        }

        ///<summary>Enlarge undo/redo stack</summary>
        private void EnlargeStack()
        {
            iMovesAlloc *= 2;
            SokoMove[] bNew = new SokoMove[iMovesAlloc];
            bMoves.CopyTo(bNew, 0);
            bMoves = bNew;
        }

        ///<summary>Enlarge reverted route array</summary>
        private void EnlargeReversStack()
        {
            iReversAlloc *= 2;
            SokoMove[] bNew = new SokoMove[iReversAlloc];
            uReversRoute.CopyTo(bNew, 0);
            uReversRoute = bNew;
        }

        /// <summary>Set player position, number of boxes/targets, calc usefull rect - after loading of level</summary>
        public void ReAnalyze()
        {
            int iMinX = int.MaxValue, iMinY = int.MaxValue, iMaxX = 0, iMaxY = 0;
            SokoCell bMaskUseless = SokoCell.Background | SokoCell.Wall;

            iPlayerX = -1; iPlayerY = -2;
            iNumFreeTargets = 0;
            iNumFreeBoxes = 0;
            iNumRemainExceedBoxes = 0;
            iFirstDeadlockMove = -1;

            for (int i = 0; i < iXsize; i++)
                for (int j = 0; j < iYsize; j++)
                {
                    switch (bCells[i, j] & SokoCell.MaskAnalyze)
                    {
                        case SokoCell.Box: iNumFreeBoxes++; break;//Just box
                        case SokoCell.Target: iNumFreeTargets++; break;//Just target
                        case SokoCell.PlayerOnTarget: //Target with player
                            iNumFreeTargets++;
                            iPlayerX = i; iPlayerY = j;
                            break;
                        case SokoCell.Player://Just player
                            iPlayerX = i; iPlayerY = j;
                            break;
                    }
                    if ((bCells[i, j] & bMaskUseless) == 0)
                    {
                        if (i < iMinX) iMinX = i;
                        iMaxX = i;//i is outer loop, so it cannot decrease
                        if (j < iMinY) iMinY = j;
                        if (j > iMaxY) iMaxY = j;
                    }
                }

            uUsefulCellsRect = new Rectangle(iMinX, iMinY, 1 + iMaxX - iMinX, 1 + iMaxY - iMinY);

            if (iNumFreeBoxes > iNumFreeTargets)
                iNumRemainExceedBoxes = iNumFreeBoxes - iNumFreeTargets;//Number of exceeding boxes
        }

        ///<summary>Decrease number of remaining exceeding boxes, if some boxes in cell-deadlocks</summary>
        public void UpdateDeadlockedBoxes()
        {
            const SokoCell bDetector = SokoCell.Box | SokoCell.CellDeadlock;
            for (int i = 0; i < iXsize; i++)
                for (int j = 0; j < iYsize; j++)
                {
                    if ((bCells[i, j]&bDetector) == bDetector)
                    {
                        iNumRemainExceedBoxes--;
                    }
                }
        }

        ///<summary>Check, that box can be pushed to specified cell (x,y coordinates)</summary>
        public bool IsCellAchivedByBoxMoveTree(int x, int y)
        {
            if (iBoxMoveTreeFromX >= 0)
            {//Check by "Achived" flag in box move tree
                if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_ACHIVED) != 0) return true;
            }
            return false;
        }


        /*
        ///<summary>Wave algorithm of building box move tree (x-start,y-start)</summary>
        public FunctionResult BuildBoxMoveTreeFull_OLD(int iXstart, int iYstart)
        {
            int x, y, z;
            int iStop;
            short iThisCheck, iNextCheck;
            short iLeftToDown, iLeftToRight, iLeftToUp, iRightToDown, iRightToUp, iUpToDown;
            int iCount = 0;

            iBoxMoveTree = new short[iXsize, iYsize, 5];
            SokobanLevel uTempLevel = new SokobanLevel(this);

            //remove box from it's source location and player from current location
            uTempLevel.bCells[iPlayerX, iPlayerY] ^= SokoCell.Player;
            uTempLevel.bCells[iXstart, iYstart] ^= SokoCell.Box;

            for (y = 0; y < iYsize; y++)
                for (x = 0; x < iXsize; x++)
                {
                    for (z = 0; z < 4; z++)
                        iBoxMoveTree[x, y, z] = MT_NOT_REACHED;
                    iBoxMoveTree[x, y, BMT_FLAGS] = 0;

                    if ((uTempLevel.bCells[x, y] & SokoCell.Obstacle) != 0 || x == 0 || y == 0 || x == (iXsize - 1) || y == (iYsize - 1))
                    {
                        for (z = 0; z < 4; z++)
                            iBoxMoveTree[x, y, z] = MT_BLOCKED;
                    }
                    else
                    {
                        if ((uTempLevel.bCells[x, y - 1] & SokoCell.Obstacle) != 0) iBoxMoveTree[x, y, PLAYER_TO_UP] = MT_BLOCKED;//blocked
                        if ((uTempLevel.bCells[x - 1, y] & SokoCell.Obstacle) != 0) iBoxMoveTree[x, y, PLAYER_TO_LEFT] = MT_BLOCKED;//blocked
                        if ((uTempLevel.bCells[x, y + 1] & SokoCell.Obstacle) != 0) iBoxMoveTree[x, y, PLAYER_TO_DOWN] = MT_BLOCKED;//blocked
                        if ((uTempLevel.bCells[x + 1, y] & SokoCell.Obstacle) != 0) iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = MT_BLOCKED;//blocked
                    }

                }
            BuildPlayerMoveTree(iPlayerX, iPlayerY);
            z = 0;
            if (iMoveTree[iXstart - 1, iYstart] >= 0) { iBoxMoveTree[iXstart, iYstart, PLAYER_TO_LEFT] = iMoveTree[iXstart - 1, iYstart]; z = 1; }
            if (iMoveTree[iXstart + 1, iYstart] >= 0) { iBoxMoveTree[iXstart, iYstart, PLAYER_TO_RIGHT] = iMoveTree[iXstart + 1, iYstart]; z = 2; }
            if (iMoveTree[iXstart, iYstart - 1] >= 0) { iBoxMoveTree[iXstart, iYstart, PLAYER_TO_UP] = iMoveTree[iXstart, iYstart - 1]; z = 3; }
            if (iMoveTree[iXstart, iYstart + 1] >= 0) { iBoxMoveTree[iXstart, iYstart, PLAYER_TO_DOWN] = iMoveTree[iXstart, iYstart + 1]; z = 4; }
            if (z == 0)
            {
                //No way to selected box from current player location - exit
                return FunctionResult.StartIsBlocked;
            }

            iBoxMoveTree[iXstart, iYstart, BMT_FLAGS] = BMTF_ACHIVED | BMTF_TO_CHECK_EVEN;

            iThisCheck = BMTF_TO_CHECK_EVEN;
            iNextCheck = BMTF_TO_CHECK_ODD;


            iStop = 0;

            while (iStop == 0)
            {
                iStop = 1;
                for (y = 1; y < (iYsize - 1); y++)
                    for (x = 1; x < (iXsize - 1); x++)
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & iThisCheck) != 0)
                        {
                            iCount++;
                            //required to check

                            uTempLevel.bCells[x, y] ^= SokoCell.Box;                        //tempr. set box
                            uTempLevel.InvalidatePlayerMoveTree();//remove player move tree to not stumble with it, if it was calculate for current cell but for other box configuration

                            uTempLevel.BuildPlayerMoveTree(x - 1, y);//build player move tree from left to box
                            iLeftToRight = uTempLevel.iMoveTree[x + 1, y];
                            iLeftToUp = uTempLevel.iMoveTree[x, y - 1];
                            iLeftToDown = uTempLevel.iMoveTree[x, y + 1];

                            uTempLevel.BuildPlayerMoveTree(x + 1, y);//from right
                            iRightToDown = uTempLevel.iMoveTree[x, y + 1];
                            iRightToUp = uTempLevel.iMoveTree[x, y - 1];

                            uTempLevel.BuildPlayerMoveTree(x, y + 1); //from down
                            iUpToDown = uTempLevel.iMoveTree[x, y - 1];

                            uTempLevel.bCells[x, y] ^= SokoCell.Box; //remove box

                            if (iLeftToDown > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_DOWN_TO_LEFT;
                            if (iLeftToRight > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_LEFT_TO_RIGHT;
                            if (iLeftToUp > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_LEFT;
                            if (iRightToDown > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_DOWN_TO_RIGHT;
                            if (iRightToUp > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_RIGHT;
                            if (iUpToDown > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_DOWN;

                            //if (x == 6 && y == 7)
                            //{
                            //    x = x;
                            //}

                            if (iBoxMoveTree[x, y, PLAYER_TO_UP] >= 0)
                            {//Player-to-up is achived
                                if (iLeftToUp > 0 && iBoxMoveTree[x, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_LEFT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_UP] + iLeftToUp); iStop = 0; }
                                if (iUpToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_DOWN] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_DOWN] = (short)(iBoxMoveTree[x, y, PLAYER_TO_UP] + iUpToDown); iStop = 0; }
                                if (iRightToUp > 0 && iBoxMoveTree[x, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_UP] + iRightToUp); iStop = 0; }
                            }
                            if (iBoxMoveTree[x, y, PLAYER_TO_LEFT] >= 0)
                            {//Player-to-left is achived
                                if (iLeftToUp > 0 && iBoxMoveTree[x, y, PLAYER_TO_UP] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_UP] = (short)(iBoxMoveTree[x, y, PLAYER_TO_LEFT] + iLeftToUp); iStop = 0; }
                                if (iLeftToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_DOWN] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_DOWN] = (short)(iBoxMoveTree[x, y, PLAYER_TO_LEFT] + iLeftToDown); iStop = 0; }
                                if (iLeftToRight > 0 && iBoxMoveTree[x, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_LEFT] + iLeftToRight); iStop = 0; }
                            }
                            if (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] >= 0)
                            {//Player-to-right is achived
                                if (iLeftToRight > 0 && iBoxMoveTree[x, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_LEFT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_RIGHT] + iLeftToRight); iStop = 0; }
                                if (iRightToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_DOWN] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_DOWN] = (short)(iBoxMoveTree[x, y, PLAYER_TO_RIGHT] + iRightToDown); iStop = 0; }
                                if (iRightToUp > 0 && iBoxMoveTree[x, y, PLAYER_TO_UP] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_UP] = (short)(iBoxMoveTree[x, y, PLAYER_TO_RIGHT] + iRightToUp); iStop = 0; }
                            }
                            if (iBoxMoveTree[x, y, PLAYER_TO_DOWN] >= 0)
                            {//Player-to-down is achived
                                if (iLeftToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_LEFT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_DOWN] + iLeftToDown); iStop = 0; }
                                if (iUpToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_UP] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_UP] = (short)(iBoxMoveTree[x, y, PLAYER_TO_DOWN] + iUpToDown); iStop = 0; }
                                if (iRightToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED)
                                { iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_DOWN] + iRightToDown); iStop = 0; }
                            }

                            if (iBoxMoveTree[x, y, PLAYER_TO_UP] >= 0)
                            {//Player-to-up is achived
                                if (iBoxMoveTree[x, y + 1, PLAYER_TO_UP] == MT_NOT_REACHED)
                                {
                                    iBoxMoveTree[x, y + 1, PLAYER_TO_UP] = (short)(iBoxMoveTree[x, y, PLAYER_TO_UP] + 1);
                                    iBoxMoveTree[x, y + 1, BMT_FLAGS] |= iNextCheck;//Set "to check" flag
                                    iStop = 0;
                                }
                            }
                            if (iBoxMoveTree[x, y, PLAYER_TO_LEFT] >= 0)
                            {//Player-to-left is achived
                                if (iBoxMoveTree[x + 1, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)
                                {
                                    iBoxMoveTree[x + 1, y, PLAYER_TO_LEFT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_LEFT] + 1);
                                    iBoxMoveTree[x + 1, y, BMT_FLAGS] |= iNextCheck;
                                    iStop = 0;
                                }
                            }
                            if (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] >= 0)
                            {//Player-to-right is achived
                                if (iBoxMoveTree[x - 1, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED)
                                {
                                    iBoxMoveTree[x - 1, y, PLAYER_TO_RIGHT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_RIGHT] + 1);
                                    iBoxMoveTree[x - 1, y, BMT_FLAGS] |= iNextCheck;
                                    iStop = 0;
                                }
                            }
                            if (iBoxMoveTree[x, y, PLAYER_TO_DOWN] >= 0)
                            {//Player-to-down is achived
                                if (iBoxMoveTree[x, y - 1, PLAYER_TO_DOWN] == MT_NOT_REACHED)
                                {
                                    iBoxMoveTree[x, y - 1, PLAYER_TO_DOWN] = (short)(iBoxMoveTree[x, y, PLAYER_TO_DOWN] + 1);
                                    iBoxMoveTree[x, y - 1, BMT_FLAGS] |= iNextCheck;
                                    iStop = 0;
                                }
                            }

                            //Remove "to check" flag from this cell
                            iBoxMoveTree[x, y, BMT_FLAGS] &= BMTF_MASK_CLEAR_TOCHECK;
                            iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_ACHIVED;
                        }
                if (iThisCheck == BMTF_TO_CHECK_ODD)
                {
                    iThisCheck = BMTF_TO_CHECK_EVEN;
                    iNextCheck = BMTF_TO_CHECK_ODD;
                }
                else
                {
                    iThisCheck = BMTF_TO_CHECK_ODD;
                    iNextCheck = BMTF_TO_CHECK_EVEN;
                }
            }
            iBoxMoveTreeFromX = iXstart;
            iBoxMoveTreeFromY = iYstart;
            return FunctionResult.OK;
        }
        */

        //with XXYY - 100ms on aenigma-42, 62ms on aborelia-22
        //with UpTo only - 120ms on aenigma-42, 62ms on aborelia-22
        //without - 140ms on aenigma-42, 70ms on aborelia-22, 66 on aenigma-39, 54 on aenigma-36
        //UpTo + 6step - 85 on aenigma-42, 60 on aenigma-39, 50 on aenigma-36, 66 on aborelia-22


        ///<summary>Wave algorithm of building box move tree (x,y of box start, flag of stop on cell-deadlock or not)</summary>
        public FunctionResult BuildBoxMoveTreeFull(int iXstart, int iYstart, bool bStopOnDeadlocks)
        {
            /*
             * Function build box move tree by wave algorithm - from current location box moving propagated
             *   to all possible (by game rulse) directions with chosing minimal (by moves) actions to reach
             *   one box position by different ways.
             * Resulting tree can be used to quickly build moving-pushing sequence to push box into any location (only if location is achived)
             */
            int x, y, z;
            short iLeftToDown, iLeftToRight, iLeftToUp, iRightToDown, iRightToUp, iUpToDown;
            bool bLeft, bRight, bUp, bDown;

            iBoxMoveTree = new short[iXsize, iYsize, 5];//Alloc box move tree array
            SokobanGame uTempLevel = new SokobanGame(this);//Temprorary level as copy of current
            CoordQueue uQueue = new CoordQueue();//Queue for verifying cells

            SokoCell bObstacle = SokoCell.MaskObstacle;//Ordinary obstacle mask
            if (bStopOnDeadlocks)
                bObstacle |= SokoCell.CellDeadlock;//Add cell-deadlock as obstacle

            //Remove box from it's source location and player from current location
            uTempLevel.bCells[iPlayerX, iPlayerY] ^= SokoCell.Player;
            uTempLevel.bCells[iXstart, iYstart] ^= SokoCell.Box;

            //Mark forbidden branches of tree
            for (y = 0; y < iYsize; y++)
                for (x = 0; x < iXsize; x++)
                {
                    for (z = 0; z < 4; z++)
                        iBoxMoveTree[x, y, z] = MT_NOT_REACHED;//All - not yet reached
                    iBoxMoveTree[x, y, BMT_FLAGS] = 0;

                    if ((uTempLevel.bCells[x, y] & bObstacle) != 0 || x == 0 || y == 0 || x == (iXsize - 1) || y == (iYsize - 1))
                    {//Obstacle on cell or border cell (topmost, bottommost, leftmost and rightmost lines of level MUST contain ONLY walls or backgrounds, so box move tree should not reach them)
                        for (z = 0; z < 4; z++)
                            iBoxMoveTree[x, y, z] = MT_BLOCKED;
                    }
                    else
                    {//Check blocking by obstacle at player position
                        if ((uTempLevel.bCells[x, y - 1] & SokoCell.MaskObstacle) != 0) iBoxMoveTree[x, y, PLAYER_TO_UP] = MT_BLOCKED;//blocked
                        if ((uTempLevel.bCells[x - 1, y] & SokoCell.MaskObstacle) != 0) iBoxMoveTree[x, y, PLAYER_TO_LEFT] = MT_BLOCKED;//blocked
                        if ((uTempLevel.bCells[x, y + 1] & SokoCell.MaskObstacle) != 0) iBoxMoveTree[x, y, PLAYER_TO_DOWN] = MT_BLOCKED;//blocked
                        if ((uTempLevel.bCells[x + 1, y] & SokoCell.MaskObstacle) != 0) iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = MT_BLOCKED;//blocked
                    }

                }

            //Check that box is reachable by player and set "seed"/root for box move tree
            BuildPlayerMoveTree(iPlayerX, iPlayerY);
            for (z = 0; z < 4; z++)
                bBoxMoveTreeRootDirections[z] = false;
            z = 0;
            if (iMoveTree[iXstart - 1, iYstart] >= 0) { iBoxMoveTree[iXstart, iYstart, PLAYER_TO_LEFT] = iMoveTree[iXstart - 1, iYstart]; z = 1; bBoxMoveTreeRootDirections[PLAYER_TO_LEFT] = true; }
            if (iMoveTree[iXstart + 1, iYstart] >= 0) { iBoxMoveTree[iXstart, iYstart, PLAYER_TO_RIGHT] = iMoveTree[iXstart + 1, iYstart]; z = 2; bBoxMoveTreeRootDirections[PLAYER_TO_RIGHT] = true; }
            if (iMoveTree[iXstart, iYstart - 1] >= 0) { iBoxMoveTree[iXstart, iYstart, PLAYER_TO_UP] = iMoveTree[iXstart, iYstart - 1]; z = 3; bBoxMoveTreeRootDirections[PLAYER_TO_UP] = true; }
            if (iMoveTree[iXstart, iYstart + 1] >= 0) { iBoxMoveTree[iXstart, iYstart, PLAYER_TO_DOWN] = iMoveTree[iXstart, iYstart + 1]; z = 4; bBoxMoveTreeRootDirections[PLAYER_TO_DOWN] = true; }
            if (z == 0)
            {
                //No way to specified box from current player location - exit
                return FunctionResult.StartIsBlocked;
            }
            
            //Root of tree - start location
            iBoxMoveTree[iXstart, iYstart, BMT_FLAGS] = BMTF_ACHIVED | BMTF_TO_CHECK;
            uQueue.AddNext(iXstart, iYstart);

            //Propagating tree to all cells - by checking cells, received from queue and adding new cells to this queue
            x = 0; y = 0;
            while (uQueue.Get(ref x, ref y))//Get next cell from queue
            {
                if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_TO_CHECK) != 0)
                {
                    //If cell is marked as required to check (i.e. was marked but not yet checked since that)

                    //Get blocking state of 4 possible player positions around box
                    bUp = (iBoxMoveTree[x, y, PLAYER_TO_UP] != MT_BLOCKED);
                    bLeft = (iBoxMoveTree[x, y, PLAYER_TO_LEFT] != MT_BLOCKED);
                    bRight = (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] != MT_BLOCKED);
                    bDown = (iBoxMoveTree[x, y, PLAYER_TO_DOWN] != MT_BLOCKED);

                    //Paths between all this 4 positions - initially they are not known
                    iLeftToRight = MT_NOT_REACHED;
                    iLeftToUp = MT_NOT_REACHED;
                    iLeftToDown = MT_NOT_REACHED;
                    iRightToDown = MT_NOT_REACHED;
                    iRightToUp = MT_NOT_REACHED;
                    iUpToDown = MT_NOT_REACHED;


                    if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_ACHIVED) != 0)
                    {
                        //Not first pass of this cell
                        iCountDebug++;//Count all additional passes
                    }


                    uTempLevel.bCells[x, y] ^= SokoCell.Box;//Temprorary put box to this cell
                    uTempLevel.InvalidatePlayerMoveTree();//Flush player move tree to not stumble with it, if it was calculate for current cell but for other box configuration

                    //If some positions around box is blocked - block all corresponded paths
                    if (!bLeft) { iLeftToDown = MT_BLOCKED; iLeftToRight = MT_BLOCKED; iLeftToUp = MT_BLOCKED; }
                    if (!bRight) { iRightToDown = MT_BLOCKED; iLeftToRight = MT_BLOCKED; iRightToUp = MT_BLOCKED; }
                    if (!bUp) { iUpToDown = MT_BLOCKED; iRightToUp = MT_BLOCKED; iLeftToUp = MT_BLOCKED; }
                    if (!bDown) { iLeftToDown = MT_BLOCKED; iRightToDown = MT_BLOCKED; iUpToDown = MT_BLOCKED; }

                    /*
                    //Check simple "turn over box in two steps"
                    if (bLeft)
                    {
                        if (bUp)
                            if ((uTempLevel.bCells[x - 1, y - 1] & SokoCell.Obstacle) ==0) iLeftToUp =2;
                        if (bDown)
                            if ((uTempLevel.bCells[x - 1, y + 1] & SokoCell.Obstacle) == 0) iLeftToDown = 2;
                        if (iLeftToUp == 2 && iLeftToDown == 2)
                            iUpToDown = 4;//two turns - go box around in 4 steps
                    }
                    if (bRight )
                    {
                        if (bUp)
                            if ((uTempLevel.bCells[x + 1, y - 1] & SokoCell.Obstacle) == 0) iRightToUp = 2;
                        if (bDown)
                            if ((uTempLevel.bCells[x + 1, y + 1] & SokoCell.Obstacle) == 0) iRightToDown = 2;
                        if (iRightToUp == 2 && iRightToDown == 2)
                            iUpToDown = 4;//two turns - go box around in 4 steps
                    }/**/


                    //Check simple "turn over box" in two steps
                    if (bLeft)
                    {
                        if (bUp)
                            if ((uTempLevel.bCells[x - 1, y - 1] & SokoCell.MaskObstacle) == 0) iLeftToUp = 2;//Left, up and left-up cells are empty - so player can move from up to left in 2 steps
                        if (bDown)
                            if ((uTempLevel.bCells[x - 1, y + 1] & SokoCell.MaskObstacle) == 0) //Left, down and left-down cells are empty ...
                            {
                                iLeftToDown = 2;
                                if (iLeftToUp == 2)
                                    iUpToDown = 4;//Two turns - player can go around box in 4 steps
                            }
                    }
                    if (bRight)
                    {//By analogue...
                        if (bUp)
                            if ((uTempLevel.bCells[x + 1, y - 1] & SokoCell.MaskObstacle) == 0) iRightToUp = 2;
                        if (bDown)
                            if ((uTempLevel.bCells[x + 1, y + 1] & SokoCell.MaskObstacle) == 0)
                            {
                                iRightToDown = 2;
                                if (iRightToUp == 2)
                                    iUpToDown = 4;//Two turns - player can go around box in 4 steps
                            }
                    }

                    //Check left-to-right pass in two-turn-combo and three-turn-combo
                    if (iRightToUp == 2 && iLeftToUp == 2)
                    {//Can go right-to-up and from up-to-left -> can go right-to-left in 4 steps
                        iLeftToRight = 4;
                        if (iLeftToDown == MT_NOT_REACHED && iRightToDown == 2)
                            iLeftToDown = 6;//Also can go right-to-down -> can go left-to-down is 6 steps (if not yet)
                        else if (iLeftToDown == 2 && iRightToDown == MT_NOT_REACHED)
                            iRightToDown = 6;//Also can go left-to-down -> can go right-to-down in 6 steps (if not yet)
                    }
                    else if (iRightToDown == 2 && iLeftToDown == 2)
                    {//By analogue...
                        iLeftToRight = 4;
                        if (iLeftToUp == MT_NOT_REACHED && iRightToUp == 2)
                            iLeftToUp = 6;
                        else if (iLeftToUp == 2 && iRightToUp == MT_NOT_REACHED)
                            iRightToUp = 6;
                    }

                    if (iLeftToRight == MT_NOT_REACHED || iLeftToUp == MT_NOT_REACHED || iLeftToDown == MT_NOT_REACHED)
                    {//Some left-to is not yet calculated - can calculate them only by player move tree
                        uTempLevel.BuildPlayerMoveTree(x - 1, y);//Build player move tree from left to box
                        //uTempLevel.BuildPlayerMoveTree_UpTo(x - 1, y, x + 1, y);//from left to right
                        //uTempLevel.BuildPlayerMoveTree_XXYY(x - 1, y, x+1,x,y-1,y+1);//from left
                        iLeftToRight = uTempLevel.iMoveTree[x + 1, y];//And get reachability from player move tree
                        iLeftToUp = uTempLevel.iMoveTree[x, y - 1];
                        iLeftToDown = uTempLevel.iMoveTree[x, y + 1];

                        /*
                         * inefficient block
                        if (iLeftToRight > 0 && iLeftToUp <0)
                        {//there is way from left to right, but no way from left to up => no way from right to up
                            iRightToUp = MT_WILL_NOT_REACH;
                        }
                        if (iLeftToRight > 0 && iLeftToDown < 0)
                        {//there is way from left to right, but no way from left to down => no way from right to down
                            iRightToDown = MT_WILL_NOT_REACH;
                        }
                        if (iLeftToUp > 0 && iLeftToDown < 0)
                        {//there is way from left to up, but no way from left to down => no way from up to down
                            iUpToDown = MT_WILL_NOT_REACH;
                        }
                        if (iLeftToRight < 0 && iLeftToUp > 0)
                        {//there is way from left to right, but no way from left to up => no way from right to up
                            iRightToUp = MT_WILL_NOT_REACH;
                        }
                        if (iLeftToRight < 0 && iLeftToDown > 0)
                        {//there is way from left to right, but no way from left to down => no way from right to down
                            iRightToDown = MT_WILL_NOT_REACH;
                        }
                        if (iLeftToUp < 0 && iLeftToDown > 0)
                        {//there is way from left to up, but no way from left to down => no way from up to down
                            iUpToDown = MT_WILL_NOT_REACH;
                        }*/

                        /*
                        if (iLeftToRight * iLeftToUp < 0)
                        {//there is way from left to right, but no way from left to up => no way from right to up
                            iRightToUp = MT_WILL_NOT_REACH;
                        }
                        if (iLeftToRight * iLeftToDown < 0)
                        {//there is way from left to right, but no way from left to down => no way from right to down
                            iRightToDown = MT_WILL_NOT_REACH;
                        }
                        if (iLeftToUp * iLeftToDown < 0)
                        {//there is way from left to up, but no way from left to down => no way from up to down
                            iUpToDown = MT_WILL_NOT_REACH;
                        }/**/
                    }

                    if (iRightToDown == MT_NOT_REACHED || iRightToUp == MT_NOT_REACHED)
                    {//By analogue...
                        uTempLevel.BuildPlayerMoveTree(x + 1, y);//from right
                        //uTempLevel.BuildPlayerMoveTree_XYY(x + 1, y,x, y+1,y-1);//from right
                        iRightToDown = uTempLevel.iMoveTree[x, y + 1];
                        iRightToUp = uTempLevel.iMoveTree[x, y - 1];

                        /*
                        inefficient block
                        if (iRightToDown * iRightToUp < 0)
                        {//there is way from right to down, but no way from right to up or visa-versa => no way from up to down
                            iUpToDown = MT_WILL_NOT_REACH;
                        }/**/
                        /*
                        if (iRightToDown >0 && iRightToUp < 0)
                        {//there is way from right to down, but no way from right to up or visa-versa => no way from up to down
                            iUpToDown = MT_WILL_NOT_REACH;
                        }
                        if (iRightToDown <0 && iRightToUp > 0)
                        {//there is way from right to down, but no way from right to up or visa-versa => no way from up to down
                            iUpToDown = MT_WILL_NOT_REACH;
                        }*/
                    }

                    if (iUpToDown == MT_NOT_REACHED)
                    {//By analogue... up-to-down is still unknown
                        uTempLevel.BuildPlayerMoveTree(x, y + 1); //from down
                        //uTempLevel.BuildPlayerMoveTree_UpTo(x, y + 1,x,y-1); //from down
                        iUpToDown = uTempLevel.iMoveTree[x, y - 1];
                    }

                    uTempLevel.bCells[x, y] ^= SokoCell.Box; //Remove temp. box

                    //Set flags of possiblity to walk by paths around box
                    if (iLeftToDown > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_DOWN_TO_LEFT;
                    if (iLeftToRight > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_LEFT_TO_RIGHT;
                    if (iLeftToUp > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_LEFT;
                    if (iRightToDown > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_DOWN_TO_RIGHT;
                    if (iRightToUp > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_RIGHT;
                    if (iUpToDown > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_DOWN;

                    //Check propagating tree "around box"
                    if (iBoxMoveTree[x, y, PLAYER_TO_UP] >= 0)
                    {//Player-to-up is achived by tree
                        if (iLeftToUp > 0 && iBoxMoveTree[x, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)//Can walk up-to-left and left is not yet achived
                        { iBoxMoveTree[x, y, PLAYER_TO_LEFT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_UP] + iLeftToUp); }//Achive left
                        if (iUpToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_DOWN] == MT_NOT_REACHED)//Can walk up-to-down and down is not yet achived
                        { iBoxMoveTree[x, y, PLAYER_TO_DOWN] = (short)(iBoxMoveTree[x, y, PLAYER_TO_UP] + iUpToDown); }//Achive down
                        if (iRightToUp > 0 && iBoxMoveTree[x, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED)//By analogue...
                        { iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_UP] + iRightToUp); }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_LEFT] >= 0)
                    {//Player-to-left is achived by tree ... by analogue...
                        if (iLeftToUp > 0 && iBoxMoveTree[x, y, PLAYER_TO_UP] == MT_NOT_REACHED)
                        { iBoxMoveTree[x, y, PLAYER_TO_UP] = (short)(iBoxMoveTree[x, y, PLAYER_TO_LEFT] + iLeftToUp); }
                        if (iLeftToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_DOWN] == MT_NOT_REACHED)
                        { iBoxMoveTree[x, y, PLAYER_TO_DOWN] = (short)(iBoxMoveTree[x, y, PLAYER_TO_LEFT] + iLeftToDown); }
                        if (iLeftToRight > 0 && iBoxMoveTree[x, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED)
                        { iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_LEFT] + iLeftToRight); }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] >= 0)
                    {//Player-to-right is achived by tree ... by analogue...
                        if (iLeftToRight > 0 && iBoxMoveTree[x, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)
                        { iBoxMoveTree[x, y, PLAYER_TO_LEFT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_RIGHT] + iLeftToRight); }
                        if (iRightToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_DOWN] == MT_NOT_REACHED)
                        { iBoxMoveTree[x, y, PLAYER_TO_DOWN] = (short)(iBoxMoveTree[x, y, PLAYER_TO_RIGHT] + iRightToDown); }
                        if (iRightToUp > 0 && iBoxMoveTree[x, y, PLAYER_TO_UP] == MT_NOT_REACHED)
                        { iBoxMoveTree[x, y, PLAYER_TO_UP] = (short)(iBoxMoveTree[x, y, PLAYER_TO_RIGHT] + iRightToUp); }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_DOWN] >= 0)
                    {//Player-to-down is achived by tree ... by analogue...
                        if (iLeftToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)
                        { iBoxMoveTree[x, y, PLAYER_TO_LEFT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_DOWN] + iLeftToDown); }
                        if (iUpToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_UP] == MT_NOT_REACHED)
                        { iBoxMoveTree[x, y, PLAYER_TO_UP] = (short)(iBoxMoveTree[x, y, PLAYER_TO_DOWN] + iUpToDown); }
                        if (iRightToDown > 0 && iBoxMoveTree[x, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED)
                        { iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_DOWN] + iRightToDown); }
                    }

                    //Check propagating tree by pushing box
                    if (iBoxMoveTree[x, y, PLAYER_TO_UP] >= 0)
                    {//Player-to-up is achived by tree
                        if (iBoxMoveTree[x, y + 1, PLAYER_TO_UP] == MT_NOT_REACHED)
                        {//Player-to-up is not achived for cell to down (from currnet) - so box can be pushed to current cell from cell-to-down
                            iBoxMoveTree[x, y + 1, PLAYER_TO_UP] = (short)(iBoxMoveTree[x, y, PLAYER_TO_UP] + 1);//Achive player-to-up for cell-to-down
                            iBoxMoveTree[x, y + 1, BMT_FLAGS] |= BMTF_TO_CHECK;//Set "to check" flag for cell-to-down
                            uQueue.AddNext(x, y + 1);//Add cell-to-down into queue
                        }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_LEFT] >= 0)
                    {//Player-to-left is achived by tree
                        if (iBoxMoveTree[x + 1, y, PLAYER_TO_LEFT] == MT_NOT_REACHED) //By analogue...
                        {
                            iBoxMoveTree[x + 1, y, PLAYER_TO_LEFT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_LEFT] + 1);
                            iBoxMoveTree[x + 1, y, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x + 1, y);
                        }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] >= 0)
                    {//Player-to-right is achived
                        if (iBoxMoveTree[x - 1, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED) //By analogue...
                        {
                            iBoxMoveTree[x - 1, y, PLAYER_TO_RIGHT] = (short)(iBoxMoveTree[x, y, PLAYER_TO_RIGHT] + 1);
                            iBoxMoveTree[x - 1, y, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x - 1, y);
                        }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_DOWN] >= 0)
                    {//Player-to-down is achived
                        if (iBoxMoveTree[x, y - 1, PLAYER_TO_DOWN] == MT_NOT_REACHED) //By analogue...
                        {
                            iBoxMoveTree[x, y - 1, PLAYER_TO_DOWN] = (short)(iBoxMoveTree[x, y, PLAYER_TO_DOWN] + 1);
                            iBoxMoveTree[x, y - 1, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x, y - 1);
                        }
                    }

                    //Remove "to check" flag for this cell
                    iBoxMoveTree[x, y, BMT_FLAGS] &= BMTF_MASK_CLEAR_TOCHECK;
                    iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_ACHIVED;//Cell is achived by tree
                }
            }

            //Queue ends - tree builded

            //Remember position of root of tree
            iBoxMoveTreeFromX = iXstart;
            iBoxMoveTreeFromY = iYstart;
            return FunctionResult.OK;
        }


        ///<summary>Flush box moving tree (on moving player or level change)</summary>
        public void InvalidateBoxMoveTree()
        {
            iBoxMoveTreeFromX = -1;
            //iBoxMoveTreeFromY = -1;
        }

        ///<summary>Flush player moving tree (on change of box position or level change)</summary>
        public void InvalidatePlayerMoveTree()
        {
            iMoveTreeFromX = -1;
        }

        ///<summary>Wave algorithm of building player moving tree (x, y of start)</summary>
        private FunctionResult BuildPlayerMoveTree(int iXstart, int iYstart)
        {
            short iDepth;
            int x, y;

            if (iMoveTreeFromX == iXstart && iMoveTreeFromY == iYstart)
                return FunctionResult.NothingToDo;//Tree is already build from exact the same location

            InvalidatePlayerMoveTree();//Flush old tree root
            iMoveTree = new short[iXsize, iYsize];//(Re)create array for tree
            CoordQueue uQueue = new CoordQueue();

            //Initiate array
            for (int i = 0; i < iXsize; i++)
                for (int j = 0; j < iYsize; j++)
                {
                    if ((bCells[i, j] & SokoCell.MaskObstacle) != 0 || i == 0 || i == (iXsize - 1) || j == 0 || j == (iYsize - 1))
                    {//Obstacle on cell or border cell (topmost, bottommost, leftmost and rightmost lines of level MUST contain ONLY walls or backgrounds, so box move tree should not reach them)
                        iMoveTree[i, j] = MT_BLOCKED;
                    }
                    else
                    {//Other cells - could be reached
                        iMoveTree[i, j] = MT_NOT_REACHED;
                    }
                }
            if (iMoveTree[iXstart, iYstart] == MT_BLOCKED)
                return FunctionResult.StartIsBlocked;//Start is blocked, so no tree

            iMoveTree[iXstart, iYstart] = 0;//Start pos

            uQueue.AddNext(iXstart, iYstart);//Add root of tree to queue
            x = 0; y = 0; //Initiate x and y, otherwise not compiled

            //Propagating tree to all cells - by checking cells, received from queue and adding new cells to this queue
            while (uQueue.Get(ref x, ref y))
            {
                iDepth = iMoveTree[x, y]; iDepth++;//Get depth from cell and increment
                if (iMoveTree[x - 1, y] == MT_NOT_REACHED) { iMoveTree[x - 1, y] = iDepth; uQueue.AddNext(x - 1, y); }//Propagate tree for nearest cells if tree is not yet reached this cells
                if (iMoveTree[x + 1, y] == MT_NOT_REACHED) { iMoveTree[x + 1, y] = iDepth; uQueue.AddNext(x + 1, y); }
                if (iMoveTree[x, y - 1] == MT_NOT_REACHED) { iMoveTree[x, y - 1] = iDepth; uQueue.AddNext(x, y - 1); }
                if (iMoveTree[x, y + 1] == MT_NOT_REACHED) { iMoveTree[x, y + 1] = iDepth; uQueue.AddNext(x, y + 1); }
            }
            //Tree Builded

            iMoveTreeFromX = iXstart;//Store root location
            iMoveTreeFromY = iYstart;

            return FunctionResult.OK;
        }

        ///<summary>Wave algorithm of building player moving tree till specific cell is reached (x, y of start, x, y of target)</summary>
        private FunctionResult BuildPlayerMoveTree_UpTo(int iXstart, int iYstart, int iXend, int iYend)
        {
            short iDepth;
            int x, y;

            if (iMoveTreeFromX == iXstart && iMoveTreeFromY == iYstart)
                return FunctionResult.NothingToDo;//Tree is already build from exact the same location

            InvalidatePlayerMoveTree();//Flush old tree root
            iMoveTree = new short[iXsize, iYsize];//(Re)create array for tree
            CoordQueue uQueue = new CoordQueue();

            //Initiate array
            for (int i = 0; i < iXsize; i++)
                for (int j = 0; j < iYsize; j++)
                {
                    /*
                    if ((bCells[i, j] & SokoCell.Obstacle) == 0)
                    {
                        iMoveTree[i, j] = MT_NOT_REACHED;
                    }
                    else
                    {
                        iMoveTree[i, j] = MT_BLOCKED;
                    }*/

                    if ((bCells[i, j] & SokoCell.MaskObstacle) != 0 || i == 0 || i == (iXsize - 1) || j == 0 || j == (iYsize - 1))
                    {//Obstacle on cell or border cell (topmost, bottommost, leftmost and rightmost lines of level MUST contain ONLY walls or backgrounds, so box move tree should not reach them)
                        iMoveTree[i, j] = MT_BLOCKED;
                    }
                    else
                    {//Other cells - could be reached
                        iMoveTree[i, j] = MT_NOT_REACHED;
                    }
                }
            if (iMoveTree[iXstart, iYstart] == MT_BLOCKED)
                return FunctionResult.StartIsBlocked;//Start is blocked, so no tree

            iMoveTree[iXstart, iYstart] = 0;//Start pos

            uQueue.AddNext(iXstart, iYstart);//Add root of tree to queue
            x = 0; y = 0; //Initiate x and y, otherwise not compiled

            //Propagating tree to all cells - by checking cells, received from queue and adding new cells to this queue
            while (uQueue.Get(ref x, ref y))
            {
                if (x == iXend)
                {
                    if (y == iYend)
                    {//Specified cell is reached - can exit
                        return FunctionResult.PathFound;
                    }
                }
                iDepth = iMoveTree[x, y]; iDepth++;//Get depth from cell and increment
                if (iMoveTree[x - 1, y] == MT_NOT_REACHED) { iMoveTree[x - 1, y] = iDepth; uQueue.AddNext(x - 1, y); }//Propagate tree for nearest cells if tree is not yet reached this cells
                if (iMoveTree[x + 1, y] == MT_NOT_REACHED) { iMoveTree[x + 1, y] = iDepth; uQueue.AddNext(x + 1, y); }
                if (iMoveTree[x, y - 1] == MT_NOT_REACHED) { iMoveTree[x, y - 1] = iDepth; uQueue.AddNext(x, y - 1); }
                if (iMoveTree[x, y + 1] == MT_NOT_REACHED) { iMoveTree[x, y + 1] = iDepth; uQueue.AddNext(x, y + 1); }
            }

            //Tree Builded

            //Root location is not stored

            return FunctionResult.OK;
        }

        /*
        ///<summary>Wave algorithm of building player moving tree (x-start,y-start)</summary>
        private FunctionResult BuildPlayerMoveTree_XYY(int iXstart, int iYstart, int iXe1, int iYe1, int iYe2)
        {
            short iDepth;
            int x, y;
            int iR = 0;

            if (iMoveTreeFromX == iXstart && iMoveTreeFromY == iYstart)
                return FunctionResult.NothingToDo;

            InvalidatePlayerMoveTree();
            iMoveTree = new short[iXsize, iYsize];
            CoordQueue uQueue = new CoordQueue();

            //?? Is there any memset-like way to set all cells?
            //Array.Clear(iMoveTree,0,0);
            for (int i = 0; i < iXsize; i++)
                for (int j = 0; j < iYsize; j++)
                {
                    if ((bCells[i, j] & SokoCell.Obstacle) == 0)
                    {
                        iMoveTree[i, j] = MT_NOT_REACHED;
                    }
                    else
                    {
                        iMoveTree[i, j] = MT_BLOCKED;
                    }
                }
            if (iMoveTree[iXstart, iYstart] == MT_BLOCKED)
                return FunctionResult.StartIsBlocked;//Start is blocked, so no tree

            iMoveTree[iXstart, iYstart] = 0;//Start pos
            uQueue.AddNext(iXstart, iYstart);
            x = 0;
            y = 0;
            while (uQueue.Get(ref x, ref y))
            {
                if (x == iXe1)
                {
                    if (y == iYe1)
                    {
                        iR += 1;
                    }
                    else if (y == iYe2)
                    {
                        iR += 2;
                    }
                    if (iR == 3) return FunctionResult.PathFound;
                }
                iDepth = iMoveTree[x, y]; iDepth++;
                if (iMoveTree[x - 1, y] == MT_NOT_REACHED) { iMoveTree[x - 1, y] = iDepth; uQueue.AddNext(x - 1, y); }
                if (iMoveTree[x + 1, y] == MT_NOT_REACHED) { iMoveTree[x + 1, y] = iDepth; uQueue.AddNext(x + 1, y); }
                if (iMoveTree[x, y - 1] == MT_NOT_REACHED) { iMoveTree[x, y - 1] = iDepth; uQueue.AddNext(x, y - 1); }
                if (iMoveTree[x, y + 1] == MT_NOT_REACHED) { iMoveTree[x, y + 1] = iDepth; uQueue.AddNext(x, y + 1); }
            };

            //Buiiled
            //iMoveTreeFromX = iXstart;
            //iMoveTreeFromY = iYstart;

            //MessageBox.Show("PlayerMoveTree builded for ("+iXstart+","+iYstart+")");
            return FunctionResult.OK;
        }

        
        ///<summary>Wave algorithm of building player moving tree (x-start,y-start)</summary>
        private FunctionResult BuildPlayerMoveTree_XXYY(int iXstart, int iYstart, int iXe1, int iXe2, int iYe1, int iYe2)
        {
            short iDepth;
            int x, y;
            int iR = 0;
            //end on (iXe1,iYstart) (iXe2, iYe1) (iXe2, iYe2)
            //uTempLevel.BuildPlayerMoveTree_XXYY(x - 1, y, x + 1, x, y - 1, y + 1);//from left

            if (iMoveTreeFromX == iXstart && iMoveTreeFromY == iYstart)
                return FunctionResult.NothingToDo;

            InvalidatePlayerMoveTree();
            iMoveTree = new short[iXsize, iYsize];
            CoordQueue uQueue = new CoordQueue();

            //?? Is there any memset-like way to set all cells?
            //Array.Clear(iMoveTree,0,0);
            for (int i = 0; i < iXsize; i++)
                for (int j = 0; j < iYsize; j++)
                {
                    if ((bCells[i, j] & SokoCell.Obstacle) == 0)
                    {
                        iMoveTree[i, j] = MT_NOT_REACHED;
                    }
                    else
                    {
                        iMoveTree[i, j] = MT_BLOCKED;
                    }
                }
            if (iMoveTree[iXstart, iYstart] == MT_BLOCKED)
                return FunctionResult.StartIsBlocked;//Start is blocked, so no tree

            iMoveTree[iXstart, iYstart] = 0;//Start pos
            uQueue.AddNext(iXstart, iYstart);
            x = 0;
            y = 0;
            while (uQueue.Get(ref x, ref y))
            {
                if (x == iXe2)
                {
                    if (y == iYe1)
                    {
                        iR += 1;
                    }
                    else if (y == iYe2)
                    {
                        iR += 2;
                    }
                    if (iR == 7) return FunctionResult.PathFound;
                }
                else if (x == iXe1)
                {
                    if (y == iYstart)
                    {
                        iR += 4;
                    }
                    if (iR == 7) return FunctionResult.PathFound;
                }
                iDepth = iMoveTree[x, y]; iDepth++;
                if (iMoveTree[x - 1, y] == MT_NOT_REACHED) { iMoveTree[x - 1, y] = iDepth; uQueue.AddNext(x - 1, y); }
                if (iMoveTree[x + 1, y] == MT_NOT_REACHED) { iMoveTree[x + 1, y] = iDepth; uQueue.AddNext(x + 1, y); }
                if (iMoveTree[x, y - 1] == MT_NOT_REACHED) { iMoveTree[x, y - 1] = iDepth; uQueue.AddNext(x, y - 1); }
                if (iMoveTree[x, y + 1] == MT_NOT_REACHED) { iMoveTree[x, y + 1] = iDepth; uQueue.AddNext(x, y + 1); }
            };

            //Buiiled
            //iMoveTreeFromX = iXstart;
            //iMoveTreeFromY = iYstart;

            //MessageBox.Show("PlayerMoveTree builded for ("+iXstart+","+iYstart+")");
            return FunctionResult.OK;
        }
         */


        ///<summary>Add to redo stack route of player moving to specified cell (x, y of destination)</summary>
        public MoveResult TravelTo(int iXdest, int iYdest)
        {
            iReversLen = 0;
            MoveResult uRv = PlayerTravelRoute(iXdest, iYdest, this, false);
            if (uRv != MoveResult.WayBlocked)
            { //If route is builded (i.e. path exist)
                FlushAllRedo();//Remove all other redo
                AddRevertedRoute();//Add route to redo stack
                MarkerCurrentMove();//Put marker of group-action
            }
            return uRv;
        }

        ///<summary>Add to redo stack route of box pushing to specified cell (x, y of destination)</summary>
        public MoveResult TravelBoxTo(int iXdest, int iYdest)
        {
            iReversLen = 0;
            MoveResult uRv = BoxTravelRoute(iXdest, iYdest, this);
            if (uRv != MoveResult.WayBlocked)
            {//If route is builded (i.e. box can be pushed there)
                FlushAllRedo();//Remove all other redo
                AddRevertedRoute();//Add route to redo stack
                MarkerCurrentMove();//Put marker of group-action
            }
            return uRv;
        }

        ///<summary>Build reverted route of player moving to specific cell (x,y of destination, route to add, build short tree or full)</summary>
        private MoveResult PlayerTravelRoute(int iXdest, int iYdest, SokobanGame uRoute, bool bUseShortTree)
        {
            if (iPlayerX == iXdest && iPlayerY == iYdest)
                return MoveResult.Moved;//Player already there - nothing to do

            if (bUseShortTree)
            {//Using short tree - only to reach destination
                BuildPlayerMoveTree_UpTo(iPlayerX, iPlayerY, iXdest, iYdest);
            }
            else
            {//Using full tree - for all level
                BuildPlayerMoveTree(iPlayerX, iPlayerY);
            }
            int iMoveLen = iMoveTree[iXdest, iYdest];

            if (iMoveLen < 0)
                return MoveResult.WayBlocked;//Negative values - destination unreachable

            //Building reverted route from destination to start
            int x = iXdest; int y = iYdest;
            int iValue = iMoveLen;
            while (iValue > 0)
            {//Find nearest cell thet have value less then iValue and move player there
                if (iMoveTree[x - 1, y] >= 0 && iMoveTree[x - 1, y] < iValue) { iValue = iMoveTree[x - 1, y]; uRoute.AddNextRevertedMove(SokoMove.Right); x--; continue; }
                if (iMoveTree[x + 1, y] >= 0 && iMoveTree[x + 1, y] < iValue) { iValue = iMoveTree[x + 1, y]; uRoute.AddNextRevertedMove(SokoMove.Left); x++; continue; }
                if (iMoveTree[x, y - 1] >= 0 && iMoveTree[x, y - 1] < iValue) { iValue = iMoveTree[x, y - 1]; uRoute.AddNextRevertedMove(SokoMove.Down); y--; continue; }
                if (iMoveTree[x, y + 1] >= 0 && iMoveTree[x, y + 1] < iValue) { iValue = iMoveTree[x, y + 1]; uRoute.AddNextRevertedMove(SokoMove.Up); y++; continue; }
                return MoveResult.WayBlocked;//Failed to travel (something very wrong)
            }
            return MoveResult.Moved;
        }

        ///<summary>Build reverted route of box pushing to specific cell (x,y of destination, route to add)</summary>
        private MoveResult BoxTravelRoute(int iXdest, int iYdest, SokobanGame uRoute)
        {
            //Box move tree must be build before calling

            int z;
            int iDir = -1, iDirPre;
            MoveResult uRv;

            //Matrix of paths
            short[,] iMatrix = new short[4, 4];
            iMatrix[PLAYER_TO_DOWN, PLAYER_TO_DOWN] = 0;
            iMatrix[PLAYER_TO_DOWN, PLAYER_TO_LEFT] = BMTF_DOWN_TO_LEFT;
            iMatrix[PLAYER_TO_DOWN, PLAYER_TO_RIGHT] = BMTF_DOWN_TO_RIGHT;
            iMatrix[PLAYER_TO_DOWN, PLAYER_TO_UP] = BMTF_UP_TO_DOWN;
            iMatrix[PLAYER_TO_LEFT, PLAYER_TO_DOWN] = BMTF_DOWN_TO_LEFT;
            iMatrix[PLAYER_TO_LEFT, PLAYER_TO_LEFT] = 0;
            iMatrix[PLAYER_TO_LEFT, PLAYER_TO_RIGHT] = BMTF_LEFT_TO_RIGHT;
            iMatrix[PLAYER_TO_LEFT, PLAYER_TO_UP] = BMTF_UP_TO_LEFT;
            iMatrix[PLAYER_TO_RIGHT, PLAYER_TO_DOWN] = BMTF_DOWN_TO_RIGHT;
            iMatrix[PLAYER_TO_RIGHT, PLAYER_TO_LEFT] = BMTF_LEFT_TO_RIGHT;
            iMatrix[PLAYER_TO_RIGHT, PLAYER_TO_RIGHT] = 0;
            iMatrix[PLAYER_TO_RIGHT, PLAYER_TO_UP] = BMTF_UP_TO_RIGHT;
            iMatrix[PLAYER_TO_UP, PLAYER_TO_DOWN] = BMTF_UP_TO_DOWN;
            iMatrix[PLAYER_TO_UP, PLAYER_TO_LEFT] = BMTF_UP_TO_LEFT;
            iMatrix[PLAYER_TO_UP, PLAYER_TO_RIGHT] = BMTF_UP_TO_RIGHT;
            iMatrix[PLAYER_TO_UP, PLAYER_TO_UP] = 0;

            int x = iXdest;
            int y = iYdest;
            if (!IsCellAchivedByBoxMoveTree(x, y))
                return MoveResult.WayBlocked; //Destination is unreachable for current box moving tree

            //Find cell around destination with minimum value - there player will stop after pushing box
            int iValue = short.MaxValue;
            for (z = 0; z < 4; z++)
                if (iBoxMoveTree[x, y, z] < iValue && iBoxMoveTree[x, y, z] >= 0)
                {
                    iDir = z;
                    iValue = iBoxMoveTree[x, y, z];
                }

            if (iValue == 0 || iDir == -1)
            {
                return MoveResult.WayBlocked; //If no cell found - destination is unreachable
            }

            //int iMoveLen = iValue;
            SokobanGame uTempLevel = new SokobanGame(this);//Create temprorary level for pathfinding

            //Remove from temp-level: player and box-beeing-moved
            uTempLevel.bCells[iPlayerX, iPlayerY] ^= SokoCell.Player;
            uTempLevel.bCells[iBoxMoveTreeFromX, iBoxMoveTreeFromY] ^= SokoCell.Box;

            //Build reverted route
        lLoop1:

            //Check pushing box
            switch (iDir)
            {
                case PLAYER_TO_DOWN:
                    y++;//Player to down, so he push box to up to reach this state
                    if (iBoxMoveTree[x, y, iDir] > iValue || iBoxMoveTree[x, y, iDir] < 0)
                    {//Player is on cell with incorrect value, something wrong or route is completed
                        y--;//Return player
                        goto lEnd;//Exit building route
                    }
                    uRoute.AddNextRevertedMove(SokoMove.Up | SokoMove.Push);//Add push-to-up to reverted route
                    break;
                case PLAYER_TO_UP://By analogue...
                    y--;
                    if (iBoxMoveTree[x, y, iDir] > iValue || iBoxMoveTree[x, y, iDir] < 0)
                    {
                        y++;
                        goto lEnd;
                    }
                    uRoute.AddNextRevertedMove(SokoMove.Down | SokoMove.Push);
                    break;
                case PLAYER_TO_LEFT:
                    x--;
                    if (iBoxMoveTree[x, y, iDir] > iValue || iBoxMoveTree[x, y, iDir] < 0)
                    {
                        x++;
                        goto lEnd;
                    }
                    uRoute.AddNextRevertedMove(SokoMove.Right | SokoMove.Push);
                    break;
                case PLAYER_TO_RIGHT:
                    x++;
                    if (iBoxMoveTree[x, y, iDir] > iValue || iBoxMoveTree[x, y, iDir] < 0)
                    {
                        x--;
                        goto lEnd;
                    }
                    uRoute.AddNextRevertedMove(SokoMove.Left | SokoMove.Push);
                    break;
                default://Unknown direction - something wrong
                    goto lEnd;
            }

            if (x == iBoxMoveTreeFromX && y == iBoxMoveTreeFromY)
            {//Box is in place - root of box move tree
                if (bBoxMoveTreeRootDirections[iDir])
                {   //This location of player is reachable from start
                    goto lEnd;
                }
            }
            
            iValue = iBoxMoveTree[x, y, iDir];//Update value to current box-move-tree cell (added 20.10.2008 to fix bug with additional moves after reaching box)
            iDirPre = iDir;//Store direction of pushing to compare later
            
            //Find cell around box with lesser value on box-move-tree than current - if found, player should move there (around box or by complex path) to proceed (reverted) pushing
            for (z = 0; z < 4; z++)//Loop on directions
                if (iBoxMoveTree[x, y, z] < iValue && iBoxMoveTree[x, y, z] >= 0)//Cell with better value (than current)
                    //if (z == iDirPre || (iMatrix[z, iDirPre] & iBoxMoveTree[x, y, BMT_FLAGS]) != 0)//This cell can be reached from current by moving around box (or this is current cell)
                    if ((iMatrix[z, iDirPre] & iBoxMoveTree[x, y, BMT_FLAGS]) != 0)//This cell can be reached from current by moving around box (current cell may not pass previous "if")
                    {
                        iDir = z;//Store new diection
                        iValue = iBoxMoveTree[x, y, z];//Store new value
                    }

            if (iDir != iDirPre)
            {//New direction is differ from previous - should build moving route around box (otherwise - go to pushing)

                //Set box to temp. level to use pathfinding around this box
                uTempLevel.bCells[x, y] ^= SokoCell.Box;
                uRv = MoveResult.WayBlocked;

                uTempLevel.InvalidatePlayerMoveTree();//Flush player move tree

                //Set player location for start of path finding - new cell near box
                uTempLevel.iPlayerX = x; uTempLevel.iPlayerY = y;
                switch (iDir)
                {
                    case PLAYER_TO_DOWN: uTempLevel.iPlayerY++; break;
                    case PLAYER_TO_UP: uTempLevel.iPlayerY--; break;
                    case PLAYER_TO_LEFT: uTempLevel.iPlayerX--; break;
                    case PLAYER_TO_RIGHT: uTempLevel.iPlayerX++; break;
                }

                //Find path from new cell to previous
                switch (iDirPre)
                {
                    case PLAYER_TO_DOWN: uRv = uTempLevel.PlayerTravelRoute(x, y + 1, uRoute, true); break;
                    case PLAYER_TO_UP: uRv = uTempLevel.PlayerTravelRoute(x, y - 1, uRoute, true); break;
                    case PLAYER_TO_LEFT: uRv = uTempLevel.PlayerTravelRoute(x - 1, y, uRoute, true); break;
                    case PLAYER_TO_RIGHT: uRv = uTempLevel.PlayerTravelRoute(x + 1, y, uRoute, true); break;
                }

                //If no path - something wrong
                if (uRv == MoveResult.WayBlocked)
                    return uRv;

                //Remove box
                uTempLevel.bCells[x, y] ^= SokoCell.Box;
            }

            goto lLoop1;//Go to next push

        lEnd:
            if (x != iBoxMoveTreeFromX || y != iBoxMoveTreeFromY)
                return MoveResult.WayBlocked; //if box not reach root of tree - path not builded

            //Need to find path for player to box

            //Set box to temp. level to use pathfinding around this box
            uTempLevel.bCells[x, y] ^= SokoCell.Box;
            uRv = MoveResult.WayBlocked;

            uTempLevel.InvalidatePlayerMoveTree();//Flush player move tree
            uTempLevel.iPlayerX = iPlayerX; uTempLevel.iPlayerY = iPlayerY;//Set player start location - get it from actual level
            switch (iDir)//Find path
            {
                case PLAYER_TO_DOWN: uRv = uTempLevel.PlayerTravelRoute(x, y + 1, uRoute, true); break;
                case PLAYER_TO_UP: uRv = uTempLevel.PlayerTravelRoute(x, y - 1, uRoute, true); break;
                case PLAYER_TO_LEFT: uRv = uTempLevel.PlayerTravelRoute(x - 1, y, uRoute, true); break;
                case PLAYER_TO_RIGHT: uRv = uTempLevel.PlayerTravelRoute(x + 1, y, uRoute, true); break;
            }

            if (uRv == MoveResult.WayBlocked)
                return uRv;//No path - something wring

            //Reverted route builded
            return MoveResult.MovedAndPushBox;
        }

        /*
        ///<summary>Check, that cell is reachable by player (x-pos,y-pos)</summary>
        public MoveResult CheckPathTo(int iXdest, int iYdest)
        {
            BuildPlayerMoveTree(iPlayerX, iPlayerY);
            if (iMoveTree[iXdest, iYdest] >= 0)
                return MoveResult.Moved;
            else
                return MoveResult.WayBlocked;
        }*/

        ///<summary>Teleport player to new location (x,y of destination)</summary>
        private void SetPlayerPos(int iNewX, int iNewY)
        {
            bCells[iPlayerX, iPlayerY] ^= SokoCell.Player;//Remove player from current position
            bCells[iNewX, iNewY] ^= SokoCell.Player;//Put player to new position
            iPlayerX = iNewX;//Update position
            iPlayerY = iNewY;
        }

        ///<summary>Check, completed level or not (force to complete recheck all level)</summary>
        public bool IsLevelCompleted(bool bForceRecount)
        {
            if (iNumFreeBoxes <= 0) return true;
            if (iNumFreeTargets <= 0) return true;
            return false;
        }

        ///<summary>Get information about undo/redo stack</summary>
        public string GetUndoInfo()
        {
            return iPosition.ToString() + "/" + iMovesNum.ToString();
        }

        ///<summary>Redo 1 move</summary>
        public MoveResult Redo()
        {
            if (iPosition < iMovesNum)//Only if some redo action exist
            {
                MoveResult bRes = MovePlayer(ref bMoves[iPosition]);//Try to move player
                if (bRes != MoveResult.WayBlocked)
                {//If move is successfull
                    uLastMove = bMoves[iPosition];//Store last move (required for redrawing)
                    AddStats(bMoves[iPosition], 1);//Update position statistics
                    iPosition++;
                }
                return bRes;//Return result of move
            }
            return MoveResult.WayBlocked;//No redo action is possible
        }

        
        ///<summary>Move player, called by main form</summary>
        public MoveResult NewMove(SokoMove uNewMove)
        {
            MoveResult bRes = MovePlayer(ref uNewMove);//Try to move player
            if (bRes != MoveResult.WayBlocked)
            {
                uLastMove = uNewMove;//Store last move (required for redrawing)
                bMoves[iPosition] = uNewMove;//Store move into undo stack
                //IncStats(uNewMove);
                AddStats(uNewMove, 1);//Update position statistics
                FlushAllRedo();
                iPosition++; iMovesNum++;
                //iMovesNum = iPosition;//This will flush all redo if it was exist
                if (iMovesNum >= iMovesAlloc)//Enlarge undo/redo stack, if necessary
                    EnlargeStack();
            }
            return bRes;
        }

        ///<summary>Perform moving of player(updatable move - to get direction; to put pushing )</summary>
        private MoveResult MovePlayer(ref SokoMove uMove)
        {
            int iDX = 0, iDY = 0;
            MoveResult uRV;// = MoveResult.WayBlocked;

            switch (uMove & SokoMove.Direction)//Get direction vector from move
            {
                case SokoMove.Up: iDY = -1; break;
                case SokoMove.Down: iDY = 1; break;
                case SokoMove.Left: iDX = -1; break;
                case SokoMove.Right: iDX = 1; break;
            }
            int iX = iPlayerX + iDX;
            int iY = iPlayerY + iDY;
            int iX2 = iPlayerX + iDX * 2;
            int iY2 = iPlayerY + iDY * 2;

            if ((GetCell(iX2, iY2) & SokoCell.Background)!=0) return MoveResult.WayBlocked;//Box can not be pushed to background
            if ((GetCell(iX, iY) & SokoCell.Background) != 0) return MoveResult.WayBlocked;//Player can not move to background

            if ((bCells[iX, iY] & SokoCell.Box) != 0)
            {//New location contains box - it should be pushed

                if ((bCells[iX2, iY2] & SokoCell.MaskObstacle) == 0)
                {//No obstacle - box can be pushed
                    uMove |= SokoMove.Push;//Add push flag to move

                    if ((bCells[iX, iY] & SokoCell.Target) != 0)
                    {//Box moved from target
                        iNumFreeBoxes++;//Increment counter of free boxes and free targets
                        iNumFreeTargets++;
                    }
                    if ((bCells[iX2, iY2] & SokoCell.Target) != 0)
                    {//Box moved to target
                        uRV = MoveResult.MovedAndPushBoxToTarget;//Notify about this, level can be completed after this move
                        iNumFreeBoxes--;//Decrement counter of free boxes and free targets
                        iNumFreeTargets--;
                    }
                    else
                    {
                        uRV = MoveResult.MovedAndPushBox;//Usual push, not to target
                    }
                    if ((bCells[iX, iY] & SokoCell.CellDeadlock) != 0)
                    {//Box moved from cell-deadlock (possible only if cell-deadlock detection will fail)
                        iNumRemainExceedBoxes++;//Increment counter of exceeding boxes
                    }
                    if ((bCells[iX2, iY2] & SokoCell.CellDeadlock) != 0)
                    {//Box moved to cell-deadlock
                        iNumRemainExceedBoxes--;//Decrement counter of exceeding boxes
                    }

                    bCells[iX, iY] ^= SokoCell.Box;//Remove box from new player location
                    bCells[iX2, iY2] ^= SokoCell.Box;//Put box to new location

                    InvalidatePlayerMoveTree();//Flush player move tree (due to change of box configuration)
                    InvalidateBoxMoveTree();//Flush box move tree
                }
                else
                {//Obstacle - box can not be pushed
                    uRV = MoveResult.WayBlocked;
                    //goto lWayBlocked;
                }
            }
            else if ((bCells[iX, iY] & SokoCell.MaskObstacle) == 0)
            {//No box and no other obstacles - just move
                uRV = MoveResult.Moved;
            }
            else
            {//Otherwise - can not move
                uRV = MoveResult.WayBlocked;
            }

            if (uRV != MoveResult.WayBlocked)
            {
                SetPlayerPos(iX, iY);//Move player to new location
                uPlayerDir = uMove;//Update player direction to direction of this move
            }
            return uRV;

        }

        ///<summary>Undo last move</summary>
        public MoveResult Undo()
        {
            if (iPosition > 0)
            {
                int iDX = 0, iDY = 0;
                MoveResult uRV;// = MoveResult.WayBlocked;

                //Check, that player there he should be    (is it really required?)
                if ((GetCell(iPlayerX, iPlayerY) & SokoCell.Player) == 0) return MoveResult.WayBlocked; //No player

                //Calc coordinates of shift
                switch (bMoves[iPosition - 1] & SokoMove.Direction)
                {
                    case SokoMove.Up: iDY = -1; break;
                    case SokoMove.Down: iDY = 1; break;
                    case SokoMove.Left: iDX = -1; break;
                    case SokoMove.Right: iDX = 1; break;
                }
                int iX = iPlayerX - iDX;//Where to move after undo
                int iY = iPlayerY - iDY;
                int iX2 = iPlayerX + iDX;//Where could be box, that was pushed in move, that now undo-ing
                int iY2 = iPlayerY + iDY;
                
                if ((GetCell(iX, iY) & SokoCell.MaskObstacle) != 0) return MoveResult.WayBlocked; //Unable to undo there

                uRV = MoveResult.Moved;

                if ((bMoves[iPosition - 1] & SokoMove.Push) != 0)
                {   //Move was push, so we need to pull box to prev location
                    if ((GetCell(iX2, iY2) & SokoCell.Box) == 0) return MoveResult.WayBlocked; //No box there it should be
                    uRV = MoveResult.MovedAndPushBox;

                    //Move box
                    bCells[iX2, iY2] ^= SokoCell.Box;
                    bCells[iPlayerX, iPlayerY] ^= SokoCell.Box;

                    //Update counters stats
                    if ((bCells[iX2, iY2] & SokoCell.Target) != 0)
                    {//Box moved from target
                        iNumFreeBoxes++;//Increment counter of free boxes and free targets
                        iNumFreeTargets++;
                    }
                    if ((bCells[iPlayerX, iPlayerY] & SokoCell.Target) != 0)
                    {//Box moved to target
                        iNumFreeBoxes--;//Decrement counter of free boxes and free targets
                        iNumFreeTargets--;
                    }
                    if ((bCells[iX2, iY2] & SokoCell.CellDeadlock) != 0)
                    {//Box unmoved from cell-deadlock 
                        iNumRemainExceedBoxes++;//Increment counter of exceeding boxes
                    }
                    if ((bCells[iPlayerX, iPlayerY] & SokoCell.CellDeadlock) != 0)
                    {//Box unmoved to deadlock (possible only if cell-deadlock detection will fail)
                        iNumRemainExceedBoxes--;//Decrement counter of exceeding boxes
                    }
                    
                    InvalidatePlayerMoveTree();//Flush player move tree (due to change of box configuration)
                    InvalidateBoxMoveTree();//Flush box move tree
                }
                SetPlayerPos(iX, iY);//Move player
                uPlayerDir = bMoves[iPosition - 1];//Update player direction to direction of previous move

                iPosition--;//Step one move down into undo stack

                AddStats(bMoves[iPosition], -1);//Update position statistics
                uLastMove = bMoves[iPosition];//Store move as last move (required for redrawing)

                return uRV;
            }
            return MoveResult.WayBlocked;//We at stack bottom, nothing to undo
        }

        ///<summary>Check for group-action marker at current move</summary>
        public bool NotMarker()
        {
            if (iPosition < iMovesNum)
            {
                if ((bMoves[iPosition] & SokoMove.Marker)==0) return true;//No marker, return true
            }
            return false;
        }


        ///<summary>Erase all moves after current position (to top of stack), required for new move after undo</summary>
        private void FlushAllRedo()
        {
            iMovesNum = iPosition;
            if (iFirstDeadlockMove > iPosition) //Undo earlier than first game-deadlock move, so now no game-deadlock
                iFirstDeadlockMove = -1;
        }

        ///<summary>Add new move to the end of reverted route</summary>
        private void AddNextRevertedMove(SokoMove uNewMove)
        {
            uReversRoute[iReversLen] = uNewMove;//Put move into route
            iReversLen++;
            if (iReversLen >= iReversAlloc)
                EnlargeReversStack();//Realloc route, if needed
        }

        ///<summary>Set group-action marker to current move</summary>
        public void MarkerCurrentMove()
        {
            if (iMovesNum > iPosition)
            {
                bMoves[iPosition] |= SokoMove.Marker;
            }
        }

        ///<summary>Add reverted route to the top of undo stack</summary>
        private void AddRevertedRoute()
        {
            int iNewMovesNum = iMovesNum + iReversLen;//Update stack len
            while (iNewMovesNum >= iMovesAlloc)
                EnlargeStack();//Realloc undo stack, if needed

            iReversLen--;//Decrement reverted route len by 1 (otherwise "-1" will be required inside loop)
            
            for (int i = 0; i <= iReversLen; i++, iMovesNum++)//Add action one-by-one in reverse order from reverted route to undo stack
                bMoves[iMovesNum] = uReversRoute[iReversLen - i];
        }

        ///<summary>Update position statistics (move, multiplier)</summary>
        private void AddStats(SokoMove uMove, int iAdd)
        {
            uStats.iMoves += iAdd;//Moves updated always
            if ((uMove & SokoMove.Push) != 0)
                uStats.iPushes += iAdd;//Pushed updated on pushes
        }

        ///<summary>Recalculate position statistics, some statistics do not calculated automatically</summary>
        public void RecalcStats()
        {
            int i;
            int dx=0, dy=0;//Shift vector for one move
            
            uStats.InitZero();//Reset all stats (to count from beginning)
            int x = 0;
            int y = 0;
            int iLastBoxX = int.MaxValue;
            int iLastBoxY = int.MaxValue;

            for (i = 0; i < iPosition; i++) //Iterate thru all moves up to current...
            {
                //Calculate shift vector
                switch (bMoves[i] & SokoMove.Direction)
                {
                    case SokoMove.Left: dx = -1; dy = 0;break;
                    case SokoMove.Right: dx = 1; dy = 0; break;
                    case SokoMove.Up: dx = 0; dy = -1; break;
                    case SokoMove.Down: dx = 0; dy = 1; break;
                }
                uStats.iMoves++;//Number of moves - updated on each move
                
                x += dx; y += dy;//Do shift player to vector

                if ((bMoves[i] & SokoMove.Push) != 0) //If move is push
                {
                    if (x != iLastBoxX || y != iLastBoxY) //Now we pushing not the same box, as in previously
                    {
                        uStats.iBoxChanges++;//Number of box changes - updated on pushing not the same box, as in previously
                    }
                    iLastBoxX = x + dx;//Remember last-pushed-box location for further comparison
                    iLastBoxY = y + dy;


                    uStats.iPushes++;//Number of pushes - updated on each push
                    if (i > 0) //Not the first move
                    {
                        if ((bMoves[i - 1] & SokoMove.Push) == 0) //Previous move was not push
                        {
                            uStats.iPushSessions ++;//Number of pushing sessions - updated on push after non-push
                            uStats.iLinearPushes ++;//Number of linear pushes - updated once during pushing box in one direction (this is first push, so update)
                        }
                        else if ((bMoves[i] & SokoMove.Direction) != (bMoves[i - 1] & SokoMove.Direction)) //Previous move was also push, but in different direction
                            uStats.iLinearPushes++;//Number of linear pushes - updated on first push box in one direction
                    }
                    else
                    {   //First move
                        uStats.iPushSessions ++;//Number of pushing sessions - updated on push after non-push (very first move and it is push, so updated)
                        uStats.iLinearPushes++;//Number of linear pushes - updated on first push box in one direction (very first move and it is push, so updated)
                    }
                }
                if (i > 0) //Not the first move
                {
                    if ((bMoves[i] & SokoMove.Direction) != (bMoves[i - 1] & SokoMove.Direction)) //Previous move was in different direction
                        uStats.iLinearMoves++;//Number of linear moves - updated on first move in one direction
                }
                else //First move
                    uStats.iLinearMoves++;//Number of linear moves - updated on first move in one direction (very first move, so updated)
            }
        }

        ///<summary>Set markers of group-actions to moves, there player change box - for loaded positions and solutions</summary>
        public void SetMarkersOnBoxChanges()
        {
            int i;
            int dx = 0, dy = 0;//Shift vector for one move
            int x = 0;
            int y = 0;
            int iLastBoxX = int.MaxValue;
            int iLastBoxY = int.MaxValue;
            int iLastBoxT = 0;

            for (i = 0; i < iPosition; i++) //Iterate thru all moves up to current...
            {
                //Calculate shift vector
                switch (bMoves[i] & SokoMove.Direction)
                {
                    case SokoMove.Left: dx = -1; dy = 0; break;
                    case SokoMove.Right: dx = 1; dy = 0; break;
                    case SokoMove.Up: dx = 0; dy = -1; break;
                    case SokoMove.Down: dx = 0; dy = 1; break;
                }
                x += dx; y += dy;//Do shift player to vector

                if ((bMoves[i] & SokoMove.Push) != 0)//If move is push
                {
                    if (x != iLastBoxX || y != iLastBoxY) //Now we pushing not the same box, as in previously
                    {
                        bMoves[iLastBoxT] |= SokoMove.Marker; //Put group-action to move, in which we last pushed previous box

                    }
                    iLastBoxX = x + dx;//Remember last-pushed-box location for further comparison
                    iLastBoxY = y + dy;
                    iLastBoxT = i+1;//Rememver move, then we last pushing box
                }
            }
            bMoves[iLastBoxT] |= SokoMove.Marker; //Put group-action to move, in which we last pushed last box
        }

        /*
        ///<summary>Save current position into file (filename, levelset name, level number, player name)
        ///!Recalc of stats should be done before calling</summary>
        public FunctionResult SavePosition(string sFileName, string sLevelSet, int iLevelNum, string sPlayerName)
        {
            if (iPosition == 0) return FunctionResult.NothingToDo;//Position is empty, nothing to save

            System.IO.StreamWriter hWrite;

            try //Protection from file operations errors
            {
                hWrite = new System.IO.StreamWriter(sFileName, false);//Open file with overwrite
                int i;
                char[] cLurd = new char[iPosition];//Array of characters - one for each move
                IniHold.IniFile uIni = new IniHold.IniFile();//IniHold object - to store position as ini-file
                uIni.SetWriter(hWrite);//Transmit file-writer into inihold

                for (i = 0; i < iPosition; i++) //Iterate thru all moves up to current...
                {
                    //Convert moves to chars
                    switch (bMoves[i] & SokoMove.DirectionPush)  //Get only direction and flag of push
                    {
                        case SokoMove.Left: cLurd[i] = 'l'; break;
                        case SokoMove.Right: cLurd[i] = 'r'; break;
                        case SokoMove.Up: cLurd[i] = 'u'; break;
                        case SokoMove.Down: cLurd[i] = 'd'; break;
                        case SokoMove.PushLeft: cLurd[i] = 'L'; break;
                        case SokoMove.PushRight: cLurd[i] = 'R'; break;
                        case SokoMove.PushUp: cLurd[i] = 'U'; break;
                        case SokoMove.PushDown: cLurd[i] = 'D'; break;
                    }
                }
                iLevelNum++;//Levels numbered from 1 in files

                //Save level info, player info, statistics of positions
                uIni.SaveItem("Title", uStats.sName);
                uIni.SaveItem("LevelSet", sLevelSet);
                uIni.SaveItem("Level", iLevelNum.ToString());
                uIni.SaveItem("Player", sPlayerName);
                uIni.SaveItem("Moves", uStats.iMoves.ToString());
                uIni.SaveItem("Pushes", uStats.iPushes.ToString());
                uIni.SaveItem("LinearMoves", uStats.iLinearMoves.ToString());
                uIni.SaveItem("LinearPushes", uStats.iLinearPushes.ToString());
                uIni.SaveItem("PushSessions", uStats.iPushSessions.ToString());
                uIni.SaveItem("BoxChanges", uStats.iBoxChanges.ToString());

                //Save sequence of moves
                uIni.SaveItem("Position", new String(cLurd));

                hWrite.Close();//Close file
                return FunctionResult.OK;
            }
            catch
            {
                return FunctionResult.FailedToOpenFile;//On error - return indication
            }
        }
         */

        ///<summary>Get current position in LuRd format</summary>
        public string GetPositionLuRd()
        {
            int i;
            if (iPosition <= 0)
                return "";
            char[] cLurd = new char[iPosition];//Array of characters - one for each move
            for (i = 0; i < iPosition; i++) //Iterate thru all moves up to current...
            {
                //Convert moves to chars
                switch (bMoves[i] & SokoMove.DirectionPush)  //Get only direction and flag of push
                {
                    case SokoMove.Left: cLurd[i] = 'l'; break;
                    case SokoMove.Right: cLurd[i] = 'r'; break;
                    case SokoMove.Up: cLurd[i] = 'u'; break;
                    case SokoMove.Down: cLurd[i] = 'd'; break;
                    case SokoMove.PushLeft: cLurd[i] = 'L'; break;
                    case SokoMove.PushRight: cLurd[i] = 'R'; break;
                    case SokoMove.PushUp: cLurd[i] = 'U'; break;
                    case SokoMove.PushDown: cLurd[i] = 'D'; break;
                }
            }
            return new String(cLurd);
        }

        ///<summary>Insert position (string with LuRd coded moves)</summary>
        public FunctionResult PastePositionLuRd(string sLuRd)
        {
            int i;
            char[] cLurd = sLuRd.ToCharArray();

            FlushAllRedo();//Flush redo stack

            for (i = 0; i < cLurd.Length; i++)//Iterate thru all moves
            {
                //Decode characters into moves
                switch (cLurd[i])
                {
                    case 'l': bMoves[iMovesNum] = SokoMove.Left; break;
                    case 'r': bMoves[iMovesNum] = SokoMove.Right; break;
                    case 'u': bMoves[iMovesNum] = SokoMove.Up; break;
                    case 'd': bMoves[iMovesNum] = SokoMove.Down; break;
                    case 'L': bMoves[iMovesNum] = SokoMove.PushLeft; break;
                    case 'R': bMoves[iMovesNum] = SokoMove.PushRight; break;
                    case 'U': bMoves[iMovesNum] = SokoMove.PushUp; break;
                    case 'D': bMoves[iMovesNum] = SokoMove.PushDown; break;
                    default: continue;//Skip all other
                }

                iMovesNum++;
                if (iMovesNum >= iMovesAlloc)
                    EnlargeStack();//Realloc stack if needed
            }

            return FunctionResult.OK;
        }


        /*
        ///<summary>Load position from file (filename, levelset name, level number, force loading level/set)</summary>
        public FunctionResult LoadPosition(string sFileName, ref string sLevelSet, ref int iLevelNum, bool bForceLoadLevel)
        {
            IniHold.IniFile uIni = new IniHold.IniFile();//IniHold object - to treat position file as ini-file
            uIni.LoadIni(sFileName);//Load file as ini-file

            string sPos = uIni.GetItemValue("Position");//Get sequence of moves
            
            //If sequence is empty or not present - exit
            if (sPos.Length == 0)
                return FunctionResult.NothingToDo;

            string sQuestion;

            string sFileLevelSet = uIni.GetItemValue("LevelSet","<unknown>").ToLower();
            int iFileLevel = OQConvertTools.string2int(uIni.GetItemValue("Level", "0"));
            iLevelNum++;//Levels numbered from 1 in files

            if (bForceLoadLevel)
            {
            }
            else
            {
                if (sFileLevelSet != sLevelSet || iFileLevel != iLevelNum) //Position may be not for current level - warn user
                {
                    string sLevelNum;
                    if (iFileLevel == 0)
                        sLevelNum = "<unknown> level"; //Level number not specified in file
                    else
                        sLevelNum = "level " + iFileLevel.ToString();//Level number specified
                    if (sFileLevelSet != sLevelSet) //Levelset is different
                        sQuestion = "This position from " + sLevelNum + " of " + sFileLevelSet + " levelset\r\nAre you sure?";
                    else //Levelset is the same, but levelnumber is different
                        sQuestion = "This position from " + sLevelNum + "\r\nAre you sure?";
                    if (MessageBox.Show(sQuestion, "Loading position", MessageBoxButtons.OKCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button2) == DialogResult.Cancel) //Ask user
                        return FunctionResult.Canceled; //Exit, if user decide so
                }
            }
            
            sLevelSet = sFileLevelSet;
            iLevelNum = iFileLevel-1;

            int i;
            char[] cLurd = sPos.ToCharArray();

            iMovesNum = 0;//Flush undo stack
            for (i = 0; i < cLurd.Length; i++)//Iterate thru all loaded moves
            {
                //Decode characters into moves
                switch(cLurd[i])
                {
                    case 'l': bMoves[iMovesNum] = SokoMove.Left; break;
                    case 'r': bMoves[iMovesNum] = SokoMove.Right; break;
                    case 'u': bMoves[iMovesNum] = SokoMove.Up; break;
                    case 'd': bMoves[iMovesNum] = SokoMove.Down; break;
                    case 'L': bMoves[iMovesNum] = SokoMove.PushLeft; break;
                    case 'R': bMoves[iMovesNum] = SokoMove.PushRight; break;
                    case 'U': bMoves[iMovesNum] = SokoMove.PushUp; break;
                    case 'D': bMoves[iMovesNum] = SokoMove.PushDown; break;
                    default: continue;//Skip all other
                }

                iMovesNum++;
                if (iMovesNum >= iMovesAlloc)
                    EnlargeStack();//Realloc stack if needed
            }

            return FunctionResult.OK;

        }*/

        ///<summary>Copy deadlocks from other game - in case of background calc</summary>
        public void DownloadDeadlocks(SokobanGame uGameWithDeadlocks)
        {
            for (int i = 0; i < iXsize; i++)
                for (int j = 0; j < iYsize; j++)
                    if ((uGameWithDeadlocks.bCells[i, j] & SokoCell.CellDeadlock)!=0)
                    {
                        bCells[i, j] |= SokoCell.CellDeadlock;
                    }
        }


        ///<summary>Calculate cell-deadlocks for current level and mark all cell-deadlock.
        ///cell-Deadlock - is cell, from there you cannot push box to target</summary>
        public FunctionResult CalcDeadlocks()
        {
            /*
             * Function build simplified box move tree for only one box at level and player stand on current location
             * Then first wave is propagated thru this tree - from targets to other cells but backward by connections of box move tree
             * Second wave is propagated thru same tree - from boxes to other cells forward by connections of box move tree
             * All branches of tree, reached by both waves - create not cell-deadlock in corresponding cell (because you can push some box by connections to some target)
             * All cells, not reached by any of waves - is cell-deadlocks
             * 
             * (estimated as better to previos version - CalcDeadlocks_old()
             */

            int x, y, z;
            short iLeftToDown, iLeftToRight, iLeftToUp, iRightToDown, iRightToUp, iUpToDown;
            int iLeft, iRight, iUp, iDown;
            int iToCheck;
            //int iNeiCells;
            //SokoCell bMask;

            iBoxMoveTree = new short[iXsize, iYsize, 5];//Alloc box move tree array
            SokobanGame uTempLevel = new SokobanGame(this);//Temprorary level as copy of current
            CoordQueue uQueue = new CoordQueue();//Queue for verifying cells

            //Remove boxes and player from level
            for (y = 0; y < iYsize; y++)
                for (x = 0; x < iXsize; x++)
                    uTempLevel.bCells[x, y] &= SokoCell.FilterForCalcCellDeadlocks;


            //Stage 1. Mark forbidden branches of tree

            for (y = 0; y < iYsize; y++)
                for (x = 0; x < iXsize; x++)
                {
                    iBoxMoveTree[x, y, BMT_FLAGS] = 0;

                    if ((uTempLevel.bCells[x, y] & SokoCell.MaskObstacle) != 0 || x == 0 || y == 0 || x == (iXsize - 1) || y == (iYsize - 1))
                    {//Obstacle on cell or border cell (topmost, bottommost, leftmost and rightmost lines of level MUST contain ONLY walls or backgrounds, so box move tree should not reach them)
                        for (z = 0; z < 4; z++)
                            iBoxMoveTree[x, y, z] = MT_BLOCKED;
                    }
                    else
                    {
                        iToCheck=0;//Call may not require deep checking
                        for (z = 0; z < 4; z++)
                            iBoxMoveTree[x, y, z] = MT_NOT_REACHED;//Not yet reached

                        uTempLevel.bCells[x, y] ^= SokoCell.Box;//Temprorary put box to this cell

                        uTempLevel.InvalidatePlayerMoveTree();//Flush player move tree to not stumble with it, if it was calculate for current cell but for other box configuration
                        uTempLevel.BuildPlayerMoveTree(iPlayerX, iPlayerY);//Build player move tree - to check reachability of box from all sides

                        if ((uTempLevel.bCells[x, y - 1] & SokoCell.MaskObstacle) != 0) //Obstacle from up
                            iBoxMoveTree[x, y, PLAYER_TO_UP] = MT_BLOCKED;//Player-to-up is blocked
                        else if (uTempLevel.iMoveTree[x,y-1]>=0)//No obstacle and move tree reach cell to up
                        { iBoxMoveTree[x, y, PLAYER_TO_UP] = 0; iToCheck = 1; }//Player-to-up is available and cell should be checked further

                        if ((uTempLevel.bCells[x - 1, y] & SokoCell.MaskObstacle) != 0) //By analogue...
                            iBoxMoveTree[x, y, PLAYER_TO_LEFT] = MT_BLOCKED;
                        else if (uTempLevel.iMoveTree[x-1, y] >= 0)
                        { iBoxMoveTree[x, y, PLAYER_TO_LEFT] = 0; iToCheck = 1; }

                        if ((uTempLevel.bCells[x, y + 1] & SokoCell.MaskObstacle) != 0) //By analogue...
                            iBoxMoveTree[x, y, PLAYER_TO_DOWN] = MT_BLOCKED;
                        else if (uTempLevel.iMoveTree[x, y+1] >= 0)
                        { iBoxMoveTree[x, y, PLAYER_TO_DOWN] = 0; iToCheck = 1; }

                        if ((uTempLevel.bCells[x + 1, y] & SokoCell.MaskObstacle) != 0) //By analogue...
                            iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = MT_BLOCKED;
                        else if (uTempLevel.iMoveTree[x+1, y] >= 0)
                        { iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = 0; iToCheck = 1; }

                        if (iToCheck>0) //Cell should be check deeply
                        {
                            uQueue.AddNext(x, y);//Add cell to queue
                            iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_TO_CHECK; //Mark cell as not cheked
                        }

                        /* //Old and unoptimized
                        uTempLevel.BuildPlayerMoveTree(x - 1, y);//build player move tree from left to box
                        iLeftToRight = uTempLevel.iMoveTree[x + 1, y];
                        iLeftToUp = uTempLevel.iMoveTree[x, y - 1];
                        iLeftToDown = uTempLevel.iMoveTree[x, y + 1];
                        uTempLevel.BuildPlayerMoveTree(x + 1, y);//from right
                        iRightToDown = uTempLevel.iMoveTree[x, y + 1];
                        iRightToUp = uTempLevel.iMoveTree[x, y - 1];
                        uTempLevel.BuildPlayerMoveTree(x, y + 1); //from down
                        //uTempLevel.BuildPlayerMoveTree_UpTo(x, y + 1,x,y-1); //from down
                        iUpToDown = uTempLevel.iMoveTree[x, y - 1];
                         /**/

                        //(more optimization?)

                        //Paths between all this 4 positions - initially they are not known
                        iLeftToRight = MT_NOT_REACHED;
                        iLeftToUp = MT_NOT_REACHED;
                        iLeftToDown = MT_NOT_REACHED;
                        iRightToDown = MT_NOT_REACHED;
                        iRightToUp = MT_NOT_REACHED;
                        iUpToDown = MT_NOT_REACHED;

                        /*  //Also old and unoptimized
                        // (sasq6, level50 - 5460 ms)
                        //0x01 - up, 0x02 - up-right, 0x04 - right, 0x08 - right-down, 0x10 - down, 0x20 - down-left, 0x40 - left, 0x80 - left-up
                        iNeiCells = 0;
                        if (uTempLevel.iMoveTree[x, y - 1] == MT_BLOCKED) iNeiCells |= 0x01;
                        if (uTempLevel.iMoveTree[x, y + 1] == MT_BLOCKED) iNeiCells |= 0x10;
                        if (uTempLevel.iMoveTree[x-1, y] == MT_BLOCKED) iNeiCells |= 0x40;
                        if (uTempLevel.iMoveTree[x+1, y] == MT_BLOCKED) iNeiCells |= 0x04;
                        if (uTempLevel.iMoveTree[x-1, y - 1] == MT_BLOCKED) iNeiCells |= 0x80;
                        if (uTempLevel.iMoveTree[x-1, y + 1] == MT_BLOCKED) iNeiCells |= 0x20;
                        if (uTempLevel.iMoveTree[x + 1, y-1] == MT_BLOCKED) iNeiCells |= 0x02;
                        if (uTempLevel.iMoveTree[x + 1, y+1] == MT_BLOCKED) iNeiCells |= 0x08;

                        if ((iNeiCells & 0x07) == 0 || (iNeiCells & 0xFD) == 0)
                            iRightToUp = 1;//iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_RIGHT;
                        if ((iNeiCells & 0xC1) == 0 || (iNeiCells & 0x7F) == 0)
                            iLeftToUp = 1;//iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_LEFT;
                        if ((iNeiCells & 0x1F) == 0 || (iNeiCells & 0xF1) == 0)
                            iUpToDown = 1;//iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_DOWN;
                        if ((iNeiCells & 0x70) == 0 || (iNeiCells & 0xDF) == 0)
                            iLeftToDown = 1;//iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_DOWN_TO_LEFT;
                        if ((iNeiCells & 0x1C) == 0 || (iNeiCells & 0xF7) == 0)
                            iRightToDown = 1;//iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_DOWN_TO_RIGHT;
                        if ((iNeiCells & 0x7C) == 0 || (iNeiCells & 0xC7) == 0)
                            iLeftToRight = 1;//iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_LEFT_TO_RIGHT;
                        /**/
                        
                        
                        //Last optimized:  (sasq6, level50 - 3280 ms)

                        //Get number of moves to reach each side of box
                        iUp = uTempLevel.iMoveTree[x, y - 1];
                        iLeft = uTempLevel.iMoveTree[x - 1, y];
                        iRight = uTempLevel.iMoveTree[x + 1, y];
                        iDown = uTempLevel.iMoveTree[x, y + 1];

                        uTempLevel.InvalidatePlayerMoveTree();//Flush player move tree to not stumble with it, if it was calculate for current cell but for other box configuration

                        //If some positions around box is blocked - block all corresponded paths
                        if (iLeft == MT_BLOCKED) { iLeftToDown = MT_BLOCKED; iLeftToRight = MT_BLOCKED; iLeftToUp = MT_BLOCKED; }
                        if (iRight == MT_BLOCKED) { iRightToDown = MT_BLOCKED; iLeftToRight = MT_BLOCKED; iRightToUp = MT_BLOCKED; }
                        if (iUp == MT_BLOCKED) { iUpToDown = MT_BLOCKED; iRightToUp = MT_BLOCKED; iLeftToUp = MT_BLOCKED; }
                        if (iDown == MT_BLOCKED) { iLeftToDown = MT_BLOCKED; iRightToDown = MT_BLOCKED; iUpToDown = MT_BLOCKED; }

                        if (iUp >= 0)
                        {
                            if (iLeft >= 0)
                                iLeftToUp = 1; //Left and Up is reachable from player start, so path between then exist
                            if (iRight >= 0)
                                iRightToUp = 1; //By analogue...
                            if (iDown >= 0)
                                iUpToDown = 1; //By analogue...
                        }
                        if (iLeft >= 0)
                        {
                            if (iRight >= 0)
                                iLeftToRight = 1; //By analogue...
                            if (iDown >= 0)
                                iLeftToDown = 1; //By analogue...
                        }
                        if (iRight >= 0)
                        {
                            if (iDown >= 0)
                                iRightToDown = 1; //By analogue...
                        }


                        //Check simple "turn over box in two steps"
                        if (iLeft != MT_BLOCKED)
                        {
                            if (iUp != MT_BLOCKED)
                                if ((uTempLevel.bCells[x - 1, y - 1] & SokoCell.MaskObstacle) == 0) iLeftToUp = 2; //Left, up and left-up cells are empty - so player can move from up to left in 2 steps
                            if (iDown != MT_BLOCKED)
                                if ((uTempLevel.bCells[x - 1, y + 1] & SokoCell.MaskObstacle) == 0) //Left, down and left-down cells are empty ...
                                {
                                    iLeftToDown = 2;
                                    if (iLeftToUp > 0)
                                        iUpToDown = 4;//Two turns - player can go around box in 4 steps
                                }
                        }
                        if (iRight != MT_BLOCKED)
                        {//By analogue...
                            if (iUp != MT_BLOCKED)
                                if ((uTempLevel.bCells[x + 1, y - 1] & SokoCell.MaskObstacle) == 0) iRightToUp = 2;
                            if (iDown != MT_BLOCKED)
                                if ((uTempLevel.bCells[x + 1, y + 1] & SokoCell.MaskObstacle) == 0)
                                {
                                    iRightToDown = 2;
                                    if (iRightToUp > 0)
                                        iUpToDown = 4;//Two turns - player can go around box in 4 steps
                                }
                        }

                        //Check left-to-right pass in two-turn-combo and three-turn-combo
                        if (iRightToUp > 0 && iLeftToUp > 0)
                        {//Can go right-to-up and from up-to-left -> can go right-to-left in 4 steps
                            iLeftToRight = 4;
                            if (iRightToDown > 0)
                                iLeftToDown = 6;//Also can go right-to-down -> can go left-to-down is 6 steps (if not yet)
                            else if (iLeftToDown > 0)
                                iRightToDown = 6;//Also can go left-to-down -> can go right-to-down in 6 steps (if not yet)
                        }
                        else if (iRightToDown == 2 && iLeftToDown == 2)
                        {//By analogue...
                            iLeftToRight = 4;
                            if (iRightToUp > 0)
                                iLeftToUp = 6;
                            else if (iLeftToUp > 0)
                                iRightToUp = 6;
                        }

                        if (iLeftToRight == MT_NOT_REACHED || iLeftToUp == MT_NOT_REACHED || iLeftToDown == MT_NOT_REACHED)
                        {//Some left-to is not yet calculated - can calculate them only by player move tree
                            uTempLevel.BuildPlayerMoveTree(x - 1, y);//Build player move tree from left to box
                            iLeftToRight = uTempLevel.iMoveTree[x + 1, y];//And get reachability from player move tree
                            iLeftToUp = uTempLevel.iMoveTree[x, y - 1];
                            iLeftToDown = uTempLevel.iMoveTree[x, y + 1];
                        }

                        if (iRightToDown == MT_NOT_REACHED || iRightToUp == MT_NOT_REACHED)
                        {//By analogue...
                            uTempLevel.BuildPlayerMoveTree(x + 1, y);//from right
                            iRightToDown = uTempLevel.iMoveTree[x, y + 1];
                            iRightToUp = uTempLevel.iMoveTree[x, y - 1];
                        }

                        if (iUpToDown == MT_NOT_REACHED)
                        {//By analogue... up-to-down is still unknown
                            uTempLevel.BuildPlayerMoveTree_UpTo(x, y + 1, x, y - 1);//from up only to down
                            iUpToDown = uTempLevel.iMoveTree[x, y - 1];
                        }

                        //Set flags of possiblity to walk by paths around box
                        if (iLeftToDown > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_DOWN_TO_LEFT;
                        if (iLeftToRight > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_LEFT_TO_RIGHT;
                        if (iLeftToUp > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_LEFT;
                        if (iRightToDown > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_DOWN_TO_RIGHT;
                        if (iRightToUp > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_RIGHT;
                        if (iUpToDown > 0) iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_UP_TO_DOWN;

                        //Remove box
                        uTempLevel.bCells[x, y] ^= SokoCell.Box;
                    }
                }

            //Stage 2. Build box move tree

            //Propagating tree to all cells - by checking cells, received from queue and adding new cells to this queue
            x = 0; y = 0;
            while (uQueue.Get(ref x, ref y))//Get next cell from queue
            {
                if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_TO_CHECK) != 0)
                {
                    //If cell is marked as required to check (i.e. was marked but not yet checked since that)

                    if (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] >= 0)
                    {//To-right is achived
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_RIGHT) != 0 && iBoxMoveTree[x, y, PLAYER_TO_DOWN] == MT_NOT_REACHED)
                        {//Right-to-down is possible and to-down is not achived - we can go around box and achive to-down
                            iBoxMoveTree[x, y, PLAYER_TO_DOWN] = 0;//To-down now achived
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_LEFT_TO_RIGHT) != 0 && iBoxMoveTree[x, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_LEFT] = 0;//By analogue...
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_RIGHT) != 0 && iBoxMoveTree[x, y, PLAYER_TO_UP] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_UP] = 0;//By analogue...
                        }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_LEFT] >= 0)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_LEFT) != 0 && iBoxMoveTree[x, y, PLAYER_TO_DOWN] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_DOWN] = 0;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_LEFT_TO_RIGHT) != 0 && iBoxMoveTree[x, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = 0;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_LEFT) != 0 && iBoxMoveTree[x, y, PLAYER_TO_UP] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_UP] = 0;
                        }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_UP] >= 0)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_DOWN) != 0 && iBoxMoveTree[x, y, PLAYER_TO_DOWN] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_DOWN] = 0;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_LEFT) != 0 && iBoxMoveTree[x, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_LEFT] = 0;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_RIGHT) != 0 && iBoxMoveTree[x, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = 0;
                        }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_DOWN] >= 0)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_LEFT) != 0 && iBoxMoveTree[x, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_LEFT] = 0;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_RIGHT) != 0 && iBoxMoveTree[x, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_RIGHT] = 0;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_DOWN) != 0 && iBoxMoveTree[x, y, PLAYER_TO_UP] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_UP] = 0;
                        }
                    }

                    if (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] >= 0)
                    {//To-right is achived
                        if (iBoxMoveTree[x - 1, y, PLAYER_TO_RIGHT] == MT_NOT_REACHED) //To-right for cell to left is not achived - we can push box to left there
                        {
                            iBoxMoveTree[x - 1, y, PLAYER_TO_RIGHT] = 0; //To-right for cell to left is now achived
                            iBoxMoveTree[x - 1, y, BMT_FLAGS] |= BMTF_TO_CHECK;//Mark cell to left for further checking
                            uQueue.AddNext(x - 1, y);//Add cell to left into queue
                        }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_LEFT] >= 0)
                    {//By analogue...
                        if (iBoxMoveTree[x + 1, y, PLAYER_TO_LEFT] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x + 1, y, PLAYER_TO_LEFT] = 0;
                            iBoxMoveTree[x + 1, y, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x + 1, y);
                        }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_UP] >= 0)
                    {//By analogue...
                        if (iBoxMoveTree[x , y+1, PLAYER_TO_UP] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x , y+1, PLAYER_TO_UP] = 0;
                            iBoxMoveTree[x , y+1, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x , y+1);
                        }
                    }
                    if (iBoxMoveTree[x, y, PLAYER_TO_DOWN] >= 0)
                    {//By analogue...
                        if (iBoxMoveTree[x , y-1, PLAYER_TO_DOWN] == MT_NOT_REACHED)
                        {
                            iBoxMoveTree[x , y-1, PLAYER_TO_DOWN] = 0;
                            iBoxMoveTree[x , y-1, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x , y-1);
                        }
                    }
                    iBoxMoveTree[x, y, BMT_FLAGS] &= BMTF_MASK_CLEAR_TOCHECK;//This cell is checked - it should not be checked again, until not marked again
                }
            }

            uQueue.Reset();//Clean queue

            //Stage 3. Mark all targets on level as non-cell-deadlocks

            //For all cells of level
            for (y = 1; y < (iYsize-1); y++)
                for (x = 1; x < (iXsize-1); x++)
                {
                    if ((bCells[x, y] & SokoCell.Target)!=0)
                    {//If cell contain target

                        uQueue.AddNext(x, y);//Add cell to queue for checking
                        iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_TO_CHECK;//Mark cell for further checking
                        for (z = 0; z < 4; z++)
                            if (iBoxMoveTree[x, y, z] >= 0)//If branch is achived by tree
                                iBoxMoveTree[x, y, z] |= MT_BACKWAVE_REACH;//Mark cell for backwave

                    }
                    if ((bCells[x, y] & SokoCell.Box) != 0)
                    {//If cell contain box

                        uQueue.AddNext(x, y);//Add cell to queue for checking
                        iBoxMoveTree[x, y, BMT_FLAGS] |= BMTF_TO_CHECK;//Mark cell for further checking
                        for (z = 0; z < 4; z++)
                            if (iBoxMoveTree[x, y, z] >= 0)//If branch is achived by tree
                                iBoxMoveTree[x, y, z] |= MT_FRONTWAVE_REACH;//Mark cell for frontwave

                    }
                }

            //Stage 4. Propagate backward and forward waves by box move tree connections

            //Propagating - by checking cells, received from queue and adding new cells to this queue
            x = 0; y = 0;
            while (uQueue.Get(ref x, ref y))//Get next cell from queue
            {
                if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_TO_CHECK) != 0)
                {
                    //If cell is marked as required to check (i.e. was marked but not yet checked since that)

                    //4.1 - backwave
                    if ((iBoxMoveTree[x, y, PLAYER_TO_RIGHT] & MT_CHECK_BACKWAVE) == MT_BACKWAVE_REACH)
                    {//To-right is non-cell-deadlock
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_DOWN] & MT_CHECK_BACKWAVE) == 0)
                        {//Right-to-down is possible and to-down is achived - we can go around box and achive to-down
                            iBoxMoveTree[x, y, PLAYER_TO_DOWN] |= MT_BACKWAVE_REACH;//To-down now non-cell-deadlock
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_LEFT_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_LEFT] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_LEFT] |= MT_BACKWAVE_REACH;//By analogue...
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_UP] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_UP] |= MT_BACKWAVE_REACH;//By analogue...
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_LEFT] & MT_CHECK_BACKWAVE) == MT_BACKWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_LEFT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_DOWN] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_DOWN] |= MT_BACKWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_LEFT_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_RIGHT] |= MT_BACKWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_LEFT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_UP] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_UP] |= MT_BACKWAVE_REACH;
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_UP] & MT_CHECK_BACKWAVE) == MT_BACKWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_DOWN) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_DOWN] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_DOWN] |= MT_BACKWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_LEFT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_LEFT] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_LEFT] |= MT_BACKWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_RIGHT] |= MT_BACKWAVE_REACH;
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_DOWN] & MT_CHECK_BACKWAVE) == MT_BACKWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_LEFT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_LEFT] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_LEFT] |= MT_BACKWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_RIGHT] |= MT_BACKWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_DOWN) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_UP] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_UP] |= MT_BACKWAVE_REACH;
                        }
                    }

                    if ((iBoxMoveTree[x, y, PLAYER_TO_RIGHT] & MT_CHECK_BACKWAVE) == MT_BACKWAVE_REACH)
                    {//To-right is non-cell-deadlock
                        if ((iBoxMoveTree[x + 1, y, PLAYER_TO_RIGHT] & MT_CHECK_BACKWAVE) == 0) //To-right for cell to right is achived - we can push box from there to here (to left), so cell to right is also non-cell-deadlock
                        {
                            iBoxMoveTree[x + 1, y, PLAYER_TO_RIGHT] |= MT_BACKWAVE_REACH;//To-right for cell to right is non-cell-deadlock now
                            iBoxMoveTree[x + 1, y, BMT_FLAGS] |= BMTF_TO_CHECK;//Mark cell to right for further checking
                            uQueue.AddNext(x + 1, y);//Add cell to right into queue
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_LEFT] & MT_CHECK_BACKWAVE) == MT_BACKWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x - 1, y, PLAYER_TO_LEFT] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x - 1, y, PLAYER_TO_LEFT] |= MT_BACKWAVE_REACH;
                            iBoxMoveTree[x - 1, y, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x - 1, y);
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_UP] & MT_CHECK_BACKWAVE) == MT_BACKWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y - 1, PLAYER_TO_UP] & MT_CHECK_BACKWAVE) == 0)
                        {
                            iBoxMoveTree[x, y - 1, PLAYER_TO_UP] |= MT_BACKWAVE_REACH;
                            iBoxMoveTree[x, y - 1, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x, y - 1);
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_DOWN] & MT_CHECK_BACKWAVE) == MT_BACKWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y + 1, PLAYER_TO_DOWN] & MT_BACKWAVE_REACH) == 0)
                        {
                            iBoxMoveTree[x, y + 1, PLAYER_TO_DOWN] |= MT_BACKWAVE_REACH;
                            iBoxMoveTree[x, y + 1, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x, y + 1);
                        }
                    }


                    //4.2 - frontwave
                    if ((iBoxMoveTree[x, y, PLAYER_TO_RIGHT] & MT_CHECK_FRONTWAVE) == MT_FRONTWAVE_REACH)
                    {//To-right is non-cell-deadlock
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_DOWN] & MT_CHECK_FRONTWAVE) == 0)
                        {//Right-to-down is possible and to-down is achived - we can go around box and achive to-down
                            iBoxMoveTree[x, y, PLAYER_TO_DOWN] |= MT_FRONTWAVE_REACH;//To-down now non-cell-deadlock
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_LEFT_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_LEFT] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_LEFT] |= MT_FRONTWAVE_REACH;//By analogue...
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_UP] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_UP] |= MT_FRONTWAVE_REACH;//By analogue...
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_LEFT] & MT_CHECK_FRONTWAVE) == MT_FRONTWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_LEFT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_DOWN] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_DOWN] |= MT_FRONTWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_LEFT_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_RIGHT] |= MT_FRONTWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_LEFT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_UP] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_UP] |= MT_FRONTWAVE_REACH;
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_UP] & MT_CHECK_FRONTWAVE) == MT_FRONTWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_DOWN) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_DOWN] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_DOWN] |= MT_FRONTWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_LEFT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_LEFT] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_LEFT] |= MT_FRONTWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_RIGHT] |= MT_FRONTWAVE_REACH;
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_DOWN] & MT_CHECK_FRONTWAVE) == MT_FRONTWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_LEFT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_LEFT] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_LEFT] |= MT_FRONTWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_DOWN_TO_RIGHT) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_RIGHT] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_RIGHT] |= MT_FRONTWAVE_REACH;
                        }
                        if ((iBoxMoveTree[x, y, BMT_FLAGS] & BMTF_UP_TO_DOWN) != 0 && (iBoxMoveTree[x, y, PLAYER_TO_UP] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y, PLAYER_TO_UP] |= MT_FRONTWAVE_REACH;
                        }
                    }

                    if ((iBoxMoveTree[x, y, PLAYER_TO_RIGHT] & MT_CHECK_FRONTWAVE) == MT_FRONTWAVE_REACH)
                    {//To-right is non-cell-deadlock
                        if ((iBoxMoveTree[x - 1, y, PLAYER_TO_RIGHT] & MT_CHECK_FRONTWAVE) == 0) //To-right for cell to right is achived - we can push box from here to there (to right)
                        {
                            iBoxMoveTree[x - 1, y, PLAYER_TO_RIGHT] |= MT_FRONTWAVE_REACH;//To-right for cell to right is non-cell-deadlock now
                            iBoxMoveTree[x - 1, y, BMT_FLAGS] |= BMTF_TO_CHECK;//Mark cell to right for further checking
                            uQueue.AddNext(x - 1, y);//Add cell to right into queue
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_LEFT] & MT_CHECK_FRONTWAVE) == MT_FRONTWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x + 1, y, PLAYER_TO_LEFT] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x + 1, y, PLAYER_TO_LEFT] |= MT_FRONTWAVE_REACH;
                            iBoxMoveTree[x + 1, y, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x + 1, y);
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_UP] & MT_CHECK_FRONTWAVE) == MT_FRONTWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y + 1, PLAYER_TO_UP] & MT_CHECK_FRONTWAVE) == 0)
                        {
                            iBoxMoveTree[x, y + 1, PLAYER_TO_UP] |= MT_FRONTWAVE_REACH;
                            iBoxMoveTree[x, y + 1, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x, y + 1);
                        }
                    }
                    if ((iBoxMoveTree[x, y, PLAYER_TO_DOWN] & MT_CHECK_FRONTWAVE) == MT_FRONTWAVE_REACH)
                    {//By analogue...
                        if ((iBoxMoveTree[x, y - 1, PLAYER_TO_DOWN] & MT_FRONTWAVE_REACH) == 0)
                        {
                            iBoxMoveTree[x, y - 1, PLAYER_TO_DOWN] |= MT_FRONTWAVE_REACH;
                            iBoxMoveTree[x, y - 1, BMT_FLAGS] |= BMTF_TO_CHECK;
                            uQueue.AddNext(x, y - 1);
                        }
                    }
                    
                    iBoxMoveTree[x, y, BMT_FLAGS] &= BMTF_MASK_CLEAR_TOCHECK;//This cell is checked - it should not be checked again, until not marked again
                }
            }

            //Stage 5. Convert all non-cell-deadlocks from box move tree into cell markers

            //For all cells of level
            for (y = 0; y < iYsize; y++)
                for (x = 0; x < iXsize; x++)
                {
                    for (z = 0; z < 4; z++)//For all positions of player around box on this cell
                        if (iBoxMoveTree[x, y, z] == MT_BOTHWAVE_REACH)//If at least one direction is reached by both waves, then whole cell is not cell-deadlock
                        //if ((iBoxMoveTree[x, y, z] & MT_CHECK_BACKWAVE) == MT_BACKWAVE_REACH)
                            goto lSkip;//Skip marking cell
                    bCells[x, y] |= SokoCell.CellDeadlock;//All cells, that was not reached by both waves - is cell-deadlocks
                lSkip: ;
                }

            return FunctionResult.OK;
        }

        ///<summary>Check game position for static-deadlocks and boxes in cell-deadlock, fast way</summary>
        //This is fast way, good perfomance, but may not detect deadlock on levels with exceeding boxes
        public FunctionResult FastCheckForDeadlocks()
        {
            bDeadlock = true;
            if (iFirstDeadlockMove>-1 && iFirstDeadlockMove <= iPosition)
                return FunctionResult.GameDeadlock;//It is game-deadlock

            //If boxes is placed into cell-deadlock (more boxes, than exceed over targets)
            if (iNumRemainExceedBoxes < 0)
                return FunctionResult.GameDeadlock;//It is game-deadlock

            if (iPosition > 0)
            {
                int x = iPlayerX;
                int y = iPlayerY;
                int x2, y2,x3,y3;
                int iBoxesWithoutTarget;
                bool bZigZag;
                SokoCell bFilter = SokoCell.Target | SokoCell.CellDeadlock;

                SokoMove bLastMove = bMoves[iPosition-1];
                if ((bLastMove&SokoMove.Push)==0)
                {
                    goto lNoMoreChecks;
                }
                switch (bLastMove & SokoMove.Direction)
                {
                    case SokoMove.Up: y--; break;
                    case SokoMove.Down: y++; break;
                    case SokoMove.Right: x++; break;
                    case SokoMove.Left: x--; break;
                }

                //1 test for static-deadlock
                //Check for 2x2 cells, that contain at least one cell with box but without targets and deadlocks
                for (x2 = x - 1; x2 <= x; x2++)
                    for (y2 = y - 1; y2 <= y; y2++)
                    {
                        iBoxesWithoutTarget = 0;
                        for (x3 = x2; x3 < (x2 + 2); x3++)
                            for (y3 = y2; y3 < (y2 + 2); y3++)
                            {
                                if ((GetCell(x3, y3) & SokoCell.MaskObstacle) == 0)
                                    goto lNextQuad;
                                if ((GetCell(x3, y3) & SokoCell.Box) != 0 && (GetCell(x3, y3) & bFilter) == 0)
                                    iBoxesWithoutTarget++; //Box, no target, no cell-deadlock
                            }

                        if (iBoxesWithoutTarget>iNumRemainExceedBoxes)
                        {   //In this particular 2x2 there is more boxes, than remain unlocked exceeding
                            bDeadlock = true;
                            iFirstDeadlockMove = iPosition;
                            return FunctionResult.GameDeadlock;//It is game-deadlock
                        }
                    lNextQuad: ;
                    }

                 //2) Check for "zig-zag lock":
                 //  #$ 
                 //   $#
                x3 = x; y3 = y;
                for (int i = 0; i < 4; i++)
                {
                    bZigZag = false;
                    x2=x;y2=y;
                    switch (i)
                    {
                        case 0: x2++; x3 = x; break;
                        case 1: x2--; x3 = x2; break;
                        case 2: y2++; y3 = y; break;
                        case 3: y2--; y3 = y2; break;
                    }
                    if ((GetCell(x2, y2) & SokoCell.Box ) == 0)
                        goto lNextDir;

                    if (i < 2)
                    {//x2++/x2--
                        // # 
                        // $$
                        //  #
                        if ((GetCell(x3, y-1) & SokoCell.MaskPermanentObstacle) != 0)
                        {
                            if ((GetCell(x3 + 1, y + 1) & SokoCell.MaskPermanentObstacle) != 0)
                            {
                                bZigZag = true;//Goto not working from if() {{{ goto }}}  if () {{{ label: }}}
                            }
                        }
                        //  #
                        // $$
                        // # 
                        if ((GetCell(x3, y + 1) & SokoCell.MaskPermanentObstacle) != 0)
                        {
                            if ((GetCell(x3 + 1, y - 1) & SokoCell.MaskPermanentObstacle) != 0)
                            {
                                bZigZag = true;
                            }
                        }
                    }
                    else
                    {//y2++/y2--

                        // $#
                        //#$
                        if ((GetCell(x + 1, y3) & SokoCell.MaskPermanentObstacle) != 0)
                        {
                            if ((GetCell(x - 1, y3 + 1) & SokoCell.MaskPermanentObstacle) != 0)
                            {
                                bZigZag = true;
                            }
                        }

                        //#$ 
                        // $#
                        if ((GetCell(x - 1, y3) & SokoCell.MaskPermanentObstacle) != 0)
                        {
                            if ((GetCell(x + 1, y3 + 1) & SokoCell.MaskPermanentObstacle) != 0)
                            {
                                bZigZag = true;
                            }
                        }
                    }

                    if (bZigZag)
                    {
                        iBoxesWithoutTarget = 0;
                        if ((GetCell(x, y) & bFilter) == 0)
                            iBoxesWithoutTarget++;//Box, but no target and no cell-deadlock
                        if ((GetCell(x2, y2) & bFilter) == 0)
                            iBoxesWithoutTarget++;//Box, but no target and no cell-deadlock
                        if (iBoxesWithoutTarget > iNumRemainExceedBoxes)
                        {
                            //In this particular zig-zag there is more boxes, than remain unlocked exceeding
                            bDeadlock = true;
                            iFirstDeadlockMove = iPosition;
                            return FunctionResult.GameDeadlock;//It is game-deadlock
                        }
                    }
                    
                lNextDir: ;
                }

                //TODO:

            lNoMoreChecks: ;
            }



            bDeadlock = false;
            return FunctionResult.OK;
        }

        ///<summary>Is level solved by player?</summary>
        public bool IsDeadlock()
        {
            return bDeadlock;
        }

    }
}
