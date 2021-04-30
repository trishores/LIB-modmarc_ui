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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using modmarc_ui.Properties;

namespace modmarc_ui
{
    internal partial class c_FrmMain : Form
    {
        #region init

        internal static c_FrmDiacritic v_frmDiacritic;
        internal static c_FrmSymbol v_frmSymbol;
        private static c_FrmSettings v_frmSettings = new c_FrmSettings();
        internal static c_FrmMessageBox v_frmMessageBox;
        private static c_FrmAbout v_frmAbout;
        private int[] v_colPadding;
        private string v_MrcOpenFilePathArg;
        private Color v_rowBackColor;
        private Color v_tabHighlightColor = Color.FromArgb(204, 245, 255);
        private bool v_highlightFixedFields = true;
        private bool v_highlightMediaFields = true;
        private string v_tempOpenMrcFilePath;
        private string v_prevDataContent;
        private int? v_LastEditRow;
        private string v_CurrentDir;

        private BindingList<c_PropertyItem> v_PropertyList = new BindingList<c_PropertyItem>();
        private BindingList<c_LeaderItem> v_LeaderList = new BindingList<c_LeaderItem>();
        private BindingList<c_ControlItem> v_ControlList = new BindingList<c_ControlItem>();
        private BindingList<c_VardataItem> v_VardataList = new BindingList<c_VardataItem>();

        private Stack<BindingList<c_LeaderItem>> v_LeaderUndoStack = new Stack<BindingList<c_LeaderItem>>();
        private Stack<BindingList<c_LeaderItem>> v_LeaderRedoStack = new Stack<BindingList<c_LeaderItem>>();
        private Stack<BindingList<c_ControlItem>> v_ControlUndoStack = new Stack<BindingList<c_ControlItem>>();
        private Stack<BindingList<c_ControlItem>> v_ControlRedoStack = new Stack<BindingList<c_ControlItem>>();
        private Stack<BindingList<c_VardataItem>> v_VardataUndoStack = new Stack<BindingList<c_VardataItem>>();
        private Stack<BindingList<c_VardataItem>> v_VardataRedoStack = new Stack<BindingList<c_VardataItem>>();

        private enum v_Tab { v_Leader, v_Control, v_Vardata, v_NotSet }
        private v_Tab v_tab = v_Tab.v_NotSet;  // initial view.

        private Color v_ErrorBackColor = Color.FromArgb(255, 200, 200);

        internal c_FrmMain(string v_mrcOpenFilePathArg = null)
        {
            InitializeComponent();

            // defaults:
            c_SettingsTools2.m_Init();
            v_frmMessageBox = new c_FrmMessageBox(this);
            v_frmDiacritic = new c_FrmDiacritic(this);
            v_frmSymbol = new c_FrmSymbol(this);
            v_frmAbout = new c_FrmAbout(this);
            this.v_MrcOpenFilePathArg = v_mrcOpenFilePathArg;
            m_SetScreenPosition();
            m_InitializeDgv();
            AllowDrop = true;
            v_frmAbout.v_pbxLogo.Image = Resources.modmarc_256;
            v_btnToggleHighlightFixedFields.Enabled = false;
            v_btnToggleHighlightMediaFields.Enabled = false;
            v_btnLeader.Enabled = false;
            v_btnControl.Enabled = false;
            v_btnVardata.Enabled = false;
            //v_btnUndo.Enabled = false;
            //v_btnRedo.Enabled = false;
            //v_btnMod.DropDown.AutoClose = false;
            v_lblMain.SendToBack();
            v_pbxMain.SendToBack();

            // events:
            LocationChanged += m_FrmMain_LocationChanged;
            Shown += m_FrmMain_Shown;
            DragEnter += m_Ui_DragEnter;
            DragDrop += m_Ui_DragDrop;
            DragLeave += m_Ui_DragLeave;
            v_btnToggleHighlightFixedFields.Click += m_btnToggleHighlightFixedFields_Click;
            v_btnToggleHighlightMediaFields.Click += m_btnToggleHighlightMediaFields_Click;
            v_btnOpen.Click += m_BtnOpen_Click;
            v_btnSettings.Click += m_BtnSettings_Click;
            v_btnSave.Click += m_BtnSave_Click;
            v_btnLeader.Click += (v_s, v_e) => { m_PopulateDgv(v_btnLeader); };
            v_btnControl.Click += (v_s, v_e) => { m_PopulateDgv(v_btnControl); };
            v_btnVardata.Click += (v_s, v_e) => { m_PopulateDgv(v_btnVardata); };
            //v_btnUndo.Click += m_BtnUndo_Click;
            //v_btnRedo.Click += m_BtnRedo_Click;
            KeyDown += m_KeyDown;
            //v_btnMod.DropDown.MouseLeave += (v_s, v_e) => v_btnMod.DropDown.Close();
            v_btnAbout.Click += m_LnkAbout_Click;
        }

        internal void m_FrmMain_Shown(object v_sender, EventArgs v_e)
        {
            // visual cues:
            m_LockCommonUi(v_lockUi: true, v_padlockEnable: true);
            m_SetStatusMessage("Opening file...");
            m_ShowStatusBarSpinner(v_show: true);

            m_SetStatusMessage("Ready");

            try
            {
                if (File.Exists(v_MrcOpenFilePathArg)) m_OpenMrcFile(v_MrcOpenFilePathArg);
                v_MrcOpenFilePathArg = "";
            }
            finally
            {
                m_LockCommonUi(v_lockUi: false);
                m_ShowStatusBarSpinner(v_show: false);
            }

            v_CurrentDir = c_SettingsTools2.m_WorkspaceDir;
            if (!Directory.Exists(v_CurrentDir)) v_CurrentDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        #endregion

        #region help

        private List<c_HelpItem> m_GetHelpUrls(string v_fieldId, string v_mnemonic)
        {
            var v_helpItemList = new List<c_HelpItem>();
            if (v_fieldId.m_IsEmpty()) return v_helpItemList;

            void m_AddFixedFieldHelp(string v_mnemonic)
            {
                if (v_mnemonic.m_IsEmpty()) return;
                if (v_mnemonic == "Rec stat") v_mnemonic = "Rec";
                if (v_mnemonic == "S/L") v_mnemonic = "Succ";
                v_helpItemList.Add(new c_HelpItem($"Help page for '{v_mnemonic}' fixed field (OCLC)", $"https://www.oclc.org/bibformats/en/fixedfield/{v_mnemonic.ToLower()}.html"));
            }

            if (v_fieldId.StartsWith("LDR"))
            {
                v_helpItemList.Add(new c_HelpItem($"Help page for Leader field (LC)", "https://www.loc.gov/marc/bibliographic/concise/bdleader.html"));
                m_AddFixedFieldHelp(v_mnemonic);
            }
            else if (v_fieldId.StartsWith("DIR"))
            {
                v_helpItemList.Add(new c_HelpItem($"Help page for Directory field (LC)", "https://www.loc.gov/marc/bibliographic/bddirectory.html"));
            }
            else
            {
                var v_fieldNum = int.Parse(v_fieldId);
                if (v_fieldNum < 10)
                {
                    v_helpItemList.Add(new c_HelpItem($"Help page for '{v_fieldId}' field (LC)", $"https://www.loc.gov/marc/bibliographic/bd{v_fieldId}.html"));
                    m_AddFixedFieldHelp(v_mnemonic);
                }
                else if (v_fieldNum >= 10 && v_fieldNum < 900)
                {
                    v_helpItemList.Add(new c_HelpItem($"Help page for '{v_fieldId}' field (LC)", $"https://www.loc.gov/marc/bibliographic/bd{v_fieldId}.html"));
                    v_helpItemList.Add(new c_HelpItem($"Help page for '{v_fieldId}' field (OCLC)", $"https://www.oclc.org/bibformats/en/{v_fieldId.Substring(0, 1)}xx/{v_fieldId}.html"));
                }
                else if (v_fieldNum >= 900 && v_fieldNum < 907)
                {
                    v_helpItemList.Add(new c_HelpItem($"Help page for '{v_fieldId}' field (OCLC)", "https://www.oclc.org/bibformats/en/9xx/901-907.html"));
                }
                else if (v_fieldNum >= 945 && v_fieldNum <= 949)
                {
                    v_helpItemList.Add(new c_HelpItem($"Help page for '{v_fieldId}' field (OCLC)", "https://www.oclc.org/bibformats/en/9xx/945-949.html"));
                }
                else if (new[] { 936, 938, 956, 987, 994 }.Any(v_x => v_x == v_fieldNum))
                {
                    v_helpItemList.Add(new c_HelpItem($"Help page for '{v_fieldId}' field (OCLC)", $"https://www.oclc.org/bibformats/en/9xx/{v_fieldId}.html"));
                }
                else if (v_fieldId.StartsWith("9"))
                {
                    v_helpItemList.Add(new c_HelpItem($"Help page for '{v_fieldId}' field (OCLC)", "https://www.oclc.org/bibformats/en/9xx.html"));
                }
                else
                {
                    v_helpItemList.Add(new c_HelpItem($"Help page for '{v_fieldId}' field: help unavailable", null));
                }
            }
            return v_helpItemList;
        }

        internal class c_HelpItem
        {
            internal string v_Desc;
            internal string v_Url;

            public c_HelpItem(string v_desc, string v_url)
            {
                v_Desc = v_desc;
                v_Url = v_url;
            }
        }

        private void m_LnkAbout_Click(object v_sender, EventArgs v_e)
        {
            v_frmAbout.ShowDialog(this);
        }

        #endregion

        #region open mrc

        private void m_BtnOpen_Click(object v_sender, EventArgs v_e)
        {
            var v_mrcOpenFilePath = "";
            using (var v_openFileDialog = new OpenFileDialog())
            {
                const string v_ext = "mrc";
                v_openFileDialog.Title = "Open mrc file";
                v_openFileDialog.DefaultExt = $"*.{v_ext}";
                v_openFileDialog.Filter = $"mrc file (*.{v_ext})|*.{v_ext}";
                v_openFileDialog.InitialDirectory = v_CurrentDir;
                if (!Directory.Exists(v_openFileDialog.InitialDirectory))
                {
                    v_openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                if (v_openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    v_CurrentDir = Path.GetDirectoryName(v_openFileDialog.FileName);
                    c_SettingsTools2.m_WorkspaceDir = v_CurrentDir;

                    if (Path.GetExtension(v_openFileDialog.FileName).Equals(".mrc", StringComparison.OrdinalIgnoreCase))
                    {
                        v_mrcOpenFilePath = v_openFileDialog.FileName;
                    }
                }
            }
            if (!File.Exists(v_mrcOpenFilePath)) return;

            m_OpenMrcFile(v_mrcOpenFilePath);
        }

        private void m_OpenMrcFile(string v_mrcOpenFilePath)
        {
            void m_HandleFailure(string v_msg)
            {
                v_dgv.Visible = false;
                v_btnLeader.Enabled = false;
                v_btnLeader.BackColor = default;
                v_btnControl.Enabled = false;
                v_btnControl.BackColor = default;
                v_btnVardata.Enabled = false;
                v_btnVardata.BackColor = default;
                v_tempOpenMrcFilePath = null;
                m_SetTitle(null);
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog(v_msg + ".", new[] { "OK" }, Resources.error);
            }

            m_LockCommonUi(v_lockUi: true);
            m_SetStatusMessage("Opening mrc file...");
            m_ShowStatusBarSpinner(v_show: true);

            try
            {
                var v_exitCode = m_DisplayMrcData_(v_mrcOpenFilePath);
                if (v_exitCode == 0)
                {
                    v_dgv.Visible = true;
                    v_isDgvVisible = null;
                }
                else
                {
                    if (v_exitCode == (int)c_StatusTools.c_StatusCode.MarcFileParseError) m_HandleFailure($"Error parsing {v_mrcOpenFilePath}");
                    else if (v_exitCode == (int)c_StatusTools.c_StatusCode.UnsupportedMarcFile) m_HandleFailure($"Unsupported MARC file");
                    else m_HandleFailure(c_StatusTools.m_GetErrorText(v_exitCode));
                }
            }
            catch (Exception v_e)
            {
                m_HandleFailure($"Unknown error");
            }
            finally
            {
                m_LockCommonUi(v_lockUi: false);
                m_ShowStatusBarSpinner(v_show: false);
            }

            v_dgv.ClearSelection();
        }

        private int m_DisplayMrcData_(string v_mrcOpenFilePath)
        {
            // clear any existing row data:
            v_PropertyList.Clear();
            v_LeaderList.Clear();
            v_ControlList.Clear();
            v_VardataList.Clear();
            v_dgv.Rows.Clear();

            // call engine to generate dat file from the mrc file:
            var lib = new viewmarc_lib.c_Engine();
            var v_exitCode = lib.RunMrcToDat(v_mrcOpenFilePath, out string[] v_lines);

            if (v_exitCode != 0) return v_exitCode;

            v_prevDataContent = string.Join("\r\n", v_lines);

            foreach (var v_line in v_lines)
            {
                if (v_line.StartsWith("PRP")) v_PropertyList.Add(new c_PropertyItem(v_line.Substring(4).Split('\u23F5')));
                if (v_line.StartsWith("LDR")) v_LeaderList.Add(new c_LeaderItem(v_line.Substring(4).Split('\u23F5')));
                if (v_line.StartsWith("CTR")) v_ControlList.Add(new c_ControlItem(v_line.Substring(4).Split('\u23F5')));
                if (v_line.StartsWith("VAR")) v_VardataList.Add(new c_VardataItem(v_line.Substring(4).Split('\u23F5')));
            }

            // populate datagridview with default view:
            v_btnLeader.Enabled = true;
            v_btnControl.Enabled = true;
            v_btnVardata.Enabled = true;

            if (v_tab == v_Tab.v_NotSet) v_tab = v_Tab.v_Vardata;

            if (v_tab == v_Tab.v_Leader) v_btnLeader.PerformClick();
            if (v_tab == v_Tab.v_Control) v_btnControl.PerformClick();
            if (v_tab == v_Tab.v_Vardata) v_btnVardata.PerformClick();

            // update form title:
            m_SetTitle(v_mrcOpenFilePath);
            v_tempOpenMrcFilePath = v_mrcOpenFilePath;

            //v_btnUndo.Enabled = false;
            //v_btnRedo.Enabled = false;

            return 0;
        }

        private void m_SetTitle(string v_mrcFilePath)
        {
            if (v_mrcFilePath.m_IsEmpty())
            {
                Text = c_AssemblyTools.v_ProductName;
                m_SetStatusMessage("");
                return;
            }
            var v_materialType = v_PropertyList.Single(v_x => v_x.v_Key == "MaterialType").v_Value;
            var v_formattedMaterialType = Regex.Replace(v_materialType, "([A-Z]{1})", m => $" {m.Groups[1].Value.ToLower()}");
            Text = $"{c_AssemblyTools.v_ProductName}   {Path.GetFileNameWithoutExtension(v_mrcFilePath)} ({v_formattedMaterialType.Trim()})";
            m_SetStatusMessage($"{Path.GetFileNameWithoutExtension(v_mrcFilePath)}");
        }

        #endregion

        #region edit mrc

        private void m_BtnModifyValue_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open an mrc file.", new[] { "OK" }, Resources.info);
                return;
            }

            if (v_dgv.SelectedCells.Count != 1)
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First select a single table cell to edit.", new[] { "OK" }, Resources.info);
                return;
            }

