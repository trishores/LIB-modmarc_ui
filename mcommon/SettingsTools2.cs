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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
//
namespace marc_common
{
    internal static class c_SettingsTools2
    {
        #region init

        [c_GlobalSetting]
        private static string organizationCode;
        [c_GlobalSetting]
        private static string workspaceFolder;
        [c_ProductSetting]
        private static string screenCoordinates;

        private static Timer v_timer = new Timer();

        public static void m_Init()
        {
            v_timer.Elapsed += (v_s, v_e) =>
            {
                try { m_SaveSettings_Sync(); }
                catch { return; }
            };
            v_timer.AutoReset = false;   // one-shot timer event.

            try { m_RestoreSettings_Sync(); }
            catch { return; }
        }

        #endregion

        #region timer

        private static void m_DelayedSaveSettings()
        {
            v_timer.Interval = 1000; // resets timer interval if previously triggered but not elapsed.
            v_timer.Enabled = true;
        }

        #endregion

        #region properties

        internal static string m_OrganizationCode
        {
            get { return organizationCode; }
            set
            {
                organizationCode = value;
                m_DelayedSaveSettings();
            }
        }

        internal static string m_WorkspaceDir
        {
            get { return workspaceFolder; }
            set
            {
                workspaceFolder = value;
                m_DelayedSaveSettings();
            }
        }

        internal static Point? m_Coordinate
        {
            get
            {
                var v_parts = screenCoordinates?.Split(',');
                if (v_parts?.Length != 2) return null;
                if (!int.TryParse(v_parts[0], out int v_x)) return null;
                if (!int.TryParse(v_parts[1], out int v_y)) return null;
                return new Point(v_x, v_y);
            }
            set
            {
                screenCoordinates = $"{((Point)value).X.ToString()},{((Point)value).Y.ToString()}";
                m_DelayedSaveSettings();
            }
        }

        #endregion

        #region settings path

        private static string m_SettingsFilePath
        {
            get
            {
                var v_settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $@"{c_AssemblyTools.v_ProductName}\settings.xml");
                return v_settingsFilePath;
            }
        }

        private static bool m_CreateSettingsFolder()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(m_SettingsFilePath));
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #region restore settings

        private static void m_RestoreSettings_Sync()
        {
            if (!File.Exists(m_SettingsFilePath)) return;

            XDocument v_xdoc = null;

            // get the existing setting file or create a new one:
            try
            {
                string v_xmlStr = null;
                if (File.Exists(m_SettingsFilePath))
                {
                    v_xmlStr = File.ReadAllText(m_SettingsFilePath);
                }
                v_xdoc = XDocument.Parse(v_xmlStr);
            }
            catch { }

            if (v_xdoc == null) return;

            var v_frmNodes = v_xdoc.Root.Elements().ToList();
            foreach (var v_field in m_GetGlobalSettingFields())
            {
                var v_val = v_frmNodes.SingleOrDefault(v_x => v_x.Name.LocalName == v_field.Name && (v_x.Attribute("product") == null || v_x.Attribute("product").Value.m_EqualsAnyXx(c_AssemblyTools.v_ProductName)))?.Value;
                if (v_val.m_IsEmpty()) continue;
                v_field.SetValue(null, v_val);
            }
            foreach (var v_field in m_GetProductSettingFields())
            {
                var v_val = v_frmNodes.SingleOrDefault(v_x => v_x.Name.LocalName == v_field.Name && (v_x.Attribute("product") == null || v_x.Attribute("product").Value.m_EqualsAnyXx(c_AssemblyTools.v_ProductName)))?.Value;
                if (v_val.m_IsEmpty()) continue;
                v_field.SetValue(null, v_val);
            }
        }

        #endregion

        #region save settings

        private static void m_SaveSettings_Sync()
        {
            //if (!File.Exists(m_SettingsFilePath)) return;

            // get the existing setting file or create a new one:
            XDocument v_xdoc = null; 
            try
            {
                string v_xmlStr = null; 
                if (File.Exists(m_SettingsFilePath))
                {
                    v_xmlStr = File.ReadAllText(m_SettingsFilePath);
                }
                if (v_xmlStr.m_IsNotEmpty()) v_xdoc = XDocument.Parse(v_xmlStr);
            }
            catch { }
            
            if (v_xdoc == null) v_xdoc = new XDocument(new XElement("root", new XAttribute("datatype", "setting"), new XAttribute("product", "predictivebib suite"), new XAttribute("version", "1.0.0")));

            foreach (var v_field in m_GetGlobalSettingFields())
            {
                var v_fieldNode = v_xdoc.Root.Element(v_field.Name);
                if (v_fieldNode != null) v_fieldNode.Value = v_field.GetValue(null)?.ToString() ?? "";
                else v_xdoc.Root.Add(new XElement(v_field.Name, v_field.GetValue(null)?.ToString() ?? ""));
            }

            foreach (var v_field in m_GetProductSettingFields())
            {
                var v_fieldNode = v_xdoc.Root.Elements(v_field.Name).SingleOrDefault(v_x => v_x.Attribute("product") == null || v_x.Attribute("product").Value.m_EqualsAnyXx(c_AssemblyTools.v_ProductName));
                if (v_fieldNode != null) v_fieldNode.Value = v_field.GetValue(null)?.ToString() ?? "";
                else v_xdoc.Root.Add(new XElement(v_field.Name, new XAttribute("product", c_AssemblyTools.v_ProductName), v_field.GetValue(null)?.ToString() ?? ""));
            }

            if (m_CreateSettingsFolder())
            {
                try
                {
                    v_xdoc.m_SaveToFile(m_SettingsFilePath);
                }
                catch
                {
                }
            }
        }

        #endregion

        #region misc

        internal static string m_EncodePwd(string v_decodeData)
        {
            var v_plainTextBytes = Encoding.UTF8.GetBytes(v_decodeData + Environment.MachineName);
            return Convert.ToBase64String(v_plainTextBytes);
        }

        internal static string m_DecodePwd(string v_encodedData)
        {
            if (v_encodedData.m_IsEmpty()) return "";

            var v_base64EncodedBytes = Convert.FromBase64String(v_encodedData);
            var v_decodedData = Encoding.UTF8.GetString(v_base64EncodedBytes);
            return v_decodedData.Substring(0, v_decodedData.Length - Environment.MachineName.Length);
        }

        private static List<FieldInfo> m_GetGlobalSettingFields()
        {
            var v_fields = typeof(c_SettingsTools2).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Where(v_x => v_x.GetCustomAttributes<c_GlobalSetting>().Any()).ToList();
            return v_fields;
        }

        private static List<FieldInfo> m_GetProductSettingFields()
        {
            var v_fields = typeof(c_SettingsTools2).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Where(v_x => v_x.GetCustomAttributes<c_ProductSetting>().Any()).ToList();
            return v_fields;
        }

        #region attribute

        [AttributeUsage(AttributeTargets.Field)]
        internal class c_GlobalSetting : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Field)]
        internal class c_ProductSetting : Attribute
        {
        }

        #endregion

        #endregion
    }
}