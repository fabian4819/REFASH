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

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for CustomerNewsDetailView.xaml
    /// </summary>
    public partial class CustomerNewsDetailView : UserControl
    {
        private Customer Customer { get; set; }
        public CustomerNewsDetailView(int id)
        {
            InitializeComponent();
            Customer = UserSession.CurrentCustomer;
            var newsContent = Customer.GetContentById(id);
            this.DataContext = newsContent;
        }
        private void btn_closeApp_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_minimizeApp_click(object sender, RoutedEventArgs e)
        {
            Window ParentWindow = Window.GetWindow(this);
            ParentWindow.WindowState = WindowState.Minimized;
        }

        private void btn_back_click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is IntegratedWindows integratedWindows)
            {
                integratedWindows.SetMainContent(new CustomerNewsView());
            }
            else
            {
                MessageBox.Show("This Window is not of type IntegratedWindows.");
            }
        }

        private void btn_navigateNews_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
