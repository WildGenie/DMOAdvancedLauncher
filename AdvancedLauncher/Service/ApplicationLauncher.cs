﻿// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// ======================================================================

using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Management;


namespace AdvancedLauncher.Service
{
    /// -------------------------------------------------------------------------------------------------
    /// <summary> Application Running Helper. </summary>
    /// -------------------------------------------------------------------------------------------------
    public static class ApplicationLauncher
    {
        /// -------------------------------------------------------------------------------------------------
        /// <summary> Execute process with AppLocale if it exists in system and allowed in SettingsProvider
        ///           or execute it directly </summary>
        /// <param name="program">Path to program</param>
        /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
        /// -------------------------------------------------------------------------------------------------
        public static bool Execute(string program, bool UseAL)
        {
            return Execute(program, string.Empty, UseAL);
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary> Execute process with AppLocale if it exists in system and allowed in SettingsProvider
        ///           or execute it directly </summary>
        /// <param name="program">Path to program</param>
        /// <param name="args">Arguments</param>
        /// <param name="UseAL">Use AppLocale</param>
        /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
        /// -------------------------------------------------------------------------------------------------
        public static bool Execute(string program, string args, bool UseAL)
        {
            if (File.Exists(program))
            {
                if (UseAL && !ParentProcessUtilities.GetParentProcess().ProcessName.ToLower().Equals("steam"))
                {
                    if (!executeApplocale(program, args))
                    {
                        if (StartProc(program, args))
                            return true;
                    }
                    else
                        return true;
                }
                else
                {
                    if (StartProc(program, args))
                        return true;
                }
            }
            return false;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary> Execute process </summary>
        /// <param name="program">Path to program</param>
        /// <param name="commandline">Command line</param>
        /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
        /// -------------------------------------------------------------------------------------------------
        public static bool StartProc(string program, string commandline)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = program;
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(program);
                proc.StartInfo.Arguments = commandline;
                proc.Start();
            }
            catch { return false; }
            return true;
        }

        /// -------------------------------------------------------------------------------------------------
        /// <summary> Execute process with AppLocale if it exists in system and allowed in SettingsProvider </summary>
        /// <param name="program">Path to program</param>
        /// <param name="args">Command line</param>
        /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
        /// -------------------------------------------------------------------------------------------------
        static bool executeApplocale(string program, string args)
        {
            string apploc_dir = System.Environment.GetEnvironmentVariable("windir") + "\\apppatch\\AppLoc.exe";

            if (!IsALSupported)
                return false;

            if (!string.IsNullOrEmpty(args))
                StartProc(apploc_dir, "\"" + program + "\" \"" + args + "\" \"/L0412\"");
            else
                StartProc(apploc_dir, "\"" + program + "\" \"/L0412\"");
            return true;
        }

        public static bool IsALInstalled { set; get; }
        public static bool IsKoreanSupported { set; get; }
        public static bool IsALSupported
        {
            get
            {
                string apploc_dir = System.Environment.GetEnvironmentVariable("windir") + "\\apppatch\\AppLoc.exe";
                IsALInstalled = File.Exists(apploc_dir);

                IsKoreanSupported = false;
                CultureInfo[] cis = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures);
                foreach (CultureInfo ci in cis)
                    if (ci.TwoLetterISOLanguageName == "ko")
                    {
                        IsKoreanSupported = true;
                        break;
                    }
                if (!IsALInstalled || !IsKoreanSupported)
                    return false;
                return true;
            }
        }
    }
}