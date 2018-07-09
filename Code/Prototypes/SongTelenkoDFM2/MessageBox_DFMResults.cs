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
    }
}
