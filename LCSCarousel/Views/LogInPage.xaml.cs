using LCSCarousel.Classes;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LCSCarousel.Views
{
    public enum LoadedFirstPage
    {
        No,
        Yes
    }
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        internal bool Cancelled { get; private set; }
        public LogInPage()
        {
            InitializeComponent();
            WebBrowserHelper.FixBrowserVersion();

        }
        public async void LoggedIn()
        {
            SharedMethods.LogInConfirmation();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Cancelled = true;
            browser.Navigate("https://lcs.dynamics.com");
        }

        private void browser_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri.ToString().StartsWith("https://lcs.dynamics.com/v2"))
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.SetLoggedInUri(e.Uri);
                Cancelled = false;
            }

        }
        private void browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                if (Cancelled == false)
                {
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow.LoadOrSelectProject();
                }

            }
            catch (Exception ex)
            {
                string exception = ex.Message.ToString();
            }

        }

    }
}
