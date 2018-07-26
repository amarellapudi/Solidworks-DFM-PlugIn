using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System.Net;
using System.Threading;
using System.Text;

namespace SculptPrint_Feedback
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string mSculptPrint_Folder;
        public string mSolidWorks_View_Location;
        public string mSculptPrint_View_Location;
        public string[] mFTP = { "ftp://www.marellapudi.com/public_ftp/", "apollome", "Aniruddh.123" };

        public MainWindow()
        {
            InitializeComponent();

            var home = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            mSculptPrint_Folder = home.Replace("\\Code\\Prototypes\\SculptPrint_Feedback\\SculptPrint_Feedback\\bin\\Debug", "\\SculptPrint\\");
            mSolidWorks_View_Location = string.Concat(mSculptPrint_Folder, "View_SW.png");
            mSculptPrint_View_Location = string.Concat(mSculptPrint_Folder, "View_SP.png");
        }
        
        #region Public Members for Drawing

        System.Windows.Point mStart_SolidWorks = new System.Windows.Point(0, 0);
        System.Windows.Point mEnd_SolidWorks = new System.Windows.Point(0, 0);
        bool mDrawing_SolidWorks = false;

        System.Windows.Point mStart_SculptPrint = new System.Windows.Point(0, 0);
        System.Windows.Point mEnd_SculptPrint = new System.Windows.Point(0, 0);
        bool mDrawing_SculptPrint = false;

        public static RenderTargetBitmap GetImage()
        {
            System.Windows.Size size = new System.Windows.Size(((Panel)Application.Current.MainWindow.Content).ActualWidth,
                ((Panel)Application.Current.MainWindow.Content).ActualHeight);
            if (size.IsEmpty)
                return null;

            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height - 45, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush((Panel)Application.Current.MainWindow.Content), null, new Rect(new System.Windows.Point(), size));
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

        private void Reset_SolidWorks_View_Click(object sender, RoutedEventArgs e)
        {
            Canvas_SolidWorks.Children.Clear();
        }

        private void Reset_SculptPrint_View_Click(object sender, RoutedEventArgs e)
        {
            Canvas_SculptPrint.Children.Clear();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(mSculptPrint_View_Location) && File.Exists(mSolidWorks_View_Location))
            {
                string message = "Already Loaded Views";
                string caption = "Error";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBox.Show(message, caption, buttons);
                return;
            }
            string fileName = "View_SP.png";

            while (true)
            {
                if (FileExistsOnServer(fileName))
                {
                    DownloadFile(fileName);
                    break;
                }
            }

            fileName = "View_SW.png";

            while (true)
            {
                if (FileExistsOnServer(fileName))
                {
                    DownloadFile(fileName);
                    break;
                }
            }

            string SolidWorks_View_Location = string.Concat(mSculptPrint_Folder, "View_SW.png");
            string SculptPrint_View_Location = string.Concat(mSculptPrint_Folder, "View_SP.png");

            View_SolidWorks.Source = new BitmapImage(new Uri(SolidWorks_View_Location, UriKind.Absolute));
            View_SculptPrint.Source = new BitmapImage(new Uri(SculptPrint_View_Location, UriKind.Absolute));
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Issue1.IsChecked = false;
            Issue2.IsChecked = false;
            Issue3.IsChecked = false;
            Issue4.IsChecked = false;
            Issue5.IsChecked = false;
            Issue6.IsChecked = false;

            Controls.Visibility = Visibility.Hidden;
            var fileStream = File.Create(mSculptPrint_Folder + "View_Researcher_Feedback.png");
            SaveAsPng(GetImage(), fileStream);
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

        #region Helper Functions

        /// <summary>
        /// Draws a line in a given color on a given canvas
        /// </summary>
        private void DrawLine(Canvas c, System.Windows.Point mStart, System.Windows.Point mEnd, System.Windows.Media.Brush color)
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
        private System.Windows.Media.Brush IssueColor()
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
                string caption = "Error: No Color Selected";
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

        #region File Methods

        private bool FileExistsOnServer(string fileName)
        {
                FtpWebResponse response = null;
                var request = (FtpWebRequest)WebRequest.Create(string.Concat(mFTP[0], fileName));
                request.Credentials = new NetworkCredential(mFTP[1], mFTP[2]);
                request.Method = WebRequestMethods.Ftp.GetFileSize;

                try
                {
                    response = (FtpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        return false;
                    }
                }
            return true;
        }

        private void DownloadFile(string fileName)
        {
            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(mFTP[1], mFTP[2]);
                byte[] fileData = request.DownloadData(string.Concat(mFTP[0], fileName));

                using (FileStream file = File.Create(string.Concat(mSculptPrint_Folder, fileName)))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }
            }
        }

        private void UploadFile(string fileName)
        {
            var request = (FtpWebRequest)WebRequest.Create(string.Concat(mFTP[0], fileName));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(mFTP[1], mFTP[2]);
            
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(string.Concat(mSculptPrint_Folder, fileName)))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }
            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
        }
        #endregion
    }
}
