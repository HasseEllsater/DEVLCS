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
    /// Interaction logic for SelectRefreshOptions.xaml
    /// </summary>
    public partial class SelectRefreshOptions : MetroWindow
    {
        public bool RefreshAllVms { get; set; }
        public bool RefreshCredentials { get; set; }
        public SelectRefreshOptions()
        {
            InitializeComponent();
            this.BorderThickness = new Thickness(0);
            this.BorderBrush = null;
            this.GlowBrush = Brushes.Black;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshAllVms = RefreshAllWM.IsChecked ?? true;
            RefreshCredentials = CacheCredentials.IsChecked ?? true;

            this.Close();
        }
    }
}
