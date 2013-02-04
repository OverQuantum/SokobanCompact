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

/*
*     Module for loading/saving ini files
* 
*     OverQuantum, 2005-2008
* 
*     20.04.2005 - 07.04.2008 VB 5.0
*     30.09.2008 - 13.10.2008 C# .NET CF 2.0
* 
* 
*	Syntax of ini-file.
*	
*	{<a>} - means nothing, or <a> - one or more times
*	[<a>] - means nothing or <a>
*	<a> | <b> - means <a> or <b>
*	
*	<wh>            ::= " " | <Tab> | <nbsp> | <LF>         ; ASCII: 32, 9, 160, 10
*	<equal>         ::= "="                                 ; ASCII  61
*	<quot>          ::= <">                                 ; ASCII  34
*	<symb>          ::= <ASCII 33, 35-60, 62-159, 161-255>
*	<hex-symb>      ::= "0"-"9" | "a-f" | "A-F"             ; hexadecimal digits
*	<nonrecom-symb> ::= <ASCII 0 - 9, 11, 12, 14 - 31>      ; symbols, that is possible to use, but strictly not recomended
*	
*	Recognizable syntax:
*	<simple-value>  ::= { <symb> }
*	<quoted-value>  ::= <quot> { { <symb> | <wh> | <equal> | <quot> <quot> | <nonrecomend-symb> } [ <quot> { <hex-symb> <hex-symb> { <wh> } } <quot> ] } <quot>
*	<value>         ::= <simple-value> | <quoted-value>
*	<item-line>     ::= {<wh>} <value> {<wh>} <equal> {<wh>} <value> {<wh>}   ; First value - ID of item, second - Value of item
*	<comment-line>  ::= { <symb> | <wh> | <quot> | <nonrecomend-symb> }       ; Comment may contain any symbols, except CR and "="
*	<any-line>      ::= <item-line> | <comment-line>
*	<ini-file>      ::= { <any-line> <CR>[<LF>] }               ; It is recomended to use <LF> for compatibility, unless highly required otherwise
*	                                                            ; Last <CR><LF> is not required
*	;Notes: 1. <hex-symb> <hex-symb> decoded to value as ASCII character with specified hexademal code
*	;       2. <quot> <quot> decoded to value as single <quot>
*	;Also recognizable: 1. <equal> in <simple-value> of item Value (i.e. second "=" outside <quot>s)
*	;                   2. Before <quot> in <quoted-value> it is possible to specify {<symb>} - they will be decoded by <simple-value> rules
*	;                   3. "0D0A" in hexadecimal form (outside <quot>s in <quoted-value>) may be shorted to "r", but this is highly NOT RECOMENDED, as this may cause decoding error
*	
*	Recomended syntax:
*	<hex-symb>      ::= "0"-"9" | "A-F"                         ; hexadecimal digits
*	<simple-value>  ::= <symb> { <symb> }                       ; Void values recomended to specify via <quoted-value>
*	<quoted-value>  ::= <quot> { { <symb> | <wh> | <equal> | <quot> <quot> } [ <quot> { <hex-symb> <hex-symb> } <quot> ] } <quot>
*	<value>         ::= <simple-value> | <quoted-value>
*	<item-line>     ::= <value> <" "> <equal> <" "> <value>
*	<comment-line>  ::= { <symb> | <wh> }
*	<any-line>      ::= <item-line> | <comment-line>
*	<ini-file>      ::= { <any-line> <CR> <LF>  }
* 
* 
*/

using System;

namespace IniHold
{
    /// <summary>Hold ID-Value pair</summary>
    public struct IniItem
    {
        /// <summary>ID - identifier, written before '='</summary>
        public string sID;
        /// <summary>Value, written after '='</summary>
        public string sValue;
    }

    /// <summary>Loading and saving ini files</summary>
    public class IniFile
    {
        /// <summary>List of all ID-Value pairs</summary>
        public IniItem[] uIniItems;

        /// <summary>Number of ID-Value pairs</summary>
        public int iNumIniItems;

        /// <summary>Allocated len for ID-Value pairs</summary>
        private int iAllocIniItems;

        /// <summary>Handle to file, being written</summary>
        private System.IO.StreamWriter hFileWriter;

        /// <summary>Basic constuctor</summary>
        public IniFile()
        {
            iAllocIniItems = 16;
            uIniItems = new IniItem[iAllocIniItems];
            iNumIniItems = 0;
            hFileWriter = null;

        }

