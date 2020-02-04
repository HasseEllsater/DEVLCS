using LCSCarousel.Model;
using LCSCarousel.ViewModels;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace LCSCarousel.Views
{
    /// <summary>
    /// Interaction logic for ShowPasswordsPage.xaml
    /// </summary>
    public partial class ShowPasswordsDialog : MetroWindow
    {
        public CloudHostedViewModel CloudHostedViewModel { set; get; }
        public MSHostedViewModel MSHostedViewModel { set; get; }
        public RDPTerminal  rdpTerminal { get; set; }
        public ShowPasswordsDialog()
        {
            InitializeComponent();
            this.BorderThickness = new Thickness(0);
            this.BorderBrush = null;
            this.GlowBrush = Brushes.Black;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            using(new WaitCursor())
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (MSHostedViewModel != null)
                {
                    var viewModel = MSHostedViewModel;
                    if (viewModel.SelectedRDPTerminal != null)
                    {
                        DataContext = new CredentialsViewModel(MSHostedViewModel);
                    }
                }
                if (CloudHostedViewModel != null)
                {
                    var viewModel = CloudHostedViewModel;
                    if (viewModel.SelectedRDPTerminal != null)
                    {
                        DataContext = new CredentialsViewModel(CloudHostedViewModel);
                    }
                }
                if (rdpTerminal != null)
                {
                    DataContext = new CredentialsViewModel(rdpTerminal);
                }

            }

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CopyPwd_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordsGrid.SelectedItems.Count > 0)
            {
                if (PasswordsGrid.SelectedItem != null)
                {
                    UserCredentials userCred = PasswordsGrid.SelectedItem as UserCredentials;
                    Clipboard.SetText(userCred.Password);
                }
            }
        }

        private void CopyUser_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordsGrid.SelectedItems.Count > 0)
            {
                if (PasswordsGrid.SelectedItem != null)
                {
                    UserCredentials userCred = PasswordsGrid.SelectedItem as UserCredentials;
                    Clipboard.SetText(userCred.UserId);
                }
            }
        }
    }
}
