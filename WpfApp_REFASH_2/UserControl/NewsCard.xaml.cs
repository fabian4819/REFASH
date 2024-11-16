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
using WpfApp_REFASH.ViewModels;
using WpfApp_REFASH.ViewModels.WpfApp_REFASH.ViewModel;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for NewsCard.xaml
    /// </summary>
    public partial class NewsCard : UserControl
    {
        public Customer Customer;
        public NewsCard()
        {
            InitializeComponent();
            Customer = UserSession.CurrentCustomer;
        }
        private void btn_detail_click(object sender, RoutedEventArgs e)
        {
            // Access Customer through DataContext (Content object)
            if(AdminSession.CurrentAdmin != null)
            {
                AdminNewsWindow adminNewsWindow = new AdminNewsWindow();
                adminNewsWindow.Show();
                Window parentWindow = Window.GetWindow(this);
                parentWindow.Close();
            }
            else if (DataContext is Content content && Customer != null)
            {
                NewsDetailWindow newsDetailWindow = new NewsDetailWindow(content.contentID);
                newsDetailWindow.Show();
                Window parentWindow = Window.GetWindow(this);
                parentWindow?.Close();
            }
            else
            {
                MessageBox.Show("Session data is missing.");
            }
        }
    }
}
