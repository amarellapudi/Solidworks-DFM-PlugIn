using System;
using System.Drawing;
using System.Windows.Forms;

namespace SongTelenkoDFM
{
    public partial class MessageBox_FeatureCritical : Form
    {
        public MessageBox_FeatureCritical(string message, string title)
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

        // Disable close button
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public static void ShowMessage(string message, string title)
        {
            MessageBox_FeatureCritical messageBox = new MessageBox_FeatureCritical(message, title);
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
