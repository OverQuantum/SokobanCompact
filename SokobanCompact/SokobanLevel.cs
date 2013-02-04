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

namespace SokobanCompact
{
    ///<summary>Whole Sokoban level</summary>
    public class SokobanLevel
    {
        //Constants for move trees

        ///<summary>Branch is not reached by tree</summary>
        public const short MT_NOT_REACHED = -4;
        ///<summary>Branch is blocked somehow</summary>
        public const short MT_BLOCKED = -12;
        ///<summary>Branch is reached by frontwave</summary>
        public const short MT_FRONTWAVE_REACH = 1;
        ///<summary>Branch is reached by backwave</summary>
        public const short MT_BACKWAVE_REACH = 2;
        ///<summary>Branch is reached by backwave and frontwave</summary>
        public const short MT_BOTHWAVE_REACH = 3;

        ///<summary>check for branch is reached by frontwave</summary>
        public const short MT_CHECK_FRONTWAVE = 5;
        ///<summary>check for branch is reached by backwave</summary>
        public const short MT_CHECK_BACKWAVE = 6;
        
        ///<summary>Width of level</summary>
        public int iXsize;

        ///<summary>Height of level</summary>
        public int iYsize;

        ///<summary>Content of level - array of cells</summary>
        protected SokoCell[,] bCells;

        ///<summary>Name of level</summary>
        public string sTitle;
        ///<summary>Author of level</summary>
        public string sAuthor;
        ///<summary>Auther comment for this level</summary>
        public string sComment;

        ///<summary>Is level solved by player?</summary>
        protected bool bSolved;

        ///<summary>Record - solution with minimum moves</summary>
        public PositionStats uBestMovesSolution;
        ///<summary>Record - solution with minimum pushes</summary>
        public PositionStats uBestPushesSolution;

        ///<summary>Debug counter, for calculation number of operations</summary>
        public int iCountDebug;

        ///<summary>Empty constructor, nothing is done</summary>
        public SokobanLevel()
        {

        }

        ///<summary>Default constructor of empty level(x-size,y-size)</summary>
        public SokobanLevel(int iRequiredXsize, int iRequiredYsize)
        {
            SetSize(iRequiredXsize, iRequiredYsize);
        }

        ///<summary>Initialize level (x-size,y-size)</summary>
        protected void SetSize(int iRequiredXsize, int iRequiredYsize)
        {
            iXsize = iRequiredXsize;//Set sizes
            iYsize = iRequiredYsize;
            bCells = new SokoCell[iXsize, iYsize];//Allocate array of cells

            //Initialize all level with Background content
            // (Is there any memset-like way to set all cells?)
            for (int i = 0; i < iXsize; i++)
                for (int j = 0; j < iYsize; j++)
                    bCells[i, j] = SokoCell.Background;

            bSolved = false;//Not solved initially
            uBestPushesSolution.InitMax();//No solutions initially, so create max-values solutions to simplify comparison
            uBestMovesSolution.InitMax();
            sTitle = "";//Initial level name and other strings
            sAuthor = "";
            sComment = "";
        }


        ///<summary>Copy-constructor for fast creation of temprorary levels (source level)</summary>
        public SokobanLevel(SokobanLevel uLevelToCopy)
        {
            CopyFrom(uLevelToCopy);
        }

        /// <summary>Remove empty cells outside level walls - required after loading from text fils</summary>
        public void CleanUp()
        {
            //Function use simple wave algorithm - Background cell consume nearest Empty cells

            int i, j, iStop = 0;
            
            while (iStop == 0)//Iterated till no cells were converted during iteration
            {
                iStop = 1;
                for (i = 0; i < iXsize; i++)
                    for (j = 0; j < iYsize; j++)
                        if (bCells[i, j] == SokoCell.Empty)
                        {
                            if (GetCell(i - 1, j) == SokoCell.Background ||
                                GetCell(i + 1, j) == SokoCell.Background ||
                                GetCell(i, j - 1) == SokoCell.Background ||
                                GetCell(i, j + 1) == SokoCell.Background)
                            {   //If Background cells are near current (note, that GetCell also return Background from outside of level, so borders will be cleaned)
                                bCells[i, j] = SokoCell.Background;//Convert current into Background
                                iStop = 0;//Next iteration required
                            }
                        }
            }
        }


        ///<summary>Get content of one cell, Background returned for cell outside level boundaries (x-pos,y-pos)</summary>
        public SokoCell GetCell(int iX, int iY)
        {
            if (iX < 0 || iY < 0 || iX >= iXsize || iY >= iYsize)
                return SokoCell.Background;//Outside level is Background
            return bCells[iX, iY];//Return content
        }

