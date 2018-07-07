using System.Windows.Forms;

namespace SongTelenkoDFM2
{
    partial class MessageBox_FeatureCritical
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
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnYes
            // 
            this.btnYes.Location = new System.Drawing.Point(66, 99);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(80, 25);
            this.btnYes.TabIndex = 0;
            this.btnYes.Text = "Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.ButtonYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point(160, 99);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(80, 25);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "No";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.ButtonNo_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.SystemColors.Control;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMessage.Location = new System.Drawing.Point(10, 7);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(290, 79);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "message";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MessageBox_FeatureCritical
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(309, 136);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(1100, 600);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageBox_FeatureCritical";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnYes;
        private Button btnNo;
        private Label lblMessage;
    }
}