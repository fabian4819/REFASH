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
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for IntegratedSidebar.xaml
    /// </summary>
    public partial class IntegratedSidebar : UserControl
    {
        private Customer Customer {  get; set; }
        public IntegratedSidebar()
        {
            InitializeComponent();
            Customer = UserSession.CurrentCustomer;
            
        }
        // Navigasi ke halaman "News"
        private void btn_navigateNews_click(object sender, RoutedEventArgs e)
        {
            NavigateToSection("NewsView");
        }

        // Navigasi ke halaman "Collection"
        private void btn_navigateCollection_click(object sender, RoutedEventArgs e)
        {
            NavigateToSection("CollectionView");
        }

        // Navigasi ke halaman "Shop"
        private void btn_navigateShop_click(object sender, RoutedEventArgs e)
        {
            NavigateToSection("ShopView");
        }

        // Fungsi untuk mengganti konten utama
        private void NavigateToSection(string sectionName)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is IntegratedWindows integratedWindows)
            {
                switch (sectionName)
                {
                    case "NewsView":
                        integratedWindows.SetMainContent(new CustomerNewsView());
                        break;
                    case "CollectionView":
                        integratedWindows.SetMainContent(new CustomerCollectionView());
                        break;
                    case "ShopView":
                        integratedWindows.SetMainContent(new CustomerShopView()); // Uncomment or fix this line
                        break;
                    default:
                        MessageBox.Show("Section not found!");
                        break;
                }
            }
            else
            {
                MessageBox.Show("MainWindow is not of type IntegratedWindows.");
            }
        }


        private void btn_logout_click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You have logged out.");
            Application.Current.Shutdown();
        }
    }
}
