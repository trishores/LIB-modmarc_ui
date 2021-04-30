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
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace marc_common
{
    partial class c_FrmAbout : FormX
    {
        #region init

        private Form v_Owner;

        internal c_FrmAbout(Form v_owner)
        {
            InitializeComponent();

            // defaults:
            v_Owner = v_owner;
            this.ShowInTaskbar = true;
            base.m_AssignIconTitle(this, "About");
            v_labelProductName.Text = c_AssemblyTools.v_ProductName;
            v_labelVersion.Text = c_AssemblyTools.v_Version;
            v_lblCopyright.Text = c_AssemblyTools.v_Copyright;
            v_toolTip.SetToolTip(v_pbxLogo, "Based on artwork by OpenClipart-Vectors");
            v_lblWebsite.Text = $"https://libsquid.org";
            v_lblLicense.Text = "GNU General Public License, Version 3";

            // events:
            v_lblWebsite.Click += m_LnkVendorWebsite_LinkClicked;
            v_lblLicense.Click += m_LnkLicenseWebsite_LinkClicked;
            Load += m_Load;
        }

        private void m_Load(object v_sender, EventArgs v_e)
        {
            // set location:
            StartPosition = FormStartPosition.Manual;   // set position manually (below center of parent).
            this.Location = new Point(v_Owner.Left + (v_Owner.Width - this.Width) / 2, v_Owner.Top + (v_Owner.Height - this.Height) - 100);
        }

        #endregion

        private void m_LnkVendorWebsite_LinkClicked(object v_sender, EventArgs v_e)
        {
            try
            {
                c_HelpTools.m_OpenUrl(v_lblWebsite.Text);
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private void m_LnkLicenseWebsite_LinkClicked(object v_sender, EventArgs v_e)
        {
            try
            {
                c_HelpTools.m_OpenUrl("https://www.gnu.org/licenses/gpl-3.0.txt");
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}
