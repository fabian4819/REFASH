using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for ShopCartWindow.xaml
    /// </summary>
    public partial class ShopCartWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Customer Customer { get; private set; }
        private ObservableCollection<Product> _cartItems;
        public ObservableCollection<Product> CartItems
        {
            get { return _cartItems; }
            set
            {
                if (_cartItems != value)
                {
                    _cartItems = value;
                    OnPropertyChanged(nameof(CartItems));
                }
            }
        }
        private void ReloadCartItems()
        {
            CartItems = new ObservableCollection<Product>(Customer.GetAllProductCart());
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ShopCartWindow(Customer customer)
        {
            InitializeComponent();
            Customer = UserSession.CurrentCustomer;
            //ViewModel = new MainViewModel(customer);
            if (Customer == null)
            {
                MessageBox.Show("Customer data is missing.");
                return;
            }
            CartItems = Customer.GetAllProductCart();
            DataContext = this;
        }

        private void btn_deleteFromCart_click(object sender, RoutedEventArgs e)
        {
            var product = (sender as FrameworkElement)?.DataContext as Product;
            if (product != null)
            {
                Customer.DeleteFromCart(product.ProductID);
                ReloadCartItems();
            }
        }

        private void btn_checkout_click(object sender, RoutedEventArgs e)
        {
            // Implement checkout functionality here
            MessageBox.Show("Proceeding to checkout");
        }

        private void btn_back_click(object sender, RoutedEventArgs e)
        {
            ShopWindow shopWindow = new ShopWindow(Customer);
            shopWindow.Show();
            this.Close();
        }
    }
}
