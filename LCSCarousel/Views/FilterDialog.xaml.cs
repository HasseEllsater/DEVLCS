using LCSCarousel.Classes;
using LCSCarousel.Model;
using LCSCarousel.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LCSCarousel.Views
{
    /// <summary>
    /// Interaction logic for FilterDialog.xaml
    /// </summary>
    public partial class FilterDialog : MetroWindow
    {
        public FilterDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            this.BorderThickness = new Thickness(0);
            this.BorderBrush = null;
            this.GlowBrush = Brushes.Black;
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyFilter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            bool activate = ActivateFilter.IsChecked ?? false;
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            bool ok = true;
            if (activate == true && Status.SelectedIndex == -1)
            {
                ok = false;
            }

            if (activate == true && Release.SelectedIndex == -1)
            {
                ok = false;
            }
            if (activate == true && PlatformRelease.SelectedIndex == -1)
            {
                ok = false;
            }

            if (ok == false)
            {
                SharedMethods.StopDialog(Properties.Resources.FilterDialogInformation, Properties.Resources.FilterNotComplete);
                mainWindow.SetFilter(false, null, null, null);
            }

            if(ok == true && activate == true)
            {
                EnvironmentState environmentstate = Status.SelectedItem as EnvironmentState;
                ReleaseInformation releaseInformation = Release.SelectedItem as ReleaseInformation;
                PlatformReleaseInformation platformReleaseInformation = PlatformRelease.SelectedItem as PlatformReleaseInformation;
                mainWindow.SetFilter(activate, environmentstate, releaseInformation, platformReleaseInformation);
            }

            if (activate == false)
            {
                SharedMethods.StopDialog(Properties.Resources.FilterDialogInformation, Properties.Resources.FilterNotActive);
                mainWindow.SetFilter(false, null, null, null);
            }

 
            this.Close();
        }

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            List<EnvironmentState> environmentstate = mainWindow.GetEnvironmentStatesList();
            List<ReleaseInformation> releaseinfo =  mainWindow.GetReleaseInformation();
            List<PlatformReleaseInformation> platformrelease = mainWindow.GetPlatformReleaseInformation();

            Status.ItemsSource = environmentstate;
            Release.ItemsSource = releaseinfo;
            PlatformRelease.ItemsSource = platformrelease;

            FilterValues filterValues = mainWindow.GetFilter();

            ActivateFilter.IsChecked = filterValues.Active;

            if (filterValues.Active == true)
            {
                Status.SelectedItem = environmentstate.FirstOrDefault(x => x.StateDescription.Equals(filterValues.environmentState.StateDescription));
                Release.SelectedItem = releaseinfo.FirstOrDefault(x => x.Release.Equals(filterValues.releaseInformation.Release));
                PlatformRelease.SelectedItem = platformrelease.FirstOrDefault(x => x.PlatformRelease.Equals(filterValues.platformReleaseInformation.PlatformRelease));
            }


        }
    }
}
