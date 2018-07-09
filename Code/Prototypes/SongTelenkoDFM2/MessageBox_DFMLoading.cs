using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
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

        // Disable close button
        //private const int CP_NOCLOSE_BUTTON = 0x200;
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams myCp = base.CreateParams;
        //        myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
        //        return myCp;
        //    }
        //}

        public void Window_ContentRendered(object sender, EventArgs e)
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;

            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            FileInfo results = new FileInfo(FileLocation);

            int i = 0;
            while (!results.Exists)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(20);
                i++;
                i = i % ProgresBar_DFM.Maximum;

                results = new FileInfo(FileLocation);
            }

            DialogResult = DialogResult.Yes;
            Thread.Sleep(20);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgresBar_DFM.Value = e.ProgressPercentage;
        }
    }
}
