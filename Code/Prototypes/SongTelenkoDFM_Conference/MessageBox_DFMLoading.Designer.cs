namespace SongTelenkoDFM_Conference
{
    partial class MessageBox_DFMLoading
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
            this.ProgresBar_DFM = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ProgresBar_DFM
            // 
            this.ProgresBar_DFM.Location = new System.Drawing.Point(49, 106);
            this.ProgresBar_DFM.MarqueeAnimationSpeed = 15;
            this.ProgresBar_DFM.Maximum = 1000000000;
            this.ProgresBar_DFM.Name = "ProgresBar_DFM";
            this.ProgresBar_DFM.Size = new System.Drawing.Size(326, 26);
            this.ProgresBar_DFM.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ProgresBar_DFM.TabIndex = 0;
            this.ProgresBar_DFM.Tag = "";
            // 
            // label1
            // 
            this.label1.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.label1.Size = new System.Drawing.Size(427, 103);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please wait while the\r\nSculptPrint DFM Result is loaded";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MessageBox_DFMLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 156);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ProgresBar_DFM);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageBox_DFMLoading";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SculptPrint DFM Result";
            this.Load += new System.EventHandler(this.Window_ContentRendered);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar ProgresBar_DFM;
        private System.Windows.Forms.Label label1;
    }
}