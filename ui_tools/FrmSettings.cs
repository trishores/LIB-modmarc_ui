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

using marc_common;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace modmarc_ui
{
    internal partial class c_FrmSettings : FormX
    {
        internal c_FrmSettings()
        {
            InitializeComponent();

            // defaults:
            base.m_AssignIconTitle(this, "Settings");
            StartPosition = FormStartPosition.CenterParent;
            c_Ui.m_PanelFocus(this);

            // event handlers:
            AcceptButton = v_btnClose; // triggers form close, but field data persists on relaunch.
            VisibleChanged += m_FrmLicense_VisibleChanged;    // triggers on form close/relaunch.
            txtOrganizationCode.TextChanged += m_TxtOrganizationCode_TextChanged;
            lnkMarcOrganizationCode.Click += m_LnkCatOrganization_Click;
        }

        private void m_TxtOrganizationCode_TextChanged(object v_sender, EventArgs v_e)
        {
            c_SettingsTools2.m_OrganizationCode = txtOrganizationCode.Text;
        }

        private void m_LnkCatOrganization_Click(object v_sender, EventArgs v_e)
        {
            Process.Start("https://www.loc.gov/marc/organizations/org-search.php");
        }

        private void m_FrmLicense_VisibleChanged(object v_sender, EventArgs v_e)
        {
            if (Visible) m_OnLaunch();
        }

        private void m_OnLaunch()
        {
            // init:
            txtOrganizationCode.Text = c_SettingsTools2.m_OrganizationCode;
        }
    }
}