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

using System.Diagnostics;

namespace marc_common
{
    internal static class c_HelpTools
    {
#if PREDICTIVEBIB_UI
        internal static void m_PageHelp(string v_productName, string v_pageName)
        {
#if DEBUG
            m_OpenUrl($"http://localhost:1313/help/{v_productName}/{v_pageName}");
#else
            m_OpenUrl($"http://{c_Constants.v_CompanyUrl}/help/{v_productName}/{v_pageName}");
#endif
        }
#endif

        internal static void m_OpenUrl(string v_url)
        {
            Process.Start(new ProcessStartInfo(v_url) { UseShellExecute = true });
        }
    }
}