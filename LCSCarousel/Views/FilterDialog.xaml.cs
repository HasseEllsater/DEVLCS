using LCSCarousel.Classes;
using LCSCarousel.Enums;
using LCSCarousel.Model;
using LCSCarousel.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyFilter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            bool activate = ActivateFilter.IsChecked ?? false;
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (activate == true && Status.SelectedItem != null && Release.SelectedItem != null && PlatformRelease.SelectedItem != null)
            {

                EnvironmentState environmentstate = Status.SelectedItem as EnvironmentState;
                ReleaseInformation releaseInformation = Release.SelectedItem as ReleaseInformation;
                PlatformReleaseInformation platformReleaseInformation = PlatformRelease.SelectedItem as PlatformReleaseInformation;
                mainWindow.SetFilter(activate, environmentstate, releaseInformation, platformReleaseInformation, CloudEnvironment.CloudHosted);
            }
            else
            {
                mainWindow.SetFilter(activate, null, null, null,CloudEnvironment.CloudHosted);
            }
            activate = MicrosoftHostedActivateFilter.IsChecked ?? false;
            if (activate == true && MicrosoftHostedStatus.SelectedItem != null && MicrosoftHostedRelease.SelectedItem != null && MicrosoftHostedPlatformRelease.SelectedItem != null)
            {
                EnvironmentState environmentstate = MicrosoftHostedStatus.SelectedItem as EnvironmentState;
                ReleaseInformation releaseInformation = MicrosoftHostedRelease.SelectedItem as ReleaseInformation;
                PlatformReleaseInformation platformReleaseInformation = MicrosoftHostedPlatformRelease.SelectedItem as PlatformReleaseInformation;
                mainWindow.SetFilter(activate, environmentstate, releaseInformation, platformReleaseInformation, CloudEnvironment.MicrosoftHosted);
            }
            else
            {
                mainWindow.SetFilter(activate, null, null, null, CloudEnvironment.MicrosoftHosted);
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

            MicrosoftHostedStatus.ItemsSource = environmentstate;
            MicrosoftHostedRelease.ItemsSource = releaseinfo;
            MicrosoftHostedPlatformRelease.ItemsSource = platformrelease;

            FilterValues filterValues = mainWindow.GetFilter(CloudEnvironment.CloudHosted);

            if(filterValues.Active == true)
            {
                Status.SelectedItem = environmentstate.FirstOrDefault(x => x.StateDescription.Equals(filterValues.environmentState.StateDescription));
                Release.SelectedItem = releaseinfo.FirstOrDefault(x => x.Release.Equals(filterValues.releaseInformation.Release));
                PlatformRelease.SelectedItem = platformrelease.FirstOrDefault(x => x.PlatformRelease.Equals(filterValues.platformReleaseInformation.PlatformRelease));
                ActivateFilter.IsChecked = true;
            }
            filterValues = mainWindow.GetFilter(CloudEnvironment.MicrosoftHosted);
            if (filterValues.Active == true)
            {
                MicrosoftHostedStatus.SelectedItem = environmentstate.FirstOrDefault(x => x.StateDescription.Equals(filterValues.environmentState.StateDescription));
                MicrosoftHostedRelease.SelectedItem = releaseinfo.FirstOrDefault(x => x.Release.Equals(filterValues.releaseInformation.Release));
                MicrosoftHostedPlatformRelease.SelectedItem = platformrelease.FirstOrDefault(x => x.PlatformRelease.Equals(filterValues.platformReleaseInformation.PlatformRelease));
                MicrosoftHostedActivateFilter.IsChecked = true;
            }
        }
    }
}
