using LCSCarousel.Model;
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
    /// Interaction logic for EditRDPPropterties.xaml
    /// </summary>
    public partial class EditRDPPropterties : Page
    {
        RDPTerminal selectedTerminal;

        public EditRDPPropterties()
        {
            InitializeComponent();
        }
        public EditRDPPropterties(CloudHostedViewModel viewModel) : this()
        {
            DataContext = viewModel as CloudHostedViewModel;
            this.Loaded += new RoutedEventHandler(EditRDPProperties_Loaded);
        }
        public EditRDPPropterties(MSHostedViewModel viewModel) : this()
        {
            DataContext = viewModel as MSHostedViewModel;
            this.Loaded += new RoutedEventHandler(EditRDPProperties_Loaded);
        }

        private void EditRDPProperties_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CloudHostedViewModel)
            {
                var viewModel = DataContext as CloudHostedViewModel;
                if (viewModel == null)
                {
                    return;
                }

                if (viewModel.SelectedRDPTerminal.ImageSource == Properties.Settings.Default.DefaultImage)
                {
                    SamplePicture.Source = new BitmapImage(new Uri(Properties.Settings.Default.DefaultImage, UriKind.Relative));
                }
                else
                {
                    Uri fileUri = new Uri(viewModel.SelectedRDPTerminal.ImageSource);
                    SamplePicture.Source = new BitmapImage(fileUri);
                }

                selectedTerminal = viewModel.SelectedRDPTerminal;
            }

            if (DataContext is MSHostedViewModel)
            {
                var viewModel = DataContext as MSHostedViewModel;
                if (viewModel == null)
                {
                    return;
                }
                if (viewModel.SelectedRDPTerminal.ImageSource == Properties.Settings.Default.DefaultImage)
                {
                    SamplePicture.Source = new BitmapImage(new Uri(Properties.Settings.Default.DefaultImage, UriKind.Relative));
                }
                else
                {
                    Uri fileUri = new Uri(viewModel.SelectedRDPTerminal.ImageSource);
                    SamplePicture.Source = new BitmapImage(fileUri);
                }
                selectedTerminal = viewModel.SelectedRDPTerminal;
            }
        }

        private void PickVMAvatar_Click(object sender, RoutedEventArgs e)
        {

            if (DataContext is CloudHostedViewModel)
            {
                var viewModel = DataContext as CloudHostedViewModel;
                if (viewModel == null)
                {
                    return;
                }
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    Uri fileUri = new Uri(openFileDialog.FileName);
                    viewModel.ChangeImage(fileUri.ToString());
                    SamplePicture.Source = new BitmapImage(fileUri);
                }
            }

            if (DataContext is MSHostedViewModel)
            {
                var viewModel = DataContext as MSHostedViewModel;
                if (viewModel == null)
                {
                    return;
                }
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    Uri fileUri = new Uri(openFileDialog.FileName);
                    viewModel.ChangeImage(fileUri.ToString());
                    SamplePicture.Source = new BitmapImage(fileUri);
                }
            }
        }
        private void ResetPicture_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CloudHostedViewModel)
            {
                var viewModel = DataContext as CloudHostedViewModel;
                if (viewModel == null)
                {
                    return;
                }

                viewModel.ResetImage();
                SamplePicture.Source = new BitmapImage(new Uri(Properties.Settings.Default.DefaultImage, UriKind.Relative));
                selectedTerminal = viewModel.SelectedRDPTerminal;
            }

            if (DataContext is MSHostedViewModel)
            {
                var viewModel = DataContext as MSHostedViewModel;
                if (viewModel == null)
                {
                    return;
                }

                viewModel.ResetImage();
                SamplePicture.Source = new BitmapImage(new Uri(Properties.Settings.Default.DefaultImage, UriKind.Relative));
                selectedTerminal = viewModel.SelectedRDPTerminal;
            }
        }
    }
}
