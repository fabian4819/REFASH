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
            string email = tb_email.Text.Trim(); // Trim to remove any extraneous whitespace
            string password = tb_password.Text.Trim(); // Assuming this is a PasswordBox for security reasons

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.");
                return; // Stop further execution if the fields are empty
            }

            User user = new User(email, password);
            var (isAuthenticated, message, name, role, phoneNumber) = user.Login(email, password);

            if (isAuthenticated)
            {
                MessageBox.Show(message); // Display the success message

                switch (role)
                {
                    case "Admin":
                        MessageBox.Show("Logged in as Admin");
                    //    AdminWindow adminWindow = new AdminWindow(); // Assuming there's an AdminWindow
                    //    adminWindow.Show();
                        break;
                    case "Customer":
                        Customer customer = new Customer(name, email, phoneNumber, password, role);
                        NewsWindow newsWindow = new NewsWindow(customer);
                        newsWindow.Show();
                        break;
                    default:
                        MessageBox.Show("Unknown role");
                        break;
                }

                this.Close(); // Close the login window only after successful login
            }
            else
            {
                MessageBox.Show(message); // Show error message if not authenticated
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
