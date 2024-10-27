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
        private UserRepository _userRepository;
        public RegisterWindow()
        {
            InitializeComponent();
            var dbManager = new DatabaseManager();
            _userRepository = new UserRepository(dbManager);
        }

        private void btn_login_click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }


        private void btn_register_Click(object sender, RoutedEventArgs e)
        {
            string name = tb_username.Text;
            string email = tb_email.Text;
            string phoneNumber = tb_phoneNumber.Text;
            string password = tb_password.Text;
            string role = ((ComboBoxItem)cb_role.SelectedItem).Content.ToString();

            bool result = _userRepository.RegisterUser(name, email, phoneNumber, password, role);

            if (result)
            {
                MessageBox.Show("Registration successful!");
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Registration failed. Please check your details.");
            }
        }
    }
}
