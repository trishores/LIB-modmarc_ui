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

using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace marc_common
{
    internal static class c_AssemblyTools
    {
        //internal static string v_Title
        //{
        //    get
        //    {
        //        var v_customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        //        if ((uint)v_customAttributes.Length > 0U)
        //        {
        //            var v_assemblyTitleAttribute = (AssemblyTitleAttribute)v_customAttributes[0];
        //            if (v_assemblyTitleAttribute.Title.Length > 0)
        //                return v_assemblyTitleAttribute.Title;
        //        }
        //        return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
        //    }
        //}

        internal static string v_Version
        {
            get
            {
                var v_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var v_match = Regex.Match(v_version, @"(\d+\.\d+\.\d+).\d+");
                if (v_match.Success && v_match.Groups.Count > 1) 
                    return v_match.Groups[1].Value;
                return "unknown";
            }
        }

        internal static string v_Description
        {
            get
            {
                var v_customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return v_customAttributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute)v_customAttributes[0]).Description;
            }
        }

        internal static string v_ProductName
        {
            get
            {
                var v_customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return v_customAttributes.Length == 0 ? "" : ((AssemblyProductAttribute)v_customAttributes[0]).Product;
            }
        }

        internal static string v_Copyright
        {
            get
            {
                var v_customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return v_customAttributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)v_customAttributes[0]).Copyright;
            }
        }

        internal static string v_CompanyName
        {
            get
            {
                var v_customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return v_customAttributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)v_customAttributes[0]).Company;
            }
        }
    }
}
