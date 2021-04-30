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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace marc_common
{
    internal partial class c_FrmSymbol : FormX
    {
        #region init

        private Form v_Owner;
        internal string v_Value;
        private XDocument v_xdocSymbol;

        internal c_FrmSymbol(Form v_owner)
        {
            InitializeComponent();

            // defaults:
            v_Owner = v_owner;
            this.ShowInTaskbar = true;
            base.m_AssignIconTitle(this, "Symbols");

            // events:
            v_cmbType.SelectedIndexChanged += m_CmbType_SelectedIndexChanged;
            v_dgv.CellMouseDoubleClick += m_Dgv_CellMouseDoubleClick;
            v_dgv.SelectionChanged += m_Dgv_SelectionChanged;
            v_btnOk.Click += m_BtnOk_Click;
            v_dgv.Resize += m_Dgv_Resize;
            v_pnlSymbol.Focus();    // clear focus.
            Load += m_Load;
            Shown += m_Shown;
        }

        private void m_Load(object v_sender, EventArgs v_e)
        {
            // set location:
            StartPosition = FormStartPosition.Manual;   // set position manually (below center of parent).
            this.Location = new Point(v_Owner.Left + (v_Owner.Width - this.Width) / 2, v_Owner.Top + (v_Owner.Height - this.Height) - 100);
        }

        private void m_Shown(object v_sender, EventArgs v_e)
        {
            m_LoadCache_();
            if (v_xdocSymbol == null)
            {
                Hide(); // error reading data_symbols.db.
                return;
            }

            v_cmbType.Items.Clear();
            var v_types = v_xdocSymbol.Descendants("item")?.Select(v_x => v_x.Attribute("type")?.Value)?.ToArray();
            if (v_types != null)
            {
                v_cmbType.Items.AddRange(v_types);
                v_cmbType.SelectedIndex = 0;
            }
        }

        #endregion

        private void m_LoadCache_()
        {
            v_dgv.Columns.Clear();

            // read cache from disk:
#if PREDICTIVE_UI
            await c_SymbolTools.m_ReadCache_Async().ConfigureAwait(c_Constants.v_ResumeOnUiThread);
            var v_symbolFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $@"{c_Constants.v_CompanyPathSegment}\Common\Data\data_symbol.xml");
#else
            var v_symbolFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data_symbol.xml");
#endif
            var v_xmlStr = File.ReadAllText(v_symbolFilePath);
            v_xdocSymbol = XDocument.Parse(v_xmlStr);

            if (v_xdocSymbol == null) return;

            var v_symbols = v_xdocSymbol.Descendants("item")?.SingleOrDefault(v_x => v_x.Attribute("type")?.Value == v_cmbType.Text)?.Value;

            if (v_symbols.m_IsNotEmpty())
            {
                var v_symbolArray = v_symbols.Split(',').Select(v_x => v_x.Trim()).ToArray();

                var v_colCount = v_dgv.Width / 45;
                var v_rowCount = v_symbolArray.Length / v_colCount;
                if (v_symbolArray.Length % v_colCount > 0) v_rowCount++;

                while (v_dgv.Columns.Count < v_colCount) v_dgv.Columns.Add("", "");
                while (v_dgv.Rows.Count < v_rowCount) v_dgv.Rows.Add();

                var v_colIdx = 0;
                var v_rowIdx = 0;
                foreach (var v_symbol in v_symbolArray)
                {
                    if (v_colIdx > v_colCount - 1)
                    {
                        v_colIdx = 0;
                        v_rowIdx++;
                    }

                    v_dgv.Rows[v_rowIdx].Cells[v_colIdx++].Value = Regex.Unescape(v_symbol);
                }
            }

            foreach (DataGridViewRow v_row in v_dgv.Rows) v_row.MinimumHeight = 40;
            foreach (DataGridViewColumn v_col in v_dgv.Columns) v_col.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 20);

            v_dgv.ClearSelection();
            v_pnlSymbol.Focus();
        }

        private void m_Dgv_Resize(object v_sender, EventArgs v_e)
        {
            m_LoadCache_();
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }    // allows parent form to initially retain focus with selected cell text highlighted.
        }

        private void m_Dgv_SelectionChanged(object v_sender, EventArgs v_e)
        {
            v_btnOk.Enabled = v_dgv.SelectedCells.Count > 0;
        }

        private void m_BtnOk_Click(object v_sender, EventArgs v_e)
        {
            this.v_Value = v_dgv.SelectedCells[0].Value?.ToString();
            Hide();
        }

        private void m_Dgv_CellMouseDoubleClick(object v_sender, DataGridViewCellMouseEventArgs v_e)
        {
            this.v_Value = v_dgv.Rows[v_e.RowIndex].Cells[v_e.ColumnIndex].Value?.ToString();
            Hide();
        }

        private void m_CmbType_SelectedIndexChanged(object v_sender, EventArgs v_e)
        {
            m_LoadCache_();
        }
    }
}
