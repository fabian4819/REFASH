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
    /// Interaction logic for CustomerNewsView.xaml
    /// </summary>
    public partial class CustomerNewsView : UserControl
    {
        public Customer Customer { get; private set; }
        public ObservableCollection<Content> ContentItem { get; set; }
        public CustomerNewsView()
        {
            InitializeComponent();
            if (UserSession.CurrentCustomer == null)
            {
                MessageBox.Show("No customer is logged in.");
                return;
            }
            Customer = UserSession.CurrentCustomer;

            ContentItem = UserSession.CurrentCustomer.GetAllContent();
            foreach (var content in ContentItem)
            {
                content.Customer = Customer; // Assign Customer to each Content item
            }
            DataContext = this;
        }
    }
}
