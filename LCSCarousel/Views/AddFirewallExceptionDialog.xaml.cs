using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace LCSCarousel.Views
{
    /// <summary>
    /// Interaction logic for AddFirewallExceptionDialog.xaml
    /// </summary>
    public partial class AddFirewallExceptionDialog : MetroWindow
    {
        public CloudHostedInstance selectedInstance { set; get; }
        public AddFirewallExceptionDialog()
        {
            InitializeComponent();
            this.BorderThickness = new Thickness(0);
            this.BorderBrush = null;
            this.GlowBrush = Brushes.Black;
            Address.Text = GetPublicIpAddress();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ValidateAndClose();
            this.Close();
        }
        private string GetPublicIpAddress()
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri("http://ifconfig.me"));

            request.UserAgent = "curl"; // this will tell the server to return the information as if the request was made by the linux "curl" command

            string publicIPAddress;

            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    publicIPAddress = reader.ReadToEnd();
                }
            }

            return publicIPAddress.Replace("\n", "");
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public async void ValidateAndClose()
        {
            if(ValidateInput() == false)
            {
                this.Hide();
                var res = await InfoBox.ShowMessageAsync(Properties.Resources.ConfirmAction, Properties.Resources.FirewallRuleNotCorrect, MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(true);
                if (res == MessageDialogResult.Negative)
                {
                    this.Close();
                    return;
                }
                this.Show();
                return;
            }

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.AddFirewallRule(selectedInstance, Name.Text, Address.Text);
        }
        private bool ValidateInput()
        {
            bool retVal = true;
            if(String.IsNullOrEmpty(Name.Text) || String.IsNullOrEmpty(Address.Text))
            {
                retVal = false;
            }
            return retVal;
        }
    }
}
