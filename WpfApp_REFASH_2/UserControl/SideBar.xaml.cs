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
    public partial class SideBar : UserControl
    {
        // Define the Customer dependency property
        public static readonly DependencyProperty CustomerProperty = DependencyProperty.Register(
            "Customer", typeof(Customer), typeof(SideBar), new PropertyMetadata(null));
        public Customer Customer
        {
            get => (Customer)GetValue(CustomerProperty);
            set => SetValue(CustomerProperty, value);
        }
        public SideBar()
        {
            InitializeComponent();
            
        }
        
        //public SideBar(Customer customer) : this()
        //{
        //    this.customer = customer;
        //    if (this.customer == null)
        //    {
        //        MessageBox.Show("Customer data is missing.");
        //        return;
        //    }
        //}

        private void btn_navigateNews_click(object sender, RoutedEventArgs e)
        {
            // Use the Customer property directly
            if (Customer == null)
            {
                MessageBox.Show("Customer data is missing.");
                return;
            }

            NewsWindow newsWindow = new NewsWindow(Customer);
            newsWindow.Show();
            Window.GetWindow(this)?.Close();
        }

        private void btn_navigateCollection_click(object sender, RoutedEventArgs e)
        {
            CollectionWindow collectionWindow = new CollectionWindow(Customer);
            collectionWindow.Show();
            Window.GetWindow(this)?.Close();
        }

        private void btn_navigateShop_click(object sender, RoutedEventArgs e)
        {
            if (Customer == null)
            {
                MessageBox.Show("Customer data is missing.");
                return;
            }

            ShopWindow shopWindow = new ShopWindow(Customer);
            shopWindow.Show();
            Window.GetWindow(this)?.Close();
        }
    }
}