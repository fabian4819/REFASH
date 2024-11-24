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
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for CustomerShopTransaction.xaml
    /// </summary>
    public partial class CustomerShopTransaction : UserControl
    {
        public ObservableCollection<Transaction> Transactions { get; set; }
        private Customer Customer { get; set; }
        public CustomerShopTransaction()
        {
            InitializeComponent();
            Customer = UserSession.CurrentCustomer;
            Transactions = Customer.GetAllTransactions();
            var collectionView = CollectionViewSource.GetDefaultView(Transactions);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("OrderID"));
            DataContext = collectionView;
        }
        private void btn_back_click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is IntegratedWindows integratedWindows)
            {
                integratedWindows.SetMainContent(new CustomerShopView());
            }
            else
            {
                MessageBox.Show("This Window is not of type IntegratedWindows.");
            }
        }
    }
}
