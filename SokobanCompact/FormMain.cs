/*
 *            SokobanCompact
 * 
 * Sokoban for Windows Mobile (6) with stylus control and usefull highlights
 * Inspired by Sokoban++ and YASC
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
 * 
 * Author contacts:
 * http://overquantum.livejournal.com
 * https://github.com/OverQuantum
 * 
 * Project homepage:
 * https://github.com/OverQuantum/SokobanCompact
 * 
 * 
 * 
 * Thanks to my friends for advises:
 *   Saracen, bass, vazooza
 * 
 * 
 * Sokoban Copyright © 1982 by THINKING RABBIT Inc. JAPAN.
 * 
 * Levelset copyrights belong to their respective owners.
 * See copyright info for levels and levelsets.
 * 
 * Background images are from www.colourlovers.com
 * used under Attribution-Noncommercial-Share Alike Creative Commons License.
 * 
 * 
 * ##################################################################################################
 * 
 * Implemented now:
 * 1) Macro move by one click
 * 2) Macro box pushing by two clicks (with highlights of all reachable cells for pushing of selected box)
 * 3) Full undo and redo, group undo and redo (1 move-by-one-click or push-by-two-clicks undone in 1 click) 
 * 4) Saving and loading positions
 * 5) Skins and scroll
 * 6) Counting 6 statistics  (moves, pushes, linear moves, linear pushes, pushing sesssions, box changes)
 * 7) List of available level sets with manual sorting and indication of solved levels amount
 * 8) Saving all solutions of each level, saving best-moves and best-pushes records for each level
 * 9) Unequal amount of boxes and targets allowed
 * 10) Work on different orientations of screen
 * 11) Highlight forbidden cells (push box here - and you are in game-deadlock)
 * Plans:
 * -  Highlight all possible pushes from current position
 * 
 *  14.07.2008 Idea for Compact formulated
 * 
 *  21.08.2008 Project started
 *  06.09.2008 Box push tree implemented
 *  07.09.2008 Save position implemented
 *  09.09.2008 Optimizing BMT
 *  09.10.2008 Added sort of levelset list
 *  11.10.2008 Support 5 statistics
 *  12.10.2008 Optimizing RedrawAround
 *  14.10.2008 Adding box changes statistics
 *  16.10.2008 Chaning menu
 *  18.10.2008 Converting SokobanLevel+SokobanPosition into SokobanGame
 *  19.10.2008 Finished converting
 *  22.10.2008 First cell-deadlock calc implemented
 *  27.10.2008 Added resources
 * 
 *  14.12.2008 Commenting code
 *  21.12.2008 Commenting code, slightly changed SokobanGame.MovePlayer()
 *  10.02.2009 Commenting code
 *  11.02.2009 Commenting code, minor modifications
 *  12.02.2009 Commenting code
 *  14.02.2009 Commenting code
 *  15.02.2009 Commenting code, generally finished
 *  17.02.2009 Fixing hints from ReSharper
 *  19.02.2009 Fixing about
 *  20.02.2009 Fixing about
 *  20.02.2009 v0.5.3338.42204 published
 * 
 *  01.03.2009 fixed "ok" on settings
 *             changed OpenFile to use DialogResult
 *             All file reading/writing now inside try-catch protectors
 *             adding prevent suspending
 *  03.03.2009 adding prevent suspending
 *             ADD: skin element "box on cell-deadlock"
 *             ADD: skin element "possbile box on cell-deadlock"
 *             adding "limit box-move-tree with cell-deadlocks" to allow push exceeding boxes into cell-deadlocks
 *  07.03.2009 ADD: skinset
 *             ADDed: prevent suspending
 *  09.03.2009 FIX: layout of status bar, now fit all (checked 640x480, 320x240, 320x320, 240x240, 480x480)
 *  10.03.2009 ADD: Random level, Random unsolved level
 *             adding texture and solid backgrounds
 *  11.03.2009 ADD: texture and solid backgrounds
 *  16.03.2009 CHG: On empty levelset list - auto update it
 *             ADD: Context menu for Choose LevelSet
 *             ADD: Menu to delete levelset from list 
 *             DEL: Button to set comment (now only in menu)
 *  24.03.2009 ADD: Animation speed
 *             ADD: Zoom in and Zoom out (by skinsets)
 *  25.03.2009 Adding simple static-deadlock detection
 *  26.03.2009 FIX: word "Deadlock" in sources is diversified into game-, cell- and static-deadlock
 *  27.03.2009 ADD: Fast game-deadlock detection
 *  28.03.2009 ADD: Nonmodal warning about game-deadlock
 *             CHG: Move iMinMove into settings (now iDragMinMove)
 *             ADD: Set iDragMinMove
 *             Adding Load position with levelset and level
 *  30.03.2009 ADD: Load position with levelset and level
 *             CHG: Position and solution saving now write levelset filename, not sID
 *  01.04.2009 FIX: bug on zig-zag static-deadlock (check on microkosmos.txt level 21)
 *             FIX: additional moves before reaching box in exact location on box-push-by-two-clicks
 *             CHG: Icon for toolbar selector
 *  03.04.2009 Adding: Handle errors on loading levels and levelsets
 *  05.04.2009 ADD: Handle errors on loading levels and levelsets
 *             ADD: Handle errors on loading skin
 *  10.04.2009 CHG: Moved loading and saving position into FormMain functions
 *             ADD: After loading position - redo it forward to check all pushes for *-deadlocks
 *             ADD: Zoom: {min, -2, -1, current, +1, +2, max}
 *  03.05.2009 ADD: System info (charge level and time)
 *             FIX: Autoname levels, if no title is specified
 *             FIX: "Maximize windows" now display check-status
 *             ADD: Menu item "Lock Scroll" (currently value not saving)
 *  04.05.2009 ADD: Option: Autosize and recenter only for usefull cells of level (ignores background and walls)
 *  15.05.2009 ADD: v-Scroll for ShowInfo form
 *             CHG: Option bScrollLock - saves "Lock Scroll"
 *             CHG: Next/prev level are cycled now
 *             CHG: main icon (now Soko3.ico)
 *             v0.6 from now
 *  16.05.2009 v0.6.3422.42426 published
 * 
 *  20.05.2009 ADD: Menuitem in levelset selection: number of levelset, total number of all known levels and number of solved levels
 * 
 *  22.08.2009 CHG: Removed "Loaded " on changing level
 *             CHG: AntiSuspend renamed into CoreDll
 *             ADD: "Minimize" menu item
 *             ADD: Show total number of levels in position-stat showing (menu)
 *             ADD: Show filename in levelset info
 *             ADD: Strict mask for loading levels - for ex. to not see positions for 70th levels while loading for 7th
 *             FIX(?): On level solving boxmovetree is not flushed, that is affects next level first click
 *             Adding log all actions
 *             Adding autosave position on exit
 *  23.08.2009 ADD: autosave position on exit and autoload after loading level
 *             FIX: crush on manual CalcDeadlock after automatic (player position was not calculated by reanalyze)
 *             ADD: calc cell-deadlock in separate thread + option to control
 * 
 *  19.11.2009 Adding comments on Level Database 
 *  20.11.2009 Adding log all actions
 *  23.11.2009 CHG: main code from ActionFullRedo is moved to FullRedo 
 *             ADD: log actions (seems to cover all for now)
 * 
 *  10.02.2010 CHG: All unknown lines of text in levelset file before first level - count as comment, if comment-comment-end pair not yet found (as found, all other is ignored)
 *  12.02.2010 ADD: Delimiters into levelset list
 *  14.02.2010 CHG: new CalcDeadlocks - now with two waves (frontwave and backwave) about 6% longer, but better - cells, there you cannot put boxes, now marked as deadlock (and corridor weaknes is removed)
 * 
 *  05.02.2013 v0.7, GPL
 * 
 * 
 *  WARNING ON CHANGES:
 *  30.03.2009 CHG: Position and solution saving now writes filename, not sID, so all sol/pos must be updated to load it with files
 * 
 * 
 * Latest version of future plans (from readme_eng.txt of v0.6):
 *  - Interface overview
 *  - Advanced indication of game deadlock
 *  - Highlight movable boxes and allowed directions for each box
 *  - Configuration of animation speed
 *  - Autoexecution of only possibly move
 *  - Autoundo in case of game deadlock
 *  - Quick save and load position (1 click)
 *  - Interface optimization to play by fingers
 *  - Counting time of solving each level
 *  - Level selection with previews
 *  - Russian interface
 *  - Additional statistics (moves in circles, pushes on goals and so on)
 *  - Support XML levelset formats
 *  - More levelsets
 *  - Tutorial game mode
 *  - Some database for level duplicates in different levelsets
 *  
 * 
 *                  
 *  Terminology:
 *  - position - 1) boxes placements and player placement
 *               2) sequence of player moves       (both definition are valid)
 *  - cell-deadlock - cell, from where you can not push box into target
 *  - game-deadlock - level+position that can not be solved
 *  - static-deadlock - group of boxes, that can not be moved anymore
 * 
 * 
 *  TODO:
 *    + Redraw only required rect 
 *    . After move - invalidate only required area (3 block max)  [done in 4 blocks, 3 block variant - in timeline\081012_diffRedrawAround\]
 *    + Completition of level
 *    + Refactor Direction into SokoMove (?)
 *    + Change move-to macro to use undo buffer
 *    + Redo
 *    + Clean outer Empty-ies on loading level
 *    + Move-box routine
 *    + Highlighted box, possible-move, possible move-on-target - to skin
 *    + Full redo
 *    + Undo to marker
 *    + Redo to marker
 *    + Backup level (to not reload level from file)
 *    + Loading levelset from textfile
 *    + Selecting level by name
 *    + Storing, that levels are completed
 *    + Loading, that levels are completed
 *    + Save positions and solutions
 *    + Load positions and solutions
 *    + Update statistic of position
 *    + Display statistic of position
 *    + Loading level names, levelset names e.t.c
 *    + Loading comments to levels and levelsets
 *    + optimize BuildBoxMoveTreeFull to use chain-of-cells-that-planned-to-check, instead of double-cycle-of-brute-scan-of-array
 *    + check level solving after box-pushing route
 *    + Load skin
 *    + Load levelset
 *    + RecenterLevel by menu 
 *    + Save cfg: levet set, level and skin, config, player name
 *    + Typing name of solution on saving solution
 *    + Set player name
 *    + "Next unsolved level" menu + behaviour on solving
 *    + Show count of unsolved levels on LevelSet info
 *    + Show nonmodal info on travel and box-travel with moves/pushes info
 *    + Display [solved] before level title (and even number?) on level select form
 *  .   ?Somehow save record and solution with one click, not two (Our own window?) [now there is option to ask record name]
 *    + Skins 6+, 8+, 10+, 12+, 14+, 16+, 20+, 24+, 28+, 32+
 *    X Optimize calls BuildPlayerMoveTree from BuildBoxMoveTreeFull to stop on achiving box neighbors... (more results obtained from 2-, 4- and 6- steps around box)
 *    + Menu item for loading most sufficient skin (autosize)
 *    + Suppress sound on non-error msgboxes
 *    + Show name of skin
 *    + Icons for status bar
 *    + New list of levelset - name, solved/total levels  [ click - load level set, info stored in separated file ]
 *    + Option for asking-nothing on first level completition (autosave)
 *    + Add moves for sorting levelset list
 *    X Simple update of levelset list - without loading already known levelsets - only checking records for them [Update optimized]
 *    + Button to delete levelset from list (for cases, then file is deleted)
 *    + Add waiting cursor
 *    + Add count of Box Changes
 *    + Add all 6 stats into record name
 *    + Add all 6 stats into solution/position file
 *    + Test stats on solution of some hard level from "online" levelset (Lev 35 - ok, except unreleased BoxChanges)
 *    + Options form - for set all options
 *    + Display size of current skin on loading skin
 *    + Display amount of boxes on level selections
 *    X Animate update of levelset list
 *    + Change "Menu" to icon
 *    + Icon for Game (rework for better quality)
 *    + On missing LevelSet - load levelset with 1 random level
 *    + On missing Skin - load 1-pixel skin
 *    + On error of loading level? - load random level (if random fails, we can relax and crush freely :) )
 *    + Add button "After load levelset open select of level" into ChooseLevelSet form
 *    + Add button "Select level set" into ChooseLevel form 
 *    + Combine Show all with Show unsolved on ChooseLevel into 1 checkable item
 *    + Why catalog Saves [removed]
 *  -   On loading position/solution from another levelset/level - ask about loading exact level
 *  . + Menu->Menu item for loading only positions - with loading exact level
 *    + Change LevelSet.lst to ini (or json)
 *    + Add manual comments for levelsets for levelsetlist view
 *  . . Tutorial level set (with SokobanCompact specific features)
 *    + Set markers during load (on BoxChanges)
 *    + Indication, that level is solved
 *  . . Indication of game-deadlock
 *    +  | Fast check (Check for static-deadlock by analyzing surrounding of pushed box - OK for levels without exceeding boxes, fast, not count static-deadlocks for exceeding boxes, not count initial static-deadlocks)
 *  -    | Full check (Check whole level for statically-deadlocked boxed                - work OK for any levels, but may be very slow for large levels)
 *  .   Indication of full redo length [now - in contextmenu of statusbar]
 *    + Fast switch toolbar buttons by special button (play set - undo/redo, levels set - next/prev, next/prev unsolved)
 *    + Button for select level from list [selector=set]
 *    + Button for select levelset [selector=set]
 *    + Button for load position [selector=level]
 *    + Button for save position [selector=level]
 *    + Add cell-deadlock picture to all skins
 *    + Option to limit box-move-tree with cell-deadlocks
 *    + Option to autocalc cell-deadlocks on load of level
 *    X Turn off autocalc cell-deadlocks for levels "boxes>goals"
 *    + Modify "limit box-move-tree with cell-deadlocks" to allow push exceeding boxes into cell-deadlocks
 *    + Update number of exceeding boxes after calc cell-deadlocks (some boxes can be initially on cell-deadlocks)
 *  -?  Option to block placing box into cell-deadlocks by buttons
 *  -   Option to warn on placing box into cell-deadlock (use precalculated deadlock ?? for cells and neighborhood of box) (calc deadlocked boxes, warn only if "num of boxes"-"num of deadlocked"<"num-of-targets")
 *    + Move iMinMove into settings
 *    + Next/prev level are cycled (as unsolved next/prev)
 *    + Set background color
 *  -   If new move = last undoned move, then not flush redo stack
 *  ?   Inform user on NothingToDo rv?
 *  -   Use sLine.TrimEnd().Length during loading of file to skip spaces in end of lines
 *  ?   Migrate to new record format
 *    + Update copyright in About to use current year or 2009
 *    + Group standard skins for auto-size
 *    + Limit autosize with minimal size
 *    + Change warning dialog on LevelSetList into auto initial scan
 *  . . Prevent device from going sleep (see bug below with exiting from suspend)
 *    + Load skinset
 *  . + Zoom out/in, quick-selection skinsize from skinset by menu (+ not recenter - try to keep center of screen)
 *    + Random level
 *    + Random unsolved level
 *    + Settings (time anti suspend, autosize, limit autosize)
 *    + Settings for animation speed (+change settings)
 *  -?  Stop/continue replay
 *    + Background image
 *  -3  Option to update status bar (moves and pushes counters) during animation
 *  -3  quicksave slot
 *    + After loading position - redo it forward to check all pushes for *-deadlocks
 *  -   Add filter for loading files - to shrink number of viewed files
 *    + Extend FormSettings to allow screen keyboard not overlap controls
 *  -   Option: Auto GroupUndo if game-deadlock is detected
 *  -   More Backgrounds
 *    + Move loading and saving position into FormMain functions, in SokobanGame should left only parsing/combining position
 *  -   Option to ask about unsaved position on exit or changing level
 *  -   Debug button to switch Backgrounds for testing them
 *    + Autoname levels, if no name in levelset file
 *  -   Calc time of solving level (millisec-s/ticks of playing level, saved into position)
 *    + Disply somewhere battery Charge
 *  - . Option: turn off level scrolling (if it is unnecessary?)
 *    + AutoZoom only for usefull cells of level (ignore background and walls)
 *  -   No highlight (or different highlight), if clicked box cannot be moved
 *  -   Intergrate tutorial mode (use tutorial level set)
 *    + v-Scroll for ShowInfo form
 *  -   Option: next/prev (both unsolved and all) is cycled (otherwise - not cycled)
 *  -   Group options by type ("powersaving" etc.)
 *    + Debug button to show total number of all known levels
 *
 *    + Remove "Loaded level"
 *  -?  Control backlight
 *    + Minimize button
 *    + Calc deadlocks in separate thread if level is larger than... 500 cells?  (_4deadlock: 3692, 3420, 3589, 3571, 1458 (146) ) / (sasquatch6 last levels (number of available):  26x26 799 (400), 27x27 1416 (545), 30x30 1719 (593), 33x33 1907 (609), 35x35 3738 (841)
 *    + Change cell-deadlocks calculation: add forward tree from starting box positions, allowed cell - only where both trees reaches
 *  - . Log all operations
 *  -?  Total list of levels
 *  -   Highlight boxes on clicks - blue for unreacable, red for unmovable
 *  -   Contrast skin for playing on sunlight
 *    + Autosave positions for all levels - in separate file *.up
 *  -   Counting number of clicks and button presses, including undo/redo - into solving statistics
 *        for each level : ms thinking, clicks, buttons pressed, moves total forward, moves total backw, leavings, box pushes (by 2 clicks), undos, redos, restarts, saves, loads
 *    + Delimiters into list of levelsets
 *    + Mask for loading levels - to not see positions for 70th levels while loading for 7th
 *  -   Option to turnoff 1-click travel
 *    + Show total number of levels in position-stat showing (menu)
 *    + Show filename in levelset info
 *  -   Graphical comparison of levelsets
 *  -   Text editor for position (rotate, mirror, copy, paste, cut e.t.c.)
 *    + All unknown lines of text in levelset file before first level - count as comment, if comment-comment-end pair not yet found (as found, all other is ignored)
 *  -   Option: random background on levelswitch (only on solve?)
 *  -   Buttons for move player (4/5/9) in the corner (1/2/3/4) of windows
 *  -   On loading levelset, "::" - ignored
 *  -   On loading levelset, allow "Name:" for level title
 * 
 * 
 *      OPTIMIZATION
 *  .   Optimize calc of cell-deadlocks
 *  -   Remove LoadTxtLevelSet from LevelSetList.UpdateList for known levelsets - number of levels should not change
 *  -   AssignRecordFile for LevelSetList.UpdateList - not check new solution against existed, just mark levels as solved
 * 
 *      ESTIMATION MODE
 *  -   backward mode
 *  -   - start (where to get boxes - from targets of)
 *  - !   - from finish (put player)
 *  -     - from other estimation start
 *  -     - from custom position (put boxes and player)
 *  -   - finish (where to get targets - from boxes of)
 *  - !   - to current
 *  - !   - to start
 *  -     - to other estimation end (choose estimation file)   
 *  -   - on saving - reached position is saved as start, moves inverted
 *  -   - on loading - start position from file advanced with moves, result will be start position of estimation
 *  -   forward mode
 *  -   - start (where to get boxes)
 *  -     - from other estimation start (choose estimation file)
 *  -     - from custom position (put boxes and player)
 *  -   - finish (where to get targets)
 *  -     - to finish
 *  -     - to other estimation start (choose estimation file)
 *  -   - on saving/loading - start position and moves treated as is
 * 
 *  .   icons:
 *      + main
 *      . button selector [rework needed]
 *      + next level
 *      + prev level
 *      + comment for levelset
 *      + select level after load levelset
 *      + select level set
 *      + select level, 
 *      + load position
 *      + save position
 *    X - new cell-deadlock picture "red X on floor"
 *    
 *    
 *      BUGS:
 *    + bug: last level of LevelSet is not loaded
 *    + bug: with highlighting possiblie-box and de-highlighting on move e.t.c.
 *    + bug on clicking level with opened menu
 *    + additional moves on reaching box on box-push-by-two-clicks (seems, that subroutine use initial-path-to-box to incorrect cell) - check on Aenigma-#42
 *    + on large screens (ex. 640x480) stats on pictureStats are too dense, because font size is larger, that on QVGA
 *    + Crush on build tree if level have connection between empty and background ("simple" levelset, level 7)
 *    X If start is on edge of level (very last or very first row/column) path finding failes (as edge of level is marked MT_BLOCKED on pathfinding) [DO NOT WANT TO FIX - FIX LEVELS]
 *    X If start is on edge of level (very last or very first row/column) level finishing fails [DO NOT WANT TO FIX - FIX LEVELS]
 *    + Num of solved levels not updated for current levelset on select-levelset list
 *    + Tooltips for buttons are not changes during switching selector (WM/.NET bug) [workaround added]
 *    + LEVEL NOT ENDED IF NUM BOX!=NUM TARGETS (bug was on ReAnalyze and goes from very start of this function)
 *  ?   What to do if level do not contain player or contain several players?
 *    + What to do if no levelset list and no standard levelset is found? [gen randomlevel]
 *    + If press "ok" in header of Settings form - settings not saved (check on other forms also)
 *  -   After exiting from Suspend (by button) - time of prevent suspending is not updated
 *      CheckForDeadlocks (???)
 *    + additional moves before reaching box in exact location on box-push-by-two-clicks (check by sasq6 level 1) - wrong location of player is used for finding first way to box
 *    + bug on zig-zag static-deadlock (check on microkosmos.txt level 21)
 *   ?+ On level solving boxmovetree is not flushed, that is affects next level first clicks
 *    + Crush (boxxle1 level 35) on manual CaclDeadlock after automatic
 *  -   After loading autosaved position - "- __ms Redo" displayed
 *  -   Crush if LogAllAction turned on via settings
 *  -   after load unsaved position - move stats do not updated
 *  
 *    
 *      MAY BE:
 *    + Standard statistics of position
 *  -   Loading logo (?)
 *  -   Set marker into Undo-stack during playing by arrows-buttons on box change only
 *  -   Hide nonmodal message after a while
 *    + (!!) Highlight forbidden cells
 *  -   (!!) Highlight all posible box pushes / with using forbidden cells list
 *  -   (!!) Limit box-move-tree with forbidden cells
 *    w Add option to rotate level or mirror (done for Level, but required two filter for action and undo/redo) - only if requested by users
 *  ?   Choose level with graphical previews
 *  -   Supaplex-style game (levels should solved 1-by-1 from 1st in levelset, but 2 levels allowed to skip)
 *  -   My own stats: 
 *       - Clockwise turns (how many turns like ur/rd/dl/lu)
 *       - Counterclockwise turns (how many turn like ru/ul/ld/dr)
 *       - Steps on targets (how many times pusher move to target)
 *       - Pushes on targets (how many times box was pushed to target)
 *  -?? Thinking marks (mark on level, where you plan to put boxes)
 *  -   bForceRecount for IsLevelCompleted
 *  -   Option to remove unnecessary moves (i.e "ud" automatically removed from undo stack as meaningless)
 *  -   Fix graphic on Win32
 *  -   Show a lowerbound of the solution length  (A program can calculate the distance of every box to the nearest goal. While doing this it is presumed only one box is on the board (hence the box does not need to be pushed around other boxes). The sum of all these box distances to their goals results in a lowerbound - no matter how the boxes are pushed to their goals the level can never be solved with fewer than the calculated number of pushes. )
 *  -   save position on exit
 *  -   background-image changer - on change level and on restart
 *  -   recent levels history
 *  -   copy/paste moves
 *  -   move player through boxes
 *  -   l3u2 format - loading and saving
 *  -   Option: Check box config for static-deadlock before box-pushing and do not push if deadlock will occur
 *  -   Skin generator for any size (by drawing on bitmap)
 *  -   Try to optimize controls - to play by fingers
 * 
 *    
 *      CODE:
 *    + Class SokoGame inherits from SokoLevel and adds functions of SokoPosition, so FormMain will call SokoGame, that holds all data about current game
 *    + Add inihold for settings and positions
 *    + All file reading/writing should be inside try-catch protectors
 *    + Move all small classes into 1 .cs
 *    + add "summary" comments to all fields, methods and classes
 *  . + comment code of all complex methods (building trees and search path on them)
 *    + move all code from interface-invoked events into Action.. methods
 *  -   revert all public fields to private and create all methods to use their values (?)
 *    + set ToLower() into all getting filenames
 *    + remove unnecessary "this."
 *    + remove unnecessary "using"
 *    + change OpenFile to use DialogResult
 *  -   Use FxCop
 *  -   Use ReSharper
 *  -   Change uRect* mess into array with enum-ed index
 *  -   Google Coding convention
 *  -   !!NOTE!!  - notes for fix
 * 
 * 
 *      LEVEL DATABASE
 *      1) Hash of each level, independent from additional unreachable walls and orientation of level, but depends on all changes on reachable area
 *      2) Lookup for similiar level - levels with different hash, but similiar configuration of targets, walls, boxes and so on  - if coridors between level part are prolonged, start coridor changed ...
 * 
 *      Hash is MD5 of aligned, striped level. Calculation procedure
 *      1) Stripe unreachable
 *       - wave algorith from start point, passing boxes and targets, but not walls
 *       - all cell, unreached by wave, are converted to walls
 *       - unnecessary wall removed, until minumum border of each level side will be 1 wall
 *      2) Start point independent
 *       - wave algorith from start point, passing targets, but not boxes and walls
 *       - all cell, reached by wave, are converted to start locations (yes, ALL CELLS!)
 *    ? 3) Aligning - rotating and mirroring
 *    ?  - if width should be larger (or equal) to height
 *    ?  - each quarter of level is weighted
 *    ?    - quarter is one of 4 level
 *    ?    - if level dimension is odd, middle cell/row are included into both quarter  (i.e. quarter overlaps)
 *    ?    - if level dimension is even, cells/rows are diveded into two quarter equally (quarter not overlaps)
 *    ?    - weight is the summ of coordinates of cells, belongs to "weigth metric"
 *    ?    - first weigth metric is all non-wall cells
 *    ?    - second weigth metric is boxes
 *    ?    - third weigth metric is targets
 *    ?  - bottom-right quarter should have maximum weight, if all quarter weights are equal - chosed other weigth metric
 *    ?  - bottom-left quarter should have weight above top-right
 *    ?  ....
 * 
 *       3) Calculating 8 hashes
 *         - hash is MD5 (16 bytes) of ASCII string of text level format
 *         - # - wall, @ - player, $ - box, . - target, * - box on target, + - player on target, " " - empty floor
 *         - line delimiter is CR+LF (two bytes: 0x0D,0x0A), inserted after each line, including last one
 *       4) Final hash is the largest hash among all 8
 *         - largest are choosen by string comparison (from start to end) of hexadecimal representation with following rule: F>E>..>A>9>...>0
 * 
 * 
 *      #####                ###################
 *      #   #                #####   ###########
 *      #$  #                #####$  ###########
 *    ###  $######           #####  $###########
 *    #  $ $ # $.#           ###  $ $@##########
 *  ### # ## ##########   -> ### # ##@##########
 *  #   # ## #####  ..#      #   # ##@#####@@++#
 *  # $  $          ..#      # $  $@@@@@@@@@@++#
 *  ##### ### #@##  ..#      #####@###@#@##@@++#
 *      #     #########      #####@@@@@#########
 *      #######              ###################
 * 
 * 
 */



