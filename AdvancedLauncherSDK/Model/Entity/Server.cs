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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AdvancedLauncher.SDK.Model.Entity {

    public class Server : BaseEntity {

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Server() {
            Guilds = new List<Guild>();
        }

        public Server(Server server) {
            this.Identifier = server.Identifier;
            this.Name = server.Name;
            this.Type = server.Type;
        }

        public enum ServerType {
            KDMO = 1, KDMO_IMBC = 2, GDMO = 3, ADMO = 4
        }

        [Required]
        public byte Identifier {
            get;
            set;
        }

        [Required]
        [StringLength(25)]
        public string Name {
            get;
            set;
        }

        [Required]
        public ServerType Type {
            get;
            set;
        }

        public virtual ICollection<Guild> Guilds {
            get;
            set;
        }

        public override string ToString() {
            return Name;
        }
    }
}