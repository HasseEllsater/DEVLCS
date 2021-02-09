using System;
using System.Linq;
using MahApps.Metro.IconPacks;

namespace LCSCarousel.ViewModels
{
    internal  class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            // Build the menus
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterialLight() { Kind = PackIconMaterialLightKind.Home}, Text = Properties.Resources.HomeMenu, NavigationDestination = new Uri("Views/MainPage.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterialLight() { Kind = PackIconMaterialLightKind.Cloud }, Text = Properties.Resources.CloudHostedMenu, NavigationDestination = new Uri("Views/CloudHostedMachinePage.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconSimpleIcons() { Kind = PackIconSimpleIconsKind.MicrosoftAzure}, Text = Properties.Resources.MSHosteMenu, NavigationDestination = new Uri("Views/MSHostedPage.xaml", UriKind.RelativeOrAbsolute) });
            this.OptionsMenu.Add(new MenuItem() { Icon = new PackIconModern() { Kind = PackIconModernKind.Settings }, Text = Properties.Resources.SettingsMenu, NavigationDestination = new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute) });
            this.OptionsMenu.Add(new MenuItem() { Icon = new PackIconMaterialLight() { Kind = PackIconMaterialLightKind.Information }, Text = Properties.Resources.AboutMenu, NavigationDestination = new Uri("Views/AboutPage.xaml", UriKind.RelativeOrAbsolute) });
        }

        public object GetItem(object uri)
        {
            return null == uri ? null : this.Menu.FirstOrDefault(m => m.NavigationDestination.Equals(uri));
        }

        public object GetOptionsItem(object uri)
        {
            return null == uri ? null : this.OptionsMenu.FirstOrDefault(m => m.NavigationDestination.Equals(uri));
        }
    }
}