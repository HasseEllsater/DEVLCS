using LCSCarousel.Classes;
using LCSCarousel.ViewModels;
using Microsoft.Win32;
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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private List<CloudHostedInstance> allWMS;
        private string environmentId;


        public SettingsPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }
  
        private void vmCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vmCombo.SelectedIndex != -1)
            {
                CloudHostedInstance cloudHostedInstance = vmCombo.SelectedItem as CloudHostedInstance;
                if (cloudHostedInstance != null)
                {
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow.SetPersonalVM(cloudHostedInstance.EnvironmentId);
                    if(cloudHostedInstance.EnvironmentId != environmentId)
                    {
                        ClearDefaultUser_Click(null, null);
                    }
                }
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            allWMS = mainWindow.AllWMs;
            vmCombo.ItemsSource = allWMS;

            CloudHostedInstance personalInstance = null;

            if(!string.IsNullOrEmpty(Properties.Settings.Default.SelectedPersonalVM))
            {
                foreach (var item in allWMS)
                {
                    if (item.EnvironmentId == Properties.Settings.Default.SelectedPersonalVM)
                    {
                        personalInstance = item;
                        break;
                    }
                }
                if(personalInstance != null)
                {
                    environmentId = personalInstance.EnvironmentId;
                    vmCombo.SelectedItem = personalInstance;
                }
            }
            MinimizeSetting.IsChecked = Properties.Settings.Default.MimimizeOnStartRDP;
            Span.IsChecked = Properties.Settings.Default.Span;
            Resolution.Value = Properties.Settings.Default.Resolution;
            Multimon.IsChecked = Properties.Settings.Default.Multimon;

            RDPConnectionDetails connectionDetails = mainWindow.GetDefaultUser();
            if (connectionDetails == null)
            {
                ClearDefaultUser.IsEnabled = false;
                UserName.Content = Properties.Resources.NoDefaultUser;
            }
            else
            {
                ClearDefaultUser.IsEnabled = true;
                UserName.Content = string.Format(Properties.Resources.ClearDefaultUserTitle,connectionDetails.Username);
            }

            RotationSpeed.Text = Properties.Settings.Default.RotationSpeed;

        }
      

        private void MinimizeSetting_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.MimimizeOnStartRDP = MinimizeSetting.IsChecked ?? false;
            Properties.Settings.Default.Save();
        }
        private void setResolution(double value)
        {
            switch (value)
            {
                case 0:
                    SelectedResolution.Content = Properties.Resources._640by480;
                    break;

                case 1:
                    SelectedResolution.Content = Properties.Resources._800by600;
                    break;

                case 2:
                    SelectedResolution.Content = Properties.Resources._1024by768;
                    break;

                case 3:
                    SelectedResolution.Content = Properties.Resources._1280by720;
                    break;

                case 4:
                    SelectedResolution.Content = Properties.Resources._1280by768;
                    break;

                case 5:
                    SelectedResolution.Content = Properties.Resources._1280by800;
                    break;

                case 6:
                    SelectedResolution.Content = Properties.Resources._1280by1024;
                    break;

                case 7:
                    SelectedResolution.Content = Properties.Resources._1366by768;
                    break;

                case 8:
                    SelectedResolution.Content = Properties.Resources._1440by900;
                    break;

                case 9:
                    SelectedResolution.Content = Properties.Resources._1400by1050;
                    break;

                case 10:
                    SelectedResolution.Content = Properties.Resources._1600by1200;
                    break;

                case 11:
                    SelectedResolution.Content = Properties.Resources._1680by1050;
                    break;

                case 12:
                    SelectedResolution.Content = Properties.Resources._1920by1080;
                    break;

                case 13:
                    SelectedResolution.Content = Properties.Resources._1920by1200;
                    break;

                case 14:
                    SelectedResolution.Content = Properties.Resources._2560by1440;
                    break;

                case 15:
                    SelectedResolution.Content = Properties.Resources._3440by1440;
                    break;

                case 16:
                    SelectedResolution.Content = Properties.Resources.FullScreen;
                    break;
                default:
                    break;
            }
            Properties.Settings.Default.Resolution = value;
            Properties.Settings.Default.Save();

        }
        private void Resolution_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = e.NewValue;
            setResolution(value);
        }

        private void Resolution_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            setResolution(Resolution.Value);

        }

        private void Span_Click(object sender, RoutedEventArgs e)
        {
            bool spanChecked = Span.IsChecked ?? false;

            if(spanChecked == true)
            {
                Resolution.Value = 16;
                setResolution(16.00);
                Resolution.IsEnabled = false;
                Multimon.IsChecked = false;
            }
            else
            {
                Resolution.IsEnabled = true;
            }

            Properties.Settings.Default.Span = spanChecked;
            Properties.Settings.Default.Save();
        }

        private void Multimon_Click(object sender, RoutedEventArgs e)
        {
            bool multiMonChecked = Multimon.IsChecked ?? false;

            if (multiMonChecked == true)
            {
                Resolution.Value = 16;
                setResolution(16.00);
                Resolution.IsEnabled = false;
                Span.IsChecked = false;
            }
            else
            {
                Resolution.IsEnabled = true;
            }
            Properties.Settings.Default.Multimon = multiMonChecked;
            Properties.Settings.Default.Save();

        }

        private void ClearDefaultUser_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DefaultUser = "";
            Properties.Settings.Default.Save();
            ClearDefaultUser.IsEnabled = false;
            UserName.Content = Properties.Resources.NoDefaultUser;
        }

        private void RotationSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.RotationSpeed = RotationSpeed.Text;
            Properties.Settings.Default.Save();
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            FilterDialog dlg = new FilterDialog()
            {
                Owner = Application.Current.MainWindow as MainWindow
            };

            dlg.ShowDialog();
        }
    }
}
