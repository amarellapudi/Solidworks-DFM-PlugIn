using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace SculptPrint_Feedback
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string SculptPrint_View_Location { get; set; }
        public string SolidWorks_View_Location { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var home = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var SculptPrint_Folder = home.Replace("\\Code\\Prototypes\\SculptPrint_Feedback\\SculptPrint_Feedback\\bin\\Debug", "\\SculptPrint\\");

            SculptPrint_View_Location = string.Concat(SculptPrint_Folder, "View_SP.png");
            SolidWorks_View_Location = string.Concat(SculptPrint_Folder, "View_SW.png");

            View_SolidWorks.Source = new BitmapImage(new Uri(SolidWorks_View_Location, UriKind.Absolute));
            View_SculptPrint.Source = new BitmapImage(new Uri(SculptPrint_View_Location, UriKind.Absolute));            
        }

        private void Load_SolidWorks_View_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Load_SculptPrint_View_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void View_SolidWorks_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window win = sender as Window;
            var p = e.GetPosition(win);

            Ellipse ellipse = new Ellipse
            {
                Fill = Brushes.Sienna,
                Width = 10,
                Height = 10,
                StrokeThickness = 2
            };

            Cnv.Children.Add(ellipse);

            Canvas.SetLeft(ellipse, e.GetPosition(View_SolidWorks).X);
            Canvas.SetTop(ellipse, e.GetPosition(View_SolidWorks).Y);

        }
    }
}