        ///<summary>!!ONLY FOR LEVEL LOADING!!  Set cell content (x-pos,y-pos,new content)</summary>
        public FunctionResult SetCell(int x, int y, SokoCell uValue)
        {
            if (x < 0 || y < 0 || x >= iXsize || y >= iYsize)
                return FunctionResult.OutOfLevel;//Outside level
            bCells[x, y] = uValue;//Copy value
            return FunctionResult.OK;
        }

        ///<summary>Is level solved by player?</summary>
        public bool IsSolved()
        {
            return bSolved;
        }

        ///<summary>Copy whole level (source level)</summary>
        public void CopyFrom(SokobanLevel uSourceLevel)
        {
            //Reinitialize level
            SetSize(uSourceLevel.iXsize, uSourceLevel.iYsize);

            //Copy all cells from source
            for (int i = 0; i < uSourceLevel.iXsize; i++)
                for (int j = 0; j < uSourceLevel.iYsize; j++)
                    bCells[i, j] = uSourceLevel.bCells[i, j];
            // (Is there any memcpy-like way for this?)

            //Copy other parameters of level
            uBestMovesSolution = uSourceLevel.uBestMovesSolution;
            uBestPushesSolution = uSourceLevel.uBestPushesSolution;
            bSolved = uSourceLevel.bSolved;
            sTitle = uSourceLevel.sTitle;
            sAuthor = uSourceLevel.sAuthor;
            sComment = uSourceLevel.sComment;
        }

        ///<summary>Check new solution against records of this level</summary>
        public SolutionFlags EstimateNewSolution(SokobanGame uSolution)
        {
            SolutionFlags uRv = SolutionFlags.Nothing;//Initially no achivments
            if (!bSolved)
                uRv |= SolutionFlags.FirstSolution;//Level was not solved? So this is first solution
            {
                if (uSolution.uStats.iMoves < uBestMovesSolution.iMoves)
                    uRv |= SolutionFlags.BestMoves;//New best moves record
                if (uSolution.uStats.iPushes < uBestPushesSolution.iPushes)
                    uRv |= SolutionFlags.BestPushes;//New best pushes record
            }
            return uRv;//Return flags
        }

        ///<summary>Update records of level with achivment sof new solution</summary>
        public void CheckSolutionForRecord(PositionStats uSolutionStats)
        {
            bSolved = true;

            if (uSolutionStats.iMoves < uBestMovesSolution.iMoves)
            {   //New best moves record
                uBestMovesSolution = uSolutionStats;
            }
            if (uSolutionStats.iPushes < uBestPushesSolution.iPushes)
            {   //New best pushes record
                uBestPushesSolution = uSolutionStats;
            }
        }

        ///<summary>Return description of level - title, sizes and number of boxes</summary>
        public string GetLevelDescription()
        {
            //return sTitle + " (" + iXsize.ToString() + "x" + iYsize.ToString() + ")";

            LevelStats uStats = CalcLevelStats();//Calculate level stats to get number of boxes
            return sTitle + " (" + iXsize.ToString() + "x" + iYsize.ToString() + "-"+uStats.iNumBoxes.ToString()+")";//Combine description and return it
        }

        ///<summary>Calculate level stats - number of cell of each type</summary>
        public LevelStats CalcLevelStats()
        {
            LevelStats uRv = new LevelStats();

            for (int i = 0; i < iXsize; i++)
                for (int j = 0; j < iYsize; j++)
                    switch (bCells[i, j]&SokoCell.FilterSkipCellDeadlocks)
                    {
                        case SokoCell.Box://Only box
                            uRv.iNumBoxes++;
                            break;
                        case SokoCell.BoxOnTarget://Box and target and box-on-target
                            uRv.iNumBoxes++;
                            uRv.iNumTargets++;
                            uRv.iNumBoxesOnTargets++;
                            break;
                        case SokoCell.PlayerOnTarget://Player and target
                            uRv.iNumTargets++;
                            uRv.iNumPlayers++;
                            break;
                        case SokoCell.Target://Only target
                            uRv.iNumTargets++;
                            break;
                        case SokoCell.Player://Only player
                            uRv.iNumPlayers++;
                            break;
                        case SokoCell.Empty://Only Empty
                            uRv.iNumEmpty++;
                            break;
                        case SokoCell.Wall://Only wall
                            uRv.iNumWalls++;
                            break;
                        case SokoCell.Background://Only background
                            uRv.iNumBackground++;
                            break;
                    }
            return uRv;
        }
    }
}
