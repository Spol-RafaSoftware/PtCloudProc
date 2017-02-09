namespace TaoComp1
{
    partial class fm_Processing
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
            this.LSTBX_Surf = new System.Windows.Forms.ListBox();
            this.LSTBX_Model = new System.Windows.Forms.ListBox();
            this.LSTBX_Oct = new System.Windows.Forms.ListBox();
            this.LSTBX_Msg = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // LSTBX_Surf
            // 
            this.LSTBX_Surf.FormattingEnabled = true;
            this.LSTBX_Surf.Location = new System.Drawing.Point(12, 4);
            this.LSTBX_Surf.Name = "LSTBX_Surf";
            this.LSTBX_Surf.Size = new System.Drawing.Size(241, 43);
            this.LSTBX_Surf.TabIndex = 0;
            // 
            // LSTBX_Model
            // 
            this.LSTBX_Model.FormattingEnabled = true;
            this.LSTBX_Model.Location = new System.Drawing.Point(12, 102);
            this.LSTBX_Model.Name = "LSTBX_Model";
            this.LSTBX_Model.Size = new System.Drawing.Size(241, 95);
            this.LSTBX_Model.TabIndex = 1;
            // 
            // LSTBX_Oct
            // 
            this.LSTBX_Oct.FormattingEnabled = true;
            this.LSTBX_Oct.Location = new System.Drawing.Point(12, 53);
            this.LSTBX_Oct.Name = "LSTBX_Oct";
            this.LSTBX_Oct.Size = new System.Drawing.Size(241, 43);
            this.LSTBX_Oct.TabIndex = 2;
            // 
            // LSTBX_Msg
            // 
            this.LSTBX_Msg.FormattingEnabled = true;
            this.LSTBX_Msg.Location = new System.Drawing.Point(259, 1);
            this.LSTBX_Msg.Name = "LSTBX_Msg";
            this.LSTBX_Msg.Size = new System.Drawing.Size(241, 199);
            this.LSTBX_Msg.TabIndex = 3;
            // 
            // fm_Processing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 213);
            this.Controls.Add(this.LSTBX_Msg);
            this.Controls.Add(this.LSTBX_Oct);
            this.Controls.Add(this.LSTBX_Model);
            this.Controls.Add(this.LSTBX_Surf);
            this.Name = "fm_Processing";
            this.Text = "fm_Processing";
            this.Load += new System.EventHandler(this.fm_Processing_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fm_Processing_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox LSTBX_Surf;
        private System.Windows.Forms.ListBox LSTBX_Model;
        private System.Windows.Forms.ListBox LSTBX_Oct;
        private System.Windows.Forms.ListBox LSTBX_Msg;

    }
}