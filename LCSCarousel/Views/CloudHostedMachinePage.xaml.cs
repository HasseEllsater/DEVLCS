using LCSCarousel.Classes;
using LCSCarousel.Enums;
using LCSCarousel.Model;
using LCSCarousel.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
            Carousel.RotationSpeed = Convert.ToInt32(Properties.Settings.Default.RotationSpeed);
            Carousel.SelectionChanged += _carouselRDPTerminals_SelectionChanged;
            
        }

        private void _carouselRDPTerminals_SelectionChanged(FrameworkElement selectedElement)
        {
            if (!(DataContext is CloudHostedViewModel viewModel))
            {
                return;
            }

            viewModel.SelectedRDPTerminal = selectedElement.DataContext as RDPTerminal;
        }

        private void _buttonLeftManyArrow_Click(object sender, RoutedEventArgs e)
        {
            Carousel.RotateIncrement(-5);
        }

        private void _buttonRightManyArrow_Click(object sender, RoutedEventArgs e)
        {
            Carousel.RotateIncrement(5);
        }

        private void EditSelectedVM_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as CloudHostedViewModel;
            EditRDPPropterties editRDPProperties = new EditRDPPropterties(viewModel);
            Navigation.Navigation.Navigate(editRDPProperties);

        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {

            ShowPasswordsDialog dlg = new ShowPasswordsDialog
            {
                Owner = Application.Current.MainWindow as MainWindow,
                CloudHostedViewModel = DataContext as CloudHostedViewModel
            };

            dlg.ShowDialog();
        }

        private void LogOnToApplication_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CloudHostedViewModel viewModel)
            {
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
        }

        private void OpenRDP_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is CloudHostedViewModel viewModel))
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
                if (!(DataContext is CloudHostedViewModel viewModel))
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
                if (!(DataContext is CloudHostedViewModel viewModel))
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

            if (Properties.Settings.Default.LimitFunctions == true)
            {
                ToggleFunctions(false);
            }
            else
            {
                ToggleFunctions(true);
            }

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            FilterValues filterValues = mainWindow.GetFilter();
            if(filterValues.Active == true)
            {
                FilterStatus.Content = FindResource("FilterIsActive").ToString();
            }
            else
            {
                FilterStatus.Content = FindResource("NoFilterIsActive").ToString();
            }


        }
        private void ToggleFunctions(bool _enabled)
        {
            StartInstance.IsEnabled = _enabled;
            StopInstance.IsEnabled  = _enabled;
            ShowPassword.IsEnabled  = _enabled;
            DeployPackage.IsEnabled = _enabled;

        }
        private void DeployPackage_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is CloudHostedViewModel viewModel))
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
    }
}
