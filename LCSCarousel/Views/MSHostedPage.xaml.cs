using LCSCarousel.ViewModels;
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

            _carouselRDPTerminals.SelectionChanged += _carouselRDPTerminals_SelectionChanged;
        }
        private void _carouselRDPTerminals_SelectionChanged(FrameworkElement selectedElement)
        {
            var viewModel = DataContext as MSHostedViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.SelectedRDPTerminal = selectedElement.DataContext as Model.RDPTerminal;
        }

        private void _buttonLeftArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselRDPTerminals.RotateRight();
        }

        private void _buttonRightArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselRDPTerminals.RotateLeft();
        }

        private void _buttonLeftManyArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselRDPTerminals.RotateIncrement(-5);
        }

        private void _buttonRightManyArrow_Click(object sender, RoutedEventArgs e)
        {
            _carouselRDPTerminals.RotateIncrement(5);
        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            ShowPasswordsDialog dlg = new ShowPasswordsDialog();
            dlg.Owner = Application.Current.MainWindow as MainWindow;
            dlg.MSHostedViewModel = DataContext as MSHostedViewModel;

            dlg.ShowDialog();
        }

        private void LogOnToApplication_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MSHostedViewModel;
            if (viewModel == null)
            {
                return;
            }

            if (viewModel.SelectedRDPTerminal != null)
            {
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
            var viewModel = DataContext as MSHostedViewModel;
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

        private void EditVMProperties_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MSHostedViewModel;
            EditRDPPropterties editRDPProperties = new EditRDPPropterties(viewModel);
            Navigation.Navigation.Navigate(editRDPProperties);
        }

        private void AddFireWallException_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MSHostedViewModel;
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

        }

        private void AvailableHotfix_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveFireWallException_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MSHostedViewModel;
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
    }
}
