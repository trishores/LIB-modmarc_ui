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
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
#if PREDICTIVEBIB_UI
using predictivebib_ui;
#elif MODMARC_UI
using modmarc_ui;
#elif VIEWMARC_UI
using viewmarc_ui;
#elif BIBMARC_UI
using bibframe_ui;
#endif

namespace marc_common
{
    internal partial class c_FrmMessageBox : FormX
    {
        #region init

        //internal enum c_Buttons { Ok, OkCancel, YesNo, YesNoCancel, SaveNotSaveCancel };

        string[] v_buttons;
        string v_dialogResult = "Cancel";
        private WebBrowser v_webBrowser;
        private Form v_Owner;
        internal static c_FrmMessageBox v_Msgbox;

        internal c_FrmMessageBox(Form v_owner)
        {
            InitializeComponent();

            // defaults:
            v_Owner = v_owner;
            this.ShowInTaskbar = true;
            base.m_AssignIconTitle(this, "Message");

            // event handlers:
            v_button1.Click += m_Btn1Action_Click;
            v_button2.Click += m_Btn2Action_Click;
            v_button3.Click += m_Btn3Action_Click;
            this.Load += m_Load;
        }

        private void m_Load(object v_sender, EventArgs v_e)
        {
            // set location:
            StartPosition = FormStartPosition.Manual;   // set position manually (below center of parent).
            this.Location = new Point(v_Owner.Left + (v_Owner.Width - this.Width) / 2, v_Owner.Top + (v_Owner.Height - this.Height) - 100);
        }

        #endregion

        internal static c_FrmMessageBox m_Singleton(Form v_owner = null)
        {
            //Console.WriteLine(v_owner?.Name ?? "null");

#if PREDICTIVEBIB_UI
            if (v_owner == null || v_owner.Name.StartsWith("Pg")) v_owner = predictivebib_ui.c_Ui.v_frmMain; // pages are not centered on frmMain.
#elif MODMARC_UI
            if (v_owner == null || v_owner.Name.StartsWith("Pg")) v_owner = modmarc_ui.c_Ui.v_frmMain; // pages are not centered on frmMain.
#elif VIEWMARC_UI
            if (v_owner == null || v_owner.Name.StartsWith("Pg")) v_owner = viewmarc_ui.c_Ui.v_frmMain; // pages are not centered on frmMain.
#endif
            //Console.WriteLine(v_owner?.Name ?? "null");

            if (v_Msgbox == null) v_Msgbox = new c_FrmMessageBox(v_owner);
            else v_Msgbox.v_Owner = v_owner;

            return v_Msgbox;
        }

        private WebBrowser m_GetWebBrowser()
        {
            if (v_webBrowser != null) return v_webBrowser;
            v_webBrowser = new WebBrowser();
            v_webBrowser.Size = v_lblMessage.Size;
            v_webBrowser.Location = v_lblMessage.Location;
            v_webBrowser.ScrollBarsEnabled = false;
            v_webBrowser.AllowNavigation = true;
            v_webBrowser.AllowWebBrowserDrop = false;
            v_webBrowser.WebBrowserShortcutsEnabled = false;
            v_webBrowser.ScriptErrorsSuppressed = true;
            ((Control)v_webBrowser).Enabled = false;
            Controls.Add(v_webBrowser);
            return v_webBrowser;
        }

        internal string m_OpenDialog(string v_str, string[] v_buttons, Image v_image = null)
        {
#if PREDICTIVEBIB_UI
            Text = "PredictiveBIB";
#elif MODMARC_UI
            Text = "ModMARC";
#elif VIEWMARC_UI
            Text = "ViewMARC";
#endif
            this.v_buttons = v_buttons;
            var v_btnSpacer = 10;
            var v_btnWidth = v_button1.Width;

            if (v_str.StartsWith("<html"))
            {
                m_GetWebBrowser().DocumentText = v_str;
                m_GetWebBrowser().BringToFront();
            }
            else
            {
                v_lblMessage.Text = v_str;
                if (v_webBrowser != null) m_GetWebBrowser().SendToBack();  // avoid unnecessary instantiation of webbrowser.
                Size = new Size(MinimumSize.Width, MinimumSize.Height + v_lblMessage.Height - v_lblMessage.MinimumSize.Height);   // adjust height to fit text.
            }

            if (v_buttons.Length == 1)
            {
                v_button1.Visible = true;
                v_button2.Visible = false;
                v_button3.Visible = false;

                v_button1.Text = v_buttons[0];

                v_button1.Left = this.ClientSize.Width / 2 - v_btnWidth / 2;
            }
            if (v_buttons.Length == 2)
            {
                v_button1.Visible = true;
                v_button2.Visible = true;
                v_button3.Visible = false;

                v_button1.Text = v_buttons[0];
                v_button2.Text = v_buttons[1];

                v_button1.Left = this.ClientSize.Width / 2 - v_btnSpacer - v_btnWidth;
                v_button2.Left = this.ClientSize.Width / 2 + v_btnSpacer;
            }
            else if (v_buttons.Length == 3)
            {
                v_button1.Visible = true;
                v_button2.Visible = true;
                v_button3.Visible = true;

                v_button1.Text = v_buttons[0];
                v_button2.Text = v_buttons[1];
                v_button3.Text = v_buttons[2];

                v_button1.Left = this.ClientSize.Width / 2 - v_btnWidth / 2 - v_btnSpacer - v_btnWidth;
                v_button2.Left = this.ClientSize.Width / 2 - v_btnWidth / 2;
                v_button3.Left = this.ClientSize.Width / 2 + v_btnWidth / 2 + v_btnSpacer;
            }

            v_pbxIcon.Image = v_image;

            ShowDialog();
            v_Owner.Refresh();   // refresh form to avoid popup ghosting.

            return v_dialogResult;
        }

        private void m_Btn1Action_Click(object v_sender, EventArgs v_e)
        {
            v_dialogResult = v_buttons[0];
            this.DialogResult = DialogResult.OK;
            Hide();
        }

        private void m_Btn2Action_Click(object v_sender, EventArgs v_e)
        {
            v_dialogResult = v_buttons[1];
            this.DialogResult = DialogResult.OK;
            Hide();
        }

        private void m_Btn3Action_Click(object v_sender, EventArgs v_e)
        {
            v_dialogResult = v_buttons[2];
            this.DialogResult = DialogResult.OK;
            Hide();
        }
    }
}
