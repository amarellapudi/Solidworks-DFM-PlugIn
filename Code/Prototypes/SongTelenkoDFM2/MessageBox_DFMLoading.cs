using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace SongTelenkoDFM2
{
    public partial class MessageBox_DFMLoading : Form
    {
        
        public MessageBox_DFMLoading(string location)
        {
            InitializeComponent();
            FileLocation = location;
        }

        public string FileLocation { get; set; }

        public void Window_ContentRendered(object sender, EventArgs e)
        {
            var worker = new BackgroundWorker();

            worker.DoWork += Worker_DoWork;

            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            FileInfo results = new FileInfo(FileLocation);
            
            while (!results.Exists) results = new FileInfo(FileLocation);

            DialogResult = DialogResult.Yes;
        }
    }
}
