using LCSCarousel.Enums;
using LCSCarousel.Model;
using LCSCarousel.ViewModels;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
        int min, max, count;
        public CloudHostedMachinePage()
        {
            InitializeComponent();
            DataContext = new CloudHostedViewModel();
            Carousel.RotationSpeed = Convert.ToInt32(Properties.Settings.Default.RotationSpeed);
            Carousel.SelectionChanged += _carouselRDPTerminals_SelectionChanged;
            SetDefaultRotateIncrement();
        }
        private void SetDefaultRotateIncrement()
        {
            CloudHostedViewModel viewModel = DataContext as CloudHostedViewModel;
            count = viewModel.NumberOfMachines;
            if (count > 1)
            {
                max = count / 2;
                min = max * -1;
                if (max > 0)
                {
                    MaxRotate.Text = max.ToString();
                }
                CurrentMaxRotation.Content = string.Format(Properties.Resources.MaxRotationInc, max.ToString());
            }

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
            Carousel.RotateIncrement(min);
        }

        private void _buttonRightManyArrow_Click(object sender, RoutedEventArgs e)
        {
            Carousel.RotateIncrement(max);
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
 
        }
        


        private void MaxRotate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(MaxRotate.Text, "  ^ [0-9]"))
            {
                SetDefaultRotateIncrement();
            }

            if (String.IsNullOrEmpty(MaxRotate.Text))
            {
                SetDefaultRotateIncrement();
            }
            else
            {
                max = Convert.ToInt32(MaxRotate.Text);
                if (max > 0 && max <= count)
                {
                    min = max * -1;
                    MaxRotate.Text = max.ToString();
                }
                else
                {
                    SetDefaultRotateIncrement();
                }
            }
        }

        private void MaxRotate_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ToggleFunctions(bool _enabled)
        {
            StartInstance.IsEnabled = _enabled;
            StopInstance.IsEnabled  = _enabled;
            ShowPassword.IsEnabled  = _enabled;
        }
    }
}
