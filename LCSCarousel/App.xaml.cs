using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LCSCarousel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ThemeHelper.ChooseTheme();
            Application.Current.MainWindow.Activate();
        }
    }
}
