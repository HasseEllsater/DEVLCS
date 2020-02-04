using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LCSCarousel.Classes
{
    public class SharedMethods
    {
        public async static void ConfirmLogout()
        {
            var res = await InfoBox.ShowMessageAsync(Properties.Resources.ConfirmAction, Properties.Resources.ConfirmLogOut, MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(true);
            if (res == MessageDialogResult.Affirmative)
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.goToLogoutPage();
            }
        }
        public async static void LogInConfirmation()
        {
            await InfoBox.ShowMessageAsync(Properties.Resources.LoggInStatus, Properties.Resources.LoggedIn, MessageDialogStyle.Affirmative).ConfigureAwait(true);
        }
        public async static void NotLoggedIn()
        {
            var res = await InfoBox.ShowMessageAsync(Properties.Resources.ConfirmAction, Properties.Resources.NotLoggedIn, MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(true);
            if (res == MessageDialogResult.Affirmative)
            {
                Navigation.Navigation.Navigate(new Uri("Views/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
        public async static void StopDialog(string _title,string _message)
        {
            await InfoBox.ShowMessageAsync(_title, _message, MessageDialogStyle.Affirmative).ConfigureAwait(true);
        }
    }
}
