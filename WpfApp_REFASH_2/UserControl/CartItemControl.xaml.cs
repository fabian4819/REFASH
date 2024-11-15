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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for CartItemControl.xaml
    /// </summary>
    public partial class CartItemControl : UserControl
    {
        public ObservableCollection<Product> CartItems { get; set; }
        public static readonly DependencyProperty CustomerProperty = DependencyProperty.Register(
            "Customer", typeof(Customer), typeof(CartItemControl), new PropertyMetadata(null));

        public Customer Customer
        {
            get { return (Customer)GetValue(CustomerProperty); }
            set { SetValue(CustomerProperty, value); }
        }

        public event Action<Product> RequestRemoveProduct;

        public CartItemControl()
        {
            InitializeComponent();
        }

        private void btn_remove_click(object sender, RoutedEventArgs e)
        {
            var product = this.DataContext as Product;
            if (product != null && Customer != null)
            {
                bool isDeleted = Customer.DeleteFromCart(product.ProductID);
                if (isDeleted)
                {
                    RequestRemoveProduct?.Invoke(product); // Raise event to inform parent
                    MessageBox.Show("Item deleted successfully from the cart.");
                }
                else
                {
                    MessageBox.Show("Failed to delete item from the cart.");
                }
            }
        }
    }
}
