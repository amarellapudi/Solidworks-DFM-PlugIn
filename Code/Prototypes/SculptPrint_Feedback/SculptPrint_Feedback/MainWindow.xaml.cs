using System.Windows;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System;
using System.Windows.Media;
using System.Threading.Tasks;

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
            var SculptPrint_Folder = home.Replace("\\Code\\Prototypes\\SculptPrint_Feedback\\bin\\Debug", "\\SculptPrint\\");
            SculptPrint_View_Location = string.Concat(SculptPrint_Folder, "View_SP.png");
            SolidWorks_View_Location = string.Concat(SculptPrint_Folder, "View_SW.png");
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
    }
}
