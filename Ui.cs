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
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

[assembly: AssemblyKeyFile(@"..\..\signing\keyFile.snk")]
[assembly: InternalsVisibleTo("modmarc_ui_test, PublicKey=002400000480000094000000060200000024000052534131000400000100010041a7d83a15933ab87d7cf17413f5b31f4d60c037154f9bceff6431ff423cdb8c045c4e34c9aec80924794af446abeb2f4dbea96f2fb7c6a7cef8cfc629e83f3adeb3caa00675729dbfd8bc0786af2d621fa92ce8963fab216c9e0dedb20909ab5e1f0c69e2e61ecdb001c17d5fd57110a78cce5bd44049ba716d64f7ae21d7d9")]

namespace modmarc_ui
{
    static class c_Ui
    {
        internal static c_FrmMain v_frmMain;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] v_args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (v_args.Length == 1 && File.Exists(v_args[0])) v_frmMain = new c_FrmMain(v_mrcOpenFilePathArg: v_args[0]);
            else v_frmMain = new c_FrmMain();

            using (v_frmMain)
            {
                Application.Run(v_frmMain);
            }
        }

        #region global page events

        // Set focus to panel/label on a mouse-click of those controls.
        // This restores TextBoxX hints.
        internal static void m_PanelFocus(Control v_control)
        {
            foreach (Control v_ctrl in v_control.Controls)
            {
                if (v_ctrl.HasChildren)
                {
                    m_PanelFocus(v_ctrl);
                }
                if (v_ctrl is Panel)
                {
                    v_ctrl.Click += (v_s, v_e) => { v_ctrl.Focus(); ButtonTip.m_HideAllTips(); };
                }
                else if (v_ctrl is GroupBox)
                {
                    v_ctrl.Click += (v_s, v_e) => { v_ctrl.Focus(); ButtonTip.m_HideAllTips(); };
                }
                else if (v_ctrl is Label)
                {
                    if (v_ctrl is LinkLabel) continue;
                    v_ctrl.Click += (v_s, v_e) => { v_ctrl.Focus(); ButtonTip.m_HideAllTips(); };
                }
                else if (v_ctrl is PictureBox)
                {
                    v_ctrl.Click += (v_s, v_e) => { v_ctrl.Focus(); ButtonTip.m_HideAllTips(); };
                }
            }
        }

        #endregion
    }
}
