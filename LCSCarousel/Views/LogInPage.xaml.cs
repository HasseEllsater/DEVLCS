using LCSCarousel.Classes;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LCSCarousel.Views
{
    public enum LoadedFirstPage
    {
        No,
        Yes
    }
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        internal bool Cancelled { get; private set; }
        public LogInPage()
        {
            InitializeComponent();
            WebBrowserHelper.FixBrowserVersion();

        }
        public async void LoggedIn()
        {
            SharedMethods.LogInConfirmation();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Cancelled = true;
            browser.Navigate("https://lcs.dynamics.com");
        }

        private void browser_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri.ToString().StartsWith("https://lcs.dynamics.com/v2"))
            {
                SetSilent(browser, true);
                browser.Visibility = Visibility.Hidden;

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.SetLoggedInUri(e.Uri);
                Cancelled = false;
            }

        }
        private void browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                if (Cancelled == false)
                {
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow.LoadOrSelectProject();
                }

            }
            catch (Exception ex)
            {
                string exception = ex.Message.ToString();
            }

        }
        public static void SetSilent(WebBrowser browser, bool silent)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            // get an IWebBrowser2 from the document
            IOleServiceProvider sp = browser.Document as IOleServiceProvider;
            if (sp != null)
            {
                Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                object webBrowser;
                sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
                if (webBrowser != null)
                {
                    webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { silent });
                }
            }
        }
        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }
    }

}
