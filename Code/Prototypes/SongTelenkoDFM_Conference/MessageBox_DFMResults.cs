using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SongTelenkoDFM_Conference
{
    public partial class MessageBox_DFMResults : Form
    {
        public MessageBox_DFMResults(string location)
        { 
            InitializeComponent();
            UI_SolidWorks_SideBar_PlugIn.Instance.DesignCheckButton.IsEnabled = false;
            UI_SolidWorks_SideBar_PlugIn.Instance.ManufacturingCheck.IsEnabled = false;
            UI_SolidWorks_SideBar_PlugIn.Instance.ReloadResults.IsEnabled = false;
            UI_SolidWorks_SideBar_PlugIn.Instance.Submit_Button.IsEnabled = false;
            Thread.Sleep(50);
            pictureBox.ImageLocation = location;

            Rectangle screenSize = GetScreen();
            Location = new Point((screenSize.Width - Width) / 2, (screenSize.Height - Height) / 2);
        }

        public Rectangle GetScreen()
        {
            return Screen.FromControl(this).Bounds;
        }

        private void MessageBox_DFMResults_FormClosing(object sender, FormClosingEventArgs e)
        {
            UI_SolidWorks_SideBar_PlugIn.Instance.DesignCheckButton.IsEnabled = true;
            UI_SolidWorks_SideBar_PlugIn.Instance.ManufacturingCheck.IsEnabled = true;
            UI_SolidWorks_SideBar_PlugIn.Instance.ReloadResults.IsEnabled = true;
            UI_SolidWorks_SideBar_PlugIn.Instance.Submit_Button.IsEnabled = true;
        }
    }
}