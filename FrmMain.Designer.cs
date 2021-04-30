namespace modmarc_ui
{
    partial class c_FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                v_frmDiacritic.Dispose();
                v_frmSymbol.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(c_FrmMain));
            this.v_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.v_toolStrip = new System.Windows.Forms.ToolStrip();
            this.v_btnHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.v_btnToggleHighlightFixedFields = new System.Windows.Forms.ToolStripMenuItem();
            this.v_btnToggleHighlightMediaFields = new System.Windows.Forms.ToolStripMenuItem();
            this.v_btnAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.v_btnSettings = new System.Windows.Forms.ToolStripButton();
            this.v_toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.v_btnSave = new System.Windows.Forms.ToolStripButton();
            this.v_toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.v_btnOpen = new System.Windows.Forms.ToolStripButton();
            this.v_btnLeader = new System.Windows.Forms.ToolStripButton();
            this.v_toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.v_btnControl = new System.Windows.Forms.ToolStripButton();
            this.v_toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.v_btnVardata = new System.Windows.Forms.ToolStripButton();
            this.v_pnlStatus = new System.Windows.Forms.Panel();
            this.v_pbxSpinner = new System.Windows.Forms.PictureBox();
            this.v_lblStatus = new System.Windows.Forms.Label();
            this.v_pnlMain = new System.Windows.Forms.Panel();
            this.v_lblMain = new System.Windows.Forms.Label();
            this.v_pbxMain = new System.Windows.Forms.PictureBox();
            this.v_dgv = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1.SuspendLayout();
            this.v_toolStrip.SuspendLayout();
            this.v_pnlStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.v_pbxSpinner)).BeginInit();
            this.v_pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.v_pbxMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.v_dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // v_toolTip
            // 
            this.v_toolTip.IsBalloon = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.v_toolStrip, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.v_pnlStatus, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.v_pnlMain, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(834, 462);
            this.tableLayoutPanel1.TabIndex = 101;
            // 
            // v_toolStrip
            // 
            this.v_toolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.v_toolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.v_toolStrip.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.v_toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.v_toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.v_btnHelp,
            this.toolStripSeparator1,
            this.v_btnSettings,
            this.v_toolStripSeparator5,
            this.v_btnSave,
            this.v_toolStripSeparator4,
            this.v_btnOpen,
            this.v_btnLeader,
            this.v_toolStripSeparator1,
            this.v_btnControl,
            this.v_toolStripSeparator2,
            this.v_btnVardata});
            this.v_toolStrip.Location = new System.Drawing.Point(0, 0);
            this.v_toolStrip.Name = "v_toolStrip";
            this.v_toolStrip.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.v_toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.v_toolStrip.Size = new System.Drawing.Size(834, 40);
            this.v_toolStrip.TabIndex = 92;
            this.v_toolStrip.Text = "toolStrip1";
            // 
            // v_btnHelp
            // 
            this.v_btnHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.v_btnHelp.AutoToolTip = false;
            this.v_btnHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.v_btnHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.v_btnToggleHighlightFixedFields,
            this.v_btnToggleHighlightMediaFields,
            this.v_btnAbout});
            this.v_btnHelp.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.v_btnHelp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.v_btnHelp.Name = "v_btnHelp";
            this.v_btnHelp.ShowDropDownArrow = false;
            this.v_btnHelp.Size = new System.Drawing.Size(48, 31);
            this.v_btnHelp.Text = "Help";
            this.v_btnHelp.ToolTipText = "Help resources";
            // 
            // v_btnToggleHighlightFixedFields
            // 
            this.v_btnToggleHighlightFixedFields.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.v_btnToggleHighlightFixedFields.Image = global::modmarc_ui.Properties.Resources.highlight;
            this.v_btnToggleHighlightFixedFields.Name = "v_btnToggleHighlightFixedFields";
            this.v_btnToggleHighlightFixedFields.Size = new System.Drawing.Size(293, 22);
            this.v_btnToggleHighlightFixedFields.Text = "Toggle fixed-field highlight";
            // 
            // v_btnToggleHighlightMediaFields
            // 
            this.v_btnToggleHighlightMediaFields.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.v_btnToggleHighlightMediaFields.Image = global::modmarc_ui.Properties.Resources.highlight_green;
            this.v_btnToggleHighlightMediaFields.Name = "v_btnToggleHighlightMediaFields";
            this.v_btnToggleHighlightMediaFields.Size = new System.Drawing.Size(293, 22);
            this.v_btnToggleHighlightMediaFields.Text = "Toggle material-type highlight";
            // 
            // v_btnAbout
            // 
            this.v_btnAbout.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.v_btnAbout.Image = global::modmarc_ui.Properties.Resources.help;
            this.v_btnAbout.Name = "v_btnAbout";
            this.v_btnAbout.Size = new System.Drawing.Size(293, 22);
            this.v_btnAbout.Text = "About";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 34);
            // 
            // v_btnSettings
            // 
            this.v_btnSettings.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.v_btnSettings.AutoToolTip = false;
            this.v_btnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.v_btnSettings.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.v_btnSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.v_btnSettings.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.v_btnSettings.Name = "v_btnSettings";
            this.v_btnSettings.Size = new System.Drawing.Size(80, 31);
            this.v_btnSettings.Text = "Settings";
            this.v_btnSettings.ToolTipText = "Save to mrc file";
            // 
            // v_toolStripSeparator5
            // 
            this.v_toolStripSeparator5.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.v_toolStripSeparator5.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.v_toolStripSeparator5.Name = "v_toolStripSeparator5";
            this.v_toolStripSeparator5.Size = new System.Drawing.Size(6, 34);
            // 
            // v_btnSave
            // 
            this.v_btnSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.v_btnSave.AutoToolTip = false;
            this.v_btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.v_btnSave.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.v_btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.v_btnSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.v_btnSave.Name = "v_btnSave";
            this.v_btnSave.Size = new System.Drawing.Size(53, 31);
            this.v_btnSave.Text = "Save";
            this.v_btnSave.ToolTipText = "Save to mrc file";
            // 
            // v_toolStripSeparator4
            // 
            this.v_toolStripSeparator4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.v_toolStripSeparator4.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.v_toolStripSeparator4.Name = "v_toolStripSeparator4";
            this.v_toolStripSeparator4.Size = new System.Drawing.Size(6, 34);
            // 
            // v_btnOpen
            // 
            this.v_btnOpen.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.v_btnOpen.AutoToolTip = false;
            this.v_btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.v_btnOpen.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.v_btnOpen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.v_btnOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.v_btnOpen.Name = "v_btnOpen";
            this.v_btnOpen.Size = new System.Drawing.Size(55, 31);
            this.v_btnOpen.Text = "Open";
            this.v_btnOpen.ToolTipText = "Open mrc file";
            // 
            // v_btnLeader
            // 
            this.v_btnLeader.AutoToolTip = false;
            this.v_btnLeader.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.v_btnLeader.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.v_btnLeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.v_btnLeader.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.v_btnLeader.Name = "v_btnLeader";
            this.v_btnLeader.Size = new System.Drawing.Size(69, 30);
            this.v_btnLeader.Text = "Leader";
            // 
            // v_toolStripSeparator1
            // 
            this.v_toolStripSeparator1.Name = "v_toolStripSeparator1";
            this.v_toolStripSeparator1.Size = new System.Drawing.Size(6, 34);
            // 
            // v_btnControl
            // 
            this.v_btnControl.AutoToolTip = false;
            this.v_btnControl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.v_btnControl.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.v_btnControl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.v_btnControl.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.v_btnControl.Name = "v_btnControl";
            this.v_btnControl.Size = new System.Drawing.Size(123, 30);
            this.v_btnControl.Text = "Control Fields";
            // 
            // v_toolStripSeparator2
            // 
            this.v_toolStripSeparator2.Name = "v_toolStripSeparator2";
            this.v_toolStripSeparator2.Size = new System.Drawing.Size(6, 34);
            // 
            // v_btnVardata
            // 
            this.v_btnVardata.AutoToolTip = false;
            this.v_btnVardata.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.v_btnVardata.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.v_btnVardata.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.v_btnVardata.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.v_btnVardata.Name = "v_btnVardata";
            this.v_btnVardata.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.v_btnVardata.Size = new System.Drawing.Size(103, 30);
            this.v_btnVardata.Text = "Data Fields";
            // 
            // v_pnlStatus
            // 
            this.v_pnlStatus.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.v_pnlStatus.Controls.Add(this.v_pbxSpinner);
            this.v_pnlStatus.Controls.Add(this.v_lblStatus);
            this.v_pnlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.v_pnlStatus.Location = new System.Drawing.Point(3, 425);
            this.v_pnlStatus.Name = "v_pnlStatus";
            this.v_pnlStatus.Size = new System.Drawing.Size(828, 34);
            this.v_pnlStatus.TabIndex = 93;
            // 
            // v_pbxSpinner
            // 
            this.v_pbxSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.v_pbxSpinner.Image = global::modmarc_ui.Properties.Resources.spinner_purple;
            this.v_pbxSpinner.Location = new System.Drawing.Point(794, 2);
            this.v_pbxSpinner.Name = "v_pbxSpinner";
            this.v_pbxSpinner.Size = new System.Drawing.Size(31, 31);
            this.v_pbxSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.v_pbxSpinner.TabIndex = 94;
            this.v_pbxSpinner.TabStop = false;
            // 
            // v_lblStatus
            // 
            this.v_lblStatus.AutoSize = true;
            this.v_lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.v_lblStatus.Font = new System.Drawing.Font("Verdana", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.v_lblStatus.ForeColor = System.Drawing.Color.White;
            this.v_lblStatus.Location = new System.Drawing.Point(15, 8);
            this.v_lblStatus.Name = "v_lblStatus";
            this.v_lblStatus.Size = new System.Drawing.Size(158, 18);
            this.v_lblStatus.TabIndex = 0;
            this.v_lblStatus.Text = "Checking license...";
            // 
            // v_pnlMain
            // 
            this.v_pnlMain.Controls.Add(this.v_lblMain);
            this.v_pnlMain.Controls.Add(this.v_pbxMain);
            this.v_pnlMain.Controls.Add(this.v_dgv);
            this.v_pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.v_pnlMain.Location = new System.Drawing.Point(3, 43);
            this.v_pnlMain.Name = "v_pnlMain";
            this.v_pnlMain.Size = new System.Drawing.Size(828, 376);
            this.v_pnlMain.TabIndex = 1;
            // 
            // v_lblMain
            // 
            this.v_lblMain.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.v_lblMain.AutoSize = true;
            this.v_lblMain.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.v_lblMain.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.v_lblMain.Location = new System.Drawing.Point(241, 172);
            this.v_lblMain.Name = "v_lblMain";
            this.v_lblMain.Size = new System.Drawing.Size(347, 29);
            this.v_lblMain.TabIndex = 102;
            this.v_lblMain.Text = "drag && drop an mrc file anywhere";
            // 
            // v_pbxMain
            // 
            this.v_pbxMain.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.v_pbxMain.Image = global::modmarc_ui.Properties.Resources.landing;
            this.v_pbxMain.Location = new System.Drawing.Point(220, 154);
            this.v_pbxMain.Name = "v_pbxMain";
            this.v_pbxMain.Size = new System.Drawing.Size(389, 68);
            this.v_pbxMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.v_pbxMain.TabIndex = 103;
            this.v_pbxMain.TabStop = false;
            // 
            // v_dgv
            // 
            this.v_dgv.AllowUserToAddRows = false;
            this.v_dgv.AllowUserToDeleteRows = false;
            this.v_dgv.AllowUserToResizeRows = false;
            this.v_dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.v_dgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.v_dgv.BackgroundColor = System.Drawing.SystemColors.Window;
            this.v_dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.v_dgv.ColumnHeadersHeight = 35;
            this.v_dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.v_dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.v_dgv.EnableHeadersVisualStyles = false;
            this.v_dgv.Location = new System.Drawing.Point(0, 0);
            this.v_dgv.Margin = new System.Windows.Forms.Padding(0);
            this.v_dgv.Name = "v_dgv";
            this.v_dgv.ReadOnly = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.v_dgv.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.v_dgv.RowTemplate.Height = 30;
            this.v_dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.v_dgv.Size = new System.Drawing.Size(828, 376);
            this.v_dgv.TabIndex = 101;
            this.v_dgv.Visible = false;
            // 
            // c_FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(834, 462);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "c_FrmMain";
            this.Text = "ModMARC";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.v_toolStrip.ResumeLayout(false);
            this.v_toolStrip.PerformLayout();
            this.v_pnlStatus.ResumeLayout(false);
            this.v_pnlStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.v_pbxSpinner)).EndInit();
            this.v_pnlMain.ResumeLayout(false);
            this.v_pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.v_pbxMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.v_dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip v_toolTip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel v_pnlMain;
        private System.Windows.Forms.DataGridView v_dgv;
        private System.Windows.Forms.Label v_lblMain;
        private System.Windows.Forms.PictureBox v_pbxMain;
        private System.Windows.Forms.ToolStrip v_toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton v_btnHelp;
        private System.Windows.Forms.ToolStripMenuItem v_btnAbout;
        private System.Windows.Forms.ToolStripMenuItem v_btnToggleHighlightFixedFields;
        private System.Windows.Forms.ToolStripMenuItem v_btnToggleHighlightMediaFields;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton v_btnSettings;
        private System.Windows.Forms.ToolStripSeparator v_toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton v_btnSave;
        private System.Windows.Forms.ToolStripSeparator v_toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton v_btnOpen;
        private System.Windows.Forms.ToolStripButton v_btnLeader;
        private System.Windows.Forms.ToolStripSeparator v_toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton v_btnControl;
        private System.Windows.Forms.ToolStripSeparator v_toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton v_btnVardata;
        private System.Windows.Forms.Panel v_pnlStatus;
        private System.Windows.Forms.Label v_lblStatus;
        private System.Windows.Forms.PictureBox v_pbxSpinner;
    }
}

