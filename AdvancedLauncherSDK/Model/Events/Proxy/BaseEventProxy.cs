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

namespace AdvancedLauncher.SDK.Model.Events.Proxy {

    /// <summary>
    /// Configuration changed event proxy for <see cref="BaseEventHandler"/>
    /// </summary>
    /// <seealso cref="EventProxy{T}"/>
    public class BaseEventProxy : EventProxy<BaseEventArgs> {

        private event BaseEventHandler EventHandler;

        /// <summary>
        /// Initializes a new instance of <see cref="BaseEventProxy"/> for specified <see cref="BaseEventHandler"/>.
        /// </summary>
        /// <param name="action">Event action</param>
        public BaseEventProxy(BaseEventHandler action) {
            EventHandler += action;
        }

        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        public override void Handler(object sender, BaseEventArgs e) {
            if (EventHandler != null) {
                EventHandler(this, e);
            }
        }
    }
}