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
        private Admin admin;
        private Customer customer;

        public LoginWindow()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();
        }

        private void btn_register_click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void btn_login_click(object sender, RoutedEventArgs e)
        {
            string email = tb_email.Text.Trim();
            string password = tb_password.Password.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }

            user = new User(email, password);
            var (isAuthenticated, message, name, role, phoneNumber) = user.Login(email, password);

            if (isAuthenticated)
            {
                MessageBox.Show(message);

                switch (role)
                {
                    case "Admin":
                        admin = new Admin(name, email, phoneNumber, password, role, Guid.NewGuid().ToString());
                        AdminDashboardWindow adminWindow = new AdminDashboardWindow();
                        adminWindow.Show();
                        break;
                    case "Customer":
                        customer = new Customer(name, email, phoneNumber, password, role);
                        NewsWindow newsWindow = new NewsWindow(customer);
                        newsWindow.Show();
                        break;
                    default:
                        MessageBox.Show("Unknown role");
                        break;
                }

                this.Close();
            }
            else
            {
                MessageBox.Show(message);
            }
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