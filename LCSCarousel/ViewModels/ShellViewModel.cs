using System;
using System.Linq;
using MahApps.Metro.IconPacks;

namespace LCSCarousel.ViewModels
{
    internal class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            // Build the menus
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterialLight() { Kind = PackIconMaterialLightKind.Home}, Text = "Home", NavigationDestination = new Uri("Views/MainPage.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterialLight() { Kind = PackIconMaterialLightKind.Account}, Text = "My RDP", NavigationDestination = new Uri("Views/MyRDPPage.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() {Icon = new PackIconMaterialLight() {Kind = PackIconMaterialLightKind.Cloud}, Text = "Cloud Hosted", NavigationDestination = new Uri("Views/CloudHostedMachinePage.xaml", UriKind.RelativeOrAbsolute)});
            this.Menu.Add(new MenuItem() { Icon = new PackIconSimpleIcons() { Kind = PackIconSimpleIconsKind.MicrosoftAzure}, Text = "MS Hosted", NavigationDestination = new Uri("Views/MSHostedPage.xaml", UriKind.RelativeOrAbsolute) });
            this.OptionsMenu.Add(new MenuItem() { Icon = new PackIconModern() { Kind = PackIconModernKind.Settings }, Text = "Settings", NavigationDestination = new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute) });
            this.OptionsMenu.Add(new MenuItem() { Icon = new PackIconMaterialLight() { Kind = PackIconMaterialLightKind.Information }, Text = "About", NavigationDestination = new Uri("Views/AboutPage.xaml", UriKind.RelativeOrAbsolute) });
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