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
    /// <summary>
    /// Interaction logic for AdminSideBar.xaml
    /// </summary>
    public partial class AdminSideBar : UserControl
    {
        public AdminSideBar()
        {
            InitializeComponent();
        }

        private void btn_navigateDashboard_click(object sender, RoutedEventArgs e)
        {
            //AdminDashboardWindow dashboardWindow = new AdminDashboardWindow();
            //dashboardWindow.Show();
            //Window.GetWindow(this)?.Close();
        }

        private void btn_navigateNews_click(object sender, RoutedEventArgs e)
        {
            //AdminNewsWindow newsWindow = new AdminNewsWindow();
            //newsWindow.Show();
            //Window.GetWindow(this)?.Close();
        }

        private void btn_navigateCollection_click(object sender, RoutedEventArgs e)
        {
            AdminCollectionWindow collectionWindow = new AdminCollectionWindow();
            collectionWindow.Show();
            Window.GetWindow(this)?.Close();
        }

        private void btn_navigateShop_click(object sender, RoutedEventArgs e)
        {
            AdminShopWindow shopWindow = new AdminShopWindow();
            shopWindow.Show();
            Window.GetWindow(this)?.Close();
        }
    }
}
