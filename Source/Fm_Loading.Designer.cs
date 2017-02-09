namespace TaoComp1
{
    partial class Fm_Loading
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
            this.components = new System.ComponentModel.Container();
            this._LoadingBar1 = new System.Windows.Forms.ProgressBar();
            this.stTXT = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // _LoadingBar1
            // 
            this._LoadingBar1.Location = new System.Drawing.Point(12, 32);
            this._LoadingBar1.Name = "_LoadingBar1";
            this._LoadingBar1.Size = new System.Drawing.Size(205, 23);
            this._LoadingBar1.TabIndex = 0;
            this._LoadingBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // stTXT
            // 
            this.stTXT.AutoSize = true;
            this.stTXT.Location = new System.Drawing.Point(12, 9);
            this.stTXT.Name = "stTXT";
            this.stTXT.Size = new System.Drawing.Size(60, 13);
            this.stTXT.TabIndex = 1;
            this.stTXT.Text = "Loading ....";
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Fm_Loading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(222, 58);
            this.ControlBox = false;
            this.Controls.Add(this.stTXT);
            this.Controls.Add(this._LoadingBar1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Fm_Loading";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Loading";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar _LoadingBar1;
        private System.Windows.Forms.Label stTXT;
        private System.Windows.Forms.Timer timer2;
    }
}