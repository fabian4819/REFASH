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
namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for ShopWindow.xaml
    /// </summary>
    public partial class ShopWindow : Window
    {
        public ObservableCollection<Product> ProductItem { get; set; }

        public ShopWindow()
        {
            InitializeComponent();
            ProductItem = new ObservableCollection<Product>
            {
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 2", "Description 2", "P002", "../Assets/Logo.png", "Rp150.000", "Category B", "M", 5)
            };

            DataContext = this; // Set DataContext to enable binding
        }

        private void btn_cart_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