using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SokobanCompact
{
    /// <summary>Main class of the game</summary>
    public partial class formMain : Form
    {
        //################################################################################################################
        //Fields

        ///<summary>Assembly of current process</summary>
        private readonly System.Reflection.Assembly hExecAssem;

        ///<summary>Name of configuration file, that holds all persistant options</summary>
        private const string sConfigFile = "Sokoban.cfg";
        ///<summary>Name of configuration file, that holds all persistant options</summary>
        private const string sLevelSetList = "LevelSet.lst";
        ///<summary>Name of log file</summary>
        private const string sLogFile = "Sokoban_log.txt";

        ///<summary>Folder of application starts</summary>
        private readonly string sApplicationDirectory;

        ///<summary>Folder with all main levelsets</summary>
        private readonly string sLevelsDirectory;
        ///<summary>Folder with all states of levelsets solutions and positions</summary>
        private readonly string sSolutionsDirectory;

        //<summary>Folder with all positions saves</summary>
        //private string sSavesDirectory;

        ///<summary>Folder with all skins</summary>
        private readonly string sSkinsDirectory;

        ///<summary>Folder with all backgrounds</summary>
        private readonly string sBackgroundsDirectory;

        ///<summary>Skin - images of all level elements</summary>
        private Image hGameSkin;

        ///<summary>SkinSet - list of skins</summary>
        private readonly SkinSet uSkinSet;

        ///<summary>Size of a skin (size of level element in pixels)</summary>
        private int iSkinSize;

        ///<summary>Intermediate render buffer</summary>
        private Bitmap uBackBuffer;

        ///<summary>Rectangle of level element inside skin bitmap</summary>
        private Rectangle uRectWall, uRectEmpty, uRectBox, uRectTarget, uRectBoxOnTarget,
            uRectPlayerDown, uRectPlayerUp, uRectPlayerLeft, uRectPlayerRight,
            uRectHighlightBox, uRectHighlightBoxOnTarget, uRectPossiblyBox, uRectPossiblyBoxOnTarget,
            uRectDeadlock, uRectPossiblyBoxOnDeadlock, uRectBoxOnDeadlock;

        ///<summary>Rectangle of solved/game-deadlock indicator on status bar</summary>
        private Rectangle uRectIndic;
        ///<summary>Rectangle of moves indicator on status bar</summary>
        private Rectangle uRectMoves;
        ///<summary>Rectangle of pushes indicator on status bar</summary>
        private Rectangle uRectPushes;
        ///<summary>Rectangle of non-modal message position on status bar</summary>
        private Rectangle uRectMessage;

        ///<summary>Bottom of rect-that-required-to-redraw (since ClientRect includes place, occupied by pictureStatus)</summary>
        private int iBottomOfForm;


        ///<summary>Current level set</summary>
        private readonly SokobanLevelSet uLevelSet;

        ///<summary>List of all known levelsets</summary>
        private readonly SokobanLevelSetList uLevelSetList;

        //<summary>Backup of currently played level</summary>
        //private SokobanLevel uBackupLevel;

        //<summary>Level, currently played</summary>
        //private SokobanLevel uCurrentLevel;

        ///<summary>Game, currently played - level, position and so on</summary>
        private readonly SokobanGame uGame;

        ///<summary>Drawing offset of level, screen coordinates of top-left corner of level</summary>
        private int iPanX, iPanY;

        ///<summary>Event of mouse click/drag beginning</summary>
        private MouseEventArgs uMouseDown;

        ///<summary>Coordinates of box, highlighted by clicking</summary>
        private int iHighlightedBoxX, iHighlightedBoxY;

        ///<summary>All settings, that stored in config-file</summary>
        private readonly Settings uSetting;

        ///<summary>Last non-modal message (displayed in statusbar)</summary>
        private String sNonModalMessage;

        ///<summary>Form was resized? (if yes, redraw buffer will be recreated)</summary>
        private bool bResize;

        ///<summary>Toolbar mode - level playing (undo/redo e.t.c) or level selection (prev/next level e.t.c)</summary>
        private int iSelectorMode;

        //<summary>Drag-n-drop sensivity</summary>
        //public const int iMinMove = 5;

        ///<summary>When device can go suspend</summary>
        private DateTime dtDeviceCanSuspend;

        //<summary>Image to draw background</summary>
        //private Bitmap hBackgroundImage;

        ///<summary>Image to draw background</summary>
        private Brush hBackgroundBrush;

        ///<summary>"Deadlock!" message showed or not</summary>
        private bool bDeadlockMessage;

        ///<summary>Level is played 1) but not finished, 2) after finishing</summary>
        private bool bHaveUnfinishedPosition;

        ///<summary>Logger object</summary>
        private LogFile uLog;

        ///<summary>Object for background calculations</summary>
        private BackgroundThread uBackgroundCalc;

        ///<summary>Thread of background calculations object</summary>
        private Thread uBackgroundThread;

        //<summary>Lock level scrolling</summary>
        //private bool bScrollLock;

        //################################################################################################################
        //Constructor

        ///<summary>Top-level contructor for SokobanCompact</summary>
        public formMain()
        {
            FunctionResult uRV;

            InitializeComponent();

            hGameSkin = null;
            uBackBuffer = null;
            bDeadlockMessage = false;
            bHaveUnfinishedPosition = false;
            Cursor.Current = Cursors.WaitCursor;//Waiting cursor - while loading levelset and skin

            //Get handle of assembly
            hExecAssem = System.Reflection.Assembly.GetExecutingAssembly();
            //Get file name of assembly
            string sAppFilePath = hExecAssem.GetModules()[0].FullyQualifiedName;
            //Get path only
            sApplicationDirectory = System.IO.Path.GetDirectoryName(sAppFilePath);
            //Add delimiter at the end
            if (!sApplicationDirectory.EndsWith(@"\"))
                sApplicationDirectory += @"\";

            //Calc paths for folders
            //sSavesDirectory = sApplicationDirectory + @"Saves\";
            sLevelsDirectory = sApplicationDirectory + @"Levels\";
            sSolutionsDirectory = sApplicationDirectory + @"Solutions\";
            sSkinsDirectory = sApplicationDirectory + @"Skins\";
            sBackgroundsDirectory = sApplicationDirectory + @"Backgrounds\";

            //Try to create all required folders, to not bother in future
            try
            {
                //System.IO.Directory.CreateDirectory(sSavesDirectory);
                System.IO.Directory.CreateDirectory(sBackgroundsDirectory);
                System.IO.Directory.CreateDirectory(sSkinsDirectory);
                System.IO.Directory.CreateDirectory(sSolutionsDirectory);
                System.IO.Directory.CreateDirectory(sLevelsDirectory);
            }
            catch (System.IO.IOException)
            {}//Dont know that to do, if creation fails


            //Calc rectangles on statusbar
            int iX = 3;


            int iStatusHeight = pictureStatus.Height;//Height of status bar
            Graphics uGr1 = CreateGraphics();//Get graphics of current form
            SizeF uCounterSizes = uGr1.MeasureString(" 0000", Font);//Measure sample string for get actual font sizes
            int iCounterWidth = (int)uCounterSizes.Width;//Width of counter - with of sample string
            int iCounterY0 = (int)(iStatusHeight-uCounterSizes.Height)/2;//Text positions - valign center
            int iSpace = iCounterWidth/10;//Space between fields - 10% if counter width
            uGr1.Dispose();//Release graphics of form

            uRectIndic = new Rectangle(iX, (iStatusHeight-16)/2, 16, 16);//for solved/not-solved indicator
            iX += uRectIndic.Width + iSpace;
            uRectMoves = new Rectangle(iX, iCounterY0, iCounterWidth, 16);//for counter of moves
            iX += uRectMoves.Width + iSpace;
            uRectPushes = new Rectangle(iX, iCounterY0, iCounterWidth, 16);//for counter of pushes
            iX += uRectPushes.Width + iSpace;
            uRectMessage = new Rectangle(iX, iCounterY0, ClientRectangle.Width - iX, 16); //all rest space - for non-modal message

            //Font f1 = this.Font;
            //f1.me
            //this.gr
            //Graphic .MeasureString

            //bScrollLock = false;
            bResize = true;
            iBottomOfForm = pictureStatus.Top;//this done here for perform recenter of level before any redraws

            //Create and load settings
            uSetting = new Settings();
            if (uSetting.Load(sApplicationDirectory + sConfigFile) != FunctionResult.OK)
            {
                //Settings not loaded - first start or failure of file
                uSetting = new Settings();//reset to default, just in case...
            }
            menuScrollLock.Checked = uSetting.bScrollLock;

            if (uSetting.bLogActions)
            {
                uLog = new LogFile();
                uLog.Start(sApplicationDirectory + sLogFile);
                uLog.LogString(ActionID.Start, "Started SokobanCompact; v" + hExecAssem.GetName().Version);
            }

            //Load level set list
            uLevelSetList = new SokobanLevelSetList(sApplicationDirectory + sLevelSetList, sLevelsDirectory, sSolutionsDirectory);
            uLevelSetList.LoadList();

            iSkinSize = 0;

            //Load skinset
            uSkinSet = new SkinSet();
            //uSkinSet.Load(sSkinsDirectory + "\\" + "oq.sks");//TODO: move skinset name into Settings
            uSkinSet.Load(sSkinsDirectory + uSetting.sSkinSet);

            if (!uSetting.bAutosize)
            {   //No autosize? Load skin now
                uRV = LoadSkin(sSkinsDirectory + uSetting.sSkin);
                if (iSkinSize == 0)
                {
                    LogSimpleLine(ActionID.ShowDialog, "Error; Failed to load skin, null skin created; " + uSetting.sSkin+"; "+uRV.ToString());
                    MessageBox.Show("Failed to load skin '" + uSetting.sSkin + "' \r\nNull skin will be loaded, " + uRV.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    GenNullSkin();
                }
            }

            UpdateBackground();//Create background according to settings

            //Create game
            uGame = new SokobanGame();

            //THREAD
            uBackgroundCalc = new BackgroundThread(BackgroundFinished);//Create thread for background

            iSelectorMode = 0;//Selector initialy in "play" mode
            SetToolBarMode();//Refresh selector

            //Load levelset
            uLevelSet = new SokobanLevelSet();
            uRV = uLevelSetList.LoadLevelSet(uLevelSet, uSetting.sLastLevelSet);
            if (uRV != FunctionResult.OK)
            {   //Something happens with levelset file
                LogSimpleLine(ActionID.ShowDialog, "Error; Failed to load levelset, random will be generated; " + uSetting.sLastLevelSet+"; "+uRV.ToString()); 
                MessageBox.Show("Unable to load LevelSet '" + uSetting.sLastLevelSet + "', result: " + uRV.ToString() + "\r\nRandom level will be loaded.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                uLevelSetList.GenNullLevelSet(uLevelSet);//Generate levelset with 1 random level
                uSetting.iLastLevelPlayed = 0;//Res
            }

            //Start last played level
            uRV = LoadLevel(uGame, uSetting.iLastLevelPlayed);
            if (uRV == FunctionResult.OK)
            {   //Loaded successfully
                AfterLoadLevel();
            }
            else
            {   //Level not loaded (only variant - FunctionResult.OutOfLevelSet)
                LogSimpleLine(ActionID.ShowDialog, "Error; Failed to load level, random will be choosen; " + uSetting.iLastLevelPlayed.ToString() + "; " + uRV.ToString());
                MessageBox.Show("Unable to load level " + uSetting.iLastLevelPlayed.ToString() + ", result: " + uRV.ToString() + "\r\nRandom level will be selected.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                ActionRandLevel();
            }

            if (iSkinSize < 1)
            {
                LogSimpleLine(ActionID.ShowDialog, "Error; No skin loaded, null skin created"); 
                MessageBox.Show("Failed to load skin\r\nNull skin will be loaded", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                GenNullSkin();
                RecenterLevel();
            }

            NonModalMessage(uLevelSet.sTitle + ", " + uGame.sTitle);


            
            Cursor.Current = Cursors.Default;//remove wait cursor

        }

        //################################################################################################################
        //Internal methods

        ///<summary>Load skin from file (filename with path)</summary>
        private FunctionResult LoadSkin(string sFilename)
        {
            Bitmap hNewBitmap;
            try
            {
                hNewBitmap = new Bitmap(sFilename);
            }
            catch
            {
                return FunctionResult.ErrorOnReadingFile;
            }

            hGameSkin = hNewBitmap;

            iSkinSize = hGameSkin.Height / 3;//By default assuming 3 rows of elements in file

            //Create rectangles of all elements
            uRectEmpty = new Rectangle(0, 0, iSkinSize, iSkinSize);
            uRectBox = new Rectangle(iSkinSize * 2, 0, iSkinSize, iSkinSize);
            uRectWall = new Rectangle(iSkinSize, iSkinSize, iSkinSize, iSkinSize);
            uRectTarget = new Rectangle(0, iSkinSize * 2, iSkinSize, iSkinSize);
            uRectBoxOnTarget = new Rectangle(iSkinSize * 2, iSkinSize * 2, iSkinSize, iSkinSize);
            uRectPlayerDown = new Rectangle(iSkinSize, iSkinSize * 2, iSkinSize, iSkinSize);
            uRectPlayerUp = new Rectangle(iSkinSize, 0, iSkinSize, iSkinSize);
            uRectPlayerLeft = new Rectangle(0, iSkinSize, iSkinSize, iSkinSize);
            uRectPlayerRight = new Rectangle(iSkinSize * 2, iSkinSize, iSkinSize, iSkinSize);

            uRectHighlightBox = new Rectangle(iSkinSize * 3, 0, iSkinSize, iSkinSize);
            uRectHighlightBoxOnTarget = new Rectangle(iSkinSize * 4, 0, iSkinSize, iSkinSize);
            uRectPossiblyBox = new Rectangle(iSkinSize * 3, iSkinSize, iSkinSize, iSkinSize);
            uRectPossiblyBoxOnTarget = new Rectangle(iSkinSize * 3, iSkinSize * 2, iSkinSize, iSkinSize);

            uRectDeadlock = new Rectangle(iSkinSize * 4, iSkinSize, iSkinSize, iSkinSize);
            uRectPossiblyBoxOnDeadlock = new Rectangle(iSkinSize * 4, iSkinSize * 2, iSkinSize, iSkinSize);
            uRectBoxOnDeadlock = new Rectangle(iSkinSize * 5, 0, iSkinSize, iSkinSize);
            return FunctionResult.OK;
        }

        ///<summary>Generate minimal skin, 1x1 pixel</summary>
        private FunctionResult GenNullSkin()
        {
            Bitmap uB1 = new Bitmap(3, 3);
            uB1.SetPixel(0, 0, Color.Black);
            uB1.SetPixel(2, 0, Color.Brown);
            uB1.SetPixel(1, 1, Color.Gray);
            uB1.SetPixel(0, 2, Color.Green);
            uB1.SetPixel(2, 2, Color.GreenYellow);
            uB1.SetPixel(1, 2, Color.Blue);
            hGameSkin = uB1;
            //Graphics ug1 = Graphics.FromImage(hGameSkin);
            //iSkinSize = 1;
            //ug1.DrawEllipse
            iSkinSize = 1;

            uRectEmpty = new Rectangle(0, 0, 1, 1);
            uRectBox = new Rectangle(2, 0, 1, 1);
            uRectWall = new Rectangle(1, 1, 1, 1);
            uRectTarget = new Rectangle(0, 2, 1, 1);
            uRectBoxOnTarget = new Rectangle(2, 2, 1, 1);
            uRectPlayerDown = new Rectangle(1, 2, 1, 1);
            uRectPlayerUp = uRectPlayerDown;
            uRectPlayerLeft = uRectPlayerDown;
            uRectPlayerRight = uRectPlayerDown;

            uRectHighlightBox = uRectBox;
            uRectHighlightBoxOnTarget = uRectBoxOnTarget;
            uRectPossiblyBox = uRectEmpty;
            uRectPossiblyBoxOnTarget = uRectTarget;

            uRectDeadlock = uRectEmpty;
            uRectPossiblyBoxOnDeadlock = uRectEmpty;
            uRectBoxOnDeadlock = uRectBox;
            return FunctionResult.OK;
        }


        ///<summary>Set image for background (filename with path)</summary>
        private FunctionResult SetBackgroundImage(string sFilename)
        {
            Bitmap hNewBitmap;
            try
            {
                hNewBitmap = new Bitmap(sFilename);
            }
            catch
            {
                return FunctionResult.ErrorOnReadingFile;
            }
            hBackgroundBrush = new TextureBrush(hNewBitmap);
            return FunctionResult.OK;
        }

        ///<summary>(Re)Create background according to settings</summary>
        private FunctionResult UpdateBackground()
        {
            if (uSetting.sBackgroundImageFile.Length > 0)
                if (SetBackgroundImage(sBackgroundsDirectory + uSetting.sBackgroundImageFile) == FunctionResult.OK)
                    return FunctionResult.OK; //Sucessfully loaded - skip SolidBrush
            hBackgroundBrush = new SolidBrush(Color.FromArgb(uSetting.iBackgroundColor));
            //Texture not loaded - use SolidBrush
            uSetting.sBackgroundImageFile = ""; //Clean filename, to skip trying to load next launches
            return FunctionResult.OK;
        }

        /*
        /// <summary>Load level set from Levels sub folder only (filename)</summary>
        private void LoadLevelSet(string sFileName)
        {
            uLevelSet = new SokobanLevelSet();
            uLevelSet.LoadTxtLevelSet(sLevelsDirectory + sFileName);
            uLevelSet.sID = SokobanLevelSet.FileName2sID(sFileName);
            uLevelSet.AssignRecordFile(sSolutionsDirectory + uLevelSet.sID + ".rec");

            uSetting.sLastLevelSet = sFileName;

            uLevelSetList.FindLevelSet(sFileName);

            //uLevelSet.LoadLevel(uCurrentLevel, 0);//__what if not loaded?
            //AfterLoadLevel();
        }
         */

        ///<summary>Switch toolbar from one mode to another</summary>
        private void SetToolBarMode()
        {
            bool bLevelButtons = false;
            bool bSetButtons = false;

            if (iSelectorMode == 0)
            {
                bLevelButtons = true;
            }
            else
            {
                bSetButtons = true;
            }

            toolBarButtonNextLevel.Visible = bSetButtons;
            toolBarButtonPrevLevel.Visible = bSetButtons;
            toolBarButtonNextUnsolved.Visible = bSetButtons;
            toolBarButtonPrevUnsolved.Visible = bSetButtons;
            toolBarButtonSelLevel.Visible = bSetButtons;
            toolBarButtonSelLevelSet.Visible = bSetButtons;

            toolBarButtonUndo.Visible = bLevelButtons;
            toolBarButtonRedo.Visible = bLevelButtons;
            //toolBarButtonFullRedo.Visible = bLevelButtons;
            //toolBarButtonFullUndo.Visible = bLevelButtons;
            toolBarButtonGroupRedo.Visible = bLevelButtons;
            toolBarButtonGroupUndo.Visible = bLevelButtons;
            toolBarButtonSavePos.Visible = bLevelButtons;
            toolBarButtonLoadPos.Visible = bLevelButtons;

            //THIS eliminate bug with not updating tooltips for toolbar buttons after changing their visibility (if not do, tooltip shows incorrectly - for hidden buttons and so on)
            toolBarButtonNextLevel.ToolTipText = toolBarButtonNextLevel.ToolTipText;
        }

        ///<summary>If level completed - display messages and move to the next</summary>
        private void CheckLevelCompletition()
        {
            if (uGame.IsLevelCompleted(false))//Check internal flag of level
            {
                DialogResult uDR;//Object for asking user
                bool bAskRecordName = uSetting.bAskRecordName;//Get option of asking about record name
                //string sMessage, sTitle;

                LogSimpleLine(ActionID.LevelSolved, "Level solved; " + (uLevelSet.GetCurrentLevel() + 1).ToString() + "; " + uLevelSet.sFileName);
                NonModalMessage("Level " + (uLevelSet.GetCurrentLevel() + 1).ToString() + " completed");//"+1" - to numerate level from 1

                uGame.RecalcStats();//This recalc stats, as not all stats calculated automatically

                //Default name of record: player, stats, data-time
                uGame.uStats.sName = uSetting.sPlayerName + ", " + uGame.uStats.ToString() + ", " + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");

                //Build all message for box
                const string sTitle = "Congratulations";
                string sMessage = "You solve level " + uGame.sTitle + "!\r\n";
                sMessage += "Your solution stats is: ";
                sMessage += uGame.uStats.ToString() + ".\r\n";
                SolutionFlags uFlags = uGame.EstimateNewSolution(uGame);//Check new solution for rectodrs
                if ((uFlags & SolutionFlags.FirstSolution) != 0)
                {
                    //First solution of this level
                    if (!uSetting.bAskSavingFirstSolution)
                    {
                        uDR = DialogResult.Yes;
                        bAskRecordName = false;
                        goto lSilentSave;
                    }
                    sMessage += "This is first solution of this level!\r\n";
                }
                else if (uFlags == SolutionFlags.Nothing)
                {
                    //No records achived
                    sMessage += "- No new records achived.\r\n";
                }
                else
                {
                    //Improved one-two records
                    if ((uFlags & SolutionFlags.BestMoves) != 0)
                        sMessage += "* This is new best-moves solution!\r\n";
                    if ((uFlags & SolutionFlags.BestPushes) != 0)
                        sMessage += "* This is new best-pushes solution!\r\n";
                }
                sMessage += "Do you want to save your solution?";

                LogSimpleLine(ActionID.ShowDialog, "Ask; Save new solution");
                //Ask user
                uDR = MessageBox.Show(sMessage, sTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            lSilentSave:
                switch (uDR)
                {
                    case DialogResult.Yes://User answer "yes" - save solution and advance to next level

                        if (bAskRecordName)//if allowed by settings
                        {//Ask name for new record
                            InputBox uRecName = new InputBox();
                            LogSimpleLine(ActionID.ShowForm, "Ask; Record name; " + uGame.uStats.sName);
                            if (uRecName.AskUser(uGame.uStats.sName, "Save record", "Please set record name:") != DialogResult.OK)
                                break;
                            uGame.uStats.sName = uRecName.GetResult();
                        }
                        if (uLevelSet.SaveNewRecord(uGame) != FunctionResult.OK) //Try to save record
                        {//Some problem with saving record
                            LogSimpleLine(ActionID.ShowDialog, "Info; Record was not saved, keep current level");
                            MessageBox.Show("Record was not saved.\r\nKeep current level...", "ERROR");
                            break;
                        }
                        uLevelSetList.UpdateSolved(uLevelSet.GetNumOfSolved());//Update number of solved levels in current levelset
                        //if (uGame.SavePosition(sSolutionsDirectory + AutoNamePosition() + ".sol", uLevelSet.sFileName, uLevelSet.GetCurrentLevel(), uSetting.sPlayerName) != 0) //Try to save solution
                        if (SavePosition(sSolutionsDirectory + AutoNamePosition() + ".sol") != FunctionResult.OK) //Try to save solution
                        {//Some problem with saving solution
                            LogSimpleLine(ActionID.ShowDialog, "Info; Solution was not saved, keep current level");
                            MessageBox.Show("Solution was not saved.\r\nKeep current level...", "ERROR");
                            break;
                        }
                        //break;
                        goto lNextLevel;
                    case DialogResult.No://User answer "no" - do not save solution but advance to next level
                    lNextLevel:
                        //bHaveUnfinishedPosition = false;//Solution saved or ignored - anyway position in ignored
                        KillAutosavedPosition();//Solution saved or ignored - autosaved position should be killed
                        //ActionNextLevel();
                        ActionNextUnsolved();
                        break;
                    default: //User answer "cancel - keep this level and do not save anything
                        break;
                }
            }
        }

        ///<summary>Save all setting into file</summary>
        public void SaveSettings()
        {
            uSetting.Save(sApplicationDirectory + sConfigFile);
        }

        ///<summary>Update game after changing or reloading level</summary>
        private void AfterLoadLevel()
        {
            uSetting.iLastLevelPlayed = uLevelSet.GetCurrentLevel();//number of last player level not saved at-once (only at load levelset, skin... or exit)

            RemoveBoxHighlight();//if some box was highlighted - highlighting should be removed

            ManageDeadlocks();
            //NonModalMessage("Loaded " + uGame.sTitle);
            NonModalMessage(uGame.sTitle);

            LogSimpleLine(ActionID.LoadLevel, "Load level; " + uLevelSet.sFileName + "; " + (uLevelSet.GetCurrentLevel() + 1).ToString());

            if (uSetting.bAutosize)
            {
                ActionAutoSize();//Load skin by using Autosize
            }
            RecenterLevel();//Draw level at center of the screen

            //HERE SHOULD BE LOADING OF UnfinishedPosition
            ActionLoadPosition(2, false);
            bHaveUnfinishedPosition = true;
            
            UpdateAntiSuspend();//Prevent device from suspending - after loading level
        }

        ///<summary>Remove highlighting of box</summary>
        private void RemoveBoxHighlight()
        {
            if (iHighlightedBoxX >= 0)
            {
                iHighlightedBoxX = -1;
                iHighlightedBoxY = -1;
                uGame.InvalidateBoxMoveTree();//Kill box move tree
                RedrawAllScreen();//Redraw level
            }
        }

        ///<summary>Move level to center of screen</summary>
        private void RecenterLevel()
        {

            if (uSetting.bAutosizeUseful)
            {   //Advanced recentering of level
                iPanX = (ClientRectangle.Width - uGame.uUsefulCellsRect.Width * iSkinSize) / 2 - uGame.uUsefulCellsRect.Left * iSkinSize;
                iPanY = (iBottomOfForm - uGame.uUsefulCellsRect.Height * iSkinSize) / 2 - uGame.uUsefulCellsRect.Top * iSkinSize;
            }
            else
            {
                //Center of screen - is half of it's size, center of level - half of it's size
                //To coiside them - subtract first from last 
                iPanX = (ClientRectangle.Width - uGame.iXsize * iSkinSize) / 2;
                iPanY = (iBottomOfForm - uGame.iYsize * iSkinSize) / 2;
            }
            RedrawAllScreen();//And redraw level
        }

        ///<summary>Display non-modal (unimportant) message</summary>
        public void NonModalMessage(string sText)
        {
            sNonModalMessage = sText;//Store message text
            bDeadlockMessage = false;//"Deadlock!" warning was replaced
            pictureStatus.Invalidate();//Ask system to redraw status bar (there non-modal messages are displayed)
        }

        ///<summary>Generate name for position/solution</summary>
        public string AutoNamePosition()
        {
            //date in "yymmdd", "_", time in "hhmmss", "_lev", level number
            return DateTime.Now.ToString("yyMMdd_HHmmss_") + uLevelSet.sID + "_lev" + (uLevelSet.GetCurrentLevel() + 1).ToString();
        }

        ///<summary>Generate name for position/solution</summary>
        public string AutoNameAutoSavePosition()
        {
            //date in "yymmdd", "_", time in "hhmmss", "_lev", level number
            return uLevelSet.sID + "_lev" + (uLevelSet.GetCurrentLevel() + 1).ToString()+".ap";
        }


        ///<summary>Filename filter for displaying only position/solution for current level in levelset</summary>
        public string FilterForPositions()
        {
            return "*" + uLevelSet.sID + "_lev" + (uLevelSet.GetCurrentLevel() + 1).ToString() + ".?o*"; //.?o* - to select .pos and .sol
        }

        /*
         * Redo:
         *   @ $
         * ^^^^^
         * 
         * 
         * Undo:
         * 
         * @ $  
         * ^^^^^
         * 
         */

        ///<summary>Redraw level around player</summary>
        public void RedrawAround()
        {
            //this.Invalidate(new Rectangle(uCurrentLevel.iPlayerX * 16 + iPanX , uCurrentLevel.iPlayerY * 16 + iPanY , 16, 16));//1x1 - debug

            //5x5 - 1100 ms on device
            //this.Invalidate(new Rectangle(uCurrentLevel.iPlayerX * iSkinSize + iPanX - iSkinSize * 2, uCurrentLevel.iPlayerY * iSkinSize + iPanY - iSkinSize * 2, iSkinSize * 5, iSkinSize*5));//5x5 - simpliest

            /*
            //5x1 (. . @ . .) - 600-650 ms on device
            switch (uPos.uLastMove & SokoMove.Direction)
            {
                case SokoMove.Left:
                case SokoMove.Right:
                    Invalidate(new Rectangle(uCurrentLevel.iPlayerX * iSkinSize + iPanX, uCurrentLevel.iPlayerY * iSkinSize + iPanY, iSkinSize * 5, iSkinSize));
                    break;
                case SokoMove.Up:
                case SokoMove.Down:
                    Invalidate(new Rectangle(uCurrentLevel.iPlayerX * iSkinSize + iPanX, uCurrentLevel.iPlayerY * iSkinSize + iPanY - iSkinSize*2, iSkinSize, iSkinSize * 5));
                    break;
            }*/

            //4x1 (. @ . .) - 550-570 ms on device
            //Redraw 4 quads - player, two in action/undo direction, one in opposite direction
            //Experimentaly fastest
            switch (uGame.uLastMove & SokoMove.Direction)
            {
                case SokoMove.Left:
                    Invalidate(new Rectangle((uGame.iPlayerX - 2) * iSkinSize + iPanX, uGame.iPlayerY * iSkinSize + iPanY, iSkinSize * 4, iSkinSize));
                    break;
                case SokoMove.Right:
                    Invalidate(new Rectangle((uGame.iPlayerX - 1) * iSkinSize + iPanX, uGame.iPlayerY * iSkinSize + iPanY, iSkinSize * 4, iSkinSize));
                    break;
                case SokoMove.Up:
                    Invalidate(new Rectangle(uGame.iPlayerX * iSkinSize + iPanX, (uGame.iPlayerY - 2) * iSkinSize + iPanY, iSkinSize, iSkinSize * 4));
                    break;
                case SokoMove.Down:
                    Invalidate(new Rectangle(uGame.iPlayerX * iSkinSize + iPanX, (uGame.iPlayerY - 1) * iSkinSize + iPanY, iSkinSize, iSkinSize * 4));
                    break;
            }
        }

        ///<summary>Redraw whole screen / whole level</summary>
        public void RedrawAllScreen()
        {
            //Ask system to redraw screen
            Invalidate();
        }

        ///<summary>Redraw on paint event</summary>
        private void Redraw(PaintEventArgs e)
        {
            if (bResize)
            {//Form was resized
                LogSimpleLine(ActionID.WindowResized, "Resized; " + ClientRectangle.Width + "; " + ClientRectangle.Height);

                uBackBuffer = null;//forger render buffer

                uRectMessage.Width = ClientRectangle.Width - uRectMessage.Left;//Update width for non-modal messages

                //recalc sizes of form
                if (pictureStatus.Visible)
                {//With status bar
                    iBottomOfForm = pictureStatus.Top;
                }
                else
                {//Without status bar
                    iBottomOfForm = ClientRectangle.Height;
                }
                bResize = false;
            }

            if (uBackBuffer == null)
            {
                //Recreate redraw buffer
                uBackBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
            }
            Rectangle uRedrawRect = e.ClipRectangle;//Get rectangle of redrawed area
            using (Graphics g = Graphics.FromImage(uBackBuffer))
            {
                Rectangle rect1 = uRectEmpty;
                //Random r = new Random();
                //g.Clear(Color.Black); //1270
                //int x1, x2, y1, y2;
                if (uRedrawRect.Bottom > iBottomOfForm) uRedrawRect.Height = iBottomOfForm - uRedrawRect.Top;//Clip redraw with bottom of form (status bar)
                
                //g.FillRectangle(new SolidBrush(Color.Black), uRedrawRect);//Draw background
                //g.FillRectangle(new TextureBrush(hGameSkin), uRedrawRect);
                g.FillRectangle(hBackgroundBrush, uRedrawRect);//Draw background

                //Convert redrawed area from screen pixels into level coordinates
                int x1 = (uRedrawRect.Left - iPanX) / iSkinSize;
                int y1 = (uRedrawRect.Top - iPanY) / iSkinSize;
                int x2 = (uRedrawRect.Right - iPanX) / iSkinSize;
                int y2 = (uRedrawRect.Bottom - iPanY) / iSkinSize;
                if (x1 < 0) x1 = 0;
                if (x1 >= uGame.iXsize) x1 = uGame.iXsize - 1;
                if (x2 < 0) x2 = 0;
                if (x2 >= uGame.iXsize) x2 = uGame.iXsize - 1;
                if (y1 < 0) y1 = 0;
                if (y1 >= uGame.iYsize) y1 = uGame.iYsize - 1;
                if (y2 < 0) y2 = 0;
                if (y2 >= uGame.iYsize) y2 = uGame.iYsize - 1;

                //Cycle on all redrawed cells of level
                for (int x = x1; x <= x2; x++)
                    for (int y = y1; y <= y2; y++)
                    {

                        switch (uGame.GetCell(x, y) & SokoCell.FilterSkipCellDeadlocks) //Select cell type, ignoring cell-deadlock flag
                        {
                            case SokoCell.Background: continue; //Background already drawed
                            case SokoCell.Box:
                                {
                                    if (x == iHighlightedBoxX && y == iHighlightedBoxY)
                                        rect1 = uRectHighlightBox;//For hightlighted box
                                    else
                                    {
                                        if ((uGame.GetCell(x, y) & SokoCell.CellDeadlock) != 0) //If cell is cell-deadlock
                                            rect1 = uRectBoxOnDeadlock;//Cell-deadlocked box
                                        else
                                            rect1 = uRectBox;//Just box
                                    }
                                    break;
                                }
                            case SokoCell.BoxOnTarget:
                                {
                                    rect1 = uRectBoxOnTarget;
                                    if (x == iHighlightedBoxX && y == iHighlightedBoxY) rect1 = uRectHighlightBoxOnTarget;//For hightlighted box on target
                                    break;
                                }
                            case SokoCell.Empty:
                                {
                                    if (uGame.IsCellAchivedByBoxMoveTree(x, y))//If cell is in box move tree
                                        if ((uGame.GetCell(x, y) & SokoCell.CellDeadlock) != 0) //If cell is cell-deadlock
                                            rect1 = uRectPossiblyBoxOnDeadlock;//Box can be placed here, but it is cell-deadlock
                                        else
                                            rect1 = uRectPossiblyBox;//Box can be placed here
                                    else
                                        if ((uGame.GetCell(x, y) & SokoCell.CellDeadlock) != 0) //If cell is cell-deadlock
                                            rect1 = uRectDeadlock;
                                        else
                                            rect1 = uRectEmpty;//Otherwise - draw floor
                                    break;
                                }
                            case SokoCell.Target:
                                {
                                    if (uGame.IsCellAchivedByBoxMoveTree(x, y))//If cell is in box move tree
                                        rect1 = uRectPossiblyBoxOnTarget;//Box can be placed here, to tager
                                    else
                                        rect1 = uRectTarget;//Otherwise - just target
                                    break;
                                }
                            case SokoCell.Wall: rect1 = uRectWall; break;//Wall is always wall
                            case SokoCell.Player:
                            case SokoCell.PlayerOnTarget: //Target below player is not visible
                                switch (uGame.uPlayerDir & SokoMove.Direction)//Get player direction
                                {
                                    case SokoMove.Up:
                                        rect1 = uRectPlayerUp;
                                        break;
                                    case SokoMove.Down:
                                        rect1 = uRectPlayerDown;
                                        break;
                                    case SokoMove.Left:
                                        rect1 = uRectPlayerLeft;
                                        break;
                                    case SokoMove.Right:
                                        rect1 = uRectPlayerRight;
                                        break;
                                }
                                break;
                        }

                        g.DrawImage(hGameSkin, x * iSkinSize + iPanX, y * iSkinSize + iPanY, rect1, GraphicsUnit.Pixel);//Draw selected rectangle from skin to render buffer
                    }
                /*
                if (uRectMessage.IntersectsWith(e.ClipRectangle) && sNonModalMessage.Length>0)
                {
                    SizeF uSizes = g.MeasureString(sNonModalMessage, this.Font);
                    //uSizes.Width;

                    g.DrawString(sNonModalMessage, this.Font, new SolidBrush(Color.White), uRectMessage.X + (uRectMessage.Width - uSizes.Width) / 2, uRectMessage.Y);
                }*/
            }
            //e.Graphics.DrawImage(backBuffer, 0, 0);
            //e.Graphics.DrawImage(backBuffer, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            
            e.Graphics.DrawImage(uBackBuffer, uRedrawRect, uRedrawRect, GraphicsUnit.Pixel);//Draw redrawed part of render buffer to screen

        }

        ///<summary>Redraw status bar - moves/pushed counters and non-modal message</summary>
        private void UpdateStatus()
        {
            pictureStatus.Invalidate();//Just inform system about requirement to redraw
        }

        ///<summary>Prolong antisuspend keep alive - called if user do something</summary>
        private void UpdateAntiSuspend()
        {
            dtDeviceCanSuspend = DateTime.Now.AddMinutes(uSetting.iKeepAliveMinutes);
            //dtDeviceCanSuspend = DateTime.Now.AddSeconds(uSetting.iKeepAliveMinutes);//!!DEBUG
            //if (uSetting.bAdditionalMessages)//TODO: REMOVE THIS DEBUG INFO
            //    NonModalMessage("Will sleep at " + dtDeviceCanSuspend.ToString("HH:mm:ss"));
        }

        //Check position for game-deadlock
        private void CheckForDeadlocks()
        {
            if (uGame.FastCheckForDeadlocks() == FunctionResult.GameDeadlock) //Check for game-deadlock
            {
                if (!bDeadlockMessage)
                    LogSimpleLine(ActionID.GameDeadlock, "Game-deadlock");
                NonModalMessage("Deadlock!");//Show warning for user 
                bDeadlockMessage = true;//This helps remove warning (see below)
            }
            else
            {
                if (bDeadlockMessage) //"Deadlock!" warning is still shown
                    NonModalMessage("");//Show "relax" for user
            }
        }

        ///<summary>Save current position into file (filename, levelset name, level number, player name)
        ///!Recalc of stats should be done before calling</summary>
        public FunctionResult SavePosition(string sFileName)
        {
            System.IO.StreamWriter hWrite;

            try //Protection from file operations errors
            {
                hWrite = new System.IO.StreamWriter(sFileName, false);//Open file with overwrite
                IniHold.IniFile uIni = new IniHold.IniFile();//IniHold object - to store position as ini-file
                uIni.SetWriter(hWrite);//Transmit file-writer into inihold

                //Save level info, player info, statistics of positions
                uIni.SaveItem("Title", uGame.uStats.sName);
                uIni.SaveItem("LevelSet", uLevelSet.sFileName);
                uIni.SaveItem("Level", (uLevelSet.GetCurrentLevel()+1).ToString());//Saved level number is 1-based
                uIni.SaveItem("Player", uSetting.sPlayerName);
                uIni.SaveItem("Moves", uGame.uStats.iMoves.ToString());
                uIni.SaveItem("Pushes", uGame.uStats.iPushes.ToString());
                uIni.SaveItem("LinearMoves", uGame.uStats.iLinearMoves.ToString());
                uIni.SaveItem("LinearPushes", uGame.uStats.iLinearPushes.ToString());
                uIni.SaveItem("PushSessions", uGame.uStats.iPushSessions.ToString());
                uIni.SaveItem("BoxChanges", uGame.uStats.iBoxChanges.ToString());

                //And position
                uIni.SaveItem("Position", uGame.GetPositionLuRd());

                hWrite.Close();//Close file
                return FunctionResult.OK;
            }
            catch
            {
                return FunctionResult.ErrorOnWritingFile;//On error - return indication
            }
        }

        void ZoomTo(int iNewSkinSize)
        {
            if (iNewSkinSize <= 0)
                return;
            string sSkin = uSkinSet.GetSkin(uSkinSet.GetNearestSize(iNewSkinSize)); //Get skin of size, that are larger
            if (sSkin == null)
                return;

            if (LoadSkin(sSkinsDirectory + sSkin) != FunctionResult.OK) //Load skin
                return; //Exit on errors
            LogSimpleLine(ActionID.SkinSizeChanged, "ZoomTo; " + iSkinSize.ToString());
            uSetting.sSkin = sSkin;//Store skin name in settings

            RecenterLevel();//Redraw level with recentering
        }

        private void LogSimpleLine(ActionID ActionCode, string sLine)
        {
            if (uSetting.bLogActions)
                uLog.LogString(ActionCode, sLine);
        }

        private void AutosavePosition()
        {
            if (bHaveUnfinishedPosition)
            {
                if (uLevelSet.sID == SokobanLevelSet.sNullName) //do not save position for random level/set
                    return;
                uGame.uStats.sName = "Autosaved position";
                SavePosition(sSolutionsDirectory + AutoNameAutoSavePosition());
            }
        }

        private void KillAutosavedPosition()
        {
            bHaveUnfinishedPosition = false;
            try
            {
                System.IO.File.Delete(sSolutionsDirectory + AutoNameAutoSavePosition());
            }
            catch { };
        }

        private FunctionResult LoadLevel(SokobanGame uDstLevel, int iLevelNumber)
        {
            AutosavePosition();
            return uLevelSet.LoadLevel(uDstLevel, iLevelNumber);
        }

        ///<summary>Called by thread on finishing background calc</summary>
        public void BackgroundFinished(int iInt)
        {
            //NonModalMessage("Callback returns: " + iInt.ToString());
            //this.Invoke (NonModalMessage,"Callback returns: " + iInt.ToString());
            //this.Invoke 
            this.Invoke(new BackgroundFinishedCallback (BackgroundFinished2), new object[] { iInt });
        }

        ///<summary>Called in main thread via Invoke, then background thread finishing its calc</summary>
        public void BackgroundFinished2(int iInt)
        {
            uBackgroundCalc.GetResults(uGame);//Copy deadlocks from temp-game
            uGame.UpdateDeadlockedBoxes();//Check for cell-deadlocked boxes
            RedrawAllScreen();
            NonModalMessage("Callback returns: " + iInt.ToString());
        }

        ///<summary>Redo all and return to end of position</summary>
        public void FullRedo()
        {
            if (uSetting.bAnimateMassUndoRedo)
            {//Animated redo - for each step of redo...
                while (uGame.Redo() != MoveResult.WayBlocked)
                {
                    RedrawAround();//Redraw level around player
                    Update();//Ask system to display redrawed
                    Thread.Sleep(uSetting.iAnimationDelayMassUndoRedo);
                }
            }
            else
            {//Not animated redo
                while (uGame.Redo() != MoveResult.WayBlocked) //Redo all stack
                {
                }
                RedrawAllScreen();//Redraw whole level
            }
        }


        //################################################################################################################
        //Action* methods
        //This methods should contains code of main game actions

        ///<summary>User want to see "about"</summary>
        private void ActionAbout()
        {
            int iYear = DateTime.Now.Year;//Autoupdate end of copyright (trick)
            if (iYear < 2013)
                iYear = 2013;//But not less, than 2013
            
            //Version info read from assembly
            LogSimpleLine(ActionID.ShowDialog, "Info; About");
            MessageBox.Show("SokobanCompact v" + hExecAssem.GetName().Version + "\r\n\r\nCopyright (c) 2008-" + iYear.ToString() + " OverQuantum\r\nGPL v3\r\nhttps://github.com/OverQuantum/SokobanCompact", "About", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        ///<summary>User want to see system information</summary>
        private void ActionSysInfo()
        {
            PowerStatus uPS = new PowerStatus();
            uPS.Update();
            String sText;
            byte bChargeLevel = uPS.BatteryLifePercent;
            if (bChargeLevel == PowerStatus.BATTERY_FLAG_UNKNOWN_BYTE)
                sText = "Charge level: unknown";
            else
                sText = "Charge level: " + bChargeLevel.ToString() + "%";
            sText += "\r\nTime: " + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            LogSimpleLine(ActionID.ShowDialog, "Info; SystemInfo");
            MessageBox.Show(sText, "System Info", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }


        ///<summary>Undo 1 move</summary>
        public void ActionUndo()
        {
            if (uGame.Undo() != MoveResult.WayBlocked)//Try to do undo, if success
            {
                //LogSimpleLine(ActionID.Undo1, "Undo1; 1");
                RemoveBoxHighlight();//Forget highlighting
                RedrawAround(); //Redraw level around player
                CheckForDeadlocks();//Check for game-deadlock
                UpdateStatus(); //Refresh status bar
            }
            /*else
            {
                LogSimpleLine(ActionID.Undo1, "Undo1; 0");
            }*/
            //this.Invalidate();
            //this.Invalidate(new Rectangle(uCurrentLevel.iPlayerX * 16 + iPanX - 32, uCurrentLevel.iPlayerY * 16 + iPanY - 32, 80, 80));//5x5 - simpliest
            LogSimpleLine(ActionID.Redo1, "Undo1; " + uGame.uStats.iMoves.ToString());
        }

        ///<summary>Redo 1 move</summary>
        public void ActionRedo()
        {
            if (uGame.Redo() != MoveResult.WayBlocked)//Try to do redo, if success
            {
                //LogSimpleLine(ActionID.Redo1, "Redo1; 1");
                RemoveBoxHighlight();//Forget highlighting
                RedrawAround(); //Redraw level around player
                CheckForDeadlocks();//Check for game-deadlock
                UpdateStatus(); //Refresh status bar
            }
/*            else
            {
                LogSimpleLine(ActionID.Redo1, "Redo1; 0");
            }*/
            LogSimpleLine(ActionID.Redo1, "Redo1; " + uGame.uStats.iMoves.ToString());
        }

        ///<summary>Undo all and return to start</summary>
        public void ActionFullUndo()
        {
            RemoveBoxHighlight();//Forget highlighting
            if (uSetting.bAdditionalMessages)
                NonModalMessage("Returning to start");//Verbose messages
            if (uSetting.bAnimateMassUndoRedo)
            {//Animated undo - for each step of undo...
                while (uGame.Undo() != MoveResult.WayBlocked)
                {
                    RedrawAround();//Redraw level around player
                    Update();//Ask system to display redrawed
                    Thread.Sleep(uSetting.iAnimationDelayMassUndoRedo);
                }
            }
            else
            {//Not animated undo
                while (uGame.Undo() != MoveResult.WayBlocked) //Undo all stack
                { }
                RedrawAllScreen();//Redraw whole level
            }
            CheckForDeadlocks();//Check for game-deadlock
            UpdateStatus(); //Refresh status bar
            LogSimpleLine(ActionID.UndoFull, "UndoFull; "+uGame.uStats.iMoves.ToString());
        }

        ///<summary>Redo all and return to end of position</summary>
        public void ActionFullRedo()
        {
            int iStart = Environment.TickCount;
            //LogSimpleLine(ActionID.RedoFull, "RedoFull");
            RemoveBoxHighlight();//Forget highlighting
            if (uSetting.bAdditionalMessages)
                NonModalMessage("Forwarding to last move");//Verbose messages
            FullRedo();//Main actions there
            iStart = iStart - Environment.TickCount;
            if (uSetting.bAdditionalMessages)
                NonModalMessage("Redo in " + iStart.ToString() + " ms");//Verbose messages with timing
            CheckForDeadlocks();//Check for game-deadlock
            UpdateStatus();//Refresh status bar
            LogSimpleLine(ActionID.RedoFull, "RedoFull; " + uGame.uStats.iMoves.ToString());
        }

        ///<summary>Undo group of moves</summary>
        public void ActionGroupUndo()
        {
            int iMoves = uGame.uStats.iMoves;
            RemoveBoxHighlight();
            if (uSetting.bAnimateMassUndoRedo)
            {//Animated undo - for each step of undo...
                while (uGame.Undo() != MoveResult.WayBlocked && uGame.NotMarker())
                {
                    RedrawAround();//Redraw level around player
                    Update();//Ask system to display redrawed
                    Thread.Sleep(uSetting.iAnimationDelayMassUndoRedo);
                }
                RedrawAround();
            }
            else
            {//Not animated undo
                while (uGame.Undo() != MoveResult.WayBlocked && uGame.NotMarker()) //Stop on end of undo or marker (marker is set for first action in group-action)
                { }
                RedrawAllScreen();//Redraw whole level
            }
            if (uSetting.bAdditionalMessages)
            {
                iMoves -= uGame.uStats.iMoves;
                if (iMoves != 0)
                    NonModalMessage(iMoves.ToString() + " moves back");//Verbose messages with count of moves
            }
            CheckForDeadlocks();//Check for game-deadlock
            UpdateStatus();//Refresh status bar
            LogSimpleLine(ActionID.UndoGroup, "UndoGroup; " + uGame.uStats.iMoves.ToString());
        }

        ///<summary>Redo group of moves</summary>
        public void ActionGroupRedo()
        {
            int iMoves = uGame.uStats.iMoves;
            RemoveBoxHighlight();
            if (uSetting.bAnimateMassUndoRedo)
            {//Animated redo - for each step of redo...
                while (uGame.Redo() != MoveResult.WayBlocked && uGame.NotMarker()) //Stop on end of undo or marker (marker is set for first action in group-action)
                {
                    RedrawAround();//Redraw level around player
                    Update();//Ask system to display redrawed
                    Thread.Sleep(uSetting.iAnimationDelayMassUndoRedo);
                }
                RedrawAround();
            }
            else
            {//Not animated redo
                while (uGame.Redo() != MoveResult.WayBlocked && uGame.NotMarker())
                {
                }
                RedrawAllScreen();//Redraw whole level
            }
            if (uSetting.bAdditionalMessages)
            {
                iMoves = uGame.uStats.iMoves - iMoves;
                if (iMoves != 0)
                    NonModalMessage(iMoves.ToString() + " moves forward");//Verbose messages with count of moves
            }
            CheckForDeadlocks();//Check for game-deadlock
            UpdateStatus();//Refresh status bar
            LogSimpleLine(ActionID.RedoGroup, "RedoGroup; " + uGame.uStats.iMoves.ToString());
        }

        ///<summary>User command to move player</summary>
        public void ActionMovePlayer(SokoMove uDir)
        {
            MoveResult bRes = uGame.NewMove(uDir);
            /*
            //MoveResult bRes = uCurrentLevel.MovePlayer(ref uDir);
            MoveResult bRes = uUndo.NewMove(uDir);
            if (bRes != MoveResult.WayBlocked)
            {
                //uUndo.AddNewMove(uDir, bRes);
                //uUndo.AddNewMove(uDir);
                //this.Refresh();
                //this.Invalidate();
            }*/
            if (bRes != MoveResult.WayBlocked)
            {
                RemoveBoxHighlight();//Box highlighting is not valid anymore
                RedrawAround();
            }
            if (bRes == MoveResult.MovedAndPushBox || bRes == MoveResult.MovedAndPushBoxToTarget)
                uGame.MarkerCurrentMove();//If player moved a box - mark action as group
            if (bRes == MoveResult.MovedAndPushBoxToTarget)
            {//If player put box on target - check, may be level is solved
                CheckLevelCompletition();
            }
            CheckForDeadlocks();//Check for game-deadlock
            UpdateStatus();//Redraw moves/pushes counter

        }

        ///<summary>Restart current level</summary>
        public void ActionRestartLevel()
        {
            LogSimpleLine(ActionID.RestartLevel, "Restart level; " + uLevelSet.sFileName + "; " + (uLevelSet.GetCurrentLevel() + 1).ToString());

            if (LoadLevel(uGame, uLevelSet.GetCurrentLevel()) == FunctionResult.OK)
            {
                RemoveBoxHighlight();//Box highlighting is not valid anymore

                ManageDeadlocks();
                NonModalMessage("Level restarted");
                RedrawAllScreen();
            }
        }

        ///<summary>Will choose how to calc deadlocks - immediately or in background </summary>
        public void ManageDeadlocks()
        {
            if (uSetting.bAutocalcDeadlocks)
            {//Calculate cell-deadlocks, if option is set

                if (uSetting.iBackgroundAutoDeadlocksLimit == 0 || uGame.iXsize * uGame.iYsize < uSetting.iBackgroundAutoDeadlocksLimit)
                {   //Small level - calc in current thread
                    Cursor.Current = Cursors.WaitCursor;
                    uGame.CalcDeadlocks();//Autocalculate cell-deadlocks on level
                    uGame.UpdateDeadlockedBoxes();//Check for cell-deadlocked boxes
                    Cursor.Current = Cursors.Default;
                }
                else
                {   //Large level - calc in separate thread
                    try
                    {
                        uBackgroundThread.Abort();
                    }
                    catch { };
                    uBackgroundThread = new Thread(new ThreadStart(uBackgroundCalc.CalcDeadlocks));
                    uBackgroundCalc.PleaseGetGame(uGame);
                    uBackgroundThread.Priority = ThreadPriority.BelowNormal;
                    uBackgroundThread.Start();
                }
            }
            CheckForDeadlocks();
        }


        ///<summary>Sequence of dialogs by selecting levelset and/or level (1 - start from selecting level, 2 - start from selecting levelset)</summary>
        /*
         * 1) Level [-> Level set [-> Level [-> Level set [-> ...]]]]
         * 2) Level set [-> Level [-> Level set [-> ...]]]
         */
        public void ActionChooseLevelAndSet(int iStart)
        {
            bool bSelectLevel = false;//Show dialog of selecting level
            bool bSelectSet = false;//Show dialog of selecting levelset
            bool bAutoLevel = false;//Autoselect level (first unsolved) of selected levelset
            DialogResult uSelectResult;
            FunctionResult uRV;

            if (iStart == 1)
                bSelectLevel = true;//Start from selecting level of levelset
            else if (iStart == 2)
                bSelectSet = true;//Start from selecting levelset

        lAgain:
            if (bSelectLevel)
            {//Selecting level
                bSelectSet = false;//Deactivate selecting levelset (if user will select level - it will end sequence)
                bSelectLevel = false;//Dectivate selecting level

                ChooseLevel hChooseLevel = new ChooseLevel();
                LogSimpleLine(ActionID.ShowForm, "Choose level");
                hChooseLevel.bShowOnlyUnsolvedLevels = uSetting.bShowOnlyUnsolvedLevels;//Transmit status of showing unsolved levels
                uSelectResult = hChooseLevel.SelectLevel(uLevelSet);//Show dialog of selecting level
                uSetting.bShowOnlyUnsolvedLevels = hChooseLevel.bShowOnlyUnsolvedLevels;//Save selected status of showing unsolved levels

                if (uSelectResult == DialogResult.OK)
                {//Level was selected
                    if (LoadLevel(uGame, hChooseLevel.iSelectedLevel) == FunctionResult.OK)
                        AfterLoadLevel();
                    if (bAutoLevel)
                        SaveSettings();//Save selected level, if it was not autoselected
                    bAutoLevel = false;//Dectivate autoselecting level
                }
                else if (uSelectResult == DialogResult.Retry)
                {//Level was not selected, user click on selecting levelset
                    bSelectSet = true;//Activate selecting levelset
                    bSelectLevel = true;//Activate selecting level (after levelset)
                }
            }

        lAgainSet:
            if (bSelectSet)
            {
                ChooseLevelSet hChooseLevelSet = new ChooseLevelSet();
                LogSimpleLine(ActionID.ShowForm, "Choose levelset");
                uSelectResult = hChooseLevelSet.SelectLevelSet(uLevelSetList);//Show dialog of selecting levelset

                if (uSelectResult == DialogResult.Retry) //Levelset was selected, but user click on selecting level also
                    bSelectLevel = true;//Activate selecting level
                if (uSelectResult == DialogResult.Cancel) //Levelset selecting was canceled
                {
                    bSelectLevel = false;//Deactivate selecting level

                    if (uLevelSet.GetCurrentLevel() < 0)
                    {//Previous levelset was lost
                        LogSimpleLine(ActionID.ShowDialog, "Error; Previous levelset lost");
                        MessageBox.Show("Previous LevelSet was lost.\r\nRandom level will be loaded", "Warning");
                        uLevelSetList.GenNullLevelSet(uLevelSet);
                        LoadLevel(uGame, 0);
                        AfterLoadLevel();
                    }
                }

                if (uSelectResult == DialogResult.OK || uSelectResult == DialogResult.Retry)
                {//Levelset was selected
                    Cursor.Current = Cursors.WaitCursor;
                    uRV = uLevelSetList.LoadLevelSet(uLevelSet, hChooseLevelSet.iSelectedSet);
                    if (uRV != FunctionResult.OK)
                    {//Levelset loading failed
                        Cursor.Current = Cursors.Default;
                        LogSimpleLine(ActionID.ShowDialog, "Error; Unable to load levelset; " + hChooseLevelSet.sSelectedSet+"; "+uRV.ToString());
                        MessageBox.Show("Unable to load LevelSet, result: " + uRV.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        goto lAgainSet;//Return to selecting levelset
                    }
                    uSetting.sLastLevelSet = uLevelSetList.GetCurrentLevelSet();//Store selected levelset into settings
                    Cursor.Current = Cursors.Default;
                    bAutoLevel = true;//Activate autoselecting level
                }
            }

            if (bSelectLevel)
                goto lAgain;//Level selecting activated - next iteration of sequence

            if (bAutoLevel)
            {//Autoselecting of first unsolved level
                int iNum = uLevelSet.GetNextUnsolved();
                if (iNum == -1)
                {
                    iNum = 0;//All solved? Load first
                }

                uRV = LoadLevel(uGame, iNum);
                if (uRV == FunctionResult.OK)
                {   //Loaded successfully
                    AfterLoadLevel();
                }
                else
                {   //Level not loaded (only variant - FunctionResult.OutOfLevelSet)
                    LogSimpleLine(ActionID.ShowDialog, "Error; Unable to load level; "+iNum.ToString()+";"+uRV.ToString());
                    MessageBox.Show("Unable to load level " + iNum.ToString() + ", result: " + uRV.ToString() + "\r\nRandom level will be selected.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    ActionRandLevel();
                }

                AfterLoadLevel();
                SaveSettings();
            }

        }

        /*
        public void ActionChooseLevel()
        {
            ChooseLevel hChooseLevel = new ChooseLevel();
            hChooseLevel.bShowOnlyUnsolvedLevels = uSetting.bShowOnlyUnsolvedLevels;
            DialogResult uSelectResult;
            uSelectResult = hChooseLevel.SelectLevel(uLevelSet);
            if (uSelectResult == DialogResult.OK)
            {
                if (uLevelSet.LoadLevel(uCurrentLevel, hChooseLevel.iSelectedLevel)==0)
                    AfterLoadLevel();
            }
            uSetting.bShowOnlyUnsolvedLevels = hChooseLevel.bShowOnlyUnsolvedLevels;
        }*/

        /*
        public void ActionChooseLevelSet()
        {
            ChooseLevelSet hChooseLevelSet = new ChooseLevelSet();
            //hChooseLevel.bShowOnlyUnsolvedLevels = uSetting.bShowOnlyUnsolvedLevels;
            if (hChooseLevelSet.SelectLevelSet(uLevelSetList) == DialogResult.OK)
            {
                FunctionResult uRV;
                Cursor.Current = Cursors.WaitCursor;
                //LoadLevelSet(hChooseLevelSet.sSelectedSet);
                uRV = uLevelSetList.LoadLevelSet(uLevelSet, hChooseLevelSet.iSelectedSet);
                if (uRV != FunctionResult.OK)
                {
                    MessageBox.Show("Unable to load LevelSet, result: " + uRV.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    goto lExit;
                }
                uSetting.sLastLevelSet = uLevelSetList.GetCurrentLevelSet();

                int iNum = uLevelSet.GetNextUnsolved();
                if (iNum == -1)
                {
                    iNum = 0;
                }

                uLevelSet.LoadLevel(uCurrentLevel, iNum);//__what if not loaded?
                AfterLoadLevel();
                SaveSettings();
            lExit:
                Cursor.Current = Cursors.Default;
            }
            //uSetting.bShowOnlyUnsolvedLevels = hChooseLevel.bShowOnlyUnsolvedLevels;
        }*/


        /*
        public void ActionChooseLevelSet_OLD()
        {
            /*
            OpenFileDialog hSelectLevelSet = openFileDialog1; //new OpenFileDialog();
            hSelectLevelSet.InitialDirectory = sApplicationDirectory + @"Levels";//sLevelsDirectory;
            hSelectLevelSet.Filter = "All files (*.*)|*.*";
            if (hSelectLevelSet.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(hSelectLevelSet.FileName);//sFileName);
                //LoadLevelSet(hSelectLevelSet.FileName.

            }
             /**/

        /*
        // * simple choose of file
        OpenFile hSelectLevelSet = new OpenFile();
        hSelectLevelSet.sFolder = sLevelsDirectory;
        hSelectLevelSet.sTitle = "Select Level Set";
        //string sFileName;

        if (hSelectLevelSet.SelectFileForLoad())
        {
            //MessageBox.Show(hSelectLevelSet.sFileName);
            LoadLevelSet(hSelectLevelSet.sFileName);

            uLevelSet.LoadLevel(uCurrentLevel, 0);//__what if not loaded?
            AfterLoadLevel();
            SaveSettings();
        }
    }*/

        /*
        public void ActionSetPlayerName()
        {
            InputBox sGetPlayerName = new InputBox();
            if (sGetPlayerName.AskUser(uSetting.sPlayerName, "Player name", "Input player name:") == DialogResult.OK)
            {
                uSetting.sPlayerName = sGetPlayerName.GetResult();
                SaveSettings();
            }

        }*/

        ///<summary>Show (modal) information about current level</summary>
        private void ActionLevelInfo()
        {
            //Main information
            string sMessage = "Level #" + (uLevelSet.GetCurrentLevel() + 1).ToString() + "\r\n";
            if (uGame.sTitle.Length > 0)
                sMessage += "Name: " + uGame.sTitle + "\r\n";
            if (uGame.sAuthor.Length > 0)
                sMessage += "Author: " + uGame.sAuthor + "\r\n";
            if (uGame.sComment.Length > 0)
                sMessage += "Comment: " + uGame.sComment + "\r\n";

            //Solving status and records
            if (uGame.IsSolved())
            {
                sMessage += "\r\nLevel is solved\r\n\r\n";//"+1" - to numerate level from 1
                sMessage += "Best moves: " + uGame.uBestMovesSolution.iMoves.ToString() + "\r\n" + "'" + uGame.uBestMovesSolution.sName + "'\r\n\r\n";
                sMessage += "Best pushes: " + uGame.uBestPushesSolution.iPushes.ToString() + "\r\n" + "'" + uGame.uBestPushesSolution.sName + "'\r\n\r\n";
            }
            else
            {
                sMessage += "\r\nLevel not solved\r\n";//"+1" - to numerate level from 1
            }

            //Sizes and statistic of elements
            sMessage += "Size: " + uGame.iXsize.ToString() + "x" + uGame.iYsize.ToString() + "\r\n";
            LevelStats uLevelStats = uGame.CalcLevelStats();
            if (uLevelStats.iNumPlayers != 1)
            {
                sMessage += uLevelStats.iNumPlayers.ToString() + " players,\r\n";
            }
            else
                sMessage += "One player,\r\n";
            if (uLevelStats.iNumBoxes == uLevelStats.iNumTargets)
            {
                sMessage += uLevelStats.iNumBoxes.ToString() + " boxes and targets,\r\n";
            }
            else
            {
                sMessage += uLevelStats.iNumBoxes.ToString() + " boxes, " + uLevelStats.iNumTargets.ToString() + " targets,\r\n";
            }
            sMessage += uLevelStats.iNumWalls.ToString() + " walls, ";
            sMessage += uLevelStats.iNumEmpty.ToString() + " spaces, ";
            sMessage += uLevelStats.iNumBackground.ToString() + " unused\r\n";
            sMessage += "Now " + uLevelStats.iNumBoxesOnTargets.ToString() + " boxes set to targets";

            sMessage += "\r\n" + uGame.iNumRemainExceedBoxes.ToString() + " exceeding boxes not in deadlock";

            //sMessage += "Best pushes: '" + uCurrentLevel.uBestPushesSolution.sName + "' " + uCurrentLevel.uBestPushesSolution.iMoves.ToString() + "/" + uCurrentLevel.uBestPushesSolution.iPushes.ToString() + "\r\n";
            //MessageBox.Show(sMessage, "Level information");

            formShowInfo frmInf = new formShowInfo();
            LogSimpleLine(ActionID.ShowForm, "Level Info; " + (uLevelSet.GetCurrentLevel() + 1).ToString() + "; " + uLevelSet.sFileName);
            frmInf.ShowInfoText(sMessage, "Level Info");//Showing to user

        }

        ///<summary>User want to go to next level</summary>
        public void ActionNextLevel()
        {
            int iNextLevel = uLevelSet.GetCurrentLevel() + 1;
            if (iNextLevel >= uLevelSet.GetLevelsNum())
                iNextLevel = 0;
            if (LoadLevel(uGame, iNextLevel) == FunctionResult.OK)//Just try to load n+1
                AfterLoadLevel();
        }
        ///<summary>User want to go to previous level</summary>
        public void ActionPreviousLevel()
        {
            int iPrevLevel = uLevelSet.GetCurrentLevel() - 1;
            if (iPrevLevel < 0)
                iPrevLevel = uLevelSet.GetLevelsNum()-1;
            if (LoadLevel(uGame, iPrevLevel) == FunctionResult.OK)//Just try to load n-1
                AfterLoadLevel();
        }
        ///<summary>User want to go to random level</summary>
        public void ActionRandLevel()
        {
            Random uR = new Random();
            FunctionResult uRV;
            
            int iRandom = uR.Next(uLevelSet.GetLevelsNum());
            uRV = LoadLevel(uGame, iRandom);//Just try to load
            if (uRV == FunctionResult.OK)
            {
                AfterLoadLevel();
            }
            else 
            {
                LogSimpleLine(ActionID.ShowDialog, "Error; Failed to load random level; " + iRandom.ToString() + "; " + uRV.ToString());
                MessageBox.Show("Failed to load level " + iRandom.ToString() + " as random", "Warning");
            }
        }

        ///<summary>User want to go to next unsolved level</summary>
        public void ActionNextUnsolved()
        {
            int iNum = uLevelSet.GetNextUnsolved();
            if (iNum == -1)
            {
                LogSimpleLine(ActionID.ShowDialog, "Info; LevelSet Solved; "+uLevelSet.sFileName);
                MessageBox.Show("Whole LevelSet is solved");
            }
            else
            {
                if (LoadLevel(uGame, iNum) == FunctionResult.OK)
                    AfterLoadLevel();
            }
        }

        ///<summary>User want to go to previous unsolved level</summary>
        public void ActionPrevUnsolved()
        {
            int iNum = uLevelSet.GetPrevUnsolved();
            if (iNum == -1)
            {
                LogSimpleLine(ActionID.ShowDialog, "Info; Whole levelset is solved; " + uLevelSet.sFileName);
                MessageBox.Show("Whole LevelSet is solved");
            }
            else
            {
                if (LoadLevel(uGame, iNum) == FunctionResult.OK)
                    AfterLoadLevel();
            }
        }

        ///<summary>User want to go to random unsolved level</summary>
        public void ActionRandUnsolved()
        {
            int iNum = uLevelSet.GetRandUnsolved();
            if (iNum == -1)
            {
                LogSimpleLine(ActionID.ShowDialog, "Info; Whole levelset is solved; " + uLevelSet.sFileName);
                MessageBox.Show("Whole LevelSet is solved");
            }
            else
            {
                if (LoadLevel(uGame, iNum) == FunctionResult.OK)
                    AfterLoadLevel();
            }
        }

        ///<summary>User want to save current position</summary>
        public void ActionSavePosition()
        {
            if (uGame.uStats.iMoves == 0)
            {
                LogSimpleLine(ActionID.ShowDialog, "Info; Position empty, nothing to save");
                MessageBox.Show("You are on level start, nothing to save");
                return;
            }
            InputBox uAskFileName = new InputBox();
            string sFileName = AutoNamePosition();

            //Ask user about position filename
            LogSimpleLine(ActionID.ShowForm, "Ask; File for save position");
            if (uAskFileName.AskUser(sFileName, "Save position", "Please input position filename:") == DialogResult.OK)
            {
                FunctionResult uRV;
                uGame.RecalcStats();//Recalc stats (some of them are not calculated automatically)
                sFileName = uAskFileName.GetResult() + ".pos";
                uGame.uStats.sName = uAskFileName.GetResult();
                //if (uGame.SavePosition(sSolutionsDirectory + sFileName, uLevelSet.sFileName, uLevelSet.GetCurrentLevel(), uSetting.sPlayerName) != 0)//Saving
                uRV = SavePosition(sSolutionsDirectory + sFileName);
                if (uRV != FunctionResult.OK)//Saving
                {
                    LogSimpleLine(ActionID.ShowDialog, "Error; Position was not saved!; " + sFileName + "; " + uRV);
                    MessageBox.Show("Position was not saved! " + uRV.ToString(), "ERROR");
                    return;
                }
                LogSimpleLine(ActionID.SavePosition, "Position saved; " + uGame.uStats.iMoves.ToString() + " moves; " + sFileName);
                NonModalMessage("Position saved");
            }
        }

        ///<summary>User want to load position (0 - chose from all know positions and solutions / 1 - only of this level / 2 - autoload autosaved position, forcing loading level with levelset)</summary>
        public void ActionLoadPosition(int iUseFilter, bool bForceLoadLevel)
        {
            FunctionResult uRV;
            OpenFile hSelectPosition = new OpenFile();
            bool bOtherLevel=false;
            hSelectPosition.sFolder = sSolutionsDirectory;
            hSelectPosition.sTitle = "Select Position";

            switch (iUseFilter)
            {
                case 0:
                    hSelectPosition.sFilter = "*.?o*";//Filter for all positions and solutions (.?o* - to select .pos and .sol)
                    LogSimpleLine(ActionID.ShowForm, "Ask; Choose position to load, from all saved");
                    break;
                case 1:
                    hSelectPosition.sFilter = FilterForPositions();//Filter for pos/sol of this level
                    LogSimpleLine(ActionID.ShowForm, "Ask; Choose position to load, for this leve");
                    break;
                case 2:
                    hSelectPosition.sFileName = AutoNameAutoSavePosition();
                    //goto lAfterFileSelection;
                    break;
            }

            if (iUseFilter==2 || hSelectPosition.SelectFileForLoad() == DialogResult.OK) //Show list of pos/sol to user, or skip it if ???
            {
            //lAfterFileSelection:
                IniHold.IniFile uIni = new IniHold.IniFile();//IniHold object - to treat position file as ini-file
                uIni.LoadIni(sSolutionsDirectory + hSelectPosition.sFileName);//Load file as ini-file

                string sLoadedPosition = uIni.GetItemValue("Position");//Get sequence of moves

                //If sequence is empty or not present - exit
                if (sLoadedPosition.Length == 0)
                    return;

                string sQuestion;//String to ask user

                string sLoadedLevelSet = uIni.GetItemValue("LevelSet", "<unknown>").ToLower();//Load levelset name
                int iLoadedLevelNumber = OQConvertTools.string2int(uIni.GetItemValue("Level", "0"));//Load level numder
                iLoadedLevelNumber--;//Levels numbered from 1 in files

                if (bForceLoadLevel)
                {
                    //Position is loaded with level and levelset

                    if (sLoadedLevelSet != uLevelSet.sFileName)
                    {   //Loading levelset is required
                        uRV = uLevelSetList.LoadLevelSet(uLevelSet, sLoadedLevelSet);
                        if (uRV != FunctionResult.OK)
                        {   //Not loaded
                            bOtherLevel = true;
                            LogSimpleLine(ActionID.ShowDialog, "Error; Levelset from position file was not loaded!; " + sLoadedLevelSet + "; " + uRV.ToString());
                            MessageBox.Show("LevelSet '" + sLoadedLevelSet + "' from position file was not loaded! Result: " + uRV.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                            uRV = uLevelSetList.LoadLevelSet(uLevelSet, uSetting.sLastLevelSet);//Reload previous levelset
                            if (uRV != FunctionResult.OK)
                            {   //Not loaded also
                                LogSimpleLine(ActionID.ShowDialog, "Error; Previous levelset was not loaded!; " + uSetting.sLastLevelSet + "; " + uRV.ToString());
                                MessageBox.Show("Previous LevelSet '" + uSetting.sLastLevelSet + "' was not loaded! Result: " + uRV.ToString() + "\r\nRandom level will be loaded.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                                uLevelSetList.GenNullLevelSet(uLevelSet);//Generate random level
                                iLoadedLevelNumber = 0;
                            }
                            else
                            {
                                //Ok, previous levelset loaded, use previos level number
                                iLoadedLevelNumber = uSetting.iLastLevelPlayed;
                            }
                        }
                        else
                        {
                            //Ok, levelset loaded, save it's name
                            uSetting.sLastLevelSet = sLoadedLevelSet;
                        }
                    }

                    uRV = LoadLevel(uGame, iLoadedLevelNumber);//Load level
                    if (uRV == FunctionResult.OK)
                    {   //Level loaded successfully
                        AfterLoadLevel();
                    }
                    else
                    {   //Level not loaded (only variant - FunctionResult.OutOfLevelSet)
                        bOtherLevel = true;
                        LogSimpleLine(ActionID.ShowDialog, "Error; Unable to load level; " + (iLoadedLevelNumber + 1).ToString() + "; " + uRV.ToString());
                        MessageBox.Show("Unable to load level " + (iLoadedLevelNumber + 1).ToString() + ", result: " + uRV.ToString() + "\r\nRandom level will be selected.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        ActionRandLevel();
                    }
                }
                else
                {
                    //Position is loaded for current level

                    if (uGame.uStats.iMoves != 0)
                    {//Restart level, if player is moved
                        ActionRestartLevel();
                    }
                    //Check, that position from same level

                    if (sLoadedLevelSet != uLevelSet.sFileName || iLoadedLevelNumber != uLevelSet.GetCurrentLevel()) //Position may be not for current level - warn user
                    {
                        string sLevelNum;
                        if (iLoadedLevelNumber == -1)
                            sLevelNum = "<unknown> level"; //Level number not specified in file
                        else
                            sLevelNum = "level " + iLoadedLevelNumber.ToString();//Level number specified
                        if (sLoadedLevelSet != uLevelSet.sFileName) //Levelset is different
                            sQuestion = "This position from " + sLevelNum + " of " + sLoadedLevelSet + " levelset\r\nAre you sure?";
                        else //Levelset is the same, but levelnumber is different
                            sQuestion = "This position from " + sLevelNum + "\r\nAre you sure?";
                        LogSimpleLine(ActionID.ShowDialog, "Ask; Load position from different level/set; " + sLevelNum + "; " + sLoadedLevelSet);
                        if (MessageBox.Show(sQuestion, "Loading position", MessageBoxButtons.OKCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button2) == DialogResult.Cancel) //Ask user
                            return; //Exit, if user decide so
                    }
                }

                if (bForceLoadLevel)
                {

                }

                if (bOtherLevel)
                {   //Exact levelset and level not loaded due to some error, so loaded position is not valid
                    return;
                }

                //Use loaded position
                uGame.PastePositionLuRd(sLoadedPosition);

                //Redo position with checks for deadlocks (this only required if called FastCheckForDeadlocks)
                while (uGame.Redo() != MoveResult.WayBlocked)
                {
                    CheckForDeadlocks();
                }
                while (uGame.Undo() != MoveResult.WayBlocked) //...And undo it back
                { }
                
                //ActionFullRedo();//Show full redo - to reach end of solution/position
                FullRedo();//Show full redo - to reach end of solution/position

                uGame.SetMarkersOnBoxChanges();//Set markers for group undo/redo - on changing box

                if (bForceLoadLevel)
                {
                    LogSimpleLine(ActionID.LoadPosition, "Position loaded with level; " + uGame.uStats.iMoves.ToString() + " moves; " + hSelectPosition.sFileName);
                    NonModalMessage("Game loaded");
                }
                else
                {
                    LogSimpleLine(ActionID.LoadPosition, "Position loaded; " + uGame.uStats.iMoves.ToString() + " moves; " + hSelectPosition.sFileName);
                    if (iUseFilter != 2)
                        NonModalMessage("Position loaded");
                }
            }

        }

        /*
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

        ///<summary>Moving player by single click (x,y of destination location)</summary>
        public void ActionTravelPlayerTo(int iX, int iY)
        {
            int iMoves = uGame.uStats.iMoves;
            LogSimpleLine(ActionID.ClickTravel, "Click Travel; " + uGame.iPlayerX.ToString() + "; " + uGame.iPlayerY.ToString() + "; " + iX.ToString() + "; " + iY.ToString());
            if (uGame.TravelTo(iX, iY) == MoveResult.Moved) //Calculate moving sequence and load it into undo/redo stack
            {
                RemoveBoxHighlight();
                if (uSetting.bAnimateTravel)
                {//Animated motion
                    while (uGame.Redo() != MoveResult.WayBlocked)
                    {//Redo moving sequence one by one with redrawing
                        RedrawAround();
                        Update();
                        Thread.Sleep(uSetting.iAnimationDelayTravel);
                    }
                }
                else
                {//Unanimated motion
                    while (uGame.Redo() != MoveResult.WayBlocked) //Redo moving sequence at once
                    { }
                    RedrawAllScreen();//And redraw all level
                }
                if (uSetting.bAdditionalMessages)
                {//Show verbose message with counter of moves
                    iMoves = uGame.uStats.iMoves - iMoves;
                    NonModalMessage("Travel with " + iMoves.ToString() + " moves");
                }
                UpdateStatus();//Redraw moves/pushes counter
            }
            else
            {//Calculation failed - not possible to move there
                LogSimpleLine(ActionID.ActionFailed, "Travel failed; Unreachable");
                NonModalMessage("Not reachable location");
            }
        }

        ///<summary>Pushing box by two clicks (x,y of destination location)</summary>
        public void ActionTravelBoxTo(int iX, int iY)
        {
            int iMoves = uGame.uStats.iMoves;
            int iPushes = uGame.uStats.iPushes;

            LogSimpleLine(ActionID.ClickPushBox, "Click MoveBox; " + iHighlightedBoxX.ToString() + "; " + iHighlightedBoxY.ToString()+"; " + iX.ToString() + "; " + iY.ToString());
            if (uGame.TravelBoxTo(iX, iY) != MoveResult.WayBlocked) //Calculate moving sequence and load it into undo/redo stack
            {
                if (uSetting.bAnimateBoxPushing)
                {//Animated motion
                    RemoveBoxHighlight();

                    while (uGame.Redo() != MoveResult.WayBlocked)
                    {//Redo moving sequence one by one with redrawing
                        RedrawAround();
                        Update();
                        Thread.Sleep(uSetting.iAnimationDelayBoxPushing);
                    }
                }
                else
                {//Unanimated motion
                    while (uGame.Redo() != MoveResult.WayBlocked) //Redo moving sequence at once
                    { }
                    RedrawAllScreen();//And redraw all level
                }

                if (uSetting.bAdditionalMessages)
                {//Show verbose message with counters of moves and pushes
                    iMoves = uGame.uStats.iMoves - iMoves;
                    iPushes = uGame.uStats.iPushes - iPushes;
                    NonModalMessage("Moved with " + iMoves.ToString() + "m " + iPushes.ToString() + "p");
                }
                CheckForDeadlocks();//Check for game-deadlock
                UpdateStatus();
                CheckLevelCompletition();//Level may be solved, so check
            }
            else
            {
                LogSimpleLine(ActionID.ActionFailed, "MoveBox failed; Unreachable");
                NonModalMessage("Can't push box there");
            }
        }

        ///<summary>User click on box - highlight it and show where it can be pushed</summary>
        public void ActionClickBox(int iX, int iY)
        {
            LogSimpleLine(ActionID.ClickBox, "Click Box; " + iX.ToString() + "; " + iY.ToString());
            if (iHighlightedBoxX == iX && iHighlightedBoxY == iY)
            {//This box is already highlighted? Remove highlighting
                RemoveBoxHighlight();
                uGame.InvalidatePlayerMoveTree();
            }
            else
            {
                int iTime = Environment.TickCount;//Start timer
                uGame.iCountDebug = 0;
                bool bStopOnDeadlocks = uSetting.bDeadlockLimitsAutopush;//Will BMT will stop on cell-deadlocks? Initially it is controlled by setting
                if (uGame.iNumRemainExceedBoxes > 0 || (uGame.GetCell(iX,iY)&SokoCell.CellDeadlock)!=0) //But also BMT will NOT stop, if there are exceeding boxes or this box is already in cell-deadlock
                    bStopOnDeadlocks = false;
                //if (uGame.BuildBoxMoveTreeFull(iX, iY, uSetting.bDeadlockLimitsAutopush) == FunctionResult.OK) //Calculated where box can be pushed
                if (uGame.BuildBoxMoveTreeFull(iX, iY, bStopOnDeadlocks) == FunctionResult.OK) //Calculated where box can be pushed
                {
                    iHighlightedBoxX = iX; iHighlightedBoxY = iY;//
                    iTime = Environment.TickCount - iTime;//End timer
                    if (uSetting.bAdditionalMessages) //Show verbose message with timing and exceeding of calcs
                        NonModalMessage("BMT in " + iTime.ToString() + " ms / " + uGame.iCountDebug.ToString());
                    RedrawAllScreen();//On this redraw will be highlighted all possible locations, where box can be pushed
                }
                else
                {//Calculation failed - box is out of range
                    LogSimpleLine(ActionID.ActionFailed, "Click box failed; Unreachable");
                    NonModalMessage("This box not reachable");
                }
            }
        }

        ///<summary>Calculate cell-deadlocks - positions, from where you can not push box to targets</summary>
        public void ActionCalcDeadlocks()
        {
            int iTime = Environment.TickCount;
            LogSimpleLine(ActionID.CalculateDeadlocks, "Calc cell-deadlocks");
            uGame.ReAnalyze();
            uGame.iCountDebug = 0;
            if (uGame.CalcDeadlocks() == FunctionResult.OK) //Calculate cell-deadlocks
            {
                iTime = Environment.TickCount - iTime;
                if (uSetting.bAdditionalMessages) //Show verbose message with timing and exceeding of calcs
                    NonModalMessage("DL in " + iTime.ToString() + " ms / " + uGame.iCountDebug.ToString());
                uGame.UpdateDeadlockedBoxes();//Check for cell-deadlocked boxes
                RedrawAllScreen(); //Redraw with showing all cell-deadlocks
            }
            else
            {//Unable
                NonModalMessage("Failed to calculate");
            }
        }

        ///<summary>Show information (modal) about current levelset</summary>
        private void ActionLevelSetInfo()
        {
            string sMessage;

            //Main information
            sMessage = "File: " + uLevelSet.sFileName + "\r\n";
            if (uLevelSet.sTitle.Length > 0)
                sMessage += "Name: " + uLevelSet.sTitle + "\r\n";
            else
                sMessage += "Unnamed levelset\r\n";
            if (uLevelSet.sAuthor.Length > 0)
                sMessage += "Author: " + uLevelSet.sAuthor + "\r\n";
            if (uLevelSet.sCopyright.Length > 0)
                sMessage += "Copyright: " + uLevelSet.sCopyright + "\r\n";
            if (uLevelSet.sComment.Length > 0)
                sMessage += "Comment: " + uLevelSet.sComment + "\r\n";

            //Statistic
            sMessage += "\r\nLevels: " + uLevelSet.GetLevelsNum().ToString() + "\r\n";
            sMessage += "Unsolved: " + uLevelSet.GetNumOfUnsolved().ToString() + "\r\n";
            sMessage += "Now loaded level " + (uLevelSet.GetCurrentLevel() + 1).ToString() + ", '" + uGame.sTitle + "'\r\n";

            formShowInfo frmInf = new formShowInfo();
            LogSimpleLine(ActionID.ShowForm, "LevelSet Info; " + uLevelSet.sFileName);
            frmInf.ShowInfoText(sMessage, "LevelSet Info");//Show
        }

        ///<summary>Show information (modal) about current skin</summary>
        private void ActionSkinInfo()
        {
            string sMessage = "Skin: " + uSetting.sSkin + "\r\n";
            sMessage += "Size: " + iSkinSize.ToString() + "x" + iSkinSize.ToString();

            LogSimpleLine(ActionID.ShowDialog, "Info; Skin info; " + iSkinSize.ToString() + "; " + uSetting.sSkin);
            MessageBox.Show(sMessage, "Current skin");
        }

        ///<summary>User want to change skin</summary>
        private void ActionLoadSkin()
        {
            OpenFile hSelectSkin = new OpenFile();
            hSelectSkin.sFolder = sSkinsDirectory;
            hSelectSkin.sTitle = "Select Skin";
            hSelectSkin.sFileName = uSetting.sSkin;//Transmit current skin filename, to highlight it in filelist

            LogSimpleLine(ActionID.ShowForm, "Ask; Choose skin to load");
            if (hSelectSkin.SelectFileForLoad() == DialogResult.OK) //Show list of skins to user
            {
                FunctionResult uRV = LoadSkin(hSelectSkin.sFolder + hSelectSkin.sFileName);
                if (uRV != FunctionResult.OK)
                {
                    LogSimpleLine(ActionID.ShowDialog, "Error; Unable to load bitmap; " + hSelectSkin.sFileName+"; "+uRV.ToString());
                    MessageBox.Show("Unable to load bitmap from this file, "+uRV.ToString());
                    return;
                }
                LogSimpleLine(ActionID.LoadSkin, "Skin loaded; " + iSkinSize.ToString() + "; " + uSetting.sSkin);
                uSetting.sSkin = hSelectSkin.sFileName;//Store skin name in settings
                SaveSettings();
                NonModalMessage("Skin " + uSetting.sSkin + " (" + iSkinSize.ToString() + "x" + iSkinSize.ToString() + ") loaded");
                RecenterLevel();//Redraw level with recentering
            }
        }

        ///<summary>User want to change background color</summary>
        private void ActionSetBackColor()
        {
            InputBox uRecName = new InputBox();
            LogSimpleLine(ActionID.ShowForm, "Ask; Background color");
            if (uRecName.AskUser(uSetting.iBackgroundColor.ToString("X").PadLeft(6, '0'), "Background Color", "Please specify background color in RRGGBB hexadecimal format:") != DialogResult.OK) //Promt user to write color in RRGGBB
                return;
            uSetting.iBackgroundColor = OQConvertTools.hex2int(uRecName.GetResult());
            uSetting.sBackgroundImageFile = "";//Clean image from settings to not use it
            if (uSetting.iBackgroundColor == -1)
                uSetting.iBackgroundColor = 0;//OQConvertTools.hex2int return -1 on errors
            LogSimpleLine(ActionID.ChangedBackground, "Set background color; " + uRecName.GetResult());
            UpdateBackground();//Recreate background according to settings
            RedrawAllScreen();
        }

        ///<summary>User want to change background image</summary>
        private void ActionLoadBackground()
        {
            OpenFile hSelectImage = new OpenFile();
            hSelectImage.sFolder = sBackgroundsDirectory;
            hSelectImage.sTitle = "Select Background Image";
            hSelectImage.sFileName = uSetting.sBackgroundImageFile;//Transmit current skin filename, to highlight it in filelist

            LogSimpleLine(ActionID.ShowForm, "Ask; Choose background image to load");
            if (hSelectImage.SelectFileForLoad() == DialogResult.OK) //Show list of skins to user
            {
                if (SetBackgroundImage(hSelectImage.sFolder + hSelectImage.sFileName) != FunctionResult.OK)
                {
                    MessageBox.Show("Unable to load bitmap from this file");
                    return;
                }
                LogSimpleLine(ActionID.ChangedBackground, "Set background image; " + hSelectImage.sFileName);
                uSetting.sBackgroundImageFile = hSelectImage.sFileName;//Store skin name in settings
                SaveSettings();
                //RecenterLevel();//Redraw level with recentering
                RedrawAllScreen();
            }
        }

        ///<summary>User want to change skin set</summary>
        private void ActionLoadSkinSet()
        {
            OpenFile hSelectSkinSet = new OpenFile();
            hSelectSkinSet.sFolder = sSkinsDirectory;
            hSelectSkinSet.sTitle = "Select SkinSet";
            hSelectSkinSet.sFilter = "*.sks";
            hSelectSkinSet.sFileName = uSetting.sSkinSet;//Transmit current skinset filename, to highlight it in filelist

            LogSimpleLine(ActionID.ShowForm, "Ask; Choose skinset to load");
            if (hSelectSkinSet.SelectFileForLoad() == DialogResult.OK) //Show list of skins to user
            {
                FunctionResult uRV = uSkinSet.Load(hSelectSkinSet.sFolder + hSelectSkinSet.sFileName);
                if (uRV != FunctionResult.OK)
                {
                    LogSimpleLine(ActionID.ShowDialog, "Error; Unable to load skinset; " + hSelectSkinSet.sFileName + "; " + uRV.ToString());
                    MessageBox.Show("Unable to load list of skins from this file, " + uRV.ToString());
                    return;
                }
                LogSimpleLine(ActionID.LoadSkinSet, "SkinSet loaded; " + uSetting.sSkin);
                if (uSetting.bAutosize)
                {
                    ActionAutoSize(); //Load skin by using Autosize
                    RecenterLevel(); //Draw level at center of the screen
                }
            }
        }


        ///<summary>User want to change settings</summary>
        private void ActionSettings()
        {
            formSettings uSetSettings = new formSettings();//Show dialog with settings
            LogSimpleLine(ActionID.ShowForm, "Settings");
            if (uSetSettings.ChangeSettings(uSetting) == DialogResult.OK)
            {
                SaveSettings();//Save changes
                /*if (uSetSettings.bBackColorChanged)
                {
                    UpdateBackground();
                    RedrawAllScreen();
                }*/
            }
        }

        ///<summary>User want to switch fullscreen/windowed mode</summary>
        private void ActionSwitchMaximize()
        {
            int iPrevHeight = ClientRectangle.Height;
            if (WindowState == FormWindowState.Maximized)
            {
                LogSimpleLine(ActionID.WindowNormalized, "Normalized");
                WindowState = FormWindowState.Normal;//Switch to windowed mode
                menuMaximize.Checked = false;
            }
            else
            {
                LogSimpleLine(ActionID.WindowMaximized, "Maximized");
                WindowState = FormWindowState.Maximized; //Switch to fullscreen mode
                menuMaximize.Checked = true;
            }
            iPanY += ClientRectangle.Height - iPrevHeight;//Keep contant actual position of level on screen
        }

        ///<summary>User want to exit</summary>
        private void ActionExit()
        {
            FunctionResult uRV;
        lTryAgain:
            uRV = uSetting.Save(sApplicationDirectory + sConfigFile);
            if (uRV != FunctionResult.OK) //Try to save settings
            {//If some error
                LogSimpleLine(ActionID.ShowDialog, "Error; Failed to save settings; " + uRV.ToString());
                switch (MessageBox.Show("Failed to save settings. " + uRV.ToString() + " Try again?", "ERROR", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1))
                {
                    case DialogResult.Cancel://Cancel - stop saving
                        break;
                    case DialogResult.Retry:
                        goto lTryAgain;//Try again
                    case DialogResult.Abort:
                        NonModalMessage("Exit aborted");//Abort - cancel exiting
                        return;
                }
            }
            AutosavePosition();
            if (uSetting.bLogActions)
            {
                uLog.LogString(ActionID.Exit,"Exit");
                uLog.End();
            }
            Close();
        }

        ///<summary>Load skin, that allow to better overview all level</summary>
        public void ActionAutoSize()
        {
            int iMaxX;
            int iMaxY;
            //Rectangle uUsefulRect;

            if (uSetting.bAutosizeUseful)
            {
                //uUsefulRect = uGame.GetUsefulCells();
                iMaxX = (int)(ClientRectangle.Width / (uGame.uUsefulCellsRect.Width+0.5));//Calc hor. and vert. size of skin to fit into useful rect of level, 0.5 - to see walls outside of usefull level
                iMaxY = (int)(iBottomOfForm / (uGame.uUsefulCellsRect.Height + 0.5));
            }
            else
            {
                iMaxX = ClientRectangle.Width / uGame.iXsize;//Calc hor. and vert. size of skin to fit into level
                iMaxY = iBottomOfForm / uGame.iYsize;
            }

            if (iMaxY < iMaxX) iMaxX = iMaxY; //Use minimal size
            if (iMaxX < uSetting.iAutosizeLowerLimit) iMaxX = uSetting.iAutosizeLowerLimit; //Limit size
            iMaxX = uSkinSet.GetNearestSize(iMaxX); //Get nearest size

            string sSkin = uSkinSet.GetSkin(iMaxX); //Get skin of specific size
            if (sSkin == null)
                return;

            FunctionResult uRV = LoadSkin(sSkinsDirectory + sSkin);
            if (uRV != FunctionResult.OK) //Load skin
            {
                LogSimpleLine(ActionID.ShowDialog, "Error; Failed to load skin from skinset; " + sSkin + "; " + uRV.ToString()); //!!NOTE!! skinset name should be logged
                MessageBox.Show("Failed to load skin '" + sSkin + "' from skinset '" + uSetting.sSkinSet + "' for autosize, " + uRV.ToString(), "ERROR");
                return; //Exit on errors
            }
            uSetting.sSkin = sSkin;//Store skin name in settings


            return;
        }

        ///<summary>Load skin, that a little bit detailed</summary>
        public void ActionZoomIn()
        {
            ZoomTo(uSkinSet.GetNextLarger(iSkinSize));
            /*
            string sSkin = uSkinSet.GetSkin(uSkinSet.GetNextLarger(iSkinSize)); //Get skin of size, that are larger
            if (sSkin == null)
                return;

            if (LoadSkin(sSkinsDirectory + sSkin) != FunctionResult.OK) //Load skin
                return; //Exit on errors
            uSetting.sSkin = sSkin;//Store skin name in settings

            RecenterLevel();//Redraw level with recentering*/
        }

        ///<summary>Load skin, that less detailed</summary>
        public void ActionZoomOut()
        {
            ZoomTo(uSkinSet.GetNextSmaller(iSkinSize));
            /*
            string sSkin = uSkinSet.GetSkin(uSkinSet.GetNextSmaller(iSkinSize)); //Get skin of size, that are smaller
            if (sSkin == null)
                return;

            if (LoadSkin(sSkinsDirectory + sSkin) != FunctionResult.OK) //Load skin
                return; //Exit on errors
            uSetting.sSkin = sSkin;//Store skin name in settings

            RecenterLevel();//Redraw level with recentering*/
        }

        //################################################################################################################
        //Interface-invoked methods
        //All this methods should contains only calls of Action* or other internal methods

        private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            UpdateAntiSuspend();//Prevent device from suspending - on clicks on toolbar

            if (e.Button == toolBarButtonUndo)
            {
                ActionUndo();
            }
            else if (e.Button == toolBarButtonRedo)
            {
                ActionRedo();
            }
            /*else if (e.Button == toolBarButtonFullRedo)
            {
                ActionFullRedo();
            }
            else if (e.Button == toolBarButtonFullUndo)
            {
                ActionFullUndo();
            }*/
            else if (e.Button == toolBarButtonGroupRedo)
            {
                ActionGroupRedo();
            }
            else if (e.Button == toolBarButtonGroupUndo)
            {
                ActionGroupUndo();
            }
            else if (e.Button == toolBarButtonNextUnsolved)
            {
                ActionNextUnsolved();
            }
            else if (e.Button == toolBarButtonPrevUnsolved)
            {
                ActionPrevUnsolved();
            }
            else if (e.Button == toolBarButtonNextLevel)
            {
                ActionNextLevel();
            }
            else if (e.Button == toolBarButtonPrevLevel)
            {
                ActionPreviousLevel();
            }
            else if (e.Button == toolBarButtonMenu)
            {
                LogSimpleLine(ActionID.ShowMenuMain, "Main Menu");
                contextMainMenu.Show(toolBarMain, new Point(20, 0)); //Show menu shifted a bit to the right from button
            }
            else if (e.Button == toolBarButtonSelector)
            {//Switch toolbar buttons set
                if (iSelectorMode == 0)
                    iSelectorMode = 1;
                else
                    iSelectorMode = 0;
                LogSimpleLine(ActionID.SwitchToolbar, "Toolbar; " + iSelectorMode.ToString());
                SetToolBarMode();//Show/hide buttons
            }
            else if (e.Button == toolBarButtonSelLevelSet)
            {
                ActionChooseLevelAndSet(2);//Select levelset (and may be level and so on)
            }
            else if (e.Button == toolBarButtonSelLevel)
            {
                ActionChooseLevelAndSet(1);//Select level (and may be levelset and so on)
            }
            else if (e.Button == toolBarButtonLoadPos)
            {
                ActionLoadPosition(1,false);
            }
            else if (e.Button == toolBarButtonSavePos)
            {
                ActionSavePosition();
            }
        }

        private void formMain_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateAntiSuspend();//Prevent device from suspending - on button clicks
            switch (e.KeyCode)
            {
                case Keys.Up:
                    // Up
                    LogSimpleLine(ActionID.ButtonUp, "Up");
                    ActionMovePlayer(SokoMove.Up);
                    break;
                case Keys.Down:
                    // Down
                    LogSimpleLine(ActionID.ButtonDown, "Down");
                    ActionMovePlayer(SokoMove.Down);
                    break;
                case Keys.Left:
                    // Left
                    LogSimpleLine(ActionID.ButtonLeft, "Left");
                    ActionMovePlayer(SokoMove.Left);
                    break;
                case Keys.Right:
                    // Right
                    LogSimpleLine(ActionID.ButtonRight, "Right");
                    ActionMovePlayer(SokoMove.Right);
                    break;
                case Keys.Enter:
                    // Enter (center button)
                    LogSimpleLine(ActionID.ButtonUndo, "Undo");
                    ActionUndo();
                    break;
                default:
                    LogSimpleLine(ActionID.ButtonOther, "Button Other; " + e.KeyCode.ToString());
                break;

            }
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            ActionExit();
        }

        /// <summary>System want to redraw form background</summary>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //Prevent redrawing of background
        } 

        private void formMain_Paint(object sender, PaintEventArgs e)
        {
            Redraw(e);
        }

        private void formMain_MouseDown(object sender, MouseEventArgs e)
        {
            uMouseDown = e;//Store mouse-down event to use it in drag-n-drop (on mouse-up event, see below)

            UpdateAntiSuspend();//Prevent device from suspending - on mouse clicks
        }

        private void formMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (uMouseDown == null) return;
            int dx = e.X - uMouseDown.X;
            int dy = e.Y - uMouseDown.Y;

            if (uSetting.bScrollLock || (Math.Abs(dx) < uSetting.iDragMinMove && Math.Abs(dy) < uSetting.iDragMinMove))
            {//Scrolling is locked or vector is less, than drag-n-drop limit - consider as click

                int iX = (e.X - iPanX) / iSkinSize;
                int iY = (e.Y - iPanY) / iSkinSize;
                SokoCell uCell = uGame.GetCell(iX, iY);
                if ((uCell & SokoCell.MaskObstacle) == 0)  //Click on the empty cell or target
                {
                    if (iHighlightedBoxX == -1)
                    {//No box was highlighted - move player to clicked cell
                        ActionTravelPlayerTo(iX, iY);
                    }
                    else
                    {//Box was highlighted - push it to clicked cell
                        ActionTravelBoxTo(iX, iY);
                    }
                }
                else if ((uCell & SokoCell.Box) != 0) //Click on the box
                {
                    ActionClickBox(iX, iY);
                }
                else
                {
                    LogSimpleLine(ActionID.Click, "Click NoResult; " + iX.ToString() + "; " + iY.ToString());
                }

            }
            else
            {//Vector is grater, than drag-n-drop limit - consider as drag-n-drop

                //Drag level for calculated vector
                iPanX = iPanX + dx;
                iPanY = iPanY + dy;
                LogSimpleLine(ActionID.DragLevel, "DragLevel; " + dx.ToString() + "; " + dy.ToString());

                RedrawAllScreen();
            }
            uMouseDown = null;

        }

        private void menuRestart_Click(object sender, EventArgs e)
        {
            ActionRestartLevel();
        }

        private void menuLevelChoose_Click(object sender, EventArgs e)
        {
            ActionChooseLevelAndSet(1);//Select level (and may be levelset and so on)
        }

        private void menuNextLevel_Click(object sender, EventArgs e)
        {
            ActionNextLevel();
        }

        private void menuPrevLevel_Click(object sender, EventArgs e)
        {
            ActionPreviousLevel();
        }

        private void menuLevelInfo_Click(object sender, EventArgs e)
        {
            ActionLevelInfo();
        }

        private void menuLevelSetLoad_Click(object sender, EventArgs e)
        {
            ActionChooseLevelAndSet(2);//Select levelset (and may be level and so on)
        }

        private void menuLoadSkin_Click(object sender, EventArgs e)
        {
            ActionLoadSkin();
        }

        private void menuCenterLevel_Click(object sender, EventArgs e)
        {
            LogSimpleLine(ActionID.RecenterLevel, "RecenterLevel");
            RecenterLevel();
        }

        private void menuFullUndo_Click(object sender, EventArgs e)
        {
            ActionFullUndo();
        }

        private void menuFullRedo_Click(object sender, EventArgs e)
        {
            ActionFullRedo();
        }

        private void menuGroupUndo_Click(object sender, EventArgs e)
        {
            ActionGroupUndo();
        }

        private void menuGroupRedo_Click(object sender, EventArgs e)
        {
            ActionGroupRedo();
        }

        private void menuUndo_Click(object sender, EventArgs e)
        {
            ActionUndo();
        }

        private void menuRedo_Click(object sender, EventArgs e)
        {
            ActionRedo();
        }

        private void menuPosSave_Click(object sender, EventArgs e)
        {
            ActionSavePosition();
        }

        private void menuLevelSetInfo_Click(object sender, EventArgs e)
        {
            ActionLevelSetInfo();
        }

        private void menuNextUnsolved_Click(object sender, EventArgs e)
        {
            ActionNextUnsolved();
        }

        private void menuSkinInfo_Click(object sender, EventArgs e)
        {
            ActionSkinInfo();
        }

        private void menuUndo__Click(object sender, EventArgs e)
        {
            ActionUndo();
        }

        ///<summary>Drawing status bar</summary>
        private void pictureStatus_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush uMainFontBrush = new SolidBrush(SystemColors.ControlText);//For main info
            SolidBrush uGrayFontBrush = new SolidBrush(SystemColors.GrayText);//For "m" and "p" letters
            e.Graphics.Clear(SystemColors.Control);

            //Draw moves counter
            string sPrint = uGame.uStats.iMoves.ToString();
            SizeF uSizes = e.Graphics.MeasureString(sPrint, Font);
            if (sPrint.Length < 4)//If 1-3 digits - draw gray letter "m" (for moves)
                e.Graphics.DrawString("m", Font, uGrayFontBrush, uRectMoves.Left, uRectMoves.Y);
            e.Graphics.DrawString(sPrint, Font, uMainFontBrush, uRectMoves.Right - uSizes.Width, uRectMoves.Y);//Draw counter

            //Draw pushes counter
            sPrint = uGame.uStats.iPushes.ToString();
            uSizes = e.Graphics.MeasureString(sPrint, Font);
            if (sPrint.Length < 4)//If 1-3 digits - draw gray letter "p" (for pushes)
                e.Graphics.DrawString("p", Font, uGrayFontBrush, uRectPushes.Left, uRectPushes.Y);
            e.Graphics.DrawString(sPrint, Font, uMainFontBrush, uRectPushes.Right - uSizes.Width, uRectPushes.Y);//Draw counter

            //Draw non-modal message
            e.Graphics.DrawString(sNonModalMessage, Font, uMainFontBrush, uRectMessage.X, uRectMessage.Y);

            //Draw level status (solved / unsoled) with icon
            if (uGame.IsDeadlock())
                e.Graphics.DrawIcon(Resources.IconDeadlock, uRectIndic.X, uRectIndic.Y);//Game-deadlock
            else
            {
                if (uGame.IsSolved())
                    e.Graphics.DrawIcon(Resources.IconSolved, uRectIndic.X, uRectIndic.Y);//Level is solved
                else
                    e.Graphics.DrawIcon(Resources.IconNotSolved, uRectIndic.X, uRectIndic.Y);//Level is not solved
            }
        }

        ///<summary>User click on status bar - show statistic of position (by "menu")</summary>
        private void pictureStatus_MouseUp(object sender, MouseEventArgs e)
        {
            uGame.RecalcStats();//Calculate all stats (some of them (box changes) are not calculated automatically)
            menuLevelSetIndic.Text = "LevelSet: " + uLevelSet.sTitle;
            menuLevelIndic.Text = "Level: " + (uLevelSet.GetCurrentLevel() + 1).ToString() + " of "+uLevelSet.GetLevelsNum().ToString();
            menuLevelTitleIndic.Text = "Title: " + uGame.sTitle;
            if (uGame.IsSolved())
                menuSolvedIndic.Text = "Solved: Yes";
            else
                menuSolvedIndic.Text = "Solved: No";
            if (uGame.IsDeadlock())
                menuSolvedIndic.Text = menuSolvedIndic.Text +" (Deadlock!)";
            menuUndoIndic.Text = "Undo: " + uGame.GetUndoInfo();
            menuStatsM.Text = "Moves: " + uGame.uStats.iMoves.ToString();
            menuStatsP.Text = "Pushes: " + uGame.uStats.iPushes.ToString();
            menuStatsLM.Text = "Linear Moves: " + uGame.uStats.iLinearMoves.ToString();
            menuStatsLP.Text = "Linear Pushes: " + uGame.uStats.iLinearPushes.ToString();
            menuStatsPS.Text = "Push Sessions: " + uGame.uStats.iPushSessions.ToString();
            menuStatsBC.Text = "Box Changes: " + uGame.uStats.iBoxChanges.ToString();
            LogSimpleLine(ActionID.ShowDialog, "Positions stats");
            contextMenuStats.Show(pictureStatus, new Point(e.X, e.Y));
        }

        private void menuMaximize_Click(object sender, EventArgs e)
        {
            ActionSwitchMaximize();
        }

        private void formMain_Resize(object sender, EventArgs e)
        {
            bResize = true;
        }

        private void menuPosLoad_Click(object sender, EventArgs e)
        {
            ActionLoadPosition(1,false);//Load position from list for this level
        }

        private void menuPosLoadAny_Click(object sender, EventArgs e)
        {
            ActionLoadPosition(0,false);//Load position from list of all positions and solutions
        }

        private void menuSettings_Click(object sender, EventArgs e)
        {
            ActionSettings();
        }

        private void menuGroupUndo2_Click(object sender, EventArgs e)
        {
            ActionGroupUndo();
        }

        private void menuLevelInfo2_Click(object sender, EventArgs e)
        {
            ActionLevelInfo();
        }

        private void menuCenterLevel2_Click(object sender, EventArgs e)
        {
            LogSimpleLine(ActionID.RecenterLevel, "RecenterLevel");
            RecenterLevel();
        }

        private void menuFullUndo2_Click(object sender, EventArgs e)
        {
            ActionFullUndo();
        }

        private void menuFullRedo2_Click(object sender, EventArgs e)
        {
            ActionFullRedo();
        }

        private void menuDeadlocks_Click(object sender, EventArgs e)
        {
            ActionCalcDeadlocks();
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            ActionAbout();
        }

        private void timerWake_Tick(object sender, EventArgs e)
        {
            //This timer prevent device from suspending, while user thinking

            if (!Focused || uSetting.iKeepAliveMinutes==0)
                return;

            if (DateTime.Now>dtDeviceCanSuspend)
            {
                //That's all folk, time out, device will go suspend after it's internal timeout
                //UpdateAntiSuspend();
                LogSimpleLine(ActionID.AntisuspendFinished, "AntisuspendFinished");
                //MessageBox.Show("Device may suspend now!");//__DEBUG__
            }
            else
            {
                //Keep device awake
                if (Environment.OSVersion.Platform == PlatformID.WinCE)
                {   //Crush on Win32, so only on CE
                    CoreDll.DontSleep();
                }

                //if (uSetting.bAdditionalMessages)//TODO: REMOVE THIS DEBUG INFO
                //    NonModalMessage("Will sleep at " + dtDeviceCanSuspend.ToString("HH:mm:ss"));
            }
        }

        private void formMain_Activated(object sender, EventArgs e)
        {
            LogSimpleLine(ActionID.WindowActivated, "Activated");
            UpdateAntiSuspend();//Prevent device from suspending - after switching to main form
        }

        private void menuAutoSize_Click(object sender, EventArgs e)
        {
            ActionAutoSize();
            LogSimpleLine(ActionID.SkinSizeChanged, "AutoSize;" + iSkinSize.ToString());
            RecenterLevel();//Redraw level with recentering
        }

        private void menuLoadSkinSet_Click(object sender, EventArgs e)
        {
            ActionLoadSkinSet();
        }

        private void menuNextUnsolved2_Click(object sender, EventArgs e)
        {
            ActionNextUnsolved();
        }

        private void menuPreviousUnsolved_Click(object sender, EventArgs e)
        {
            ActionPrevUnsolved();
        }

        private void menuRandomLevel_Click(object sender, EventArgs e)
        {
            ActionRandLevel();
        }

        private void menuRandomUnsolved_Click(object sender, EventArgs e)
        {
            ActionRandUnsolved();
        }

        private void menuLoadBackground_Click(object sender, EventArgs e)
        {
            ActionLoadBackground();
        }

        private void menuSetBackColor_Click(object sender, EventArgs e)
        {
            ActionSetBackColor();

        }

        private void menuZoomIn_Click(object sender, EventArgs e)
        {
            ActionZoomIn();
        }

        private void menuZoomOut_Click(object sender, EventArgs e)
        {
            ActionZoomOut();
        }

        private void menuPosLoadWithLevel_Click(object sender, EventArgs e)
        {
            ActionLoadPosition(0, true);
        }

        private void menuZoomSel_Popup(object sender, EventArgs e)
        {
            int iCurrent = iSkinSize;

            menuZoomP1.Text = uSkinSet.GetSmallest().ToString();
            menuZoomP2.Text = " ";
            menuZoomP3.Text = " ";
            menuZoomP4.Text = iCurrent.ToString();
            menuZoomP5.Text = " ";
            menuZoomP6.Text = " ";
            menuZoomP7.Text = uSkinSet.GetLargest().ToString();

            int i1 = uSkinSet.GetNextSmaller(iCurrent);
            if (i1 > 0)
            {
                menuZoomP3.Text = i1.ToString();
                i1 = uSkinSet.GetNextSmaller(i1);
                if (i1 > 0)
                {
                    menuZoomP2.Text = i1.ToString();
                    /*i1 = uSkinSet.GetNextSmaller(i1);
                    if (i1 > 0)
                        menuZoomP1.Text = i1.ToString();*/
                }
            }
            i1 = uSkinSet.GetNextLarger(iCurrent);
            if (i1 > 0)
            {
                menuZoomP5.Text = i1.ToString();
                i1 = uSkinSet.GetNextLarger(i1);
                if (i1 > 0)
                {
                    menuZoomP6.Text = i1.ToString();
                    /*i1 = uSkinSet.GetNextLarger(i1);
                    if (i1 > 0)
                        menuZoomP7.Text = i1.ToString();*/
                }
            }
        }

        private void menuZoomP1_Click(object sender, EventArgs e)
        {
            ZoomTo(OQConvertTools.string2int(menuZoomP1.Text));
        }

        private void menuZoomP2_Click(object sender, EventArgs e)
        {
            ZoomTo(OQConvertTools.string2int(menuZoomP2.Text));
        }

        private void menuZoomP3_Click(object sender, EventArgs e)
        {
            ZoomTo(OQConvertTools.string2int(menuZoomP3.Text));
        }

        private void menuZoomP4_Click(object sender, EventArgs e)
        {
            ZoomTo(OQConvertTools.string2int(menuZoomP4.Text));
        }

        private void menuZoomP5_Click(object sender, EventArgs e)
        {
            ZoomTo(OQConvertTools.string2int(menuZoomP5.Text));
        }

        private void menuZoomP6_Click(object sender, EventArgs e)
        {
            ZoomTo(OQConvertTools.string2int(menuZoomP6.Text));
        }

        private void menuZoomP7_Click(object sender, EventArgs e)
        {
            ZoomTo(OQConvertTools.string2int(menuZoomP7.Text));
        }

        private void menuSystemInfo_Click(object sender, EventArgs e)
        {
            ActionSysInfo();
        }

        private void menuScrollLock_Click(object sender, EventArgs e)
        {
            uSetting.bScrollLock = !uSetting.bScrollLock;
            menuScrollLock.Checked = uSetting.bScrollLock;
        }

        private void menuMinimize_Click(object sender, EventArgs e)
        {
            //CoreDll.ShowWindowEx(this.Handle, SW_MINIMIZED);
            LogSimpleLine(ActionID.WindowMinimizedManually, "Minimized manually");
            CoreDll.MinimizeWindow(this);
        }

        private void contextMenuScreen_Popup(object sender, EventArgs e)
        {
            LogSimpleLine(ActionID.ShowMenuScreen, "Screen Menu");
        }

        private void formMain_Deactivate(object sender, EventArgs e)
        {
            LogSimpleLine(ActionID.WindowDeactivated, "Deactivated");
        }

        private void formMain_GotFocus(object sender, EventArgs e)
        {
            LogSimpleLine(ActionID.WindowGotFocus, "GotFocus");
        }

        private void formMain_LostFocus(object sender, EventArgs e)
        {
            LogSimpleLine(ActionID.WindowLostFocus, "LostFocus");
        }


        /*
          //Not required, in fact, as WM5 and WM6 have button to rotate desktop
        private void menuRotateLevel_Click(object sender, EventArgs e)
        {
            uCurrentLevel.Rotate();
            RemoveBoxHighlight();
            RecenterLevel();
            //iHighlightedBoxX
        }*/

    }
}