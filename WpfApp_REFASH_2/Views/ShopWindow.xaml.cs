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
using System.IO;
using WpfApp_REFASH.ViewModels;
namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for ShopWindow.xaml
    /// </summary>
    public partial class ShopWindow : Window
    {
        public Customer Customer { get; private set; }
        public ObservableCollection<Product> ProductItem { get; set; }

        public ShopWindow(Customer customer)
        {
            InitializeComponent();
            Customer = customer;
            if (Customer == null)
            {
                MessageBox.Show("Customer data is missing.");
                return;
            }
            ProductItem = customer.GetAllProductOffer();
            DataContext = this;
            Customer = UserSession.CurrentCustomer;
            upperBar.WelcomeName = Customer.Name;
        }

        private void btn_cart_click(object sender, RoutedEventArgs e)
        {
            ShopCartWindow shopCartWindow = new ShopCartWindow(Customer);
            shopCartWindow.Show();
            this.Close();
        }
        private void btn_transaction_click(object sender, RoutedEventArgs e)
        {
            ShopTransactionWindow shopTransactionWindow = new ShopTransactionWindow();
            shopTransactionWindow.Show();
            this.Close();
        }
    }
}
