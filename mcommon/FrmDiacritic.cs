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
    internal partial class c_FrmDiacritic : FormX
    {
        #region init

        internal enum c_DiacriticType { Combining, Precomposed }

        internal string v_Value;
        private Form v_Owner;
        private XDocument v_xdocDiacritic;

        internal c_FrmDiacritic(Form v_owner)
        {
            InitializeComponent();

            // defaults:
            v_Owner = v_owner;
            this.ShowInTaskbar = true;
            base.m_AssignIconTitle(this, "Diacritics");

            // events:
            v_cmbType.SelectedIndexChanged += m_CmbType_SelectedIndexChanged;
            v_cmbLetter.SelectedIndexChanged += m_CmbLetterCase_TextChanged;
            v_cmbCase.SelectedIndexChanged += m_CmbLetterCase_TextChanged;
            v_dgv.CellMouseDoubleClick += m_Dgv_CellMouseDoubleClick;
            v_dgv.SelectionChanged += m_Dgv_SelectionChanged;
            v_btnOk.Click += m_BtnOk_Click;
            v_dgv.Resize += m_Dgv_Resize;
            v_pnlDiacritic.Focus();    // clear focus.
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
            if (v_xdocDiacritic == null)
            {
                Hide(); // error reading data_diacritics.db.
                return;
            }

            v_cmbType.Items.Clear();
            v_cmbLetter.Items.Clear();
            v_cmbCase.Items.Clear();

            v_cmbType.Items.AddRange(new[] { c_DiacriticType.Combining.ToString(), c_DiacriticType.Precomposed.ToString() });
            v_cmbLetter.Items.AddRange(new[]
            {
                "Aa", "Bb", "Cc", "Dd", "Ee", "Ff", "Gg", "Hh", "Ii", "Jj", "Kk", "Ll", "Mm",
                "Nn", "Oo", "Pp", "Qq", "Rr", "Ss", "Tt", "Uu", "Vv", "Ww", "Xx", "Yy", "Zz",
            });
            v_cmbCase.Items.AddRange(new[] { "Lowercase", "Uppercase" });
            v_cmbType.SelectedIndex = 0;
            v_cmbLetter.SelectedIndex = 0;
            v_cmbCase.SelectedIndex = 0;
        }

        #endregion

        private void m_LoadCache_()
        {
            v_dgv.Columns.Clear();

            // read cache from disk:
#if PREDICTIVE_UI
            await c_DiacriticTools.m_ReadCache_Async().ConfigureAwait(c_Constants.v_ResumeOnUiThread);
            var v_diacriticFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $@"{c_Constants.v_CompanyPathSegment}\Common\Data\data_diacritic.xml");
#else
            var v_diacriticFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data_diacritic.xml");
#endif
            var v_xmlStr = File.ReadAllText(v_diacriticFilePath);
            v_xdocDiacritic = XDocument.Parse(v_xmlStr);

            if (v_xdocDiacritic == null) return;

            var v_id = ((char)(v_cmbLetter.SelectedIndex + (v_cmbCase.SelectedIndex == 0 ? 97 : 65))).ToString();
            var v_diacritics = "";
            if (v_cmbType.Text == c_DiacriticType.Combining.ToString())
            {
                v_diacritics = v_xdocDiacritic.Descendants("item")?.SingleOrDefault(v_x => v_x.Attribute("type")?.Value == "combining")?.Value;
            }
            else
            {
                v_diacritics = v_xdocDiacritic.Descendants("item")?.Where(v_x => v_x.Attribute("type")?.Value == "precomposed")?.SingleOrDefault(v_x => v_x.Attribute("id")?.Value == v_id)?.Value;
            }

            if (v_diacritics.m_IsNotEmpty())
            {
                var v_diacriticArray = v_diacritics.Split(',').Select(v_x => v_x.Trim()).ToArray();

                var v_colCount = v_dgv.Width / 45;
                var v_rowCount = v_diacriticArray.Length / v_colCount;
                if (v_diacriticArray.Length % v_colCount > 0) v_rowCount++;

                while (v_dgv.Columns.Count < v_colCount) v_dgv.Columns.Add("", "");
                while (v_dgv.Rows.Count < v_rowCount) v_dgv.Rows.Add();

                var v_colIdx = 0;
                var v_rowIdx = 0;
                foreach (var v_diacritic in v_diacriticArray)
                {
                    if (v_colIdx > v_colCount - 1)
                    {
                        v_colIdx = 0;
                        v_rowIdx++;
                    }

                    v_dgv.Rows[v_rowIdx].Cells[v_colIdx++].Value = Regex.Unescape(v_diacritic);
                }
            }

            foreach (DataGridViewRow v_row in v_dgv.Rows) v_row.MinimumHeight = 40;
            foreach (DataGridViewColumn v_col in v_dgv.Columns) v_col.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 20);

            v_dgv.ClearSelection();
            v_pnlDiacritic.Focus();
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
            v_cmbLetter.Enabled = v_cmbType.Text == c_DiacriticType.Precomposed.ToString();
            v_cmbCase.Enabled = v_cmbType.Text == c_DiacriticType.Precomposed.ToString();

            if (v_cmbType.Text == c_DiacriticType.Combining.ToString())
            {
                v_btnTip.TipText = "Insert a combining diacritic after the base character.";
            }
            else
            {
                v_btnTip.TipText = "Precomposed diacritics include the base character.";
            }

            m_LoadCache_();
        }

        private void m_CmbLetterCase_TextChanged(object v_sender, EventArgs v_e)
        {
            m_LoadCache_();
        }
    }
}