using Renci.SshNet;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace SongTelenkoDFM2
{
    public partial class MessageBox_DFMLoading : Form
    {
        public string FileName { get; set; }
        public SftpClient Client { get; set; }

        public MessageBox_DFMLoading(string fileName, SftpClient client)
        {
            InitializeComponent();
            FileName = fileName;
            Client = client;

            CustomPropertiesUI.SFTPUploadFile(client, "View_SW.png");
            CustomPropertiesUI.SFTPUploadFile(client, "test.stl");
            CustomPropertiesUI.CreateFinishedFlag();
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

        public void Window_ContentRendered(object sender, EventArgs e)
        {
            var worker = new BackgroundWorker();

            worker.DoWork += Worker_DoWork;

            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!CustomPropertiesUI.DownloadFile(Client, "DONE_researcher"))
            {
                Thread.Sleep(75);
            }
            CustomPropertiesUI.DownloadFile(Client, FileName);
            Client.DeleteFile("DONE_researcher");
            Client.DeleteFile("View_Researcher_Feedback.png");

            DialogResult = DialogResult.Yes;
        }
    }
}
