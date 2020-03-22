using LCSCarousel.Classes;
using LCSCarousel.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
    /// Interaction logic for SelectRDPUserDialog.xaml
    /// </summary>
    public partial class SelectRDPUserDialog : MetroWindow
    {
        private RDPConnectionDetails selectedUser;
        private List<RDPConnectionDetails> _rdpList;

        public bool SelectedEnvironment { get; internal set; }

        public void SetRDPList(List<RDPConnectionDetails> RdpList)
        { 
            _rdpList = RdpList;
        }
        public RDPConnectionDetails getSelectedUser()
        {
            return selectedUser;
        }
        public SelectRDPUserDialog()
        {
            InitializeComponent();
            this.BorderThickness = new Thickness(0);
            this.BorderBrush = null;
            this.GlowBrush = Brushes.Black;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RDPConnectionDetailsViewModel viewModel = new RDPConnectionDetailsViewModel(_rdpList);
            //DataContext = viewModel.RDPConnectionDetailsList;
            UserGrid.ItemsSource = viewModel.RDPConnectionDetailsList;
            DefaultUser.Visibility = Visibility.Collapsed;
            if (SelectedEnvironment == true)
            {
                DefaultUser.Visibility = Visibility.Visible;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (UserGrid.SelectedItems.Count == 1)
            {
                if (UserGrid.SelectedItem != null)
                {
                    selectedUser = UserGrid.SelectedItem as RDPConnectionDetails;
                    if(SelectedEnvironment == true)
                    {
                        if (DefaultUser.IsChecked == true)
                        {
                            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                            mainWindow.SetDefaultUser(selectedUser);
                        }
                    }
                    this.Close();
                }
            }
            if (UserGrid.SelectedItems.Count == 0 || UserGrid.SelectedItems.Count > 1)
            {
                return;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    
    }
}
