using System.Drawing;

namespace marc_common
{
    partial class c_FrmSymbol
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(c_FrmSymbol));
            this.v_pnlSymbol = new System.Windows.Forms.Panel();
            this.v_btnOk = new System.Windows.Forms.Button();
            this.v_dgv = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.v_cmbType = new System.Windows.Forms.ComboBox();
            this.v_pnlSymbol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.v_dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // v_pnlSymbol
            // 
            this.v_pnlSymbol.BackColor = System.Drawing.SystemColors.Window;
            this.v_pnlSymbol.Controls.Add(this.v_btnOk);
            this.v_pnlSymbol.Controls.Add(this.v_dgv);
            this.v_pnlSymbol.Controls.Add(this.v_cmbType);
            this.v_pnlSymbol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.v_pnlSymbol.Location = new System.Drawing.Point(0, 0);
            this.v_pnlSymbol.Name = "v_pnlSymbol";
            this.v_pnlSymbol.Size = new System.Drawing.Size(444, 210);
            this.v_pnlSymbol.TabIndex = 0;
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
            this.v_dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6});
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
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Column4";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Column5";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Column6";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // v_cmbType
            // 
            this.v_cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.v_cmbType.Location = new System.Drawing.Point(7, 9);
            this.v_cmbType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.v_cmbType.Name = "v_cmbType";
            this.v_cmbType.Size = new System.Drawing.Size(224, 28);
            this.v_cmbType.TabIndex = 4;
            // 
            // c_FrmSymbol
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 210);
            this.Controls.Add(this.v_pnlSymbol);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(286, 168);
            this.Name = "c_FrmSymbol";
            this.Text = "PredictiveBIB Symbols";
            this.v_pnlSymbol.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.v_dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel v_pnlSymbol;
        private System.Windows.Forms.DataGridView v_dgv;
        private System.Windows.Forms.ComboBox v_cmbType;
        private System.Windows.Forms.Button v_btnOk;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
    }
}