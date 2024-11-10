using System;
using System.Collections.Generic;
using System.Data;
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
using System.Xml.Linq;
using WpfApp_REFASH.DataAccess;
using WpfApp_REFASH.Utilities;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private DatabaseManager dbManager;
        private User user;
        private Customer customer;
        public LoginWindow()
        {
            InitializeComponent();
            var dbManager = new DatabaseManager();
        }

        private void btn_register_click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void btn_login_click(object sender, RoutedEventArgs e)
        {
            string email = tb_email.Text;
            string password = tb_password.Text;
            user = new User(email, password);

            //var (isAuthenticated, message, name, role, phoneNumber) = user.Login(email, password);
            //if (isAuthenticated)
            //{
            //    MessageBox.Show("Login successfull");
            //    if (role == "Admin")
            //    {
            //        MessageBox.Show("Login as Admin");
            //    }
            //    else if (role == "Customer")
            //    {
            //        customer = new Customer(name, email, phoneNumber, password, role);
            //        NewsWindow newsWindow = new NewsWindow(customer);
            //        newsWindow.Show();
            //        this.Close();
            //    }
            //}
            //else
            //{
            //    MessageBox.Show(role);
            //}
            customer = new Customer("name", "email", "phoneNumber", "password", "role");
            NewsWindow newsWindow = new NewsWindow(customer);
            newsWindow.Show();
            this.Close();
        }

        private void btn_closeApp_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_minimizeApp_click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
