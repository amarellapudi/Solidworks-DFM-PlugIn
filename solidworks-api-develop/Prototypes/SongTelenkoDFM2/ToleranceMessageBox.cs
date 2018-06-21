using AngelSix.SolidDna;
using System;
using System.Drawing;
using System.Windows.Forms;
using static AngelSix.SolidDna.SolidWorksEnvironment;

namespace SongTelenkoDFM2
{
    public partial class ToleranceMessageBox : Form
    {
        public ToleranceMessageBox(string message, string title)
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

        public string Tolerance_Value
        {
            get => domainUpDown1.Text;
            set => domainUpDown1.Text = value;
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
            ToleranceMessageBox messageBox = new ToleranceMessageBox(message, title);
            messageBox.ShowDialog();
        }

        private void ButtonAccept_Click(object sender, EventArgs e)
        {
            if (domainUpDown1.Text == "Select Tolerance")
            {
                SolidWorksEnvironment.Application.ShowMessageBox("Please select a tolerance value from the drop-down" +
                    " menu before continuing", SolidWorksMessageBoxIcon.Stop);
            }
            else Close();
        }

        private void DropDown_SelectedItemChanged(object sender, EventArgs e)
        {
            btnAccept.Enabled = true;
        }
    }
}
