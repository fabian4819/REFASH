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
using System.Windows.Shapes;
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for AdminDashboardWindow.xaml
    /// </summary>
    public partial class AdminDashboardWindow : Window
    {
        private Admin Admin;
        public AdminDashboardWindow()
        {
            InitializeComponent();
            Admin = AdminSession.CurrentAdmin;
            int NewsCount = Admin.GetAllContent().Count;
            int CollectionCount = Admin.GetAllContent().Count;
            int ProductCount = Admin.GetAllProductOffer().Count;
            NewsTotalText.Text = NewsCount.ToString();
            CollectionTotalText.Text = CollectionCount.ToString();
            TotalProductsText.Text = ProductCount.ToString();
            upperBar.WelcomeName = Admin.Name;
        }
    }
}
