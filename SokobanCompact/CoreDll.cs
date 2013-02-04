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

using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>Class to prevent system from suspending</summary>
    public static class CoreDll
    {
        ///<summary>Power on / wake up device</summary>
        public const int POWER_STATE_ON = 0x00010000;

        ///<summary>Turn off device</summary>
        public const int POWER_STATE_OFF = 0x00020000;

        ///<summary>Suspend device</summary>
        public const int POWER_STATE_SUSPEND = 0x00200000;

        ///<summary>Reset device</summary>
        public const int POWER_STATE_RESET = 0x00800000;

        ///<summary>State transfer is urgent</summary>
        public const int POWER_FORCE = 4096;

        /// <summary>Reset system suspend timers</summary>
        [DllImport("CoreDll.dll")]
        public static extern void SystemIdleTimerReset();

        /// <summary>Prevent system backlight off</summary>
        [DllImport("coredll.dll", SetLastError = true)]
        public static extern int SetSystemPowerState(string psState, int StateFlags, int Options);

        /// <summary>Prevent system from suspending</summary>
        public static void DontSleep()
        {
            SetSystemPowerState(null, POWER_STATE_ON, POWER_FORCE);
            SystemIdleTimerReset();
        }

        /// <summary>Sets the specified window's show state</summary>
        [DllImport("coredll.dll")]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>Minimize window</summary>
        public const int SW_MINIMIZED = 6;

        /// <summary>Advanced ShowWindow</summary>
        public static void ShowWindowEx(IntPtr hWnd, int nCmdShow)
        {
            ShowWindow(hWnd, nCmdShow);
        }

        /// <summary>Minimize window</summary>
        public static void MinimizeWindow(Form hForm)
        {
            ShowWindow(hForm.Handle, SW_MINIMIZED);
        }
        
    }
}
