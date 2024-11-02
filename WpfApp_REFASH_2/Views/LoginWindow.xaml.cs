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

            var (isAuthenticated, role) = User.Login(email, password);
            if (isAuthenticated)
            {
                MessageBox.Show("Login successfull");
                if (role == "Admin")
                {
                    MessageBox.Show("Login as Admin");
                }
                else if (role == "Customer")
                {
                    MessageBox.Show("Login as Customer");
                }
            }
            else
            {
                MessageBox.Show(role);
            }
        }
    }
}
