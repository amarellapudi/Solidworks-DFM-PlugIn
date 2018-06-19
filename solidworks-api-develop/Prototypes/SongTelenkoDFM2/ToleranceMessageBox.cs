using System;
using System.Windows.Forms;

namespace SongTelenkoDFM2
{
    public partial class ToleranceMessageBox : Form
    {
        public ToleranceMessageBox(string message, string title)
        {
            InitializeComponent();
            lblMessage.Text = "\r\n" + message;
            Text = title;
        }

        public string Message
        {
            get => lblMessage.Text;
            set => lblMessage.Text = value;
        }

        public static void ShowMessage(string message, string title)
        {
            ToleranceMessageBox messageBox = new ToleranceMessageBox(message, title);
            messageBox.ShowDialog();
        }

        private void ButtonNo_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonYes_Click(object sender, EventArgs e)
        {
            Close();
        }
        
    }
}
