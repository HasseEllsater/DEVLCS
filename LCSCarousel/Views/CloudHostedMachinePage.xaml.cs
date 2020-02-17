using LCSCarousel;
using LCSCarousel.Model;
using LCSCarousel.ViewModels;
using Microsoft.Win32;
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
    /// Interaction logic for CloudHostedMachinePage.xaml
    /// </summary>
    public partial class CloudHostedMachinePage : Page
    {
        public CloudHostedMachinePage()
        {
            InitializeComponent();
            DataContext = new CloudHostedViewModel();
            _carouselRDPTerminals.SelectionChanged += _carouselRDPTerminals_SelectionChanged;
        }

        private void _carouselRDPTerminals_SelectionChanged(FrameworkElement selectedElement)
        {
            var viewModel = DataContext as CloudHostedViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.SelectedRDPTerminal = selectedElement.DataContext as Model.RDPTerminal;
        }

        private void _buttonLeftManyArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselRDPTerminals.RotateIncrement(-5);
        }

        private void _buttonRightManyArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselRDPTerminals.RotateIncrement(5);
        }

        private void EditSelectedVM_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as CloudHostedViewModel;
            EditRDPPropterties editRDPProperties = new EditRDPPropterties(viewModel);
            Navigation.Navigation.Navigate(editRDPProperties);

        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {

            ShowPasswordsDialog dlg = new ShowPasswordsDialog();
            dlg.Owner = Application.Current.MainWindow as MainWindow;
            dlg.CloudHostedViewModel = DataContext as CloudHostedViewModel;

            dlg.ShowDialog();
        }

        private void LogOnToApplication_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as CloudHostedViewModel;
            if (viewModel == null)
            {
                return;
            }

            if (viewModel.SelectedRDPTerminal != null)
            {
                if (Properties.Settings.Default.MimimizeOnStartRDP == true)
                {
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow.WindowState = WindowState.Minimized;
                }

                var item = viewModel.SelectedRDPTerminal;
                foreach (var link in item.NavigationLinks)
                {
                    if (link.DisplayName == "Log on to environment")
                    {
                        
                        Process.Start(link.NavigationUri);
                        break;
                    }
                }
            }
        }

        private void OpenRDP_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as CloudHostedViewModel;
            if (viewModel == null)
            {
                return;
            }

            if (viewModel.SelectedRDPTerminal != null)
            {
                var item = viewModel.SelectedRDPTerminal;
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.OpenRDPSession(item.EnvironmentId);
                
            }
        }

        private void StartInstance_Click(object sender, RoutedEventArgs e)
        {
            using(new WaitCursor())
            {
                var viewModel = DataContext as CloudHostedViewModel;
                if (viewModel == null)
                {
                    return;
                }

                if (viewModel.SelectedRDPTerminal != null)
                {
                    var item = viewModel.SelectedRDPTerminal;
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow.StartStopRDPSession(item.EnvironmentId, VMAction.Start);
                }
            }
        }

        private void StopInstance_Click(object sender, RoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                var viewModel = DataContext as CloudHostedViewModel;
                if (viewModel == null)
                {
                    return;
                }

                if (viewModel.SelectedRDPTerminal != null)
                {
                    var item = viewModel.SelectedRDPTerminal;
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow.StartStopRDPSession(item.EnvironmentId, VMAction.Stop);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            //RDPTerminal myTerminal = mainWindow.getMyRDP(Properties.Settings.Default.SelectedPersonalVM);

            //if (myTerminal is null) return;
            //var viewModel = DataContext as CloudHostedViewModel;
            //if (viewModel == null)
            //{
            //    return;
            //}
            //viewModel.SelectedRDPTerminal = myTerminal;
            //_carouselRDPTerminals.SelectedItem = viewModel.SelectedRDPTerminal;

        }

        private void DeployPackage_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as CloudHostedViewModel;
            if (viewModel == null)
            {
                return;
            }

            if (viewModel.SelectedRDPTerminal != null)
            {
                var item = viewModel.SelectedRDPTerminal;
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.DeployPackage(item.EnvironmentId);
            }
        }

        private void AvailableHotfix_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as CloudHostedViewModel;
            if (viewModel == null)
            {
                return;
            }

            if (viewModel.SelectedRDPTerminal != null)
            {
                var item = viewModel.SelectedRDPTerminal;
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.FindAvailableHotfixes(item.EnvironmentId);
            }

        }
    }
}
