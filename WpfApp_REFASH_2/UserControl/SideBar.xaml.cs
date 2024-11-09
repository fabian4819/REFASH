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
            NewsWindow newsWindow = new NewsWindow();
            newsWindow.Show();
            Window.GetWindow(this)?.Close();
        }

        private void btn_navigateCollection_click(object sender, RoutedEventArgs e)
        {
            CollectionWindow collectionWindow = new CollectionWindow();
            collectionWindow.Show();
            Window.GetWindow(this)?.Close();
        }

        private void btn_navigateShop_click(object sender, RoutedEventArgs e)
        {
            ShopWindow shopWindow = new ShopWindow();
            shopWindow.Show();
            Window.GetWindow(this)?.Close();
        }
    }
}