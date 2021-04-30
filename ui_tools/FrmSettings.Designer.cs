namespace modmarc_ui
{
    partial class c_FrmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(c_FrmSettings));
            this.v_btnClose = new System.Windows.Forms.Button();
            this.txtOrganizationCode = new System.Windows.Forms.TextBox();
            this.lnkMarcOrganizationCode = new System.Windows.Forms.LinkLabel();
            this.buttonTip1 = new marc_common.ButtonTip();
            this.SuspendLayout();
            // 
            // v_btnClose
            // 
            this.v_btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.v_btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.v_btnClose.Location = new System.Drawing.Point(348, 119);
            this.v_btnClose.Name = "v_btnClose";
            this.v_btnClose.Size = new System.Drawing.Size(86, 33);
            this.v_btnClose.TabIndex = 3;
            this.v_btnClose.Text = "Close";
            this.v_btnClose.UseVisualStyleBackColor = true;
            // 
            // txtOrganizationCode
            // 
            this.txtOrganizationCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtOrganizationCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOrganizationCode.Location = new System.Drawing.Point(24, 50);
            this.txtOrganizationCode.Name = "txtOrganizationCode";
            this.txtOrganizationCode.Size = new System.Drawing.Size(216, 26);
            this.txtOrganizationCode.TabIndex = 188;
            // 
            // lnkMarcOrganizationCode
            // 
            this.lnkMarcOrganizationCode.AutoSize = true;
            this.lnkMarcOrganizationCode.Location = new System.Drawing.Point(20, 22);
            this.lnkMarcOrganizationCode.Name = "lnkMarcOrganizationCode";
            this.lnkMarcOrganizationCode.Size = new System.Drawing.Size(224, 20);
            this.lnkMarcOrganizationCode.TabIndex = 191;
            this.lnkMarcOrganizationCode.TabStop = true;
            this.lnkMarcOrganizationCode.Text = "Your MARC organization code";
            // 
            // buttonTip1
            // 
            this.buttonTip1.BackColor = System.Drawing.Color.Transparent;
            this.buttonTip1.Location = new System.Drawing.Point(245, 51);
            this.buttonTip1.Name = "buttonTip1";
            this.buttonTip1.Size = new System.Drawing.Size(25, 25);
            this.buttonTip1.TabIndex = 193;
            this.buttonTip1.TipText = "Identify your organization as the author of any edits.";
            this.buttonTip1.TipVisible = false;
            this.buttonTip1.TipWidth = 0;
            // 
            // c_FrmSettings
            // 
            this.AcceptButton = this.v_btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 161);
            this.Controls.Add(this.buttonTip1);
            this.Controls.Add(this.lnkMarcOrganizationCode);
            this.Controls.Add(this.txtOrganizationCode);
            this.Controls.Add(this.v_btnClose);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "c_FrmSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button v_btnClose;
        private System.Windows.Forms.TextBox txtOrganizationCode;
        private System.Windows.Forms.LinkLabel lnkMarcOrganizationCode;
        private marc_common.ButtonTip buttonTip1;
    }
}

