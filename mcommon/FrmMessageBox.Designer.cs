namespace marc_common
{
    partial class c_FrmMessageBox
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
            if (v_webBrowser != null) v_webBrowser.Dispose();
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
            this.v_button2 = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.Panel();
            this.v_pbxIcon = new System.Windows.Forms.PictureBox();
            this.v_lblMessage = new System.Windows.Forms.Label();
            this.v_button3 = new System.Windows.Forms.Button();
            this.v_button1 = new System.Windows.Forms.Button();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.v_pbxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // v_button2
            // 
            this.v_button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.v_button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v_button2.Location = new System.Drawing.Point(167, 102);
            this.v_button2.Name = "v_button2";
            this.v_button2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.v_button2.Size = new System.Drawing.Size(86, 26);
            this.v_button2.TabIndex = 2;
            this.v_button2.Text = "Btn2";
            this.v_button2.UseVisualStyleBackColor = true;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.BackColor = System.Drawing.SystemColors.Window;
            this.panel.Controls.Add(this.v_pbxIcon);
            this.panel.Controls.Add(this.v_lblMessage);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(421, 96);
            this.panel.TabIndex = 6;
            // 
            // v_pbxIcon
            // 
            this.v_pbxIcon.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.v_pbxIcon.Location = new System.Drawing.Point(13, 32);
            this.v_pbxIcon.Name = "v_pbxIcon";
            this.v_pbxIcon.Size = new System.Drawing.Size(32, 32);
            this.v_pbxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.v_pbxIcon.TabIndex = 7;
            this.v_pbxIcon.TabStop = false;
            // 
            // v_lblMessage
            // 
            this.v_lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.v_lblMessage.AutoSize = true;
            this.v_lblMessage.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v_lblMessage.Location = new System.Drawing.Point(58, 9);
            this.v_lblMessage.MaximumSize = new System.Drawing.Size(345, 0);
            this.v_lblMessage.MinimumSize = new System.Drawing.Size(345, 78);
            this.v_lblMessage.Name = "v_lblMessage";
            this.v_lblMessage.Size = new System.Drawing.Size(345, 78);
            this.v_lblMessage.TabIndex = 6;
            this.v_lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.v_lblMessage.UseMnemonic = false;
            // 
            // v_button3
            // 
            this.v_button3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.v_button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v_button3.Location = new System.Drawing.Point(259, 102);
            this.v_button3.Name = "v_button3";
            this.v_button3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.v_button3.Size = new System.Drawing.Size(86, 26);
            this.v_button3.TabIndex = 0;
            this.v_button3.Text = "Btn3";
            this.v_button3.UseVisualStyleBackColor = true;
            this.v_button3.Visible = false;
            // 
            // v_button1
            // 
            this.v_button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.v_button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v_button1.Location = new System.Drawing.Point(75, 102);
            this.v_button1.Name = "v_button1";
            this.v_button1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.v_button1.Size = new System.Drawing.Size(86, 26);
            this.v_button1.TabIndex = 1;
            this.v_button1.Text = "Btn1";
            this.v_button1.UseVisualStyleBackColor = true;
            this.v_button1.Visible = false;
            // 
            // c_FrmMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 133);
            this.Controls.Add(this.v_button1);
            this.Controls.Add(this.v_button3);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.v_button2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(437, 171);
            this.Name = "c_FrmMessageBox";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.v_pbxIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button v_button2;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label v_lblMessage;
        private System.Windows.Forms.PictureBox v_pbxIcon;
        private System.Windows.Forms.Button v_button3;
        private System.Windows.Forms.Button v_button1;
    }
}

