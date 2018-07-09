using System.Threading;
using System.Windows.Forms;

namespace SongTelenkoDFM2
{
    public partial class MessageBox_DFMResults : Form
    {
        public MessageBox_DFMResults(string location)
        { 
            InitializeComponent();
            Thread.Sleep(250);
            pictureBox.ImageLocation = location;
        }
    }
}