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
        }

        private void btn_login_click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }


        private void btn_register_Click(object sender, RoutedEventArgs e)
        {
            // Membaca input dari form
            string name = tb_username.Text.Trim();  // Trim untuk menghilangkan whitespace di awal dan akhir
            string email = tb_email.Text.Trim();
            string phoneNumber = tb_phoneNumber.Text.Trim(); 
            string password = tb_password.Text;
            string role = ((ComboBoxItem)cb_role.SelectedItem)?.Content.ToString();

            // Mengecek apakah semua field telah diisi
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Menciptakan instance user baru dan mendaftarkannya
            User newUser = new User(name, email, phoneNumber, password, role);
            bool result = newUser.Register();
            if (result)
            {
                MessageBox.Show("Registration successful!");
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();  // Menutup jendela registrasi saat ini
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
    }
}