        /// <summary>Transmit handle to opened file</summary>
        public void SetWriter(System.IO.StreamWriter hWriter)
        {
            hFileWriter = hWriter;
        }

        /// <summary>After this, IniFile will not use handle of written file</summary>
        public void ForgetWriter()
        {
            hFileWriter = null;
        }

        /// <summary>Wrap ID or Value according to syntax (source string, ID (0) or Value (1) ) </summary>
        public string WrapString(string sValue, int iFlags)
        {
            char b;

            int i,n = sValue.Length;
            int iNeedQuota = 0;
            int iState = 0;
            string sRv = "\"";

            char[] cValue = sValue.ToCharArray();//Convert source string into char array

            for (i=0;i<n;i++)//Iterate thru all chars
            {
                b = cValue[i];
                if (i == 0 || i == (n - 1))
                {   //First or last char
                    if (b == 32 || b == 160 || b == 9)//Whitespace in begin or end of value - quots needed
                        iNeedQuota = 1;
                }
                if (b < 32)
                {   //ASCII control chars - write as hex
                    if (iState == 0)
                    {   //Inside quots
                        sRv += "\"";//Add close quot
                        iState = 1;//Exit quots
                    }
                    sRv += OQConvertTools.char2hex(b);//Get hexadecimal value of char
                    iNeedQuota = 1;//Quots needed
                }
                else
                {   //Normal chars
                    if (iState == 1)
                    {   //Outside quots
                        sRv +=  "\"";//Add new quot
                        iState = 0;//Enter quots
                    }
                    sRv += b;//Add char
                    if (b == 34)
                    {   //Quot char in value - should be escaped
                        sRv += "\"";//Add additional quot
                        iNeedQuota = 1;//Quots in value - quots needed
                    }
                    else if (b == 61)
                    {   //Equal char "="
                        if ((iFlags & 1) == 0)  //This is ID, so quots needed (in Value equal may be not escaped)
                            iNeedQuota = 1;//Quots needed
                    }
                }
            }
            if (iState == 0)    //Outside quots
                sRv += "\"";//Add close quot

            if (iNeedQuota == 0 && sValue.Length != 0)
                    sRv = sValue;//Quots not needed, and value is not empty - return original string (otherwise will be returned string escaped by quots)
            return sRv;
        }

        /// <summary>Save ID = Value pair without flags (ID, Value)</summary>
        public int SaveItem(string sID, string sValue)
        {
            return SaveItem(sID, sValue, 0);
        }

        /// <summary>Save ID = Value pair with flags (ID, Value, flags)</summary>
        public int SaveItem(string sID, string sValue, int iFlags)
        {
            if (hFileWriter == null)
                return 1;//No handle of file-writer - no saving

            hFileWriter.WriteLine(WrapString(sID, 0) + " = " + WrapString(sValue,1));//Write single line: escaped ID, equal sign, escaped Value

            return 0;//Saving successfull
        }

