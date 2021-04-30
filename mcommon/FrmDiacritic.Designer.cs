using System.Drawing;

namespace marc_common
{
    partial class c_FrmDiacritic
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="v_disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool v_disposing)
        {
            if (v_disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(v_disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(c_FrmDiacritic));
            this.v_pnlDiacritic = new System.Windows.Forms.Panel();
            this.v_btnTip = new marc_common.ButtonTip();
            this.v_cmbType = new System.Windows.Forms.ComboBox();
            this.v_btnOk = new System.Windows.Forms.Button();
            this.v_dgv = new System.Windows.Forms.DataGridView();
            this.v_cmbLetter = new System.Windows.Forms.ComboBox();
            this.v_cmbCase = new System.Windows.Forms.ComboBox();
            this.v_pnlDiacritic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.v_dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // v_pnlDiacritic
            // 
            this.v_pnlDiacritic.BackColor = System.Drawing.SystemColors.Window;
            this.v_pnlDiacritic.Controls.Add(this.v_btnTip);
            this.v_pnlDiacritic.Controls.Add(this.v_cmbType);
            this.v_pnlDiacritic.Controls.Add(this.v_btnOk);
            this.v_pnlDiacritic.Controls.Add(this.v_dgv);
            this.v_pnlDiacritic.Controls.Add(this.v_cmbLetter);
            this.v_pnlDiacritic.Controls.Add(this.v_cmbCase);
            this.v_pnlDiacritic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.v_pnlDiacritic.Location = new System.Drawing.Point(0, 0);
            this.v_pnlDiacritic.Name = "v_pnlDiacritic";
            this.v_pnlDiacritic.Size = new System.Drawing.Size(444, 210);
            this.v_pnlDiacritic.TabIndex = 0;
            // 
            // v_btnTip
            // 
            this.v_btnTip.BackColor = System.Drawing.Color.Transparent;
            this.v_btnTip.Location = new System.Drawing.Point(333, 11);
            this.v_btnTip.Name = "v_btnTip";
            this.v_btnTip.Size = new System.Drawing.Size(25, 25);
            this.v_btnTip.TabIndex = 224;
            this.v_btnTip.TipText = "A minimal level record omits: DDC call number, LCSH, notes, and contributors.";
            this.v_btnTip.TipVisible = true;
            this.v_btnTip.TipWidth = 300;
            // 
            // v_cmbType
            // 
            this.v_cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.v_cmbType.Location = new System.Drawing.Point(7, 9);
            this.v_cmbType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.v_cmbType.Name = "v_cmbType";
            this.v_cmbType.Size = new System.Drawing.Size(126, 28);
            this.v_cmbType.TabIndex = 7;
            // 
            // v_btnOk
            // 
            this.v_btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.v_btnOk.Location = new System.Drawing.Point(392, 8);
            this.v_btnOk.Name = "v_btnOk";
            this.v_btnOk.Size = new System.Drawing.Size(45, 30);
            this.v_btnOk.TabIndex = 6;
            this.v_btnOk.Text = "Ok";
            this.v_btnOk.UseVisualStyleBackColor = true;
            // 
            // v_dgv
            // 
            this.v_dgv.AllowUserToAddRows = false;
            this.v_dgv.AllowUserToDeleteRows = false;
            this.v_dgv.AllowUserToResizeColumns = false;
            this.v_dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.v_dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.v_dgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.v_dgv.BackgroundColor = System.Drawing.SystemColors.Control;
            this.v_dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.v_dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.v_dgv.ColumnHeadersVisible = false;
            this.v_dgv.Location = new System.Drawing.Point(0, 49);
            this.v_dgv.MultiSelect = false;
            this.v_dgv.Name = "v_dgv";
            this.v_dgv.ReadOnly = true;
            this.v_dgv.RowHeadersVisible = false;
            this.v_dgv.RowTemplate.Height = 30;
            this.v_dgv.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.v_dgv.Size = new System.Drawing.Size(444, 161);
            this.v_dgv.TabIndex = 5;
            // 
            // v_cmbLetter
            // 
            this.v_cmbLetter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.v_cmbLetter.Enabled = false;
            this.v_cmbLetter.Location = new System.Drawing.Point(140, 9);
            this.v_cmbLetter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.v_cmbLetter.Name = "v_cmbLetter";
            this.v_cmbLetter.Size = new System.Drawing.Size(61, 28);
            this.v_cmbLetter.TabIndex = 4;
            // 
            // v_cmbCase
            // 
            this.v_cmbCase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.v_cmbCase.Enabled = false;
            this.v_cmbCase.Location = new System.Drawing.Point(208, 9);
            this.v_cmbCase.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.v_cmbCase.Name = "v_cmbCase";
            this.v_cmbCase.Size = new System.Drawing.Size(114, 28);
            this.v_cmbCase.TabIndex = 3;
            // 
            // c_FrmDiacritic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 210);
            this.Controls.Add(this.v_pnlDiacritic);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(286, 168);
            this.Name = "c_FrmDiacritic";
            this.Text = "";
            this.v_pnlDiacritic.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.v_dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel v_pnlDiacritic;
        private System.Windows.Forms.DataGridView v_dgv;
        private System.Windows.Forms.ComboBox v_cmbLetter;
        private System.Windows.Forms.ComboBox v_cmbCase;
        private System.Windows.Forms.Button v_btnOk;
        private System.Windows.Forms.ComboBox v_cmbType;
        private marc_common.ButtonTip v_btnTip;
    }
}