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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace System
{
    /// <summary>Power line status.</summary>
    public enum PowerLineStatus : byte
    {
        /// <summary>AC power is offline.</summary>
        Offline = 0x00,
        /// <summary>AC power is online.</summary>
        Online = 0x01,
        /// <summary>Unit is on backup power.</summary>
        BackupPower = 0x02,
        /// <summary>AC line status is unknown.</summary>
        Unknown = 0xFF,
    }

    /// <summary>Defines identifiers that indicate the current battery charge level or charging state information.</summary>
    [Flags()]
    public enum BatteryChargeStatus : byte
    {
        /// <summary>Indicates a high level of battery charge.</summary>
        High = 0x01,
        /// <summary>Indicates a low level of battery charge.</summary>
        Low = 0x02,
        /// <summary>Indicates a critically low level of battery charge.</summary>
        Critical = 0x04,
        /// <summary>Indicates a battery is charging.</summary>
        Charging = 0x08,
        /// <summary>Indicates that no battery is present.</summary>
        NoSystemBattery = 0x80,
        /// <summary>Indicates an unknown battery condition.</summary>
        Unknown = 0xFF,
    }

    /// <summary>Indicates current system power status information.</summary>
    public class PowerStatus
    {
#pragma warning disable 0169, 0649
        private byte mACLineStatus;
        private byte mBatteryFlag;
        private byte mBatteryLifePercent;
        private byte mReserved1;
        private uint mBatteryLifeTime;
        private uint mBatteryFullLifeTime;
        private byte mReserved2;
        private byte mBackupBatteryFlag;
        private byte mBackupBatteryLifePercent;
        private byte mReserved3;
        private uint mBackupBatteryLifeTime;
        private uint mBackupBatteryFullLifeTime;

        /// <summary>Unknown result</summary>
        public const uint BATTERY_FLAG_UNKNOWN_UINT = 0xFFFFFFFF;

        /// <summary>Unknown result</summary>
        public const byte BATTERY_FLAG_UNKNOWN_BYTE = 0xFF;

        internal PowerStatus()
        {
            Update(true);
        }

        /// <summary>AC power status.</summary>
        public PowerLineStatus PowerLineStatus
        {
            get
            {
                return (PowerLineStatus)mACLineStatus;
            }
        }

        /// <summary>Gets the current battery charge status.</summary>
        public BatteryChargeStatus BatteryChargeStatus
        {
            get
            {
                return (BatteryChargeStatus)mBatteryFlag;
            }
        }

        /// <summary>Gets the approximate percentage of full battery time remaining.</summary>
        /// <remarks>TThe approximate percentage, from 0 to 100, of full battery time remaining, or 255 if the percentage is unknown.</remarks>
        public byte BatteryLifePercent
        {
            get
            {
                return mBatteryLifePercent;
            }
        }

        /// <summary>Gets the approximate number of seconds of battery time remaining.</summary>
        /// <value>The approximate number of seconds of battery life remaining, or -1 if the approximate remaining battery life is unknown.</value>
        public uint BatteryLifeRemaining
        {
            get
            {
                return mBatteryLifeTime;
            }
        }

        /// <summary>Gets the reported full charge lifetime of the primary battery power source in seconds.</summary>
        /// <value>The reported number of seconds of battery life available when the battery is fullly charged, or -1 if the battery life is unknown.</value>
        public uint BatteryFullLifeTime
        {
            get
            {
                return mBatteryFullLifeTime;
            }
        }

        /// <summary>Gets the backup battery charge status.</summary>
        public BatteryChargeStatus BackupBatteryChargeStatus
        {
            get
            {
                return (BatteryChargeStatus)mBackupBatteryFlag;
            }
        }

        /// <summary>Percentage of full backup battery charge remaining. Must be in the range 0 to 100.</summary>
        public byte BackupBatteryLifePercent
        {
            get
            {
                return mBackupBatteryLifePercent;
            }
        }

        /// <summary>Number of seconds of backup battery life remaining.</summary>
        public uint BackupBatteryLifeRemaining
        {
            get
            {
                return mBackupBatteryLifeTime;
            }
        }

        /// <summary>Number of seconds of backup battery life when at full charge. Or -1 If unknown.</summary>
        public uint BackupBatteryFullLifeTime
        {
            get
            {
                return mBackupBatteryFullLifeTime;
            }
        }

        /// <summary>Update with forcing flag</summary>
        public bool Update(bool update)
        {
            if (GetSystemPowerStatusEx(this, update) == false)
            {
                Debug.Write(Marshal.GetLastWin32Error());
                return false;
            }
            return true;
        }

        /// <summary>Simply update without forcing</summary>
        public bool Update()
        {
            return Update(false);
        }

        [DllImport("coredll.dll", EntryPoint = "GetSystemPowerStatusEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetSystemPowerStatusEx(PowerStatus pStatus, bool fUpdate);
    }
}
