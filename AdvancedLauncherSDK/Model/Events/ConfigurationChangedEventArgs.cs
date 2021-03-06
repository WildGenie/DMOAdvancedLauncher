﻿// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2015 Ilya Egorov (goldrenard@gmail.com)

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

using AdvancedLauncher.SDK.Management.Configuration;

namespace AdvancedLauncher.SDK.Model.Events {

    /// <summary>
    /// Configuration change event handler
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    public delegate void ConfigurationChangedEventHandler(object sender, ConfigurationChangedEventArgs e);

    /// <summary>
    /// Configuration change event args
    /// </summary>
    public class ConfigurationChangedEventArgs : BaseEventArgs {

        /// <summary>
        /// Gets related configuration
        /// </summary>
        public IConfiguration Configuration {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurationChangedEventArgs"/> for specified <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="Configuration">Configuration</param>
        public ConfigurationChangedEventArgs(IConfiguration Configuration) {
            this.Configuration = Configuration;
        }
    }
}