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
    /// Interaction logic for AdminIntegratedSidebar.xaml
    /// </summary>
    public partial class AdminIntegratedSidebar : UserControl
    {
        private Admin Admin {  get; set; }
        public AdminIntegratedSidebar()
        {
            InitializeComponent();
            Admin = UserSession.CurrentAdmin;
        }

        private void btn_navigateDashboard_click(object sender, RoutedEventArgs e)
        {
            NavigateToSection("AdminDashboardView");
        }

        private void btn_navigateNews_click(object sender, RoutedEventArgs e)
        {
            NavigateToSection("AdminNewsView");
        }

        private void btn_navigateCollection_click(object sender, RoutedEventArgs e)
        {
            NavigateToSection("AdminCollectionView");
        }

        private void btn_navigateShop_click(object sender, RoutedEventArgs e)
        {
            NavigateToSection("AdminShopView");
        }
        private void NavigateToSection(string sectionName)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is AdminIntegratedWindow adminIntegratedWindow)
            {
                switch (sectionName)
                {
                    case "AdminDashboardView":
                        adminIntegratedWindow.SetMainContent(new AdminDashboardView());
                        break;
                    case "AdminNewsView":
                        adminIntegratedWindow.SetMainContent(new AdminNewsView());
                        break;
                    case "AdminCollectionView":
                        adminIntegratedWindow.SetMainContent(new AdminCollectionView()); // Uncomment or fix this line
                        break;
                    case "AdminShopView":
                        adminIntegratedWindow.SetMainContent(new AdminShopView()); // Uncomment or fix this line
                        break;
                    default:
                        MessageBox.Show("Section not found!");
                        break;
                }
            }
            else
            {
                MessageBox.Show("MainWindow is not of type adminIntegratedWindow.");
            }
        }

        private void btn_logout_click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Call the logout method from User class
                Admin?.Logout();

                // Create and show new MainWindow
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                // Close the current window
                Window currentWindow = Window.GetWindow(this);
                currentWindow?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during logout: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
