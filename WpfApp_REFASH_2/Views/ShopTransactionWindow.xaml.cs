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

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for ShopTransactionWindow.xaml
    /// </summary>
    public partial class ShopTransactionWindow : Window
    {
        public ObservableCollection<Transaction> Transactions { get; set; }
        private Customer Customer { get; set; }

        public ShopTransactionWindow()
        {
            InitializeComponent();

            // Assuming `UserSession.CurrentCustomer` gives the logged-in customer object
            Customer = UserSession.CurrentCustomer;

            // Fetch all transactions for the logged-in customer
            Transactions = Customer.GetAllTransactions();

            // Group Transactions by OrderID
            var collectionView = CollectionViewSource.GetDefaultView(Transactions);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("OrderID"));

            // Bind grouped transactions to the DataContext
            DataContext = collectionView;
        }

        private void btn_back_click(object sender, RoutedEventArgs e)
        {
            ShopWindow shopWindow = new ShopWindow(Customer);
            shopWindow.Show();
            this.Close();
        }

        private void btn_checkout_click(object sender, RoutedEventArgs e)
        {
            // Checkout logic (if needed)
            MessageBox.Show("Checkout process initiated!");
        }
    }
}
