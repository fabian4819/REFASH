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

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for ShopCartWindow.xaml
    /// </summary>
    public partial class ShopCartWindow : Window
    {
        public Customer Customer { get; private set; }
        public ObservableCollection<Product> CartItems { get; set; }
        public ShopCartWindow(Customer customer)
        {
            InitializeComponent();
            Customer = customer;
            if (Customer == null)
            {
                MessageBox.Show("Customer data is missing.");
                return;
            }
            CartItems = new ObservableCollection<Product>
            {
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 2", "Description 2", "P002", "../Assets/Logo.png", "Rp150.000", "Category B", "M", 5)
            };
            DataContext = this;
        }
        private void btn_deleteFromCart_click(object sender, RoutedEventArgs e)
        {
            var product = (sender as FrameworkElement)?.DataContext as Product;
            if (product != null)
            {
                CartItems.Remove(product);
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
