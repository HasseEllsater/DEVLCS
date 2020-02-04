using LCSCarousel.Classes;
using LCSCarousel.Model;
using LCSCarousel.ViewModels;
using MahApps.Metro.Controls.Dialogs;
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

namespace LCSCarousel.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private RDPTerminal myTerminal;


        public MainPage()
        {
            InitializeComponent();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigation.Navigate(new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigation.Navigate(new Uri("Views/LoginPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Project_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.SelectProject(true);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.RefreshAllInstances();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            SharedMethods.ConfirmLogout();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            List<CloudHostedInstance> allWMS = mainWindow.AllWMs;
            if (allWMS.Count == 0)
            {
                mainWindow.EnableMenuOptions(false);
                SelectedVMName.Content = Properties.Resources.NotSignedIn;
                SelectedVMName.Background = Brushes.DarkRed;
                SelectedVMName.Foreground = Brushes.White;
                SelectedVMName.FontWeight = FontWeights.Bold;
            }
            else
            {
                mainWindow.EnableMenuOptions(true);
                myTerminal = mainWindow.getMyRDP(Properties.Settings.Default.SelectedPersonalVM);

                if (myTerminal != null)
                {
                    SelectedVMName.Background = Brushes.Transparent;
                    SelectedVMName.Foreground = Brushes.Black;
                    SelectedVMName.FontWeight = FontWeights.Light;

                    Open365.Visibility = Visibility.Visible;
                    DataContext = myTerminal;
                    SelectedVMName.Content = string.Format(Properties.Resources.MyRDPTitle, myTerminal.DisplayName);
                    OpenTerminal.Visibility = Visibility.Visible;

                    if (myTerminal.ImageSource == Properties.Settings.Default.DefaultImage)
                    {
                        SelectedVMImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.DefaultImage, UriKind.Relative));
                    }
                    else
                    {
                        Uri fileUri = new Uri(myTerminal.ImageSource);
                        SelectedVMImage.Source = new BitmapImage(fileUri);
                    }
                }
            }
        }

     

        private void OpenTerminal_Click(object sender, RoutedEventArgs e)
        {
            if (myTerminal == null) return;

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.OpenRDPSession(myTerminal.EnvironmentId);
        }

        private void Open365_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as RDPTerminal;
            if (viewModel == null)
            {
                return;
            }

            if (viewModel != null)
            {
                var item = viewModel;
                foreach (var link in item.NavigationLinks)
                {
                    if (link.DisplayName == "Log on to environment")
                    {

                        if (Properties.Settings.Default.MimimizeOnStartRDP == true)
                        {
                            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                            mainWindow.WindowState = WindowState.Minimized;
                        }

                        Process.Start(link.NavigationUri);
                        break;
                    }
                }
            }
        }

    }
}
