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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace marc_common
{
    public partial class ButtonTip : UserControl
    {
        private c_PopupPanel v_popupPanel;
        private static List<c_PopupPanel> v_popupPanelList = new List<c_PopupPanel>();
        private string v_tipText;
        public event EventHandler TipTextVisibleChanged;

        public ButtonTip()
        {
            InitializeComponent();

            // defaults:
            v_popupPanel = new c_PopupPanel(this);

            AutoScaleMode = AutoScaleMode.None;
#if PREDICTIVEBIB_UI
            v_button.Image = predictivebib_ui.Properties.Resources.infotip;
#elif MODMARC_UI
            v_button.Image = modmarc_ui.Properties.Resources.infotip;
#elif VIEWMARC_UI
            v_button.Image = viewmarc_ui.Properties.Resources.infotip;
#elif BIBMARC_UI
            v_button.Image = bibframe_ui.Properties.Resources.infotip;
#endif

            // events:
            Click += m_ButtonT_Click;
        }

        private void m_ButtonT_Click(object v_sender, EventArgs v_e)
        {
            if (!this.Parent.Controls.Contains(v_popupPanel.v_Panel))
            {
                this.Parent.Controls.Add(v_popupPanel.v_Panel);
                v_popupPanelList.Add(v_popupPanel);
            }

            if (this.TipVisible && v_popupPanel.v_Owner == this)
            {
                this.TipVisible = false;
                m_TriggerTipTextVisibleChangedEvent();
                return;
            }

            var v_spaceRight = this.Parent.Width - this.Right - 10;
            var v_spaceLeft = this.Left - 10;
            var v_actualWidth = v_popupPanel.m_GetExpandedWidth(v_text: v_tipText);
            var v_locationX = v_spaceRight > Math.Max(100, this.TipWidth) ? this.Right - 1 : this.Left - v_actualWidth + 3;
            v_popupPanel.m_SetText(v_text: v_tipText, v_textWidthMax: v_spaceRight > 100 ? v_spaceRight : v_spaceLeft);
            v_popupPanel.m_SetLocation(v_x: v_locationX, v_y: this.Top + 1);
            v_popupPanel.v_Owner = this;
            v_popupPanel.v_Visible = true;
            m_TriggerTipTextVisibleChangedEvent();
            this.Parent.Controls.SetChildIndex(v_popupPanel.v_Panel, 0);
        }

        internal static void m_HideAllTips()
        {
            v_popupPanelList.ForEach(v_x => v_x.v_Visible = false);
            v_popupPanelList.ForEach(v_x => ((ButtonTip)v_x.v_parent).m_TriggerTipTextVisibleChangedEvent());
        }

        internal void m_HideTipPopup()
        {
            v_popupPanel.v_Visible = false;
            m_TriggerTipTextVisibleChangedEvent();
        }

        internal void m_TriggerTipTextVisibleChangedEvent()
        {
            var v_handler = TipTextVisibleChanged;
            v_handler?.Invoke(this, null);    // generate TipTextVisibleChanged event.
        }

        #region properties

        internal new event EventHandler Click
        {
            add { v_button.Click += value; }
            remove { v_button.Click -= value; }
        }

        [EditorAttribute(typeof(MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public string TipText
        {
            get { return v_tipText; }
            set 
            {
                v_tipText = value;
                m_HideTipPopup();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public bool TipVisible
        {
            get { return v_popupPanel.v_Visible; }
            set
            {
                v_popupPanel.v_Visible = value;
                v_popupPanel.v_Owner = null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public int TipWidth
        {
            get { return v_popupPanel.v_TipWidth; }
            set 
            { 
                v_popupPanel.v_TipWidth = value;
            }
        }

        #endregion

        #region popup panel

        private class c_PopupPanel : IDisposable
        {
            private Panel v_pnlPanel;
            private Label v_lblPopup;
            private int v_tipWidth;
            internal ButtonTip v_parent;

            public c_PopupPanel(ButtonTip v_parent)
            {
                this.v_parent = v_parent;
                v_lblPopup = new Label();
                v_lblPopup.BackColor = SystemColors.GradientInactiveCaption;
                v_lblPopup.ForeColor = SystemColors.WindowText;
                v_lblPopup.Padding = new Padding(3, 1, 2, 2);
                v_lblPopup.Margin = new Padding(0);
                v_lblPopup.AutoSize = true;

                v_pnlPanel = new Panel();
                v_pnlPanel.BackColor = SystemColors.Window;
                v_pnlPanel.Padding = new Padding(0);
                v_pnlPanel.Margin = new Padding(2);
                v_pnlPanel.BorderStyle = BorderStyle.None;
                v_pnlPanel.AutoSize = true;
                v_pnlPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                v_pnlPanel.Controls.Add(v_lblPopup);
            }

            public Control v_Panel 
            { 
                get { return v_pnlPanel; } 
            }

            internal ButtonTip v_Owner { get; set; }

            internal bool v_Visible 
            { 
                get { return v_pnlPanel.Visible; }
                set { v_pnlPanel.Visible = value; }
            }

            internal int v_TipWidth
            {
                get { return v_tipWidth; }
                set 
                { 
                    v_tipWidth = value;
                    m_SetWidth(v_tipWidth);
                }
            }

            internal void m_SetWidth(int v_width)
            {
                v_lblPopup.MaximumSize = new Size(v_width, 0);
            }

            internal int m_GetExpandedWidth(string v_text)
            {
                v_lblPopup.MaximumSize = new Size(9999, 0);
                m_SetText(v_text);
                return v_pnlPanel.Width;
            }

            internal void m_SetLocation(int v_x, int v_y)
            {
                v_pnlPanel.Location = new Point(v_x, v_y);
            }

            internal void m_SetText(string v_text = null, int v_textWidthMax = 0)
            {
                if (v_text.m_IsEmpty()) return;

                if (v_TipWidth > 0) v_textWidthMax = v_TipWidth;

                v_lblPopup.Text = v_text;
                if (v_textWidthMax == 0) return;
                v_lblPopup.MaximumSize = new Size(v_textWidthMax, 0);
            }

            public void Dispose()
            {
                v_pnlPanel.Dispose();
                v_lblPopup.Dispose();
            }
        }

        #endregion
    }
}
