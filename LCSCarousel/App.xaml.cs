using LCSCarousel.Classes;
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
        //bool checkedStartup = false;
        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ThemeHelper.ChooseTheme();
            Application.Current.MainWindow.Activate();
            //if(checkedStartup == false)
            //{
            //    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            //    mainWindow.CheckStartup();
            //    checkedStartup = true;
            //}

        }
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
    
            switch (e.Exception.Source)
            {
                case "System.Net.Http" when e.Exception.Message == $"Response status code does not indicate success: 498 ().":
                    MessageBox.Show("Please login to LCS again. Your cookie is probably invalid or expired.");
                    break;
                default:
                    MessageBox.Show(e.Exception.Message.ToString());
                    throw e.Exception;
            }
        }
    }
}
