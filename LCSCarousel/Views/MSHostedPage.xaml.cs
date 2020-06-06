using LCSCarousel.Classes;
using LCSCarousel.Enums;
using LCSCarousel.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static LCSCarousel.MainWindow;

namespace LCSCarousel.Views
{
    /// <summary>
    /// Interaction logic for MSHostedPage.xaml
    /// </summary>
    public partial class MSHostedPage : Page
    {
        public MSHostedPage()
        {
            InitializeComponent();
            DataContext = new MSHostedViewModel();
            Carousel.RotationSpeed = Convert.ToInt32(Properties.Settings.Default.RotationSpeed);
            Carousel.SelectionChanged += _carouselRDPTerminals_SelectionChanged;
        }
        private void _carouselRDPTerminals_SelectionChanged(FrameworkElement selectedElement)
        {
            MSHostedViewModel viewModel = DataContext as MSHostedViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.SelectedRDPTerminal = selectedElement.DataContext as Model.RDPTerminal;
        }

        private void _buttonLeftArrow_Click(object sender, RoutedEventArgs e)
        {
            Carousel.RotateRight();
        }

        private void _buttonRightArrow_Click(object sender, RoutedEventArgs e)
        {
            Carousel.RotateLeft();
        }

        private void _buttonLeftManyArrow_Click(object sender, RoutedEventArgs e)
        {
            Carousel.RotateIncrement(-5);
        }

        private void _buttonRightManyArrow_Click(object sender, RoutedEventArgs e)
        {
            Carousel.RotateIncrement(5);
        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            ShowPasswordsDialog dlg = new ShowPasswordsDialog
            {
                Owner = Application.Current.MainWindow as MainWindow,
                MSHostedViewModel = DataContext as MSHostedViewModel
            };

            dlg.ShowDialog();
        }

        private void LogOnToApplication_Click(object sender, RoutedEventArgs e)
        {
            MSHostedViewModel viewModel = DataContext as MSHostedViewModel;
            if (viewModel != null)
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
            MSHostedViewModel viewModel = DataContext as MSHostedViewModel;
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
            using (new WaitCursor())
            {
                CloudHostedViewModel viewModel = DataContext as CloudHostedViewModel;
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
                CloudHostedViewModel viewModel = DataContext as CloudHostedViewModel;
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

        private void EditVMProperties_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MSHostedViewModel;
            EditRDPPropterties editRDPProperties = new EditRDPPropterties(viewModel);
            Navigation.Navigation.Navigate(editRDPProperties);
        }

        private void AddFireWallException_Click(object sender, RoutedEventArgs e)
        {
            MSHostedViewModel viewModel = DataContext as MSHostedViewModel;
            if (viewModel == null)
            {
                return;
            }
            if (viewModel.SelectedRDPTerminal != null)
            {
                var item = viewModel.SelectedRDPTerminal;
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.NewFirewallRule(item.EnvironmentId);
            }
        }

        private void DeployPackage_Click(object sender, RoutedEventArgs e)
        {
            MSHostedViewModel viewModel = DataContext as MSHostedViewModel;
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

        private void RemoveFireWallException_Click(object sender, RoutedEventArgs e)
        {
            MSHostedViewModel viewModel = DataContext as MSHostedViewModel;
            if (viewModel == null)
            {
                return;
            }

            if (viewModel.SelectedRDPTerminal != null)
            {
                var item = viewModel.SelectedRDPTerminal;
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.RemoveFirewallRule(item.EnvironmentId);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
            if (filterValues.Active == true)
            {
                FilterStatus.Content = FindResource("FilterIsActive").ToString();
            }
            else
            {
                FilterStatus.Content = FindResource("NoFilterIsActive").ToString();
            }

        }
        private void ToggleFunctions(bool _enable)
        {
            StartInstance.IsEnabled = _enable;
            StopInstance.IsEnabled  = _enable;
            ShowPassword.IsEnabled = _enable;
            AddFireWallException.IsEnabled = _enable;
            RemoveFireWallException.IsEnabled = _enable;
            DeployPackage.IsEnabled = _enable;
        }
    }
}
