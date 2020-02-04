using MahApps.Metro.Controls;
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
    /// Interaction logic for RemoveFirewallException.xaml
    /// </summary>
    public partial class RemoveFirewallException : MetroWindow
    {
        public CloudHostedInstance selectedInstance { set; get; }
        private NetworkSecurityGroup NetworkSecurityGroup;
        public NSGRule NSGRule { get; set; }
        public RemoveFirewallException()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            using(new WaitCursor())
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                NetworkSecurityGroup = mainWindow.GetNetworkSecurityGroup(selectedInstance);
                if (NetworkSecurityGroup != null)
                {
                    DataContext = NetworkSecurityGroup.Rules.OrderBy(f => f.Name);
                    RulesGrid.ItemsSource = NetworkSecurityGroup.Rules.OrderBy(f => f.Name);

                }
            }

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (RulesGrid.SelectedItems.Count > 0)
            {
                if (RulesGrid.SelectedItem != null)
                {
                    NSGRule = RulesGrid.SelectedItem as NSGRule;
                    this.Close();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
