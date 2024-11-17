using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfApp_REFASH.ViewModels;
using WpfApp_REFASH.ViewModels.WpfApp_REFASH.ViewModel;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for NewsWindow.xaml
    /// </summary>
    public partial class NewsWindow : Window
    {
        public Customer Customer { get; private set; }
        public ObservableCollection<Content> ContentItem { get; set; }
        public NewsWindow()
        {
            InitializeComponent();
            
            if (UserSession.CurrentCustomer == null)
            {
                MessageBox.Show("No customer is logged in.");
                Close();
                return;
            }

            Customer = UserSession.CurrentCustomer;
            upperBar.WelcomeName = Customer.Name;

            ContentItem = UserSession.CurrentCustomer.GetAllContent();
            foreach (var content in ContentItem)
            {
                content.Customer = Customer; // Assign Customer to each Content item
            }
            DataContext = this;
            sideBar.Customer = UserSession.CurrentCustomer;
            sideBar.DataContext = new SideBarViewModel(UserSession.CurrentCustomer);
        }

        private void btn_closeApp_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_minimizeApp_click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_navigateNews_click(object sender, RoutedEventArgs e)
        {
            NewsWindow newsWindow = new NewsWindow();
            newsWindow.Show();
            this.Close();
        }
        private void btn_navigateCollection_click(object sender, RoutedEventArgs e)
        {
            CollectionWindow collectionWindow = new CollectionWindow(Customer);
            collectionWindow.Show();
            this.Close();
        }
    }
}
