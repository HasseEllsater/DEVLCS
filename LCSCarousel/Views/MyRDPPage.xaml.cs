using LCSCarousel.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static LCSCarousel.MainWindow;

namespace LCSCarousel.Views
{
    /// <summary>
    /// Interaction logic for MyRDPPage.xaml
    /// </summary>
    public partial class MyRDPPage : Page
    {
        private RDPTerminal myTerminal;

        public MyRDPPage()
        {
            InitializeComponent();
        }

        private void StartInstance_Click(object sender, RoutedEventArgs e)
        {
            if (myTerminal == null) return;
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.StartStopRDPSession(myTerminal.EnvironmentId, VMAction.Start);
        }

        private void StopInstance_Click(object sender, RoutedEventArgs e)
        {
            if (myTerminal == null) return;
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.StartStopRDPSession(myTerminal.EnvironmentId, VMAction.Stop);

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            myTerminal = mainWindow.getMyRDP(Properties.Settings.Default.SelectedPersonalVM);

            if (myTerminal is null)
            {
                OpenTerminal.Visibility = Visibility.Hidden;
                StartInstance.IsEnabled = false;
                StopInstance.IsEnabled = false; 
                ShowPwd.IsEnabled = false;
                LogOnToApplication.IsEnabled = false;

                return;
            }
            OpenTerminal.Visibility = Visibility.Visible;

            DataContext = myTerminal;
            MyRDP.Text = string.Format(Properties.Resources.MyRDPTitle, myTerminal.DisplayName);
            MyTitle.Content = string.Format(Properties.Resources.SelectedRDPSession,myTerminal.DisplayName);

        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            ShowPasswordsDialog dlg = new ShowPasswordsDialog();
            dlg.Owner = Application.Current.MainWindow as MainWindow;
            dlg.rdpTerminal = myTerminal;
            dlg.ShowDialog();
        }

        private void LogOnToApplication_Click(object sender, RoutedEventArgs e)
        {
            if (myTerminal == null) return;
 
            var item = myTerminal;
            foreach (var link in item.NavigationLinks)
            {
                if (link.DisplayName == "Log on to environment")
                {

                    Process.Start(link.NavigationUri);
                    break;
                }
            }
        }
        private void OpenTerminal_Click(object sender, RoutedEventArgs e)
        {
            if (myTerminal == null) return;

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.OpenRDPSession(myTerminal.EnvironmentId);
        }
    }
}
