namespace TaoComp1
{
    partial class fm_Keyinc
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
            this.BTN_SETPOSINC = new System.Windows.Forms.Button();
            this.BTN_SETANGINC = new System.Windows.Forms.Button();
            this.LBL_POSINC = new System.Windows.Forms.Label();
            this.TXTBX_POSINC = new System.Windows.Forms.TextBox();
            this.TXTBX_ANGINC = new System.Windows.Forms.TextBox();
            this.LBL_ANGINC = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BTN_SETPOSINC
            // 
            this.BTN_SETPOSINC.Location = new System.Drawing.Point(159, 12);
            this.BTN_SETPOSINC.Name = "BTN_SETPOSINC";
            this.BTN_SETPOSINC.Size = new System.Drawing.Size(49, 23);
            this.BTN_SETPOSINC.TabIndex = 0;
            this.BTN_SETPOSINC.Text = "set";
            this.BTN_SETPOSINC.UseVisualStyleBackColor = true;
            this.BTN_SETPOSINC.Click += new System.EventHandler(this.BTN_SETPOSINC_Click);
            // 
            // BTN_SETANGINC
            // 
            this.BTN_SETANGINC.Location = new System.Drawing.Point(159, 43);
            this.BTN_SETANGINC.Name = "BTN_SETANGINC";
            this.BTN_SETANGINC.Size = new System.Drawing.Size(49, 21);
            this.BTN_SETANGINC.TabIndex = 1;
            this.BTN_SETANGINC.Text = "set";
            this.BTN_SETANGINC.UseVisualStyleBackColor = true;
            this.BTN_SETANGINC.Click += new System.EventHandler(this.BTN_SETANGINC_Click);
            // 
            // LBL_POSINC
            // 
            this.LBL_POSINC.AutoSize = true;
            this.LBL_POSINC.Location = new System.Drawing.Point(6, 17);
            this.LBL_POSINC.Name = "LBL_POSINC";
            this.LBL_POSINC.Size = new System.Drawing.Size(41, 13);
            this.LBL_POSINC.TabIndex = 2;
            this.LBL_POSINC.Text = "pos inc";
            // 
            // TXTBX_POSINC
            // 
            this.TXTBX_POSINC.Location = new System.Drawing.Point(53, 15);
            this.TXTBX_POSINC.MaxLength = 5;
            this.TXTBX_POSINC.Name = "TXTBX_POSINC";
            this.TXTBX_POSINC.Size = new System.Drawing.Size(100, 20);
            this.TXTBX_POSINC.TabIndex = 3;
            // 
            // TXTBX_ANGINC
            // 
            this.TXTBX_ANGINC.Location = new System.Drawing.Point(53, 43);
            this.TXTBX_ANGINC.MaxLength = 5;
            this.TXTBX_ANGINC.Name = "TXTBX_ANGINC";
            this.TXTBX_ANGINC.Size = new System.Drawing.Size(100, 20);
            this.TXTBX_ANGINC.TabIndex = 5;
            // 
            // LBL_ANGINC
            // 
            this.LBL_ANGINC.AutoSize = true;
            this.LBL_ANGINC.Location = new System.Drawing.Point(6, 46);
            this.LBL_ANGINC.Name = "LBL_ANGINC";
            this.LBL_ANGINC.Size = new System.Drawing.Size(44, 13);
            this.LBL_ANGINC.TabIndex = 4;
            this.LBL_ANGINC.Text = "Ang Inc";
            // 
            // fm_Keyinc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 78);
            this.Controls.Add(this.TXTBX_ANGINC);
            this.Controls.Add(this.LBL_ANGINC);
            this.Controls.Add(this.TXTBX_POSINC);
            this.Controls.Add(this.LBL_POSINC);
            this.Controls.Add(this.BTN_SETANGINC);
            this.Controls.Add(this.BTN_SETPOSINC);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fm_Keyinc";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Movement Increment";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BTN_SETPOSINC;
        private System.Windows.Forms.Button BTN_SETANGINC;
        private System.Windows.Forms.Label LBL_POSINC;
        private System.Windows.Forms.TextBox TXTBX_POSINC;
        private System.Windows.Forms.TextBox TXTBX_ANGINC;
        private System.Windows.Forms.Label LBL_ANGINC;
    }
}