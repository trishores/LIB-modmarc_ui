namespace marc_common
{
    partial class ButtonTip
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
                this.v_popupPanel.Dispose();
            }
            base.Dispose(v_disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.v_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // v_button
            // 
            this.v_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.v_button.FlatAppearance.BorderSize = 0;
            this.v_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.v_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.v_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.v_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v_button.Location = new System.Drawing.Point(0, 0);
            this.v_button.Margin = new System.Windows.Forms.Padding(0);
            this.v_button.Name = "v_button";
            this.v_button.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.v_button.Size = new System.Drawing.Size(25, 25);
            this.v_button.TabIndex = 0;
            // 
            // ButtonTip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.v_button);
            this.Name = "ButtonTip";
            this.Size = new System.Drawing.Size(25, 25);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button v_button;
    }
}
