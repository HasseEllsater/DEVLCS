using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LCSCarousel.Views
{
    /// <summary>
    /// Interaction logic for LogOutPage.xaml
    /// </summary>
    public partial class LogOutPage : Page
    {

        public LogOutPage()
        {
            InitializeComponent();
            WebBrowserHelper.FixBrowserVersion();

        }

        private void browser_Navigated(object sender, NavigationEventArgs e)
        {
            if(e.Uri == null || e.Uri.ToString().StartsWith("https://lcs.dynamics.com/Logon"))
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.ClearAndClose();
            }

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            browser.Navigate(mainWindow.GetLoggedInUri());
        }


    }
}
