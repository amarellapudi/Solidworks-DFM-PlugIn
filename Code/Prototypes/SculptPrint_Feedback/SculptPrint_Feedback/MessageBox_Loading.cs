﻿using Renci.SshNet;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Threading;
using System.Windows.Forms;

namespace SculptPrint_Feedback
{
    public partial class MessageBox_Loading : Form
    {
        public SftpClient Client { get; set; }

        public MessageBox_Loading(SftpClient client)
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            Client = client;
        }

        public void Window_ContentRendered(object sender, EventArgs e)
        {
            var worker = new BackgroundWorker();

            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!MainWindow.DownloadFile(Client, "DONE_subject"))
            {
                Thread.Sleep(75);
            }
            Thread.Sleep(100);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (MainWindow.DownloadFile(Client, "FINISHED_subject.txt"))
            {
                DownloadFiles_SubjectFinished();
            }
            else
            {
                DownloadFilesAndRun();
            }
        }

        private void DownloadFilesAndRun()
        {
            progressBar1.MarqueeAnimationSpeed = 0;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Value = 2;

            progressBar1.Value = 20;
            label1.Text = "Downloading SolidWorks View";
            Refresh();
            MainWindow.DownloadFile(Client, "View_SW.png");
            
            progressBar1.Value = 40;
            label1.Text = "Downloading Test Part";
            Refresh();
            MainWindow.DownloadFile(Client, "test.stl");
            
            progressBar1.Value = 80;
            label1.Text = "Running SculptPrint Script";
            Refresh();
            MainWindow.DownloadFile(Client, "DONE_subject_info.txt");

            // This text can include many comma-separated-values
            // Currently this just tells us if we have a mill piece or lathe piece
            string mill = File.ReadAllText(MainWindow.MSculptPrint_Folder+"DONE_subject_info.txt").Split(',')[0];

            // Determine if we run the SculptPrint mill setup or the lathe setup
            // TO DO: Fill out the mill script so that mill pieces are properly simulated
            string text;
            if (mill=="mill")
            {
                text = "pushd \"\\\\prism.nas.gatech.edu\\rsong8\\vlab\\desktop\\SculptPrint\\Scripts\\\"\r\n\r\npython SculptPrint_Setup_Mill.py";
            }
            else
            {
                text = "pushd \"\\\\prism.nas.gatech.edu\\rsong8\\vlab\\desktop\\SculptPrint\\Scripts\\\"\r\n\r\npython SculptPrint_Setup_Lathe.py";
            }

            // Run a powershell script that invokes either the mill or lathe SculptPrint script
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript(text);
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
            }

            progressBar1.Value = 100;

            DialogResult = DialogResult.Yes;
        }

        private void DownloadFiles_SubjectFinished()
        {
            progressBar1.MarqueeAnimationSpeed = 0;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Value = 2;

            progressBar1.Value = 20;
            label1.Text = "Downloading SolidWorks View";
            Refresh();
            MainWindow.DownloadFile(Client, "View_SW.png");

            progressBar1.Value = 40;
            label1.Text = "Downloading Final Part";
            Refresh();
            MainWindow.DownloadFile(Client, "test.stl");

            progressBar1.Value = 100;

            DialogResult = DialogResult.No;
        }
    }
}
