/*
Copyright (C) 2020-2021 Tris Shores

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text;

namespace marc_common
{
    internal static class c_MarcSymbols
    {
        internal static class c_Human
        {
            internal static char v_Delimiter => 'ǂ';  // ǂ or $ or |
            internal static char v_FillCharacter => '|';

            internal static char v_Blank = '\u2423';  // ␣
        }

        internal static class c_Machine
        {
            // note: '\u00NN' hex value may also be used.
            internal static char v_Delimiter => (char)31;   // subfield delimiter (ascii: 31, hex 1F, unit separator)
            internal static char v_FieldTerminator => (char)30;   // end of field (ascii: 30, hex 1E, record separator)
            internal static char v_RecordTerminator => (char)29;   // end of record (ascii: 29, hex 1D,  group separator)

            internal static Encoding v_UTF8EncodingNoBom = new UTF8Encoding(false);
        }

        internal static class c_Control
        {
            internal static char v_InvisibleChar => (char)129;    // delineates subfield joins (ascii: 129, hex 81, control)
        }
    }
}
