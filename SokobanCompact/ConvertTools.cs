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

namespace System
{
    /// <summary>OverQuantum tools for converting values</summary>
    public static class OQConvertTools
    {
        /// <summary>Convert string into int without exceptions, return 0 on errors (source string)</summary>
        public static int string2int(string sSource)
        {
            try
            {
                //Try basic convertor
                return int.Parse(sSource);
            }
            catch
            {
                //If basic convertor fails, return 0
                return 0;
            }
        }

        /// <summary>Convert string into bool without exceptions, return false on errors (source string)</summary>
        public static bool string2bool(string sSource)
        {
            try
            {
                //Try basic convertor
                return bool.Parse(sSource);
            }
            catch
            {
                //If basic convertor fails, return false
                return false;
            }
        }

        /// <summary>Convert hexadecimal string to int, return -1 on errors (source string)</summary>
        public static int hex2int(string str)
        {
            try
            {
                //Try basic convertor
                return Convert.ToInt32(str, 16);
            }
            catch
            {
                //If basic convertor fails, return -1
                return -1;
            }
        }
        /// <summary>Convert char to hexadecimal two-chars string (source char)</summary>
        public static string char2hex(char cChar)
        {
            try
            {
                //Return 0F, EF, 17 and so on
                return ((int)cChar).ToString("X").PadLeft(2, '0');
            }
            catch
            {
                //If something goes wrong, return void string
                return "";
            }
        }

    }

}