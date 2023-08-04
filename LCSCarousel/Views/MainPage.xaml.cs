using LCSCarousel.Classes;
using LCSCarousel.Enums;
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
using static LCSCarousel.MainWindow;

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
            mainWindow.SelectProject();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.SetLastActivity(DateTime.Now);
            SelectRefreshOptions SelectOptionsDlg = new SelectRefreshOptions
            {
                Owner = mainWindow
            };

            SelectOptionsDlg.ShowDialog();

            bool refreshVM = SelectOptionsDlg.RefreshAllVms;
            bool refreshCredentials = SelectOptionsDlg.RefreshCredentials;

            if(refreshVM == false && refreshCredentials == false)
            {
                SharedMethods.StopDialog(Properties.Resources.CancellingRefresh, Properties.Resources.NoRefreshOptionsSelected);
                return;
            }

            mainWindow.RefreshAllInstances(refreshVM, refreshCredentials);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            SharedMethods.ConfirmLogout();
        }
        public void CheckSessionTime()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if(mainWindow == null)
            {
                return;
            }
            mainWindow.CheckStartup();
            SessionInfo.Title = mainWindow.Logouttime;
            if (Properties.Settings.Default.LimitFunctions == true)
            {
                UpdateStatus();
            }
            else
            {
                TurnOnAccess(mainWindow);
            }

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            UpdateStatus();
            mainWindow.SessionChangedEvent += CheckSessionTime;
        }

        private void UpdateStatus()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            SessionInfo.Title = mainWindow.Logouttime;
            if (mainWindow.Logouttime == Properties.Resources.SessionEnded)
            {
                ForceLogon();
            }
            else
            {
                TurnOnAccess(mainWindow);
            }
            CurrentProject.Title = mainWindow.SelectedProjectName;

        }

        private void TurnOnAccess(MainWindow mainWindow)
        {
            ToggleFunctions(Visibility.Visible);
            if (Properties.Settings.Default.LimitFunctions == true)
            {
                ToggleFunctions(Visibility.Collapsed);
            }

            mainWindow.EnableMenuOptions(true);
            if (Open365.Visibility != Visibility.Visible)
            {
                TurnOnMyTerminal();
            }
        }

        private void ForceLogon()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.EnableMenuOptions(false);
            SelectedVMName.Content = Properties.Resources.NotSignedIn;
            SelectedVMName.Background = Brushes.DarkRed;
            SelectedVMName.Foreground = Brushes.White;
            SelectedVMName.FontWeight = FontWeights.Bold;
        }

        private void TurnOnMyTerminal()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            myTerminal = mainWindow.GetMyRDP(Properties.Settings.Default.SelectedPersonalVM);

            if (myTerminal != null)
            {
                SelectedVMName.Background = Brushes.Transparent;
                SelectedVMName.Foreground = Brushes.Black;
                SelectedVMName.FontWeight = FontWeights.Light;

                Open365.Visibility = Visibility.Visible;

                DataContext = myTerminal;
                SelectedVMName.Content = string.Format(Properties.Resources.MyRDPTitle, myTerminal.DisplayName);
                SelectedVMName.Visibility = Visibility.Hidden;
                OpenTerminal.Visibility = Visibility.Visible;
                OpenTerminal.Title = string.Format(Properties.Resources.MyRDPTitle, myTerminal.DisplayName);

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
        private void ToggleFunctions(Visibility visibility)
        {
            if(StartInstance.Visibility != visibility)
            {
                StartInstance.Visibility = visibility;
            }

            if (StopInstance.Visibility != visibility)
            {
                StopInstance.Visibility = visibility;
            }
            if (ShowPwd.Visibility != visibility)
            {
                ShowPwd.Visibility = visibility;
            }
            if (Project.Visibility != visibility)
            {
                Project.Visibility = visibility;
            }
            if (Refresh.Visibility != visibility)
            {
                Refresh.Visibility = visibility;
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
            if (!(DataContext is RDPTerminal viewModel))
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

        private void ShowPwd_Click(object sender, RoutedEventArgs e)
        {
            ShowPasswordsDialog dlg = new ShowPasswordsDialog
            {
                Owner = Application.Current.MainWindow as MainWindow,
                RdpTerminal = myTerminal
            };
            dlg.ShowDialog();
        }

        private void SelectRegion_Click(object sender, RoutedEventArgs e)
        {
            SelectRegionDialog dlg = new SelectRegionDialog
            {
                Owner = Application.Current.MainWindow as MainWindow
            };
            dlg.ShowDialog();
        }
    }
}
