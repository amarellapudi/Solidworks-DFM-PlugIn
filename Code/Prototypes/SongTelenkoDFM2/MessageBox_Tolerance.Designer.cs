using System.Windows.Forms;

namespace SongTelenkoDFM2
{
    partial class MessageBox_Tolerance
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
            this.btnAccept = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
            this.SuspendLayout();
            // 
            // btnAccept
            // 
            this.btnAccept.Enabled = false;
            this.btnAccept.Location = new System.Drawing.Point(130, 132);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(70, 25);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.ButtonAccept_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.SystemColors.Control;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMessage.Location = new System.Drawing.Point(10, 7);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(314, 65);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "message";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // domainUpDown1
            // 
            this.domainUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.163636F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.domainUpDown1.Items.Add("+/- 0.1 mm");
            this.domainUpDown1.Items.Add("+/- 0.01 mm");
            this.domainUpDown1.Items.Add("+/- 0.001 mm");
            this.domainUpDown1.Location = new System.Drawing.Point(93, 91);
            this.domainUpDown1.Margin = new System.Windows.Forms.Padding(2);
            this.domainUpDown1.Name = "domainUpDown1";
            this.domainUpDown1.ReadOnly = true;
            this.domainUpDown1.Size = new System.Drawing.Size(146, 23);
            this.domainUpDown1.TabIndex = 3;
            this.domainUpDown1.Text = "Select Tolerance";
            this.domainUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.domainUpDown1.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.domainUpDown1.SelectedItemChanged += new System.EventHandler(this.DropDown_SelectedItemChanged);
            // 
            // ToleranceMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(334, 169);
            this.Controls.Add(this.domainUpDown1);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnAccept);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(1100, 600);
            this.Name = "ToleranceMessageBox";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnAccept;
        private Label lblMessage;
        private DomainUpDown domainUpDown1;
    }
}