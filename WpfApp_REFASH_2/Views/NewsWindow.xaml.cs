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
    /// Interaction logic for NewsWindow.xaml
    /// </summary>
    public partial class NewsWindow : Window
    {
        public Customer Customer { get; private set; }
        public ObservableCollection<Content> ContentItem { get; set; }
        public NewsWindow(Customer customer)
        {
            InitializeComponent();
            Customer = customer;
            if (Customer == null)
            {
                MessageBox.Show("Customer data is missing.");
                return;
            }
            ContentItem = Customer.GetAllContent();
            foreach (var content in ContentItem)
            {
                content.Customer = Customer; // Assign Customer to each Content item
            }
            DataContext = this;
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

        }
    }
}
