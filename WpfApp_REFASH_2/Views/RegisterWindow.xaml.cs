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
using WpfApp_REFASH.DataAccess;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private DatabaseManager dbManager;
        public RegisterWindow()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();

            // Move the initialization to after components are loaded
            this.Loaded += (s, e) => {
                addressPanel.Visibility = cb_role.SelectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
            };
        }

        private void cb_role_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addressPanel != null) // Add null check
            {
                addressPanel.Visibility = cb_role.SelectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void btn_register_Click(object sender, RoutedEventArgs e)
        {
            // Reading input from form
            string name = tb_username.Text.Trim();
            string email = tb_email.Text.Trim();
            string phoneNumber = tb_phoneNumber.Text.Trim();
            string password = tb_password.Password;
            string role = ((ComboBoxItem)cb_role.SelectedItem)?.Content.ToString();
            string address = tb_address.Text.Trim();

            // Validate required fields
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            // Validate address for Customer role
            if (role == "Customer" && string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Please enter your address.");
                return;
            }

            // Create new user instance and register
            User newUser = new User(name, email, phoneNumber, password, role, address);
            bool result = newUser.Register();

            if (result)
            {
                MessageBox.Show("Registration successful!");
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Registration failed. Please check your details and try again.");
            }
        }

        private void btn_close_app_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_minimaize_app_click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_login_click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
