using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Text;
using Renci.SshNet.Sftp;

namespace SculptPrint_Feedback
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Public Members and Initialization

        // SculptPrint folder and SW/SP view locations
        public static string MHome_Directory;
        public static string MSculptPrint_Folder;
        public string mSolidWorks_View_Location;
        public string mSculptPrint_View_Location;
        public static SftpClient MClient;
        public byte mRun_Number = 0;
        public BitmapImage mSW;
        public BitmapImage mSP;

        // Initialization
        public MainWindow()
        {
            InitializeComponent();

            // Set SculptPrint Folder and SW/SP view locations
            MHome_Directory = string.Concat(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "\\");

            // IF DEBUGGING, UNCOMMENT THIS
            //mSculptPrint_Folder = mHome_Directory.Replace("\\Code\\Prototypes\\SculptPrint_Feedback\\SculptPrint_Feedback\\bin\\Debug\\", "\\SculptPrint\\Experiment Files\\");

            // IF BUILDING FOR SCULPTPRINT DIRECTOR, UNCOMMENT THIS
            MSculptPrint_Folder = MHome_Directory.Replace("\\SculptPrint Feeback Tool\\", "\\Experiment Files\\");

            // Set locations for SculptPrint and SolidWorks Views
            mSolidWorks_View_Location = string.Concat(MSculptPrint_Folder, "View_SW.png");
            mSculptPrint_View_Location = string.Concat(MSculptPrint_Folder, "View_SP.png");

            // Connect SFTP client
            MClient = SFTPConnect();

            // Delete previous local experiment files
            DeleteLocal(MSculptPrint_Folder + "DONE_researcher");
            DeleteLocal(MSculptPrint_Folder + "DONE_subject");
            DeleteLocal(MSculptPrint_Folder + "test.stl");
            DeleteLocal(MSculptPrint_Folder + "View_SW.png");
            DeleteLocal(MSculptPrint_Folder + "View_SP.png");
            DeleteLocal(MSculptPrint_Folder + "View_Researcher_Feedback.png");

            // Delete previous cloud experiment files
            DeleteFile_Cloud("DONE_subject");
            DeleteFile_Cloud("DONE_researcher");
            DeleteFile_Cloud("View_Researcher_Feedback.png");
            DeleteFile_Cloud("test.stl");
            DeleteFile_Cloud("View_SW.png");
        }

        #endregion

        #region Public Members for Drawing

        // Start and end points for drawing on SolidWorks view
        Point mStart_SolidWorks = new Point(0, 0);
        Point mEnd_SolidWorks = new Point(0, 0);
        bool mDrawing_SolidWorks = false;

        // Start and end points for drawing on SculptPrint view
        Point mStart_SculptPrint = new Point(0, 0);
        Point mEnd_SculptPrint = new Point(0, 0);
        bool mDrawing_SculptPrint = false;

        // Methods for saving current screen as PNG
        public static RenderTargetBitmap GetImage()
        {
            Size size = new Size(((Panel)Application.Current.MainWindow.Content).ActualWidth,
                ((Panel)Application.Current.MainWindow.Content).ActualHeight);
            if (size.IsEmpty)
                return null;

            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height - 45, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush((Panel)Application.Current.MainWindow.Content), null, new Rect(new Point(), size));
                context.Close();
            }

            result.Render(drawingvisual);
            return result;
        }
        public static void SaveAsPng(RenderTargetBitmap src, Stream outputStream)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            encoder.Save(outputStream);
        }
        #endregion

        #region UI Button Click Events

        // Called when "Reset SolidWorks View" is clicked
        private void Reset_SolidWorks_View_Click(object sender, RoutedEventArgs e)
        {
            Canvas_SolidWorks.Children.Clear();
        }

        // Called when "Reset SculptPrint View" is clicked
        private void Reset_SculptPrint_View_Click(object sender, RoutedEventArgs e)
        {
            Canvas_SculptPrint.Children.Clear();
        }

        // Called when "Load" is clicked. Loads SW/SP views from server into this app
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            MessageBox_Loading loading = new MessageBox_Loading(MClient);
            var result = loading.ShowDialog();
            
            // Save image into stream so we can do more operations on the View_SW.png
            BitmapImage res = new BitmapImage();
            Stream stream = new MemoryStream();
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(mSolidWorks_View_Location);
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);  
            bitmap.Dispose();   
            res.BeginInit();
            res.StreamSource = stream; 
            res.EndInit();

            // Save image into stream so we can do more operations on the View_SP.png
            BitmapImage res2 = new BitmapImage();
            Stream stream2 = new MemoryStream();
            System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap(mSculptPrint_View_Location);
            bitmap2.Save(stream2, System.Drawing.Imaging.ImageFormat.Png);
            bitmap2.Dispose();
            res2.BeginInit();
            res2.StreamSource = stream2;
            res2.EndInit();
            
            View_SolidWorks.Source = res;
            View_SculptPrint.Source = res2;

            LoadButton.IsEnabled = false;
        }

        // Called when "Submit" is clicked. Creates feedback screenshot and uploads it to server
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            // Remove DONE_subject
            MClient.DeleteFile("DONE_subject");

            // Prepare view for screenshot. Uncheck all check check boxes, and hide the control buttons
            Issue1.IsChecked = false; Issue2.IsChecked = false; Issue3.IsChecked = false;
            Issue4.IsChecked = false; Issue5.IsChecked = false; Issue6.IsChecked = false;
            Controls.Visibility = Visibility.Hidden;

            // Create the screenshot
            using (var fileStream = File.Create(MSculptPrint_Folder + "View_Researcher_Feedback.png"))
            {
                SaveAsPng(GetImage(), fileStream);
                Controls.Visibility = Visibility.Visible;
            }

            // Re-enable the control buttons
            Controls.Visibility = Visibility.Visible;

            // Upload the screenshot to the server
            UploadFile(MClient, "View_Researcher_Feedback.png");
            CreateFinishedFlag();

            Clean(MClient);
            Reset_SculptPrint_View_Click(sender, e);
            Reset_SolidWorks_View_Click(sender, e);

            Load_Click(sender, e);
        }
        
        // Called when the window is closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        // Called when the window is loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Click(sender, e);
        }
        
        #endregion

        #region SolidWorks View Button Click Events

        private void View_SolidWorks_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mStart_SolidWorks = e.GetPosition(Canvas_SolidWorks);
            mDrawing_SolidWorks = true;
        }

        private void View_SolidWorks_MouseMove(object sender, MouseEventArgs e)
        {
            if (mDrawing_SolidWorks)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
            if (mDrawing_SolidWorks)
                    mEnd_SolidWorks = e.GetPosition(Canvas_SolidWorks);
                    DrawLine(Canvas_SolidWorks, mStart_SolidWorks, mEnd_SolidWorks, IssueColor());
                }
                mStart_SolidWorks = mEnd_SolidWorks;
            }
        }

        private void View_SolidWorks_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mDrawing_SolidWorks = false;
            DrawLine(Canvas_SolidWorks, mStart_SolidWorks, mEnd_SolidWorks, IssueColor());
        }

        #endregion

        #region SculptPrint View Button Click Events

        private void View_SculptPrint_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mStart_SculptPrint = e.GetPosition(Canvas_SculptPrint);
            mDrawing_SculptPrint = true;
        }

        private void View_SculptPrint_MouseMove(object sender, MouseEventArgs e)
        {
            if (mDrawing_SculptPrint)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    mEnd_SculptPrint = e.GetPosition(Canvas_SculptPrint);
                    DrawLine(Canvas_SculptPrint, mStart_SculptPrint, mEnd_SculptPrint, IssueColor());
                }
                mStart_SculptPrint = mEnd_SculptPrint;
            }
        }

        private void View_SculptPrint_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mDrawing_SculptPrint = false;
            DrawLine(Canvas_SculptPrint, mStart_SculptPrint, mEnd_SculptPrint, IssueColor());
        }

        #endregion

        #region Drawing Helper Functions

        /// <summary>
        /// Draws a line in a given color on a given canvas
        /// </summary>
        private void DrawLine(Canvas c, Point mStart, Point mEnd, Brush color)
        {
            var thickness = -1;
            if (color == System.Windows.Media.Brushes.Black) thickness = 0;
            else thickness = 2; 

            Line newLine = new Line()
            {
                Stroke = color,
                X1 = mStart.X,
                Y1 = mStart.Y,
                X2 = mEnd.X,
                Y2 = mEnd.Y,

                StrokeThickness = thickness
            };

            c.Children.Add(newLine);
        }

        /// <summary>
        /// Sets brush color based on which manufacturing issue box is checked
        /// </summary>
        /// <returns></returns>
        private Brush IssueColor()
        {
            if (Issue1.IsChecked == true) return Issue1.Background;
            else if (Issue2.IsChecked == true) return Issue2.Background;
            else if (Issue3.IsChecked == true) return Issue3.Background;
            else if (Issue4.IsChecked == true) return Issue4.Background;
            else if (Issue5.IsChecked == true) return Issue5.Background;
            else if (Issue6.IsChecked == true) return Issue6.Background;
            else
            {
                string message = "Please select a color check-box";
                string caption = "No Color Selected";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBox.Show(message, caption, buttons);
                return System.Windows.Media.Brushes.Black;
            }
        }

        #endregion

        #region Issue Check Box Handlers
        /// <summary>
        /// Creates mutually exclusive checkboxes
        /// </summary>

        private void Issue1_Checked(object sender, RoutedEventArgs e)
        {
            Issue2.IsChecked = false;
            Issue3.IsChecked = false;
            Issue4.IsChecked = false;
            Issue5.IsChecked = false;
            Issue6.IsChecked = false;
        }

        private void Issue2_Checked(object sender, RoutedEventArgs e)
        {
            Issue1.IsChecked = false;
            Issue3.IsChecked = false;
            Issue4.IsChecked = false;
            Issue5.IsChecked = false;
            Issue6.IsChecked = false;
        }

        private void Issue3_Checked(object sender, RoutedEventArgs e)
        {
            Issue1.IsChecked = false;
            Issue2.IsChecked = false;
            Issue4.IsChecked = false;
            Issue5.IsChecked = false;
            Issue6.IsChecked = false;
        }

        private void Issue4_Checked(object sender, RoutedEventArgs e)
        {
            Issue1.IsChecked = false;
            Issue2.IsChecked = false;
            Issue3.IsChecked = false;
            Issue5.IsChecked = false;
            Issue6.IsChecked = false;
        }

        private void Issue5_Checked(object sender, RoutedEventArgs e)
        {
            Issue1.IsChecked = false;
            Issue2.IsChecked = false;
            Issue3.IsChecked = false;
            Issue4.IsChecked = false;
            Issue6.IsChecked = false;
        }

        private void Issue6_Checked(object sender, RoutedEventArgs e)
        {
            Issue1.IsChecked = false;
            Issue2.IsChecked = false;
            Issue3.IsChecked = false;
            Issue4.IsChecked = false;
            Issue5.IsChecked = false;
        }
        #endregion

        #region SFTP and File Methods

        // Connect to the domain via SFTP
        public static SftpClient SFTPConnect()
        {
            // Find private key and set connection information
            PrivateKeyFile key = new PrivateKeyFile(string.Concat(MHome_Directory, "rsa.key"), "Aniruddh123");
            var connectionInfo = new ConnectionInfo("marellapudi.com", 18765, "apollome",
                                        new PrivateKeyAuthenticationMethod("apollome", key));

            // Connect STFP client
            SftpClient client = new SftpClient(connectionInfo);
            client.Connect();

            // Change directory to ~/home/public_ftp/
            client.ChangeDirectory(client.WorkingDirectory + "/public_ftp/");

            // return the client as a global variable
            return client;
        }

        // Download a file given it's file name with extension
        public static bool DownloadFile(SftpClient client, string fileName)
        {
            // Try downloading the file
            try
            {
                using (Stream fileStream = File.Create(string.Concat(MSculptPrint_Folder, fileName)))
                {
                    client.DownloadFile(fileName, fileStream);
                }
            }
            catch (SftpPathNotFoundException ex)
            {
                // The path/file was not found
                return false;
            }

            // If we get here, we have successfully downloaded the file
            return true;
        }

        // Upload a file given it's file name with extension
        public static bool UploadFile(SftpClient client, string fileName)
        {
            // Try uploading the file
            try
            {
                using (var fileStream = File.Open((MSculptPrint_Folder + fileName), FileMode.Open))
                {
                    client.UploadFile(fileStream, fileName);
                }
            }
            catch (SftpPathNotFoundException ex)
            {
                // The path/file was not found
                return false;
            }
            // If we get here, we have successfully uploaded the file
            return true;
        }

        // Upload a file named "DONE_subject" to reflect completion of subject's portion
        // of the work flow (uploading test.stl and View_SW.png)
        public static void CreateFinishedFlag()
        {
            try
            {
                // Create the file.
                using (FileStream fs = File.Create(MSculptPrint_Folder + "DONE_researcher"))
                {
                    var info = new UTF8Encoding(true).GetBytes("");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            UploadFile(MClient, "DONE_researcher");
        }

        // Remove experiment files from the server
        public void Clean(SftpClient client)
        {
            // Reset SolidWorks and SculptPrint views to default images
            View_SolidWorks.Source = new BitmapImage(
                new Uri(string.Concat(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "\\IMG_No_Image.png"),
                UriKind.Absolute));
            View_SculptPrint.Source = new BitmapImage(
                new Uri(string.Concat(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "\\IMG_No_Image.png"),
                UriKind.Absolute));

            // Move STL and PNG view files
            var now = DateTime.Now.ToString("ddMMyyyy_HHmmss");
            var dataFolder = now + "/";
            MClient.CreateDirectory(now);
            MClient.RenameFile("test.stl", dataFolder + "test.stl");
            MClient.RenameFile("View_SW.png", dataFolder + "View_SW.png");

            MClient.UploadFile( File.Open((MSculptPrint_Folder + "View_Researcher_Feedback.png"), FileMode.Open), 
                dataFolder + "View_Researcher_Feedback.png" );

            MClient.UploadFile( File.Open((MSculptPrint_Folder + "View_SP.png"), FileMode.Open),
                dataFolder + "View_SP.png" );
        }

        // Delete local file - used to clear SculptPrint experiment files when intializing
        private void DeleteLocal(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        // Delete cloud file - used to clear cloud directory before running experiment
        private void DeleteFile_Cloud(string fileName)
        {
            try
            {
                MClient.DeleteFile(fileName);
            }
            catch(Exception a)
            {
                return;
            }
            return;
        }
        #endregion
    }
}
