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
using System.Windows.Controls;

namespace AdvancedLauncher.Validators {

    internal class NameValidationRule : ValidationRule {

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {
            if (value.ToString().IndexOfAny("(*^%@)&^@#><>!.,$|`~?:\":\\/';=-+_".ToCharArray()) != -1) {
                return new ValidationResult(false, null);
            }

            foreach (char chr in value.ToString()) {
                if (Char.IsWhiteSpace(chr) || Char.IsControl(chr)) {
                    return new ValidationResult(false, null);
                }
            }
            return new ValidationResult(true, null);
        }
    }
}