using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SongTelenkoDFM
{
    public partial class MessageBox_DFMResults : Form
    {
        public MessageBox_DFMResults(string location)
        { 
            InitializeComponent();
            Thread.Sleep(250);
            pictureBox.ImageLocation = location;

            Rectangle screenSize = GetScreen();
            Location = new Point((screenSize.Width - Width) / 2, (screenSize.Height - Height) / 2);
        }

        public Rectangle GetScreen()
        {
            return Screen.FromControl(this).Bounds;
        }
    }
}