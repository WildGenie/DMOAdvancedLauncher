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

using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace AdvancedLauncher.Validators {

    internal class URLValidationRule : ValidationRule {

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {
            Regex r = new Regex(@"^(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*$");
            while (r.Match(value.ToString()).Success) {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, null);
        }
    }
}