using LCSCarousel.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Converters;
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
    /// Interaction logic for SelectRegionDialog.xaml
    /// </summary>
    public partial class SelectRegionDialog : MetroWindow
    {
        public SelectRegionDialog()
        {
            InitializeComponent();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LCSUrlsViewModel lCSUrlsViewModel = this.DataContext as LCSUrlsViewModel;
            lCSUrlsViewModel.LoadUrlsFromSettings();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //Make the user confirm the region change, show the value of Properties.Settings.Default.SelectedRegion in the message box
            LCSUrlsViewModel lCSUrlsViewModel = this.DataContext as LCSUrlsViewModel;
            string message = string.Format(Properties.Resources.ConfirmRegionChangeMessage, Properties.Settings.Default.SelectedRegion, lCSUrlsViewModel.SelectedItem.Region);

            MessageBoxResult result = MessageBox.Show(message, Properties.Resources.ConfirmRegionChange, MessageBoxButton.YesNo,MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                lCSUrlsViewModel.SetSelectedRegion();
                Properties.Settings.Default.LoggedInUri = lCSUrlsViewModel.SelectedItem.Url;
                Properties.Settings.Default.Save();
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.ClearAndClose();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            LCSUrlsViewModel lCSUrlsViewModel = this.DataContext as LCSUrlsViewModel;
            lCSUrlsViewModel.SaveUrlsToSettings();

        }
    }
}
