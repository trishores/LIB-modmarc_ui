using System.ComponentModel;
using System.Windows.Forms;

namespace marc_common
{
    partial class c_FrmAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.components = new System.ComponentModel.Container();
            this.v_labelProductName = new System.Windows.Forms.Label();
            this.v_labelVersion = new System.Windows.Forms.Label();
            this.v_lblCopyright = new System.Windows.Forms.Label();
            this.v_lblWebsite = new System.Windows.Forms.Label();
            this.v_pbxLogo = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.v_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.v_lblLicense = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.v_pbxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // v_labelProductName
            // 
            this.v_labelProductName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(247)))), ((int)(((byte)(239)))));
            this.v_labelProductName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v_labelProductName.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.v_labelProductName.Location = new System.Drawing.Point(208, 13);
            this.v_labelProductName.Margin = new System.Windows.Forms.Padding(0);
            this.v_labelProductName.Name = "v_labelProductName";
            this.v_labelProductName.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.v_labelProductName.Size = new System.Drawing.Size(184, 28);
            this.v_labelProductName.TabIndex = 27;
            this.v_labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // v_labelVersion
            // 
            this.v_labelVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(247)))), ((int)(((byte)(239)))));
            this.v_labelVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v_labelVersion.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.v_labelVersion.Location = new System.Drawing.Point(460, 13);
            this.v_labelVersion.Margin = new System.Windows.Forms.Padding(0);
            this.v_labelVersion.Name = "v_labelVersion";
            this.v_labelVersion.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.v_labelVersion.Size = new System.Drawing.Size(108, 28);
            this.v_labelVersion.TabIndex = 25;
            this.v_labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // v_lblCopyright
            // 
            this.v_lblCopyright.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(247)))), ((int)(((byte)(239)))));
            this.v_lblCopyright.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v_lblCopyright.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.v_lblCopyright.Location = new System.Drawing.Point(208, 67);
            this.v_lblCopyright.Margin = new System.Windows.Forms.Padding(0);
            this.v_lblCopyright.Name = "v_lblCopyright";
            this.v_lblCopyright.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.v_lblCopyright.Size = new System.Drawing.Size(360, 28);
            this.v_lblCopyright.TabIndex = 28;
            this.v_lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // v_lblWebsite
            // 
            this.v_lblWebsite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(247)))), ((int)(((byte)(239)))));
            this.v_lblWebsite.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v_lblWebsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.v_lblWebsite.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.v_lblWebsite.ForeColor = System.Drawing.Color.Blue;
            this.v_lblWebsite.Location = new System.Drawing.Point(208, 40);
            this.v_lblWebsite.Margin = new System.Windows.Forms.Padding(0);
            this.v_lblWebsite.Name = "v_lblWebsite";
            this.v_lblWebsite.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.v_lblWebsite.Size = new System.Drawing.Size(360, 28);
            this.v_lblWebsite.TabIndex = 29;
            this.v_lblWebsite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // v_pbxLogo
            // 
            this.v_pbxLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.v_pbxLogo.Location = new System.Drawing.Point(13, 13);
            this.v_pbxLogo.Margin = new System.Windows.Forms.Padding(4);
            this.v_pbxLogo.Name = "v_pbxLogo";
            this.v_pbxLogo.Size = new System.Drawing.Size(115, 109);
            this.v_pbxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.v_pbxLogo.TabIndex = 26;
            this.v_pbxLogo.TabStop = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(139, 67);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(70, 28);
            this.label1.TabIndex = 43;
            this.label1.Text = "Notice";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(139, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(70, 28);
            this.label2.TabIndex = 44;
            this.label2.Text = "Website";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(391, 13);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label3.Size = new System.Drawing.Size(70, 28);
            this.label3.TabIndex = 41;
            this.label3.Text = "Version";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(139, 13);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label4.Size = new System.Drawing.Size(70, 28);
            this.label4.TabIndex = 42;
            this.label4.Text = "Product";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // v_toolTip
            // 
            this.v_toolTip.AutomaticDelay = 0;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(139, 94);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label5.Size = new System.Drawing.Size(70, 28);
            this.label5.TabIndex = 48;
            this.label5.Text = "License";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // v_lblLicense
            // 
            this.v_lblLicense.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(247)))), ((int)(((byte)(239)))));
            this.v_lblLicense.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v_lblLicense.Cursor = System.Windows.Forms.Cursors.Hand;
            this.v_lblLicense.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.v_lblLicense.ForeColor = System.Drawing.Color.Blue;
            this.v_lblLicense.Location = new System.Drawing.Point(208, 94);
            this.v_lblLicense.Margin = new System.Windows.Forms.Padding(0);
            this.v_lblLicense.Name = "v_lblLicense";
            this.v_lblLicense.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.v_lblLicense.Size = new System.Drawing.Size(360, 28);
            this.v_lblLicense.TabIndex = 47;
            this.v_lblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // c_FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(585, 134);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.v_lblLicense);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.v_pbxLogo);
            this.Controls.Add(this.v_lblCopyright);
            this.Controls.Add(this.v_lblWebsite);
            this.Controls.Add(this.v_labelVersion);
            this.Controls.Add(this.v_labelProductName);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "c_FrmAbout";
            this.Padding = new System.Windows.Forms.Padding(12, 11, 12, 11);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.v_pbxLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Label v_labelProductName;
        private Label v_labelVersion;
        private Label v_lblCopyright;
        private Label v_lblWebsite;
        internal PictureBox v_pbxLogo;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private ToolTip v_toolTip;
        private Label label5;
        private Label v_lblLicense;
    }
}
