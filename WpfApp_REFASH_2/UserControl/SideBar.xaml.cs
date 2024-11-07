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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_REFASH
{
    public partial class SideBar : UserControl
    {
        public SideBar()
        {
            InitializeComponent();
        }

        private void btn_navigateNews_click(object sender, RoutedEventArgs e)
        {
            // Navigate to News window
        }

        private void btn_navigateCollection_click(object sender, RoutedEventArgs e)
        {
            // Show the CollectionWindow
            CollectionWindow collectionWindow = new CollectionWindow();
            collectionWindow.Show();
        }

        private void btn_navigateShop_click(object sender, RoutedEventArgs e)
        {
            // Navigate to Shop window
        }
    }
}