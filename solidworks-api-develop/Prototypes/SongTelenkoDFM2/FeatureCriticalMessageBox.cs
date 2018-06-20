﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace SongTelenkoDFM2
{
    public partial class FeatureCriticalMessageBox : Form
    {
        public FeatureCriticalMessageBox(string message, string title)
        {
            InitializeComponent();
            lblMessage.Text = message;
            Text = title;

            Rectangle thisScreen = GetScreen();
            int xpos = (int)((0.55) * thisScreen.Width);
            int ypos = (int)((0.55) * thisScreen.Height);
            this.Location = new Point(xpos, ypos);
        }

        public Rectangle GetScreen()
        {
            return Screen.FromControl(this).Bounds;
        }

        public string Message
        {
            get => lblMessage.Text;
            set => lblMessage.Text = value;
        }

        public static void ShowMessage(string message, string title)
        {
            FeatureCriticalMessageBox messageBox = new FeatureCriticalMessageBox(message, title);
            messageBox.ShowDialog();
        }

        private void ButtonNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void ButtonYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }
        
    }
}