            var v_row = v_dgv.SelectedCells[0].RowIndex;
            if (!m_IsEditable(v_row)) return;

            m_SaveSnapshop();

            var v_col = v_tab == v_Tab.v_Vardata ? v_dgv.SelectedCells[0].ColumnIndex : v_dgv.Rows[v_row].Cells["v_Value"].ColumnIndex;

            // change edit mode for duration of edits to same row:
            v_LastEditRow = v_row;
            v_dgv.CurrentCell = v_dgv.Rows[v_row].Cells[v_col];

            v_dgv.EditMode = DataGridViewEditMode.EditOnEnter;
            v_dgv.BeginEdit(true);
        }

        private void m_BtnDuplicateField_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open an mrc file.", new[] { "OK" }, Resources.info);
                return;
            }

            if (v_dgv.SelectedCells.Count == 0 || v_dgv.SelectedCells.Cast<DataGridViewCell>().Any(v_x => v_x.RowIndex != v_dgv.SelectedCells[0].RowIndex))
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First select a single table row below which a duplicate row will be added.", new[] { "OK" }, Resources.info);
                return;
            }

            m_SaveSnapshop();

            var v_row = v_dgv.SelectedCells[0].RowIndex;
            var v_col = v_dgv.SelectedCells[0].ColumnIndex;
            var v_oldRowContents = new[]
            {
                v_dgv.Rows[v_row].Cells[0].Value.ToString(),
                v_dgv.Rows[v_row].Cells[1].Value.ToString(),
                v_dgv.Rows[v_row].Cells[2].Value.ToString(),
                v_dgv.Rows[v_row].Cells[3].Value.ToString(),
            };

            if (v_tab == v_Tab.v_Vardata)
            {
                void v_dgv_RowsAdded(object v_sender, DataGridViewRowsAddedEventArgs v_e)
                {
                    v_dgv.ClearSelection();
                    v_dgv.Rows[v_e.RowIndex].Cells[v_col].Selected = true;
                    m_HighlightRow(v_e.RowIndex);
                    v_dgv.RowsAdded -= v_dgv_RowsAdded;
                }
                v_dgv.RowsAdded += v_dgv_RowsAdded;

                if (v_dgv.SelectedCells[0].RowIndex < v_dgv.Rows.Count - 1)
                {
                    v_VardataList.Insert(v_row + 1, new c_VardataItem(v_oldRowContents));
                }
                else
                {
                    v_VardataList.Add(new c_VardataItem(v_oldRowContents));
                }
            }
        }

        private void m_BtnAddField_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open an mrc file.", new[] { "OK" }, Resources.info);
                return;
            }

            if (v_dgv.SelectedCells.Count == 0 || v_dgv.SelectedCells.Cast<DataGridViewCell>().Any(v_x => v_x.RowIndex != v_dgv.SelectedCells[0].RowIndex))
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First select a single table row below which a new row will be added.", new[] { "OK" }, Resources.info);
                return;
            }

            m_SaveSnapshop();

            var v_row = v_dgv.SelectedCells[0].RowIndex;
            var v_col = v_dgv.SelectedCells[0].ColumnIndex;
            if (v_tab == v_Tab.v_Vardata)
            {
                void v_dgv_RowsAdded(object v_sender, DataGridViewRowsAddedEventArgs v_e)
                {
                    v_dgv.ClearSelection();
                    v_dgv.Rows[v_e.RowIndex].Cells[v_col].Selected = true;
                    m_HighlightRow(v_e.RowIndex);
                    v_dgv.RowsAdded -= v_dgv_RowsAdded;
                }
                v_dgv.RowsAdded += v_dgv_RowsAdded;

                if (v_dgv.SelectedCells[0].RowIndex < v_dgv.Rows.Count - 1)
                {
                    v_VardataList.Insert(v_row + 1, new c_VardataItem(new[] { "", "", "", "" }));
                }
                else
                {
                    v_VardataList.Add(new c_VardataItem(new[] { "", "", "", "" }));
                }

                // change edit mode for duration of edits to same row:
                v_LastEditRow = v_row + 1;
                v_dgv.CurrentCell = v_dgv.Rows[v_row + 1].Cells[0];

                v_dgv.EditMode = DataGridViewEditMode.EditOnEnter;
                v_dgv.BeginEdit(true);
            }
        }

        private void m_BtnDelField_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open an mrc file.", new[] { "OK" }, Resources.info);
                return;
            }

            if (v_dgv.SelectedCells.Count == 0 || v_dgv.SelectedCells.Cast<DataGridViewCell>().Any(v_x => v_x.RowIndex != v_dgv.SelectedCells[0].RowIndex))
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First select a single table row to delete.", new[] { "OK" }, Resources.info);
                return;
            }

            m_SaveSnapshop();

            var v_row = v_dgv.SelectedCells[0].RowIndex;
            if (!m_IsEditable(v_row)) return;

            if (v_tab == v_Tab.v_Vardata)
            {
                v_VardataList.RemoveAt(v_dgv.SelectedCells[0].RowIndex);
                v_dgv.ClearSelection();
            }
        }

        private void m_BtnMoveUpField_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open an mrc file.", new[] { "OK" }, Resources.info);
                return;
            }

            if (v_dgv.SelectedCells.Count == 0 || v_dgv.SelectedCells.Cast<DataGridViewCell>().Any(v_x => v_x.RowIndex != v_dgv.SelectedCells[0].RowIndex))
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First select a single table row.", new[] { "OK" }, Resources.info);
                return;
            }

            m_SaveSnapshop();

            var v_row = v_dgv.SelectedCells[0].RowIndex;
            var v_col = v_dgv.SelectedCells[0].ColumnIndex;
            if (v_tab == v_Tab.v_Vardata)
            {
                if (v_col - 1 < 0) return;
                var v_upperEntry = v_VardataList[v_row - 1];
                var v_lowerEntry = v_VardataList[v_row];
                v_VardataList[v_row] = v_upperEntry;
                v_VardataList[v_row - 1] = v_lowerEntry;
                v_dgv.ClearSelection();
                v_dgv.Rows[v_row - 1].Cells[v_col].Selected = true;
                m_HighlightRow(v_row - 1);
            }
        }

        private void m_BtnMoveDnField_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open an mrc file.", new[] { "OK" }, Resources.info);
                return;
            }

            if (v_dgv.SelectedCells.Count == 0 || v_dgv.SelectedCells.Cast<DataGridViewCell>().Any(v_x => v_x.RowIndex != v_dgv.SelectedCells[0].RowIndex))
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First select a single table row.", new[] { "OK" }, Resources.info);
                return;
            }

            m_SaveSnapshop();

            var v_row = v_dgv.SelectedCells[0].RowIndex;
            var v_col = v_dgv.SelectedCells[0].ColumnIndex;
            if (v_tab == v_Tab.v_Vardata)
            {
                var v_currRowIndex = v_dgv.SelectedCells[0].RowIndex;
                var v_currColIndex = v_dgv.SelectedCells[0].ColumnIndex;
                if (v_currRowIndex + 1 > v_dgv.RowCount - 1) return;
                var v_upperEntry = v_VardataList[v_currRowIndex];
                var v_lowerEntry = v_VardataList[v_currRowIndex + 1];
                v_VardataList[v_currRowIndex] = v_lowerEntry;
                v_VardataList[v_currRowIndex + 1] = v_upperEntry;
                v_dgv.ClearSelection();
                v_dgv.Rows[v_currRowIndex + 1].Cells[v_currColIndex].Selected = true;
                m_HighlightRow(v_currRowIndex + 1);
            }
        }

        private bool m_IsEditable(int v_rowIndex)
        {
            if (v_rowIndex < 0)
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First select a single table cell to edit.", new[] { "OK" }, Resources.info);
                return false;
            }

            var v_isEditable = true;

            // tab specific messages:
            string v_msg = null;
            if (v_tab == v_Tab.v_Leader)
            {
                var v_autoFields = new[] { "LDR [0-4]", "LDR [11]", "LDR [12-16]" };
                var v_blockedFields = new[] { "LDR [9]", "LDR [10]", "LDR [11]", "LDR [20]", "LDR [21]", "LDR [22]", "LDR [23]" };
                var v_selectedField = v_dgv.Rows[v_rowIndex]?.Cells["v_Field"]?.Value?.ToString();

                if (v_autoFields.Contains(v_selectedField))
                {
                    v_msg = $"This field's value will auto-update and is not editable.";
                    v_isEditable = false;
                }
                else if (v_blockedFields.Contains(v_selectedField))
                {
                    v_msg = $"This field's value is not editable.";
                    v_isEditable = false;
                }
            }
            else if (v_tab == v_Tab.v_Control)
            {
                var v_autoFields = new[] { "005 [0-15]" };
                var v_blockedFields = new[] { "008 [0-5]" };
                var v_selectedField = v_dgv.Rows[v_rowIndex]?.Cells["v_Field"]?.Value?.ToString();
                if (v_autoFields.Contains(v_selectedField))
                {
                    v_msg = $"This field's value will auto-update and is not editable.";
                    v_isEditable = false;
                }
                else if (v_blockedFields.Contains(v_selectedField))
                {
                    v_msg = $"This field's value is not editable.";
                    v_isEditable = false;
                }
            }
            else if (v_tab == v_Tab.v_Vardata)
            {
                var v_fieldId = v_dgv.Rows[v_rowIndex].Cells["v_Field"]?.Value?.ToString() ?? "";
                var v_subfieldData = v_dgv.Rows[v_rowIndex].Cells["v_Value"]?.Value?.ToString();
                if (v_fieldId == "260" || v_fieldId == "504" || v_fieldId == "264" || v_fieldId == "300")
                {
                    v_msg = $"Modifying field {v_fieldId} may require you to update one or more control fields.";
                }
                else if (v_fieldId == "500" && v_subfieldData == "ǂa Includes index.")
                {
                    v_msg = $"Modifying field 500 with 'index' subfield may require you to update one or more control fields.";
                }
            }

            if (v_msg.m_IsNotEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog(v_msg, new[] { "OK" }, Resources.warn);
            }

            return v_isEditable;
        }

        #endregion

        #region undo / redo

        private void m_SaveSnapshop()
        {
            // save snapshot:
            if (v_tab == v_Tab.v_Leader)
            {
                // save current state into undo stack:
                var v_cbl = new BindingList<c_LeaderItem>();
                v_LeaderList.ToList().ForEach(v_x => v_cbl.Add(v_x.m_DeepClone()));
                v_LeaderUndoStack.Push(v_cbl);
                //v_btnUndo.Enabled = true;
                // clear redo stack:
                v_LeaderRedoStack.Clear();
                //v_btnRedo.Enabled = false;
            }
            else if (v_tab == v_Tab.v_Control)
            {
                // save current state into undo stack:
                var v_cbl = new BindingList<c_ControlItem>();
                v_ControlList.ToList().ForEach(v_x => v_cbl.Add(v_x.m_DeepClone()));
                v_ControlUndoStack.Push(v_cbl);
                //v_btnUndo.Enabled = true;
                // clear redo stack:
                v_ControlRedoStack.Clear();
                //v_btnRedo.Enabled = false;
            }
            else if (v_tab == v_Tab.v_Vardata)
            {
                // save current state into undo stack:
                var v_cbl = new BindingList<c_VardataItem>();
                v_VardataList.ToList().ForEach(v_x => v_cbl.Add(v_x.m_DeepClone()));
                v_VardataUndoStack.Push(v_cbl);
                //v_btnUndo.Enabled = true;
                // clear redo stack:
                v_VardataRedoStack.Clear();
                //v_btnRedo.Enabled = false;
            }
        }

        private void m_BtnUndo_Click(object v_sender, EventArgs v_e)
        {
            if (v_tab == v_Tab.v_Leader && v_LeaderUndoStack.Count > 0)
            {
                // save current state in redo stack:
                var v_cbl = new BindingList<c_LeaderItem>();
                v_LeaderList.ToList().ForEach(v_x => v_cbl.Add(v_x.m_DeepClone()));
                v_LeaderRedoStack.Push(v_cbl);
                //v_btnRedo.Enabled = true;

                // perform undo:
                m_PopulateDgv(v_btnLeader, v_leaderList: v_LeaderUndoStack.Pop());
                //v_btnUndo.Enabled = v_LeaderUndoStack.Count > 0;
                v_dgv.ClearSelection();
            }
            else if (v_tab == v_Tab.v_Control && v_ControlUndoStack.Count > 0)
            {
                // save current state in redo stack:
                var v_cbl = new BindingList<c_ControlItem>();
                v_ControlList.ToList().ForEach(v_x => v_cbl.Add(v_x.m_DeepClone()));
                v_ControlRedoStack.Push(v_cbl);
                //v_btnRedo.Enabled = true;

                // perform undo:
                m_PopulateDgv(v_btnControl, v_controlList: v_ControlUndoStack.Pop());
                //v_btnUndo.Enabled = v_ControlUndoStack.Count > 0;
                v_dgv.ClearSelection();
            }
            else if (v_tab == v_Tab.v_Vardata && v_VardataUndoStack.Count > 0)
            {
                // save current state in redo stack:
                var v_cbl = new BindingList<c_VardataItem>();
                v_VardataList.ToList().ForEach(v_x => v_cbl.Add(v_x.m_DeepClone()));
                v_VardataRedoStack.Push(v_cbl);
                //v_btnRedo.Enabled = true;

                // perform undo:
                m_PopulateDgv(v_btnVardata, v_vardataList: v_VardataUndoStack.Pop());
                //v_btnUndo.Enabled = v_VardataUndoStack.Count > 0;
                v_dgv.ClearSelection();
            }
        }

        private void m_BtnRedo_Click(object v_sender, EventArgs v_e)
        {
            if (v_tab == v_Tab.v_Leader && v_LeaderRedoStack.Count > 0)
            {
                // save current state in undo stack:
                var v_cbl = new BindingList<c_LeaderItem>();
                v_LeaderList.ToList().ForEach(v_x => v_cbl.Add(v_x.m_DeepClone()));
                v_LeaderUndoStack.Push(v_cbl);
                //v_btnUndo.Enabled = true;

                // perform redo:
                m_PopulateDgv(v_btnLeader, v_leaderList: v_LeaderRedoStack.Pop());
                //v_btnRedo.Enabled = v_LeaderRedoStack.Count > 0;
                v_dgv.ClearSelection();
            }
            else if (v_tab == v_Tab.v_Control && v_ControlRedoStack.Count > 0)
            {
                // save current state in undo stack:
                var v_cbl = new BindingList<c_ControlItem>();
                v_ControlList.ToList().ForEach(v_x => v_cbl.Add(v_x.m_DeepClone()));
                v_ControlUndoStack.Push(v_cbl);
                //v_btnUndo.Enabled = true;

                // perform redo:
                m_PopulateDgv(v_btnControl, v_controlList: v_ControlRedoStack.Pop());
                //v_btnRedo.Enabled = v_ControlRedoStack.Count > 0;
                v_dgv.ClearSelection();
            }
            else if (v_tab == v_Tab.v_Vardata && v_VardataRedoStack.Count > 0)
            {
                // save current state in undo stack:
                var v_cbl = new BindingList<c_VardataItem>();
                v_VardataList.ToList().ForEach(v_x => v_cbl.Add(v_x.m_DeepClone()));
                v_VardataUndoStack.Push(v_cbl);
                //v_btnUndo.Enabled = true;

                // perform redo:
                m_PopulateDgv(v_btnVardata, v_vardataList: v_VardataRedoStack.Pop());
                //v_btnRedo.Enabled = v_VardataRedoStack.Count > 0;
                v_dgv.ClearSelection();
            }
        }

        private void m_UpdateLeaderBindingList(BindingList<c_LeaderItem> v_leaderList)
        {
            var v_excludeFields = new[] { "LDR [0-4]", "LDR [9]", "LDR [10]", "LDR [11]", "LDR [12-16]", "LDR [20]", "LDR [21]", "LDR [22]", "LDR [23]" };
            v_dgv.DataSource = v_leaderList;
        }

        private void m_UpdateControlBindingList(BindingList<c_ControlItem> v_controlList)
        {
            var v_excludeFields = new[] { "", "005 [0-15]", "008 [0-5]", "008 [38]" };   // "" for divider row.
            v_dgv.DataSource = v_controlList;
        }

        private void m_UpdateVardataBindingList(BindingList<c_VardataItem> v_vardataList)
        {
            var v_excludeFields = Array.Empty<string>();
            v_dgv.DataSource = v_vardataList;
        }

        private void m_KeyDown(object v_sender, KeyEventArgs v_e)
        {
            if (v_e.KeyCode == Keys.Z && v_e.Control)
            {
                m_BtnUndo_Click(null, null);
            }
            else if (v_e.KeyCode == Keys.Y && v_e.Control)
            {
                m_BtnRedo_Click(null, null);
            }
        }

        #endregion

        #region insert

        private void m_BtnDiacritic_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open an mrc file.", new[] { "OK" }, Resources.info);
                return;
            }
            if (!(ActiveControl is DataGridViewTextBoxEditingControl))
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"First place cursor in table cell or select cell text.", new[] { "OK" }, Resources.info);
                return;
            }

            v_frmDiacritic.v_Value = "";
            v_frmDiacritic.ShowDialog(this);
            if (v_frmDiacritic.v_Value.m_IsNotEmpty())
            {
                var v_txt = (TextBox)v_dgv.EditingControl;
                var v_startIdx = v_txt.SelectionStart;
                var v_endIdx = v_txt.SelectionStart + v_txt.SelectionLength - 1;
                var v_startStr = v_startIdx > 0 ? v_txt.Text.Substring(0, v_startIdx) : "";
                var v_endStr = v_endIdx < v_txt.Text.Length ? v_txt.Text.Substring(v_endIdx + 1) : "";
                var v_newText = v_startStr + v_frmDiacritic.v_Value + v_endStr;
                v_dgv.CurrentCell.Value = v_newText;
                v_dgv.RefreshEdit();
                ((TextBox)v_dgv.EditingControl).Select(v_startStr.Length + 1, 0);
                ((TextBox)v_dgv.EditingControl).ScrollToCaret();
                Clipboard.SetText(v_frmDiacritic.v_Value);
            }
        }

        private void m_BtnSymbol_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open an mrc file.", new[] { "OK" }, Resources.info);
                return;
            }
            if (!(ActiveControl is DataGridViewTextBoxEditingControl))
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"First place cursor in table cell or select cell text.", new[] { "OK" }, Resources.info);
                return;
            }

            v_frmSymbol.v_Value = "";
            v_frmSymbol.ShowDialog(this);
            if (v_frmSymbol.v_Value.m_IsNotEmpty())
            {
                var v_txt = (TextBox)v_dgv.EditingControl;
                var v_startIdx = v_txt.SelectionStart;
                var v_endIdx = v_txt.SelectionStart + v_txt.SelectionLength - 1;
                var v_startStr = v_startIdx > 0 ? v_txt.Text.Substring(0, v_startIdx) : "";
                var v_endStr = v_endIdx < v_txt.Text.Length ? v_txt.Text.Substring(v_endIdx + 1) : "";
                var v_newText = v_startStr + v_frmSymbol.v_Value + v_endStr;
                v_dgv.CurrentCell.Value = v_newText;
                v_dgv.RefreshEdit();
                ((TextBox)v_dgv.EditingControl).Select(v_startStr.Length + 1, 0);
                ((TextBox)v_dgv.EditingControl).ScrollToCaret();
                Clipboard.SetText(v_frmSymbol.v_Value);
            }
        }

        private void m_BtnSubfieldDelim_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open an mrc file.", new[] { "OK" }, Resources.info);
                return;
            }

            if (!(ActiveControl is DataGridViewTextBoxEditingControl))
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"First place cursor at insert point.", new[] { "OK" }, Resources.info);
                return;
            }
            var v_txt = (TextBox)v_dgv.EditingControl;
            var v_startIdx = v_txt.SelectionStart;
            var v_endIdx = v_txt.SelectionStart + v_txt.SelectionLength - 1;
            var v_startStr = v_startIdx > 0 ? v_txt.Text.Substring(0, v_startIdx) : "";
            var v_endStr = v_endIdx < v_txt.Text.Length ? v_txt.Text.Substring(v_endIdx + 1) : "";
            var v_newText = v_startStr + "ǂ" + v_endStr;
            v_dgv.CurrentCell.Value = v_newText;
            v_dgv.RefreshEdit();
            ((TextBox)v_dgv.EditingControl).Select(v_startStr.Length + 1, 0);
            ((TextBox)v_dgv.EditingControl).ScrollToCaret();
        }

        private void m_BtnBlank_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open an mrc file.", new[] { "OK" }, Resources.info);
                return;
            }

            if (!(ActiveControl is DataGridViewTextBoxEditingControl))
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"First place cursor at insert point.", new[] { "OK" }, Resources.info);
                return;
            }
            var v_txt = (TextBox)v_dgv.EditingControl;
            var v_startIdx = v_txt.SelectionStart;
            var v_endIdx = v_txt.SelectionStart + v_txt.SelectionLength - 1;
            var v_startStr = v_startIdx > 0 ? v_txt.Text.Substring(0, v_startIdx) : "";
            var v_endStr = v_endIdx < v_txt.Text.Length ? v_txt.Text.Substring(v_endIdx + 1) : "";
            var v_newText = v_startStr + '\u2423' + v_endStr;
            v_dgv.CurrentCell.Value = v_newText;
            v_dgv.RefreshEdit();
            ((TextBox)v_dgv.EditingControl).Select(v_startStr.Length + 1, 0);
            ((TextBox)v_dgv.EditingControl).ScrollToCaret();
        }

        #endregion

        #region save to mrc

        private void m_BtnSave_Click(object v_sender, EventArgs v_e)
        {
            if (v_tempOpenMrcFilePath.m_IsEmpty())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("First open a MARC file", new[] { "OK" }, Resources.info);
                return;
            }

            // end any in-progress edit and commit the change:
            v_dgv.EndEdit();

            // validate leader:
            if (!m_ValidateLeader())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("Error in Leader. Cancelled save.", new[] { "OK" }, Resources.error);
                return;
            }
            // validate control fields:
            if (!m_ValidateControlFields())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("Error in Control Fields. Cancelled save.", new[] { "OK" }, Resources.error);
                return;
            }
            // validate data fields:
            if (!m_ValidateDataFields())
            {
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog("Error in Data Fields. Cancelled save.", new[] { "OK" }, Resources.error);
                return;
            }

            // save modified table data:
            var v_newDataContent = m_ConvertUiToData();
            if (!v_prevDataContent.Equals(v_newDataContent))
            {
                var v_dr = c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"Has this record been previously released?", new[] { "Yes", "No", "Cancel" }, Resources.question);
                if (v_dr == "Cancel")
                {
                    return;
                }
                else if (v_dr == "Yes")
                {
                    // Set record status to 'corrected/revised (LDR 05):
                    if (v_LeaderList.Single(v_x => v_x.v_Field == "LDR [5]").v_Value != "c")
                    {
                        v_LeaderList.Single(v_x => v_x.v_Field == "LDR [5]").v_Value = "c";
                        if (v_tab == v_Tab.v_Leader) v_btnLeader.PerformClick();    // refresh leader tab.
                        c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"The record status in the Leader has been updated to indicate a corrected/revised record.", new[] { "OK" }, Resources.info);
                    }

                    // Add subfield d to 040 variable data field:
                    if (c_SettingsTools2.m_OrganizationCode.m_IsEmpty())
                    {
                        c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"First add your MARC organization code in ModMARC Settings (see Support menu).\r\n\r\nCancelled record save.", new[] { "OK" }, Resources.info);
                        return;
                    }
                    var v_field040 = v_VardataList.SingleOrDefault(v_x => v_x.v_Field == "040");
                    if (v_field040 == null)
                    {
                        c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"Unable to add Modifying Agency due to missing 040 variable data field.\r\n\r\nCancelled record save.", new[] { "OK" }, Resources.info);
                        return;
                    }
                    if (!v_field040.v_Value.EndsWith($"ǂd {c_SettingsTools2.m_OrganizationCode}"))
                    {
                        v_field040.v_Value += $" ǂd {c_SettingsTools2.m_OrganizationCode}";
                        c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"The modifying agency subfield for the 040 variable data field has been updated with your MARC organization code.", new[] { "OK" }, Resources.info);
                    }
                    if (v_tab == v_Tab.v_Vardata) v_btnVardata.PerformClick();    // refresh variable data fields tab.
                }
            }

            // generate unique save file path:
            var v_tempOpenMrcDir = Path.GetDirectoryName(v_tempOpenMrcFilePath);
            var v_tempOpenMrcFileName = Path.GetFileNameWithoutExtension(v_tempOpenMrcFilePath);
            var v_uniqueFilename = Regex.Match(v_tempOpenMrcFileName, @"_([0-9]+)$").Success ? v_tempOpenMrcFileName : v_tempOpenMrcFileName + "_1";
            var v_counter = 1;
            while (Directory.GetFiles(v_tempOpenMrcDir, $"{v_uniqueFilename}.*", SearchOption.AllDirectories).Length > 0)
            {
                v_uniqueFilename = Regex.Replace(v_uniqueFilename, @"_([0-9]+)$", m => $"_{++v_counter}");
            }
            var v_tempSaveMrcPath = Path.Combine(v_tempOpenMrcDir, $"{v_uniqueFilename}.mrc");

            // open save dialog with suggested unique save file path:
            var v_mrcSaveFilePath = "";
            using (var v_saveFileDialog = new SaveFileDialog())
            {
                v_saveFileDialog.Title = "Save to mrc file";
                v_saveFileDialog.DefaultExt = "*.mrc";
                v_saveFileDialog.Filter = "mrc files (*.mrc)|*.mrc";
                v_saveFileDialog.DefaultExt = ".mrc";
                v_saveFileDialog.OverwritePrompt = false;
                v_saveFileDialog.InitialDirectory = v_CurrentDir.m_IsSubDir(c_SettingsTools2.m_WorkspaceDir) ? v_CurrentDir : c_SettingsTools2.m_WorkspaceDir;
                v_saveFileDialog.FileName = v_tempSaveMrcPath;
                if (!Directory.Exists(v_saveFileDialog.InitialDirectory))
                {
                    v_saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                if (v_saveFileDialog.ShowDialog(this) == DialogResult.Cancel)
                {
                    c_FrmMessageBox.m_Singleton(this).m_OpenDialog("Cancelled record save.", new[] { "OK" }, Resources.info);
                    //File.WriteAllText(v_datFilePath, v_prevDatFileContent);
                    return;
                }
                v_CurrentDir = Path.GetDirectoryName(v_saveFileDialog.FileName);
                if (!Path.GetExtension(v_saveFileDialog.FileName).Equals(".mrc", StringComparison.OrdinalIgnoreCase))
                {
                    v_saveFileDialog.FileName += ".mrc";
                }
                if (File.Exists(v_saveFileDialog.FileName))
                {
                    var v_dr = c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"{v_saveFileDialog.FileName} already exists. Do you want to replace it?", new[] { "Yes", "No" }, Resources.warn);
                    if (v_dr != "Yes")
                    {
                        c_FrmMessageBox.m_Singleton(this).m_OpenDialog("Cancelled record save.", new[] { "OK" }, Resources.info);
                        //File.WriteAllText(v_datFilePath, v_prevDatFileContent);
                        return;
                    }
                }
                v_mrcSaveFilePath = v_saveFileDialog.FileName;
            }
            v_tempOpenMrcFilePath = v_mrcSaveFilePath;
            m_SetTitle(v_mrcSaveFilePath);

            // call engine to generate dat file from the mrc file:
            var lib = new modmarc_lib.c_Engine();
            var v_exitCode = lib.RunDatToMrc_Async(v_newDataContent, v_mrcSaveFilePath);

            if (v_exitCode != 0)
            {
                var v_msg = $"Failed to save record to {v_mrcSaveFilePath}. {c_StatusTools.m_GetErrorText(v_exitCode)}.";
                c_FrmMessageBox.m_Singleton(this).m_OpenDialog(v_msg, new[] { "OK" }, Resources.error);
                return;
            }

            v_prevDataContent = v_newDataContent;

            c_FrmMessageBox.m_Singleton(this).m_OpenDialog($"Record saved as {v_mrcSaveFilePath}.", new[] { "OK" }, Resources.check_mark);
        }

        private string m_ConvertUiToData()
        {
            var v_lines = new List<string>();
            v_PropertyList.ToList().ForEach(v_x => v_lines.Add($"PRP\u23F5{v_x.v_Key}\u23F5{v_x.v_Value}"));
            v_LeaderList.ToList().ForEach(v_x => v_lines.Add($"LDR\u23F5{v_x.v_Field}\u23F5{v_x.v_Name}\u23F5{v_x.v_Mnemonic}\u23F5{v_x.v_Value}"));
            v_ControlList.ToList().ForEach(v_x => v_lines.Add($"CTR\u23F5{v_x.v_Field}\u23F5{v_x.v_Name}\u23F5{v_x.v_Mnemonic}\u23F5{v_x.v_Value}"));
            v_VardataList.ToList().ForEach(v_x => v_lines.Add($"VAR\u23F5{v_x.v_Field}\u23F5{v_x.v_Ind1}\u23F5{v_x.v_Ind2}\u23F5{v_x.v_Value}"));

            return string.Join("\r\n", v_lines.Select(v_x => v_x.Replace(c_MarcSymbols.c_Human.v_Blank, ' ')));
            //File.WriteAllLines(v_datFilePath, v_lines.Select(v_x => v_x.Replace(v_BlankSymbol, " ")));
        }

        #endregion

        #region settings

        private void m_BtnSettings_Click(object v_sender, EventArgs v_e)
        {
            v_frmSettings.ShowDialog(this);
        }

        #endregion

        #region async visual

        internal void m_SetStatusMessage(string v_msg)
        {
            v_lblStatus.Text = v_msg;
        }

        internal void m_ShowStatusBarSpinner(bool v_show)
        {
            v_pbxSpinner.Enabled = v_show;
            v_pbxSpinner.Visible = v_show;
        }

        internal void m_LockCommonUi(bool v_lockUi, bool v_padlockEnable = false)
        {
            if (v_lockUi)
            {
                v_btnOpen.Enabled = false;
                //v_btnMod.Enabled = false;
                v_btnSave.Enabled = false;
                v_btnSettings.Enabled = false;
                v_btnHelp.Enabled = false;
            }
            else
            {
                v_btnOpen.Enabled = true;
                //v_btnMod.Enabled = true;
                v_btnSave.Enabled = true;
                v_btnSettings.Enabled = true;
                v_btnHelp.Enabled = true;
            }
        }

        #endregion

        #region window position

        private void m_FrmMain_LocationChanged(object v_sender, EventArgs v_e)
        {
            c_SettingsTools2.m_Coordinate = Location;
        }

        public void m_SetScreenPosition()
        {
            var v_location = c_SettingsTools2.m_Coordinate;
            if (v_location != null)
            {
                var v_windowRectangle = new Rectangle((Point)v_location, new Size(Width, Height));
                foreach (var v_screen in Screen.AllScreens)
                {
                    if (v_screen.WorkingArea.Contains(v_windowRectangle))
                    {
                        StartPosition = FormStartPosition.Manual;
                        Location = (Point)v_location;
                        return;
                    }
                }
            }

            StartPosition = FormStartPosition.CenterScreen;
        }

        #endregion

        #region drag drop file

        private bool? v_isDgvVisible;

        private void m_Ui_DragEnter(object v_sender, DragEventArgs v_e)
        {
            if (v_e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                v_isDgvVisible = v_dgv.Visible;

                var v_files = (string[])v_e.Data.GetData(DataFormats.FileDrop);
                if (v_files.Length == 1 && Path.GetExtension(v_files[0]).Equals(".mrc", StringComparison.OrdinalIgnoreCase) && File.Exists(v_files[0]))
                {
                    v_e.Effect = DragDropEffects.Link;
                    v_btnOpen.BackColor = Color.FromArgb(137, 209, 133);
                    v_dgv.Visible = false;
                }
                else
                {
                    v_e.Effect = DragDropEffects.None;
                }
            }
        }

        private void m_Ui_DragDrop(object v_sender, DragEventArgs v_e)
        {
            v_btnOpen.BackColor = default;
            if (v_e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var v_files = (string[])v_e.Data.GetData(DataFormats.FileDrop);
                if (v_files.Length == 1 && Path.GetExtension(v_files[0]).Equals(".mrc", StringComparison.OrdinalIgnoreCase) && File.Exists(v_files[0]))
                {
                    m_OpenMrcFile(v_files[0]);
                }
                else
                {
                    v_dgv.Visible = false;
                }
            }

            v_dgv.ClearSelection();
        }

        private void m_Ui_DragLeave(object v_sender, EventArgs v_e)
        {
            v_btnOpen.BackColor = default;
            v_dgv.Visible = v_isDgvVisible ?? false;
        }

        #endregion

        #region datagridview

        private void m_InitializeDgv()
        {
            // defaults:
            v_dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            v_dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            v_dgv.MultiSelect = true;
            v_dgv.ReadOnly = true;
            v_dgv.AllowUserToAddRows = false;
            v_dgv.AllowUserToDeleteRows = false;
            v_dgv.AllowUserToResizeRows = false;
            v_dgv.ColumnHeadersVisible = true;
            v_dgv.RowHeadersVisible = false;
            v_dgv.AutoGenerateColumns = true;
            v_dgv.BackgroundColor = Color.FromArgb(250, 250, 250);
            v_dgv.BorderStyle = BorderStyle.None;
            v_dgv.ReadOnly = false;
            v_dgv.EditMode = DataGridViewEditMode.EditOnEnter;
            v_dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(160, 160, 160);
            v_dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 11.25F, FontStyle.Bold);
            v_dgv.RowsDefaultCellStyle.Font = new Font("Verdana", 14F, FontStyle.Regular);
            v_rowBackColor = Color.FromArgb(254, 255, 254);

            // events:
            v_dgv.Resize += m_Dgv_Resize;
            //v_dgv.MouseEnter += (v_s, v_e) => { v_dgv.Focus(); };
            //v_dgv.MouseLeave += (v_s, v_e) => { lblMain.Focus(); };
            v_dgv.CellClick += m_Dgv_CellClick;
            //v_dgv.SelectionChanged += v_dgv_SelectionChanged;
            v_dgv.CellPainting += m_Dgv_CellPainting;
            v_dgv.CellContextMenuStripNeeded += m_Dgv_CellContextMenuStripNeeded;
            v_dgv.EditingControlShowing += m_Dgv_EditingControlShowing;
            v_dgv.CellEndEdit += m_Dgv_CellEndEdit;
            v_dgv.CellBeginEdit += m_Dgv_CellBeginEdit;
            v_dgv.KeyDown += m_KeyDown;
        }

        private void m_Dgv_CellBeginEdit(object v_sender, DataGridViewCellCancelEventArgs v_e)
        {
            // only allow continued edits to same row:
            if (v_LastEditRow != v_e.RowIndex)
            {
                v_e.Cancel = true;
                v_LastEditRow = null;
                return;
            }
        }

        private void m_Dgv_CellEndEdit(object v_sender, DataGridViewCellEventArgs v_e)
        {
            var v_cellText = (v_dgv.Rows[v_e.RowIndex].Cells[v_e.ColumnIndex]?.Value ?? "").ToString();
            if ((v_tab == v_Tab.v_Leader || v_tab == v_Tab.v_Control) && v_e.ColumnIndex == 3)
            {
                v_dgv.Rows[v_e.RowIndex].Cells[v_e.ColumnIndex].Value = v_cellText.Replace(' ', c_MarcSymbols.c_Human.v_Blank);
            }
            else if (v_tab == v_Tab.v_Vardata && (v_e.ColumnIndex == 1 || v_e.ColumnIndex == 2))
            {
                if (v_cellText.Length == 0 || string.Equals(v_cellText, " "))
                {
                    v_dgv.Rows[v_e.RowIndex].Cells[v_e.ColumnIndex].Value = c_MarcSymbols.c_Human.v_Blank;
                }
                else if (v_cellText.Length > 1)
                {
                    v_dgv.Rows[v_e.RowIndex].Cells[v_e.ColumnIndex].Value = v_cellText.Substring(0, 1);
                }
            }

            // keep track of last row edited:
            v_LastEditRow = v_e.RowIndex;

            // validate row on completion of edit:
            m_ValidateUiRow(v_dgv.Rows[v_e.RowIndex]);
        }

        private void m_PopulateDgv(ToolStripButton v_tsb, BindingList<c_LeaderItem> v_leaderList = null, BindingList<c_ControlItem> v_controlList = null, BindingList<c_VardataItem> v_vardataList = null)
        {
            v_dgv.SelectionChanged -= m_Dgv_SelectionChanged;

            v_dgv.EditMode = DataGridViewEditMode.EditProgrammatically; // make EndEdit() work when another cell is right-clicked.
            v_dgv.EndEdit();    // critical to have any current edit closed (triggers CellEndEdit event) while on same tab.

            // name column headers
            if (v_tsb == v_btnLeader)
            {
                v_tab = v_Tab.v_Leader;
                m_UpdateLeaderBindingList(v_leaderList ?? v_LeaderList);
                v_colPadding = new[] { 10, 5, 5, 0 };
                v_btnToggleHighlightFixedFields.Enabled = true;
                v_btnToggleHighlightMediaFields.Enabled = false;
                m_IndicateFixedFields();

                //v_btnUndo.Enabled = v_LeaderUndoStack.Count > 0;
                //v_btnRedo.Enabled = v_LeaderRedoStack.Count > 0;
            }
            else if (v_tsb == v_btnControl)
            {
                v_tab = v_Tab.v_Control;
                m_UpdateControlBindingList(v_controlList ?? v_ControlList);
                v_colPadding = new[] { -35, 5, 5, 0 };
                v_btnToggleHighlightFixedFields.Enabled = true;
                v_btnToggleHighlightMediaFields.Enabled = true;
                m_IndicateFixedFields();
                m_IndicateMediaFields();

                //v_btnUndo.Enabled = v_ControlUndoStack.Count > 0;
                //v_btnRedo.Enabled = v_ControlRedoStack.Count > 0;
            }
            else if (v_tsb == v_btnVardata)
            {
                v_tab = v_Tab.v_Vardata;
                m_UpdateVardataBindingList(v_vardataList ?? v_VardataList);
                v_colPadding = new[] { -15, -20, -20, 0 };
                v_btnToggleHighlightFixedFields.Enabled = false;
                v_btnToggleHighlightMediaFields.Enabled = false;
                v_dgv.Columns["v_Value"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                //v_btnUndo.Enabled = v_VardataUndoStack.Count > 0;
                //v_btnRedo.Enabled = v_VardataRedoStack.Count > 0;
            }
            else throw new Exception("Unknown tab selection");

            // set column defaults:
            foreach (DataGridViewColumn v_col in v_dgv.Columns)
            {
                v_col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // row stuff
            for (var v_i = 0; v_i < v_dgv.Rows.Count; v_i++)
            {
                // set row height:
                v_dgv.Rows[v_i].MinimumHeight = 30;
            }

            // tooltip for blank symbol:
            m_IndicateBlankSymbol();

            // handle column widths and horiz scrollbar:
            m_Dgv_Resize(null, null);

            // set selected tab backcolor:
            v_btnLeader.ForeColor = Color.FromArgb(220, 220, 220);
            v_btnControl.ForeColor = Color.FromArgb(220, 220, 220);
            v_btnVardata.ForeColor = Color.FromArgb(220, 220, 220);
            v_tsb.ForeColor = v_tabHighlightColor;

            v_dgv.ClearSelection();

            v_dgv.SelectionChanged += m_Dgv_SelectionChanged;

            m_ValidateUiTab();
        }

        private void m_Dgv_Resize(object v_sender, EventArgs v_e)
        {
            v_dgv.SelectionChanged -= m_Dgv_SelectionChanged;

            // expand last column to fill table:
            var v_widthList = new List<int>();

            // set column mode and store widths:
            for (var v_i = 0; v_i < v_dgv.Columns.Count; v_i++)
            {
                v_dgv.Columns[v_i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                v_widthList.Add(v_dgv.Columns[v_i].Width);
            }

            // remove padding from columns:
            for (var v_i = 0; v_i < v_widthList.Count; v_i++)
            {
                v_dgv.Columns[v_i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                v_dgv.Columns[v_i].Width = v_widthList[v_i] + v_colPadding[v_i];
            }

            // reset column modes:
            for (var v_i = 0; v_i < v_dgv.Columns.Count - 1; v_i++)
            {
                v_dgv.Columns[v_i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            // fill last column width:
            if (v_widthList.Sum() < v_dgv.Width && v_dgv.Columns.Count > 0)
            {
                v_dgv.Columns[v_dgv.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                v_widthList.RemoveAt(v_dgv.Columns.Count - 1);
            }
            else
            {
                if (v_dgv.Columns.Count > 0)
                {
                    // add padding back to last column:
                    v_dgv.Columns[v_dgv.Columns.Count - 1].Width -= v_colPadding.Sum();
                }
            }

            v_dgv.SelectionChanged += m_Dgv_SelectionChanged;
        }

        private void m_Dgv_CellClick(object v_sender, DataGridViewCellEventArgs v_e)
        {
            if (v_e.RowIndex == -1)
            {
                // user click on any column header clears all selected cells in table:
                v_dgv.ClearSelection();
            }
        }

        private void m_Dgv_SelectionChanged(object v_sender, EventArgs v_e)
        {
            // triggered when cells are selected by user.
            // triggered once for single-cell selection and twice for multi-cell selection.

            // only allow continued edits to same row:
            if (v_dgv.SelectedCells.Count > 0 && v_LastEditRow != v_dgv.SelectedCells[0].RowIndex)
            {
                v_dgv.EditMode = DataGridViewEditMode.EditProgrammatically; // make EndEdit() work when another cell is right-clicked.
                v_dgv.EndEdit();    // end any in-progress edit and commit the change.
            }

            m_ClearAllRows();
            if (v_dgv.SelectedCells.Count == 0) return;
            v_dgv.SelectedCells.Cast<DataGridViewCell>().ToList().ForEach(v_x => m_HighlightRow(v_x.RowIndex));
        }

        private void m_ClearAllRows()
        {
            foreach (DataGridViewRow v_row in v_dgv.Rows)
            {
                v_row.DefaultCellStyle.BackColor = v_rowBackColor;
            }
        }

        private void m_HighlightRow(int v_rowIndex)
        {
            v_dgv.Rows[v_rowIndex].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#C2E0FF");
        }

        private void m_IndicateFixedFields()
        {
            if (v_tab == v_Tab.v_Vardata || v_dgv.ColumnCount < 4) return;

            foreach (DataGridViewRow v_row in v_dgv.Rows)
            {
                if (v_row.Cells["v_Mnemonic"].Value.ToString().m_IsEmpty()) continue;
                v_row.Cells["v_Mnemonic"].Style.BackColor = v_highlightFixedFields ? Color.FromArgb(252, 248, 189) : v_rowBackColor;
                v_row.Cells["v_Mnemonic"].ToolTipText = "Fixed Field";
            }
        }

        private void m_btnToggleHighlightFixedFields_Click(object v_sender, EventArgs v_e)
        {
            v_highlightFixedFields = !v_highlightFixedFields;
            m_IndicateFixedFields();
        }

        private void m_IndicateMediaFields()
        {
            if (v_tab == v_Tab.v_Vardata || v_dgv.ColumnCount < 4) return;

            foreach (DataGridViewRow v_row in v_dgv.Rows)
            {
                var v_value = v_row.Cells["v_Field"]?.Value?.ToString();
                var v_mediaMatch = Regex.Match(v_value, @"008 \[(\d\d)");
                if (v_mediaMatch.Success)
                {
                    var v_posIndex = int.Parse(v_mediaMatch.Groups[1].Value);
                    if (v_posIndex < 18 || v_posIndex > 34) continue;

                    // set backcolor:
                    v_row.Cells["v_Field"].Style.BackColor = v_highlightMediaFields ? Color.FromArgb(215, 237, 216) : v_rowBackColor;
                    v_row.Cells["v_Name"].Style.BackColor = v_highlightMediaFields ? Color.FromArgb(215, 237, 216) : v_rowBackColor;

                    // set tooltip:
                    var v_materialType = v_PropertyList.Single(v_x => v_x.v_Key == "MaterialType").v_Value;
                    v_row.Cells["v_Field"].ToolTipText = $"Material type: {v_materialType}";
                    v_row.Cells["v_Name"].ToolTipText = $"Material type: {v_materialType}";
                }
            }
        }

        private void m_btnToggleHighlightMediaFields_Click(object v_sender, EventArgs v_e)
        {
            v_highlightMediaFields = !v_highlightMediaFields;
            m_IndicateMediaFields();
        }

        private void m_IndicateBlankSymbol()
        {
            if (v_tab == v_Tab.v_Leader || v_tab == v_Tab.v_Control)
            {
                foreach (DataGridViewRow v_row in v_dgv.Rows)
                {
                    if (v_row.Cells["v_Value"].Value.ToString() == c_MarcSymbols.c_Human.v_Blank.ToString())
                    {
                        v_row.Cells["v_Value"].ToolTipText = "Blank symbol";
                    }
                }
            }
            if (v_tab == v_Tab.v_Vardata)
            {
                foreach (DataGridViewRow v_row in v_dgv.Rows)
                {
                    if (v_row.Cells["v_Ind1"].Value.ToString() == c_MarcSymbols.c_Human.v_Blank.ToString())
                    {
                        v_row.Cells["v_Ind1"].ToolTipText = "Blank symbol";
                    }
                    if (v_row.Cells["v_Ind2"].Value.ToString() == c_MarcSymbols.c_Human.v_Blank.ToString())
                    {
                        v_row.Cells["v_Ind2"].ToolTipText = "Blank symbol";
                    }
                }
            }
        }

        private void m_Dgv_CellPainting(object v_sender, DataGridViewCellPaintingEventArgs v_e)
        {
            if (v_e.RowIndex == -1)
            {
                v_e.Paint(v_e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                using (Pen v_customPen = new Pen(SystemColors.Control, 1))
                {
                    Rectangle v_rect = v_e.CellBounds;
                    v_rect.Width -= 2;
                    v_rect.Height -= 2;
                    v_e.Graphics.DrawRectangle(v_customPen, v_rect);
                }
                v_e.Handled = true;
            }
        }

        private void m_Dgv_EditingControlShowing(object v_sender, DataGridViewEditingControlShowingEventArgs v_e)
        {
            v_e.Control.ContextMenuStrip = new ContextMenuStrip
            {
                //ShowCheckMargin = false,
                //ShowImageMargin = false
            };
            v_e.Control.ContextMenuStrip.Items.Add("Insert diacritic", Resources.diacritic, (v_s, v_e) => { m_BtnDiacritic_Click(null, null); });
            v_e.Control.ContextMenuStrip.Items.Add("Insert symbol", Resources.symbol, (v_s, v_e) => { m_BtnSymbol_Click(null, null); });
            v_e.Control.ContextMenuStrip.Items.Add("Insert subfield delimiter", Resources.delimiter, (v_s, v_e) => { m_BtnSubfieldDelim_Click(null, null); });
            v_e.Control.ContextMenuStrip.Items.Add("Insert blank character", Resources.blank, (v_s, v_e) => { m_BtnBlank_Click(null, null); });

            v_e.Control.KeyPress += m_DgvCell_KeyPress; // catch key presses during edit.
            var v_tb = (DataGridViewTextBoxEditingControl)v_e.Control;
            if (v_tab == v_Tab.v_Leader && v_dgv.CurrentCell.ColumnIndex == 3) v_tb.MaxLength = 1;
            if (v_tab == v_Tab.v_Vardata && v_dgv.CurrentCell.ColumnIndex == 0) v_tb.MaxLength = 3;
            if (v_tab == v_Tab.v_Vardata && v_dgv.CurrentCell.ColumnIndex == 1) v_tb.MaxLength = 1;
            if (v_tab == v_Tab.v_Vardata && v_dgv.CurrentCell.ColumnIndex == 2) v_tb.MaxLength = 1;

            if (v_tab == v_Tab.v_Control && v_dgv.CurrentCell.ColumnIndex == 3)
            {
                // set max length:
                var v_selectedField = v_dgv.Rows[v_dgv.CurrentRow.Index].Cells["v_Field"]?.Value?.ToString();
                var v_singleCharLenMatch = Regex.Match(v_selectedField, @"\[([\d]{1,2})\]");
                var v_multiCharLenMatch = Regex.Match(v_selectedField, @"\[([\d]{1,2})-([\d]{1,2})\]");
                if (v_singleCharLenMatch.Success) v_tb.MaxLength = 1;
                else if (v_multiCharLenMatch.Success)
                {
                    var v_upperVal = int.Parse(v_multiCharLenMatch.Groups[2].Value);
                    var v_lowerVal = int.Parse(v_multiCharLenMatch.Groups[1].Value);
                    v_tb.MaxLength = v_upperVal - v_lowerVal + 1;
                }
            }
        }

        private void m_DgvCell_KeyPress(object v_sender, KeyPressEventArgs v_e)
        {
            if (v_tab == v_Tab.v_Vardata && v_dgv.CurrentCell.ColumnIndex == 0 && !v_e.KeyChar.ToString().m_IsNumeric() && !char.IsControl(v_e.KeyChar)) v_e.Handled = true;
            if (v_tab == v_Tab.v_Vardata && (v_dgv.CurrentCell.ColumnIndex == 1 || v_dgv.CurrentCell.ColumnIndex == 2) &&
                !v_e.KeyChar.ToString().m_IsNumeric() && v_e.KeyChar != ' ' && !char.IsControl(v_e.KeyChar)) v_e.Handled = true;
        }

        private void m_Dgv_CellContextMenuStripNeeded(object v_sender, DataGridViewCellContextMenuStripNeededEventArgs v_e)
        {
            if (v_e.RowIndex < 0) return;
            v_dgv.ClearSelection();
            v_dgv.Rows[v_e.RowIndex].Cells[v_e.ColumnIndex].Selected = true;
            m_HighlightRow(v_e.RowIndex);

            var v_cms = new ContextMenuStrip
            {
                //ShowCheckMargin = false,
                //ShowImageMargin = false
            };
            if (v_e.RowIndex > -1)
            {
                if (v_tab == v_Tab.v_Leader)
                {
                    if (v_e.ColumnIndex == 3)
                    {
                        v_cms.Items.Add("Modify value", Resources.edit, (v_s1, v_e1) => { m_BtnModifyValue_Click(null, null); });
                    }
                }
                else if (v_tab == v_Tab.v_Control)
                {
                    var v_name = v_dgv.Rows[v_e.RowIndex].Cells["v_Name"]?.Value?.ToString();
                    if (v_e.ColumnIndex == 3 && v_name != "Undefined")
                    {
                        v_cms.Items.Add("Modify value", Resources.edit, (v_s1, v_e1) => { m_BtnModifyValue_Click(null, null); });
                    }
                }
                else if (v_tab == v_Tab.v_Vardata)
                {
                    v_cms.Items.Add("Modify field", Resources.edit, (v_s, v_e) => { m_BtnModifyValue_Click(null, null); });
                    v_cms.Items.Add("Duplicate field", Resources.copy, (v_s, v_e) => { m_BtnDuplicateField_Click(null, null); });
                    v_cms.Items.Add("Add new field", Resources.add, (v_s, v_e) => { m_BtnAddField_Click(null, null); });
                    v_cms.Items.Add("Delete field", Resources.delete, (v_s, v_e) => { m_BtnDelField_Click(null, null); });
                    v_cms.Items.Add("Move field up", Resources.arrow_up, (v_s, v_e) => { m_BtnMoveUpField_Click(null, null); });
                    v_cms.Items.Add("Move field down", Resources.arrow_dn, (v_s, v_e) => { m_BtnMoveDnField_Click(null, null); });
                }

                var v_fieldVal = v_dgv.Rows[v_e.RowIndex].Cells[0].Value?.ToString() ?? "";
                var v_fieldId = v_fieldVal.Length >= 3 ? v_fieldVal.Substring(0, 3) : "";
                var v_tempVal = v_dgv.Rows[v_e.RowIndex].Cells[2]?.Value?.ToString();
                var v_mnemonic = (v_tab == v_Tab.v_Leader || v_tab == v_Tab.v_Control) && v_tempVal.m_IsNotEmpty() ? v_tempVal : null;
                var v_helpList = m_GetHelpUrls(v_fieldId, v_mnemonic);
                foreach (var v_helpItem in v_helpList)
                {
                    ToolStripMenuItem v_tsItem;
                    if (v_helpItem.v_Url.m_IsEmpty()) v_tsItem = new ToolStripMenuItem(v_helpItem.v_Desc, image: null, onClick: null);
                    else v_tsItem = new ToolStripMenuItem(v_helpItem.v_Desc, Resources.help, (v_s, v_e) => { c_HelpTools.m_OpenUrl(v_helpItem.v_Url); });
                    v_tsItem.ImageScaling = ToolStripItemImageScaling.None;
                    v_cms.Items.Add(v_tsItem);
                }
            }
            v_e.ContextMenuStrip = v_cms;
        }

        #endregion

        #region validation

        private bool m_ValidateLeader()
        {
            var v_isSuccess = true;

            foreach (var v_item in v_LeaderList)
            {
                if (!m_ValidateLdrCtrlLength(v_item.v_Field, v_item.v_Value, out string _)) v_isSuccess = false;
                if (!m_ValidateLdrValue(v_item.v_Field, v_item.v_Name, v_item.v_Mnemonic, v_item.v_Value, out string _)) v_isSuccess = false;
            }

            return v_isSuccess;
        }

        private bool m_ValidateControlFields()
        {
            var v_isSuccess = true;

            foreach (var v_item in v_ControlList)
            {
                if (!m_ValidateLdrCtrlLength(v_item.v_Field, v_item.v_Value, out string _)) v_isSuccess = false;
                if (!m_ValidateCtrlValue(v_item.v_Field, v_item.v_Name, v_item.v_Mnemonic, v_item.v_Value, out string _)) v_isSuccess = false;
            }

            return v_isSuccess;
        }

        private bool m_ValidateDataFields()
        {
            var v_isSuccess = true;

            foreach (var v_item in v_VardataList)
            {
                if (!m_ValidateDataLength(3, v_item.v_Field, out string _)) v_isSuccess = false;
                if (!m_ValidateDataLength(1, v_item.v_Ind1, out string _)) v_isSuccess = false;
                if (!m_ValidateDataLength(1, v_item.v_Ind2, out string _)) v_isSuccess = false;
                if (!m_ValidateDataIndicator1(v_item.v_Field, v_item.v_Ind1, out string _)) v_isSuccess = false;
                if (!m_ValidateDataIndicator2(v_item.v_Field, v_item.v_Ind2, out string _)) v_isSuccess = false;
                if (!m_ValidateDataSubfields(v_item.v_Field, v_item.v_Ind1, v_item.v_Ind2, v_item.v_Value, out string _)) v_isSuccess = false;
            }

            return v_isSuccess;
        }

        private void m_ValidateUiTab()
        {
            foreach (DataGridViewRow v_row in v_dgv.Rows)
            {
                m_ValidateUiRow(v_row);
            }
        }

        private bool m_ValidateUiRow(DataGridViewRow v_row)
        {
            var v_isValid = false;

            if (v_tab == v_Tab.v_Leader)
            {
                var v_field = v_row.Cells["v_Field"]?.Value?.ToString();
                var v_name = v_row.Cells["v_Name"]?.Value?.ToString();
                var v_mnemonic = v_row.Cells["v_Mnemonic"]?.Value?.ToString();
                var v_value = v_row.Cells["v_Value"]?.Value?.ToString();
                v_isValid = m_ValidateLdrCtrlLength(v_field, v_value, out string v_errMsg1);
                v_isValid &= m_ValidateLdrValue(v_field, v_name, v_mnemonic, v_value, out string v_errMsg2);
                v_row.Cells["v_Value"].Style.BackColor = v_isValid ? v_rowBackColor : v_ErrorBackColor;
                var v_errMsg = string.Join("\r\n", new[] { v_errMsg1, v_errMsg2 }.Where(v_x => v_x.m_IsNotEmpty()));
                if (v_errMsg.m_IsNotEmpty()) v_row.Cells["v_Value"].ToolTipText = v_errMsg;
            }
            else if (v_tab == v_Tab.v_Control)
            {
                var v_field = v_row.Cells["v_Field"]?.Value?.ToString();
                var v_name = v_row.Cells["v_Name"]?.Value?.ToString();
                var v_mnemonic = v_row.Cells["v_Mnemonic"]?.Value?.ToString();
                var v_value = v_row.Cells["v_Value"]?.Value?.ToString();
                v_isValid = m_ValidateLdrCtrlLength(v_field, v_value, out string v_errMsg1);
                v_isValid &= m_ValidateCtrlValue(v_mnemonic, v_name, v_mnemonic, v_value, out string v_errMsg2);
                v_row.Cells["v_Value"].Style.BackColor = v_isValid ? v_rowBackColor : v_ErrorBackColor;
                var v_errMsg = string.Join("\r\n", new[] { v_errMsg1, v_errMsg2 }.Where(v_x => v_x.m_IsNotEmpty()));
                if (v_errMsg.m_IsNotEmpty()) v_row.Cells["v_Value"].ToolTipText = v_errMsg;
            }
            else if (v_tab == v_Tab.v_Vardata)
            {
                var v_tag = v_row.Cells["v_Field"]?.Value?.ToString();
                var v_ind1 = v_row.Cells["v_Ind1"]?.Value?.ToString();
                var v_ind2 = v_row.Cells["v_Ind2"]?.Value?.ToString();
                var v_value = v_row.Cells["v_Value"]?.Value?.ToString();

                var v_isValidTagLen = m_ValidateDataLength(3, v_tag, out string v_errMsg);
                v_row.Cells["v_Field"].Style.BackColor = v_isValidTagLen ? v_rowBackColor : Color.FromArgb(255, 128, 128);
                if (v_errMsg.m_IsNotEmpty()) v_row.Cells["v_Field"].ToolTipText = v_errMsg;

                var v_isValidInd1Len = m_ValidateDataLength(1, v_ind1, out v_errMsg);
                v_row.Cells["v_Ind1"].Style.BackColor = v_isValidInd1Len ? v_rowBackColor : Color.FromArgb(255, 128, 128);
                if (v_errMsg.m_IsNotEmpty()) v_row.Cells["v_Ind1"].ToolTipText = v_errMsg;

                var v_isValidInd2Len = m_ValidateDataLength(1, v_ind2, out v_errMsg);
                v_row.Cells["v_Ind2"].Style.BackColor = v_isValidInd2Len ? v_rowBackColor : Color.FromArgb(255, 128, 128);
                if (v_errMsg.m_IsNotEmpty()) v_row.Cells["v_Ind2"].ToolTipText = v_errMsg;

                var v_isValidInd1Value = m_ValidateDataIndicator1(v_tag, v_ind1, out v_errMsg);
                v_row.Cells["v_Ind1"].Style.BackColor = v_isValidInd1Value ? v_rowBackColor : Color.FromArgb(255, 128, 128);
                if (v_errMsg.m_IsNotEmpty()) v_row.Cells["v_Ind1"].ToolTipText = v_errMsg;

                var v_isValidInd2Value = m_ValidateDataIndicator2(v_tag, v_ind2, out v_errMsg);
                v_row.Cells["v_Ind2"].Style.BackColor = v_isValidInd2Value ? v_rowBackColor : Color.FromArgb(255, 128, 128);
                if (v_errMsg.m_IsNotEmpty()) v_row.Cells["v_Ind2"].ToolTipText = v_errMsg;

                var v_isValidSubfields = m_ValidateDataSubfields(v_tag, v_ind1, v_ind2, v_value, out v_errMsg);
                v_row.Cells["v_Value"].Style.BackColor = v_isValidSubfields ? v_rowBackColor : Color.FromArgb(255, 128, 128);
                if (v_errMsg.m_IsNotEmpty()) v_row.Cells["v_Value"].ToolTipText = v_errMsg;

                v_isValid = v_isValidTagLen && v_isValidInd1Len && v_isValidInd2Len && v_isValidInd1Value && v_isValidInd2Value && v_isValidSubfields;
            }

            return v_isValid;
        }

        private bool m_ValidateLdrValue(string v_field, string v_name, string v_mnemonic, string v_value, out string v_errMsg)
        {
            var v_isValidVal = true;
            var v_blank = c_MarcSymbols.c_Human.v_Blank.ToString();

            if (v_mnemonic == "Rec stat") v_isValidVal = v_value.m_ContainsOnly(new[] { "a", "c", "d", "n", "p" });
            else if (v_mnemonic == "Type") v_isValidVal = v_value.m_ContainsOnly(new[] { "a", "c", "d", "e", "f", "g", "i", "j", "k", "m", "o", "p", "r", "t" });
            else if (v_mnemonic == "BLvl") v_isValidVal = v_value.m_ContainsOnly(new[] { "a", "b", "c", "d", "i", "m", "s" });
            else if (v_mnemonic == "Ctrl") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "a" });
            else if (v_mnemonic == "ELvl") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "1", "2", "3", "4", "5", "7", "8", "I", "J", "K", "M", "u", "z" });
            else if (v_mnemonic == "Desc") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "a", "c", "i", "n", "u" });
            else if (v_field == "LDR [19]") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "a", "b", "c" });

            v_errMsg = v_isValidVal ? "" : $"Invalid value (see field help page)";
            return v_isValidVal;
        }

        private bool m_ValidateCtrlValue(string v_field, string v_name, string v_mnemonic, string v_value, out string v_errMsg)
        {
            var v_isValidVal = true;
            var v_blank = c_MarcSymbols.c_Human.v_Blank.ToString();

            if (v_mnemonic == "DtSt") v_isValidVal = v_value.m_ContainsOnly(new[] { "|", "b", "c", "d", "e", "i", "k", "m", "n", "p", "q", "r", "s", "t", "u" });
            else if (v_mnemonic == "Dates") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "|", "u", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            else if (v_mnemonic == "Ctry") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "|", "-", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" });
            else if (v_mnemonic == "Ills") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "|", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "o", "p" });
            else if (v_mnemonic == "Audn") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "|", "a", "b", "c", "d", "e", "f", "g", "j" });
            else if (v_mnemonic == "Form") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "|", "a", "b", "c", "d", "f", "o", "q", "r", "s" });
            else if (v_mnemonic == "Cont") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "|", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "2", "5", "6" });
            else if (v_mnemonic == "GPub") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "|", "a", "c", "f", "i", "l", "m", "o", "s", "u", "z" });
            else if (v_mnemonic == "Conf") v_isValidVal = v_value.m_ContainsOnly(new[] { "|", "0", "1" });
            else if (v_mnemonic == "Fest") v_isValidVal = v_value.m_ContainsOnly(new[] { "|", "0", "1" });
            else if (v_mnemonic == "Indx") v_isValidVal = v_value.m_ContainsOnly(new[] { "|", "0", "1" });
            else if (v_mnemonic == "LitF") v_isValidVal = v_value.m_ContainsOnly(new[] { "|", "0", "1", "d", "e", "f", "h", "i", "j", "m", "p", "s", "u" });
            else if (v_mnemonic == "Biog") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "|", "a", "b", "c", "d" });
            else if (v_mnemonic == "Lang") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "|", "-", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" });
            else if (v_mnemonic == "Srce") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank, "|", "c", "d" });
            else if (v_name == "Undefined") v_isValidVal = v_value.m_ContainsOnly(new[] { v_blank });

            v_errMsg = v_isValidVal ? "" : $"Invalid value (see field help page)";
            return v_isValidVal;
        }

        private bool m_ValidateLdrCtrlLength(string v_field, string v_value, out string v_errMsg)
        {
            var v_isValidLen = true;

            // determine correct number of characters:
            var v_singleCharLenMatch = Regex.Match(v_field, @"\[([\d]{1,2})\]");
            var v_multiCharLenMatch = Regex.Match(v_field, @"\[([\d]{1,2})-([\d]{1,2})\]");
            int v_reqLength = -1;
            if (v_singleCharLenMatch.Success)
            {
                v_reqLength = 1;
                v_isValidLen = v_reqLength == v_value.Length;
            }
            else if (v_multiCharLenMatch.Success)
            {
                var v_upperVal = int.Parse(v_multiCharLenMatch.Groups[2].Value);
                var v_lowerVal = int.Parse(v_multiCharLenMatch.Groups[1].Value);
                v_reqLength = v_upperVal - v_lowerVal + 1;
                v_isValidLen = v_reqLength == v_value.Length;
            }

            v_errMsg = v_isValidLen ? "" : $"Invalid length (must be {v_reqLength} characters)";
            return v_isValidLen;
        }

        private bool m_ValidateDataLength(int v_reqLength, string v_value, out string v_errMsg)
        {
            var v_isValidLen = v_reqLength == v_value?.Length;

            v_errMsg = v_isValidLen ? "" : $"Invalid length (must be {v_reqLength} characters)";
            return v_isValidLen;
        }

        private bool m_ValidateDataIndicator1(string v_tag, string v_ind, out string v_errMsg)
        {
            var v_isValidVal = true;
            var v_blank = c_MarcSymbols.c_Human.v_Blank.ToString();

            if (string.IsNullOrEmpty(v_ind)) v_isValidVal = false;
            else if (v_tag == "010" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "013" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "015" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "016" && !v_ind.m_ContainsOnly(new[] { v_blank, "7" })) v_isValidVal = false;
            else if (v_tag == "017" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "018" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "020" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "022" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "024" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "025" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "026" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "027" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "028" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6" })) v_isValidVal = false;
            else if (v_tag == "030" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "031" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "032" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "033" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "034" && !v_ind.m_ContainsOnly(new[] { "0", "1", "3" })) v_isValidVal = false;
            else if (v_tag == "035" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "036" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "037" && !v_ind.m_ContainsOnly(new[] { v_blank, "2", "3" })) v_isValidVal = false;
            else if (v_tag == "040" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "041" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "042" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "043" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "044" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "045" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "046" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "047" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "048" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "050" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "051" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "052" && !v_ind.m_ContainsOnly(new[] { v_blank, "1", "7" })) v_isValidVal = false;
            else if (v_tag == "055" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "060" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "061" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "066" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "070" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "071" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "072" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "074" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "080" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "082" && !v_ind.m_ContainsOnly(new[] { "0", "1", "7" })) v_isValidVal = false;
            else if (v_tag == "083" && !v_ind.m_ContainsOnly(new[] { "0", "1", "7" })) v_isValidVal = false;
            else if (v_tag == "084" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "085" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "086" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "088" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "100" && !v_ind.m_ContainsOnly(new[] { "0", "1", "3" })) v_isValidVal = false;
            else if (v_tag == "110" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "111" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "130" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "210" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "222" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "240" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "242" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "243" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "245" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "246" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3" })) v_isValidVal = false;
            else if (v_tag == "247" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "250" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "251" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "254" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "255" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "256" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "257" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "258" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "260" && !v_ind.m_ContainsOnly(new[] { v_blank, "2", "3" })) v_isValidVal = false;
            else if (v_tag == "263" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "264" && !v_ind.m_ContainsOnly(new[] { v_blank, "2", "3" })) v_isValidVal = false;
            else if (v_tag == "270" && !v_ind.m_ContainsOnly(new[] { v_blank, "1", "2" })) v_isValidVal = false;
            else if (v_tag == "300" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "306" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "307" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "310" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "321" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "336" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "337" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "338" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "340" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "341" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "342" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "343" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "344" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "345" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "346" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "347" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "348" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "351" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "352" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "355" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "8" })) v_isValidVal = false;
            else if (v_tag == "357" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "362" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "363" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "365" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "366" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "370" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "377" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "380" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "381" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "382" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "383" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "384" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "385" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "386" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "388" && !v_ind.m_ContainsOnly(new[] { v_blank, "1", "2" })) v_isValidVal = false;
            else if (v_tag == "490" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "500" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "501" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "502" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "504" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "505" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "8" })) v_isValidVal = false;
            else if (v_tag == "506" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "507" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "508" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "510" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4" })) v_isValidVal = false;
            else if (v_tag == "511" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "513" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "514" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "515" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "516" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "518" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "520" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2", "3", "4", "8" })) v_isValidVal = false;
            else if (v_tag == "521" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2", "3", "4", "8" })) v_isValidVal = false;
            else if (v_tag == "522" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "524" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "525" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "526" && !v_ind.m_ContainsOnly(new[] { "0", "8" })) v_isValidVal = false;
            else if (v_tag == "530" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "532" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "8" })) v_isValidVal = false;
            else if (v_tag == "533" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "534" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "535" && !v_ind.m_ContainsOnly(new[] { "1", "2" })) v_isValidVal = false;
            else if (v_tag == "536" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "538" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "540" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "541" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "542" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "544" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "545" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "546" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "547" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "550" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "552" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "555" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "8" })) v_isValidVal = false;
            else if (v_tag == "556" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "561" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "562" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "563" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "565" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "8" })) v_isValidVal = false;
            else if (v_tag == "567" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "580" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "581" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "583" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "584" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "585" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "586" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "588" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "600" && !v_ind.m_ContainsOnly(new[] { "0", "1", "3" })) v_isValidVal = false;
            else if (v_tag == "610" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "611" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "630" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "647" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "648" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "650" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "651" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "653" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "654" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "655" && !v_ind.m_ContainsOnly(new[] { v_blank, "0" })) v_isValidVal = false;
            else if (v_tag == "656" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "657" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "658" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "662" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "700" && !v_ind.m_ContainsOnly(new[] { "0", "1", "3" })) v_isValidVal = false;
            else if (v_tag == "710" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "711" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "720" && !v_ind.m_ContainsOnly(new[] { v_blank, "1", "2" })) v_isValidVal = false;
            else if (v_tag == "730" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "740" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "751" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "752" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "753" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "754" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "758" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "760" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "762" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "765" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "767" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "770" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "772" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "773" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "774" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "775" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "776" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "777" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "780" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "785" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "786" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "787" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "800" && !v_ind.m_ContainsOnly(new[] { "0", "1", "3" })) v_isValidVal = false;
            else if (v_tag == "810" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "811" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "830" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "856" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2", "3", "4", "7" })) v_isValidVal = false;
            else if (v_tag == "882" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "883" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "884" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "885" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "886" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "887" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;

            v_errMsg = v_isValidVal ? "" : $"Invalid indicator 1 (see field help page)";
            return v_isValidVal;
        }

        private bool m_ValidateDataIndicator2(string v_tag, string v_ind, out string v_errMsg)
        {
            var v_isValidVal = true;
            var v_blank = c_MarcSymbols.c_Human.v_Blank.ToString();

            if (string.IsNullOrEmpty(v_ind)) v_isValidVal = false;
            else if (v_tag == "010" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "013" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "015" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "016" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "017" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "018" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "020" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "022" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "024" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "025" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "026" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "027" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "028" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3" })) v_isValidVal = false;
            else if (v_tag == "030" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "031" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "032" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "033" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2" })) v_isValidVal = false;
            else if (v_tag == "034" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "035" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "036" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "037" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "040" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "041" && !v_ind.m_ContainsOnly(new[] { v_blank, "7" })) v_isValidVal = false;
            else if (v_tag == "042" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "043" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "044" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "045" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "046" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "047" && !v_ind.m_ContainsOnly(new[] { v_blank, "7" })) v_isValidVal = false;
            else if (v_tag == "048" && !v_ind.m_ContainsOnly(new[] { v_blank, "7" })) v_isValidVal = false;
            else if (v_tag == "050" && !v_ind.m_ContainsOnly(new[] { "0", "4" })) v_isValidVal = false;
            else if (v_tag == "051" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "052" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "055" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "060" && !v_ind.m_ContainsOnly(new[] { "0", "4" })) v_isValidVal = false;
            else if (v_tag == "061" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "066" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "070" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "071" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "072" && !v_ind.m_ContainsOnly(new[] { "0", "7" })) v_isValidVal = false;
            else if (v_tag == "074" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "080" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "082" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "4" })) v_isValidVal = false;
            else if (v_tag == "083" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "084" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "085" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "086" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "088" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "100" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "110" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "111" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "130" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "210" && !v_ind.m_ContainsOnly(new[] { v_blank, "0" })) v_isValidVal = false;
            else if (v_tag == "222" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "240" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "242" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "243" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "245" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "246" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2", "3", "4", "5", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "247" && !v_ind.m_ContainsOnly(new[] { "0", "1" })) v_isValidVal = false;
            else if (v_tag == "250" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "251" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "254" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "255" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "256" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "257" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "258" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "260" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "263" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "264" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4" })) v_isValidVal = false;
            else if (v_tag == "270" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "7" })) v_isValidVal = false;
            else if (v_tag == "300" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "306" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "307" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "310" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "321" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "336" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "337" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "338" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "340" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "341" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "342" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "343" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "344" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "345" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "346" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "347" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "348" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "351" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "352" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "355" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "357" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "362" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "363" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "365" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "366" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "370" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "377" && !v_ind.m_ContainsOnly(new[] { v_blank, "7" })) v_isValidVal = false;
            else if (v_tag == "380" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "381" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "382" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1" })) v_isValidVal = false;
            else if (v_tag == "383" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "384" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "385" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "386" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "388" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "490" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "500" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "501" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "502" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "504" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "505" && !v_ind.m_ContainsOnly(new[] { v_blank, "0" })) v_isValidVal = false;
            else if (v_tag == "506" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "507" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "508" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "510" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "511" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "513" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "514" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "515" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "516" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "518" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "520" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "521" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "522" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "524" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "525" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "526" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "530" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "532" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "533" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "534" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "535" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "536" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "538" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "540" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "541" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "542" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "544" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "545" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "546" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "547" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "550" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "552" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "555" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "556" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "561" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "562" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "563" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "565" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "567" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "580" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "581" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "583" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "584" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "585" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "586" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "588" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "600" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7" })) v_isValidVal = false;
            else if (v_tag == "610" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7" })) v_isValidVal = false;
            else if (v_tag == "611" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7" })) v_isValidVal = false;
            else if (v_tag == "630" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7" })) v_isValidVal = false;
            else if (v_tag == "647" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7" })) v_isValidVal = false;
            else if (v_tag == "648" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7" })) v_isValidVal = false;
            else if (v_tag == "650" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7" })) v_isValidVal = false;
            else if (v_tag == "651" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7" })) v_isValidVal = false;
            else if (v_tag == "653" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2", "3", "4", "5", "6" })) v_isValidVal = false;
            else if (v_tag == "654" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "655" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7" })) v_isValidVal = false;
            else if (v_tag == "656" && !v_ind.m_ContainsOnly(new[] { "7" })) v_isValidVal = false;
            else if (v_tag == "657" && !v_ind.m_ContainsOnly(new[] { "7" })) v_isValidVal = false;
            else if (v_tag == "658" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "662" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "700" && !v_ind.m_ContainsOnly(new[] { v_blank, "2" })) v_isValidVal = false;
            else if (v_tag == "710" && !v_ind.m_ContainsOnly(new[] { v_blank, "2" })) v_isValidVal = false;
            else if (v_tag == "711" && !v_ind.m_ContainsOnly(new[] { v_blank, "2" })) v_isValidVal = false;
            else if (v_tag == "720" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "730" && !v_ind.m_ContainsOnly(new[] { v_blank, "2" })) v_isValidVal = false;
            else if (v_tag == "740" && !v_ind.m_ContainsOnly(new[] { v_blank, "2" })) v_isValidVal = false;
            else if (v_tag == "751" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "752" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "753" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "754" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "758" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "760" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "762" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "765" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "767" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "770" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "772" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "8" })) v_isValidVal = false;
            else if (v_tag == "773" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "774" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "775" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "776" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "777" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "780" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7" })) v_isValidVal = false;
            else if (v_tag == "785" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "786" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "787" && !v_ind.m_ContainsOnly(new[] { v_blank, "8" })) v_isValidVal = false;
            else if (v_tag == "800" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "810" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "811" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "830" && !v_ind.m_ContainsOnly(new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })) v_isValidVal = false;
            else if (v_tag == "856" && !v_ind.m_ContainsOnly(new[] { v_blank, "0", "1", "2", "8" })) v_isValidVal = false;
            else if (v_tag == "882" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "883" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "884" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "885" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "886" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;
            else if (v_tag == "887" && !v_ind.m_ContainsOnly(new[] { v_blank })) v_isValidVal = false;

            v_errMsg = v_isValidVal ? "" : $"Invalid indicator 2 (see field help page)";
            return v_isValidVal;
        }

        private bool m_ValidateDataSubfields(string v_tag, string v_ind1, string v_ind2, string v_value, out string v_errMsg)
        {
            var v_isValidVal = true;
            var v_blank = c_MarcSymbols.c_Human.v_Blank.ToString();

            v_errMsg = "";
            if (v_value.m_IsEmpty()) return false;

            var v_sfIds = Regex.Matches(v_value, "ǂ(.)")?.Cast<Match>()?.Select(v_x => v_x.Groups[1].Value)?.ToArray();

            if (v_sfIds.Length == 0) v_isValidVal = false;    // must contain at least one subfield.

            if (!v_value.StartsWith("ǂ")) v_isValidVal = false;   // must start with delimiter.
            //if (!v_value.StartsWith("ǂa ") && v_tag != "264") v_isValidVal = false;   // must contain subfield a (except 264).

            if (Regex.Match(v_value, @"ǂ\s").Success) v_isValidVal = false; // delimiter must not be followed by a space.
            if (Regex.Match(v_value, @"ǂ.\S").Success) v_isValidVal = false; // second char after delimiter must be a space.
            if (v_value.Length < 2 || Regex.Match(v_value.Substring(1), @"\Sǂ").Success) v_isValidVal = false; // delimiter must follow by a space.

            if (v_tag == "010" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "z", "8" })) v_isValidVal = false;
            else if (v_tag == "013" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "015" && !v_sfIds.m_ContainsOnly(new[] { "a", "q", "z", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "016" && !v_sfIds.m_ContainsOnly(new[] { "a", "z", "2", "8" })) v_isValidVal = false;
            else if (v_tag == "017" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "d", "i", "z", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "018" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "020" && !v_sfIds.m_ContainsOnly(new[] { "a", "c", "q", "z", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "022" && !v_sfIds.m_ContainsOnly(new[] { "a", "l", "m", "y", "z", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "024" && !v_sfIds.m_ContainsOnly(new[] { "a", "c", "d", "q", "z", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "025" && !v_sfIds.m_ContainsOnly(new[] { "a", "8" })) v_isValidVal = false;
            else if (v_tag == "026" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "2", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "027" && !v_sfIds.m_ContainsOnly(new[] { "a", "q", "z", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "028" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "q", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "030" && !v_sfIds.m_ContainsOnly(new[] { "a", "z", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "031" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "g", "m", "n", "o", "p", "q", "r", "s", "t", "u", "y", "z", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "032" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "033" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "p", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "034" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "m", "n", "p", "r", "s", "t", "x", "y", "z", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "035" && !v_sfIds.m_ContainsOnly(new[] { "a", "z", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "036" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "037" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "f", "g", "n", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "040" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "041" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "d", "e", "f", "g", "h", "i", "j", "k", "m", "n", "p", "q", "r", "t", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "042" && !v_sfIds.m_ContainsOnly(new[] { "a" })) v_isValidVal = false;
            else if (v_tag == "043" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "044" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "045" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "046" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "j", "k", "l", "m", "n", "o", "p", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "047" && !v_sfIds.m_ContainsOnly(new[] { "a", "2", "8" })) v_isValidVal = false;
            else if (v_tag == "048" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "2", "8" })) v_isValidVal = false;
            else if (v_tag == "050" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "051" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "8" })) v_isValidVal = false;
            else if (v_tag == "052" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "d", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "055" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "060" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "8" })) v_isValidVal = false;
            else if (v_tag == "061" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "8" })) v_isValidVal = false;
            else if (v_tag == "066" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c" })) v_isValidVal = false;
            else if (v_tag == "070" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "8" })) v_isValidVal = false;
            else if (v_tag == "071" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "8" })) v_isValidVal = false;
            else if (v_tag == "072" && !v_sfIds.m_ContainsOnly(new[] { "a", "x", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "074" && !v_sfIds.m_ContainsOnly(new[] { "a", "z", "8" })) v_isValidVal = false;
            else if (v_tag == "080" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "x", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "082" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "m", "q", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "083" && !v_sfIds.m_ContainsOnly(new[] { "a", "c", "m", "q", "y", "z", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "084" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "q", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "085" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "f", "r", "s", "t", "u", "v", "w", "y", "z", "0", "1", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "086" && !v_sfIds.m_ContainsOnly(new[] { "a", "z", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "088" && !v_sfIds.m_ContainsOnly(new[] { "a", "z", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "100" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "j", "k", "l", "n", "p", "q", "t", "u", "0", "1", "2", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "110" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "k", "l", "n", "p", "t", "u", "0", "1", "2", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "111" && !v_sfIds.m_ContainsOnly(new[] { "a", "c", "d", "e", "f", "g", "j", "k", "l", "n", "p", "q", "t", "u", "0", "1", "2", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "130" && !v_sfIds.m_ContainsOnly(new[] { "a", "d", "f", "g", "h", "k", "l", "m", "n", "o", "p", "r", "s", "t", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "210" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "222" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "240" && !v_sfIds.m_ContainsOnly(new[] { "a", "d", "f", "g", "h", "k", "l", "m", "n", "o", "p", "r", "s", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "242" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "h", "n", "p", "y", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "243" && !v_sfIds.m_ContainsOnly(new[] { "a", "d", "f", "g", "h", "k", "l", "m", "n", "o", "p", "r", "s", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "245" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "f", "g", "h", "k", "n", "p", "s", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "246" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "f", "g", "h", "i", "n", "p", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "247" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "f", "g", "h", "n", "p", "x", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "250" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "251" && !v_sfIds.m_ContainsOnly(new[] { "a", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "254" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "255" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "256" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "257" && !v_sfIds.m_ContainsOnly(new[] { "a", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "258" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "260" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "e", "f", "g", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "263" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "264" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "270" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "p", "q", "r", "z", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "300" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "e", "f", "g", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "306" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "307" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "310" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "321" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "336" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "337" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "338" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "340" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "m", "n", "o", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "341" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "342" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "343" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "344" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "345" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "346" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "347" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "348" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "351" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "352" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "i", "q", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "355" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "j", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "357" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "g", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "362" && !v_sfIds.m_ContainsOnly(new[] { "a", "z", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "363" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "u", "v", "x", "z", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "365" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "m", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "366" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "j", "k", "m", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "370" && !v_sfIds.m_ContainsOnly(new[] { "c", "f", "g", "i", "s", "t", "u", "v", "0", "1", "2", "3", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "377" && !v_sfIds.m_ContainsOnly(new[] { "a", "l", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "380" && !v_sfIds.m_ContainsOnly(new[] { "a", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "381" && !v_sfIds.m_ContainsOnly(new[] { "a", "u", "v", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "382" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "d", "e", "n", "p", "r", "s", "t", "v", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "383" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "384" && !v_sfIds.m_ContainsOnly(new[] { "a", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "385" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "m", "n", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "386" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "i", "m", "n", "0", "1", "2", "3", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "388" && !v_sfIds.m_ContainsOnly(new[] { "a", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "490" && !v_sfIds.m_ContainsOnly(new[] { "a", "l", "v", "x", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "500" && !v_sfIds.m_ContainsOnly(new[] { "a", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "501" && !v_sfIds.m_ContainsOnly(new[] { "a", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "502" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "o", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "504" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "505" && !v_sfIds.m_ContainsOnly(new[] { "a", "g", "r", "t", "u", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "506" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "q", "u", "2", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "507" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "508" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "510" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "u", "x", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "511" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "513" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "514" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "m", "u", "z", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "515" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "516" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "518" && !v_sfIds.m_ContainsOnly(new[] { "a", "d", "o", "p", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "520" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "u", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "521" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "522" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "524" && !v_sfIds.m_ContainsOnly(new[] { "a", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "525" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "526" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "i", "x", "z", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "530" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "u", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "532" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "533" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "m", "n", "3", "5", "7", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "534" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "e", "f", "k", "l", "m", "n", "o", "p", "t", "x", "z", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "535" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "536" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "538" && !v_sfIds.m_ContainsOnly(new[] { "a", "i", "u", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "540" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "f", "g", "q", "u", "2", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "541" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "h", "n", "o", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "542" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "u", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "544" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "n", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "545" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "u", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "546" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "547" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "550" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "552" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "u", "z", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "555" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "u", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "556" && !v_sfIds.m_ContainsOnly(new[] { "a", "z", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "561" && !v_sfIds.m_ContainsOnly(new[] { "a", "u", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "562" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "563" && !v_sfIds.m_ContainsOnly(new[] { "a", "u", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "565" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "567" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "580" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "581" && !v_sfIds.m_ContainsOnly(new[] { "a", "z", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "583" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "h", "i", "j", "k", "l", "n", "o", "u", "x", "z", "2", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "584" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "585" && !v_sfIds.m_ContainsOnly(new[] { "a", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "586" && !v_sfIds.m_ContainsOnly(new[] { "a", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "588" && !v_sfIds.m_ContainsOnly(new[] { "a", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "600" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "x", "y", "z", "0", "1", "2", "3", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "610" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "v", "x", "y", "z", "0", "1", "2", "3", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "611" && !v_sfIds.m_ContainsOnly(new[] { "a", "c", "d", "e", "f", "g", "h", "j", "k", "l", "n", "p", "q", "s", "t", "u", "v", "x", "y", "z", "0", "1", "2", "3", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "630" && !v_sfIds.m_ContainsOnly(new[] { "a", "d", "e", "f", "g", "h", "k", "l", "m", "n", "o", "p", "r", "s", "t", "v", "x", "y", "z", "0", "1", "2", "3", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "647" && !v_sfIds.m_ContainsOnly(new[] { "a", "c", "d", "g", "v", "x", "y", "z", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "648" && !v_sfIds.m_ContainsOnly(new[] { "a", "v", "x", "y", "z", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "650" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "g", "v", "x", "y", "z", "0", "1", "2", "3", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "651" && !v_sfIds.m_ContainsOnly(new[] { "a", "e", "g", "v", "x", "y", "z", "0", "1", "2", "3", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "653" && !v_sfIds.m_ContainsOnly(new[] { "a", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "654" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "e", "v", "y", "z", "0", "1", "2", "3", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "655" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "v", "x", "y", "z", "0", "1", "2", "3", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "656" && !v_sfIds.m_ContainsOnly(new[] { "a", "k", "v", "x", "y", "z", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "657" && !v_sfIds.m_ContainsOnly(new[] { "a", "v", "x", "y", "z", "0", "1", "2", "3", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "658" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "662" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "0", "1", "2", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "700" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "x", "0", "1", "2", "3", "4", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "710" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "x", "0", "1", "2", "3", "4", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "711" && !v_sfIds.m_ContainsOnly(new[] { "a", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "n", "p", "q", "s", "t", "u", "x", "0", "1", "2", "3", "4", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "720" && !v_sfIds.m_ContainsOnly(new[] { "a", "e", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "730" && !v_sfIds.m_ContainsOnly(new[] { "a", "d", "f", "g", "h", "i", "k", "l", "m", "n", "o", "p", "r", "s", "t", "x", "0", "1", "2", "3", "4", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "740" && !v_sfIds.m_ContainsOnly(new[] { "a", "h", "n", "p", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "751" && !v_sfIds.m_ContainsOnly(new[] { "a", "e", "g", "0", "1", "2", "3", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "752" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "0", "1", "2", "4", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "753" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "754" && !v_sfIds.m_ContainsOnly(new[] { "a", "c", "d", "x", "z", "0", "1", "2", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "758" && !v_sfIds.m_ContainsOnly(new[] { "a", "i", "0", "1", "2", "3", "4", "5", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "760" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "m", "n", "o", "s", "t", "w", "x", "y", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "762" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "m", "n", "o", "s", "t", "w", "x", "y", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "765" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "767" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "770" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "772" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "773" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "d", "g", "h", "i", "k", "m", "n", "o", "p", "q", "r", "s", "t", "u", "w", "x", "y", "z", "3", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "774" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "775" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "776" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "777" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "780" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "785" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "786" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "j", "k", "m", "n", "o", "p", "r", "s", "t", "u", "v", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "787" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "g", "h", "i", "k", "m", "n", "o", "r", "s", "t", "u", "w", "x", "y", "z", "4", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "800" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "0", "1", "2", "3", "4", "5", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "810" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "e", "f", "g", "h", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "v", "w", "x", "0", "1", "2", "3", "4", "5", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "811" && !v_sfIds.m_ContainsOnly(new[] { "a", "c", "d", "e", "f", "g", "h", "j", "k", "l", "n", "p", "q", "s", "t", "u", "v", "w", "x", "0", "1", "2", "3", "4", "5", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "830" && !v_sfIds.m_ContainsOnly(new[] { "a", "d", "f", "g", "h", "k", "l", "m", "n", "o", "p", "r", "s", "t", "v", "w", "x", "0", "1", "2", "3", "5", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "856" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "f", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "2", "3", "6", "7", "8" })) v_isValidVal = false;
            else if (v_tag == "880" && !v_sfIds.m_ContainsOnly(new[] { "6" })) v_isValidVal = false;
            else if (v_tag == "882" && !v_sfIds.m_ContainsOnly(new[] { "a", "i", "w", "6", "8" })) v_isValidVal = false;
            else if (v_tag == "883" && !v_sfIds.m_ContainsOnly(new[] { "a", "c", "d", "q", "x", "u", "w", "0", "1", "8" })) v_isValidVal = false;
            else if (v_tag == "884" && !v_sfIds.m_ContainsOnly(new[] { "a", "g", "k", "q", "u" })) v_isValidVal = false;
            else if (v_tag == "885" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "c", "d", "w", "x", "z", "0", "1", "2", "5" })) v_isValidVal = false;
            else if (v_tag == "886" && !v_sfIds.m_ContainsOnly(new[] { "a", "b", "2" })) v_isValidVal = false;
            else if (v_tag == "887" && !v_sfIds.m_ContainsOnly(new[] { "a", "2" })) v_isValidVal = false;

            v_errMsg = v_isValidVal ? "" : $"Invalid subfields (see field help page)";
            return v_isValidVal;
        }

        #endregion

        #region item classes

        public class c_PropertyItem
        {
            public string v_Key { get; set; }

            public string v_Value { get; set; }

            public c_PropertyItem(string[] v_val)
            {
                v_Key = v_val[0];
                v_Value = v_val[1];
            }
        }

        public class c_LeaderItem
        {
            [DisplayName("Field")]
            public string v_Field { get; set; }

            [DisplayName("Name")]
            public string v_Name { get; set; }

            [DisplayName("Mnemonic")]
            public string v_Mnemonic { get; set; }

            [DisplayName("Value")]
            public string v_Value { get; set; }

            public c_LeaderItem(string[] v_val)
            {
                v_Field = v_val[0];
                v_Name = v_val[1];
                v_Mnemonic = v_val[2];
                v_Value = v_val[3].Replace(' ', c_MarcSymbols.c_Human.v_Blank);
            }

            public c_LeaderItem m_DeepClone()
            {
                return new c_LeaderItem(new[] { this.v_Field, this.v_Name, this.v_Mnemonic, this.v_Value });
            }
        }

        public class c_ControlItem
        {
            [DisplayName("Field")]
            public string v_Field { get; set; }

            [DisplayName("Name")]
            public string v_Name { get; set; }

            [DisplayName("Mnemonic")]
            public string v_Mnemonic { get; set; }

            [DisplayName("Value")]
            public string v_Value { get; set; }

            public c_ControlItem(string[] v_val)
            {
                v_Field = v_val[0];
                v_Name = v_val[1];
                v_Mnemonic = v_val[2];
                v_Value = v_val[3].Replace(' ', c_MarcSymbols.c_Human.v_Blank);
            }

            public c_ControlItem m_DeepClone()
            {
                return new c_ControlItem(new[] { this.v_Field, this.v_Name, this.v_Mnemonic, this.v_Value });
            }
        }

        public class c_VardataItem
        {
            [DisplayName("Field")]
            public string v_Field { get; set; }

            [DisplayName("I\u2081")]
            public string v_Ind1 { get; set; }

            [DisplayName("I\u2082")]
            public string v_Ind2 { get; set; }

            [DisplayName("Value")]
            public string v_Value { get; set; }

            public c_VardataItem(string[] v_val)
            {
                v_Field = v_val[0];
                v_Ind1 = v_val[1].Replace(' ', c_MarcSymbols.c_Human.v_Blank);
                v_Ind2 = v_val[2].Replace(' ', c_MarcSymbols.c_Human.v_Blank);
                v_Value = v_val[3];
            }

            public c_VardataItem m_DeepClone()
            {
                return new c_VardataItem(new[] { this.v_Field, this.v_Ind1, this.v_Ind2, this.v_Value });
            }
        }

        #endregion

        #region fixed field acronyms

        private KeyValuePair<string, string>[] FixedFieldsAcronyms = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("AccM", "Accompanying Matter"),
            new KeyValuePair<string, string>("Alph", "Original Alphabet or Script of Title"),
            new KeyValuePair<string, string>("Audn", "Target Audience"),
            new KeyValuePair<string, string>("Biog", "Biography"),
            new KeyValuePair<string, string>("BLvl", "Bibliographic Level"),
            new KeyValuePair<string, string>("Comp", "Form of Composition"),
            new KeyValuePair<string, string>("Conf", "Conference Publication"),
            new KeyValuePair<string, string>("Cont", "Nature of Contents"),
            new KeyValuePair<string, string>("CrTp", "Type of Cartographic Material"),
            new KeyValuePair<string, string>("Ctrl", "Type of Control"),
            new KeyValuePair<string, string>("Ctry", "Country of Publication, etc."),
            new KeyValuePair<string, string>("Dates", "Date 1 and Date 2"),
            new KeyValuePair<string, string>("Desc", "Descriptive Cataloging Form"),
            new KeyValuePair<string, string>("DtSt", "Type of Date/Publication Status"),
            new KeyValuePair<string, string>("ELvl", "Encoding Level"),
            new KeyValuePair<string, string>("Entered", "Date Entered"),
            new KeyValuePair<string, string>("EntW", "Nature of Entire Work"),
            new KeyValuePair<string, string>("Fest", "Festschrift"),
            new KeyValuePair<string, string>("File", "Type of Computer File"),
            new KeyValuePair<string, string>("FMus", "Format of Music"),
            new KeyValuePair<string, string>("Form", "Form of Item"),
            new KeyValuePair<string, string>("Freq", "Frequency"),
            new KeyValuePair<string, string>("GPub", "Government publication"),
            new KeyValuePair<string, string>("Ills", "Illustrations"),
            new KeyValuePair<string, string>("Indx", "Index"),
            new KeyValuePair<string, string>("Lang", "Language Code"),
            new KeyValuePair<string, string>("LitF", "Literary Form"),
            new KeyValuePair<string, string>("LTxt", "Literary Text for Sound Recordings"),
            new KeyValuePair<string, string>("MRec", "Modified Record"),
            new KeyValuePair<string, string>("OCLC", "OCLC Control Number"),
            new KeyValuePair<string, string>("Orig", "Form of Original Item"),
            new KeyValuePair<string, string>("Part", "Music Parts"),
            new KeyValuePair<string, string>("Proj", "Projection"),
            new KeyValuePair<string, string>("Rec stat", "Record Status"),
            new KeyValuePair<string, string>("Regl", "Regularity"),
            new KeyValuePair<string, string>("Relf", "Relief"),
            new KeyValuePair<string, string>("Replaced", "Date of Last Replace"),
            new KeyValuePair<string, string>("SpFm", "Special Format Characteristics"),
            new KeyValuePair<string, string>("Srce", "Cataloging Source"),
            new KeyValuePair<string, string>("SrTp", "Type of Continuing Resource"),
            new KeyValuePair<string, string>("S/L", "Entry Convention"),
            new KeyValuePair<string, string>("Tech", "Technique"),
            new KeyValuePair<string, string>("Time", "Running Time"),
            new KeyValuePair<string, string>("TMat", "Type of Visual Material"),
            new KeyValuePair<string, string>("TrAr", "Transposition and Arrangement"),
            new KeyValuePair<string, string>("Type", "Type of Record")
        };

        #endregion
    }
}