        /// <summary>Load ini file (filename)</summary>
        public void LoadIni(string sFileName)
        {
            try
            {
                System.IO.StreamReader hReader = new System.IO.StreamReader(sFileName);//Open file
                string sLine;
                int n,k,i,iState1,iState2,iLastChar,iQuot,bPrev;
                char b;
                string[] sv = new string[2];
                char[] cLine;

                while (!hReader.EndOfStream)//Iterate thru whole file
                {
                    sLine = hReader.ReadLine();//Read full line
                    if (sLine.Length == 0 || sLine.Trim().Length == 0) continue;//Empty line - skip
                    n = sLine.Length;
        
                    iLastChar = 0; //Last position of char in id/value
                    iState1 = 0;//Begin of line may contain whitespaces
                    iState2 = 0;//ID is first
                    iQuot = 0;//Quotas not used
                    sv[0] = "";//ID - empty
                    sv[1] = "";//Value - empty
                    bPrev = -1;//Previous char - none

                    cLine = sLine.ToCharArray();//Convert line into array of chars

                    for (i = 0; i < n; i++)//Iterate thru all chars
                    {
                        b = cLine[i];//Get single char
                        if (iState1 == 0) //Before value
                        {   
                            //Begin of line may contain whitespaces
                            if (b == 32 || b == 160 || b == 9) goto lNextChar;  //Whitespaces in begining - skip
                            iState1 = 1;//Value began
                            if (b == 61 && iState2 == 0) goto lNext;//ID may not began with equal -> this line is comment
                            if (b == 34) iQuot = 1;//Quot - this is quoted value
                        }
                        if (iState1 == 1) //Inside value
                        {
                            //Value (out of quotas or non-quoted)
                            if (b == 32 || b == 160 || b == 9)
                            {
                                //Whitespace - adding to value, but do not updating iLastChar
                            }
                            else
                            {
                                if (b == 61 && iState2 == 0) //Equal in ID - now Value began
                                {
                                    iState2 = 1;//Value began
                                    iState1 = 0;//Begin of Value may contain whitespaces
                                    sv[0] = sv[0].Substring(0,iLastChar);//Store accumulated ID
                                    iLastChar = 0;//Flush last char
                                    iQuot = 0;//Flush quotas
                                    goto lNextChar;
                                }
                                if (iQuot == 1)
                                {
                                    //Value is coded with quots - analyze possinle out-of-quots chars
                                    if (b == 34) //Quot
                                    {
                                        iState1 = 2;//Now we are inside quots
                                        if (bPrev == 34)//Check for double quots - this is encoding of quot in value
                                        {
                                            sv[iState2] += "\"";//Add qout to value
                                            iLastChar = sv[iState2].Length;//Update last-char position
                                        }
                                        goto lNextChar;
                                    }
                                    if (b == 114) //Char 'r' - encoding of CR+LF in value
                                    {
                                        sv[iState2] += "\r\n";//Add CR and LF to value
                                        iLastChar = sv[iState2].Length;//Update last-char position
                                        goto lNextChar;
                                    }

                                    //Otherwise outside quots are allowed only hexadecimal representation of chars
                                    if (i == n) goto lNextChar;// Last char - it can not not hold hex of char
                                    
                                    k = OQConvertTools.hex2int(sLine.Substring(i, 2)); //Try to decode two chars as hex into byte

                                    if (k==-1) goto lNextChar; //If decoding fails - skip this char
                                    sv[iState2] += Convert.ToChar(k);//If ok - add char with decoded code to value
                                    iLastChar = sv[iState2].Length;//Update last-char position
                                    bPrev = -2;
                                    i++;
                                    goto lNextChar2;
                                }
                                iLastChar = sv[iState2].Length + 1;//Update last-char position (+1 - to count next source-code-line, where char will be added)
                            }
                            sv[iState2] +=  b;//Add char to value
                        }
                        if (iState1 == 2)
                        {   //Inside quots
                            if (b == 34)
                            {//Quot
                                iState1 = 1;//Now we are outside of quots
                            }
                            else
                            {
                                sv[iState2] += b;//Add char to value
                                iLastChar = sv[iState2].Length;//Update last-char position
                            }
                        }
lNextChar:
                        bPrev = b;//Update previos char (required only for testing double-quots)
lNextChar2:;
                    }
                    
                    if (iState2 == 0) goto lNext;//No equal char - skip line

                    if (iState1 == 1) //Only if quot was closed
                    {
                        sv[1] = sv[1].Substring(0, iLastChar);//Trim last whitespaces
                    }
                    
                    //Store decoded values
                    uIniItems[iNumIniItems].sID = sv[0];
                    uIniItems[iNumIniItems].sValue = sv[1];
                    iNumIniItems++;
                    
                    //Realloc arrays, if ended
                    if (iNumIniItems >= iAllocIniItems)
                    {
                        iAllocIniItems *= 2;
                        IniItem[] uNew = new IniItem[iAllocIniItems];
                        uIniItems.CopyTo(uNew, 0);
                        uIniItems = uNew;
                    }
lNext:;
                }
                hReader.Close();//Close file
            }
            catch
            {
                //On any exeption - exit loading
                return;
            }
        }

        /// <summary>Get Value by known ID (ID)</summary>
        public string GetItemValue(string sID)
        {
            return GetItemValue(sID, "");
        }

        /// <summary>Get Value by known ID (ID, default value)</summary>
        public string GetItemValue(string sID, string sDefautValue)
        {
            int i;
            for (i = 0; i < iNumIniItems; i++)//Iterate thru all ID-Value pairs
                if (uIniItems[i].sID == sID)//Compare ID
                    return uIniItems[i].sValue;//If ID found - return Value
            return sDefautValue;//If ID is not found - return defalut value

        }
    }
}
