using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for ShopCard.xaml
    /// </summary>

    public partial class ShopCard : UserControl
    {
        private Customer Customer = UserSession.CurrentCustomer;
        
        public ShopCard()
        {
            InitializeComponent();
            if (Customer == null)
            {
                MessageBox.Show("Shop Card Customer is null");
            }
        }

        private void btn_addToCart_click(object sender, RoutedEventArgs e)
        {
            var product = this.DataContext as Product;
            if (product != null)
            {
                if (!int.TryParse(tb_qty.Text, out int quantity))
                {
                    MessageBox.Show("Fill the quantity.");
                    return; // Early exit if parsing fails
                }

                if (Customer == null)
                {
                    MessageBox.Show("Customer is null");
                    return; // Early exit if customer is not set
                }
                quantity = int.Parse(tb_qty.Text);
                if (Customer == null)
                {
                    MessageBox.Show("Customer is null");
                    return;
                }
                Customer.AddToCart(product.ProductID, quantity);
                MessageBox.Show($"Adding {quantity} of product ID {product.ProductID} to the cart. by {Customer.Name}");
            }
            else
            {
                MessageBox.Show("Product data is not available.");
            }
        }
    }
}
