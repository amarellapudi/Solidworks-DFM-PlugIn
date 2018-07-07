using System;
using System.Drawing;
using System.Windows.Forms;

namespace SongTelenkoDFM2
{
    public partial class MessageBox_DFMResults : Form
    {
        public MessageBox_DFMResults(string location)
        { 
            InitializeComponent();
            pictureBox.ImageLocation = location;
        }

        private void MessageBox_DFMResults_Load(object sender, EventArgs e)
        {
            //Image img = Image.FromFile(pictureBox.ImageLocation);

            //int height = img.Height;

            //// no smaller than design time size
            //MinimumSize = new Size(Width, Height);

            //// no larger than screen size
            //MaximumSize = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            //AutoSize = true;
            //AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
    }
}
