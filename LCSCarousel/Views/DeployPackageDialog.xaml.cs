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
    /// Interaction logic for DeployPackageDialog.xaml
    /// </summary>
    public partial class DeployPackageDialog : MetroWindow
    {
        private List<DeployablePackage> Packages;
        internal DeployablePackage DeployablePackage { get; private set; }
         public DeployPackageDialog()
        {
            InitializeComponent();
            this.BorderThickness = new Thickness(0);
            this.BorderBrush = null;
            this.GlowBrush = Brushes.Black;
        }
        public void SetPackagesList(List<DeployablePackage> _packagesList)
        {
            Packages = _packagesList;
            DataContext = Packages;
            PackagesGrid.ItemsSource = Packages.OrderBy(f => f.ModifiedDate);
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DeployPackage_Click(object sender, RoutedEventArgs e)
        {
            if (PackagesGrid.SelectedItems.Count > 0)
            {
                if (PackagesGrid.SelectedItem != null)
                {
                    DeployablePackage = PackagesGrid.SelectedItem as DeployablePackage;
                    this.Close();
                }
            }

        }
    }
}
