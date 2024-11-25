using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using WpfApp_REFASH.DataAccess;

namespace WpfApp_REFASH.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _email;
        private string _password;
        private bool _isLoading;
        private readonly DatabaseManager _dbManager;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public ICommand LoginCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<Customer> LoginSuccess;
        public event Action<Admin> LoginSuccessAdmin;

        public LoginViewModel()
        {
            _dbManager = new DatabaseManager();
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        private bool CanExecuteLogin()
        {
            return !IsLoading; // Disable login button while loading
        }

        private async void ExecuteLogin()
        {
            try
            {
                IsLoading = true;

                // Validasi input
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Please enter both email and password.");
                    return;
                }

                // Simulate network delay (optional, remove in production)
                await Task.Delay(1000);

                // Logika autentikasi
                User user = new User(Email, Password);
                var (isAuthenticated, message, name, role, phoneNumber) = user.Login(Email, Password);

                if (isAuthenticated)
                {
                    switch (role?.ToLower())
                    {
                        case "admin":
                            Admin admin = new Admin(name, Email, phoneNumber, Password, role);
                            AdminSession.CurrentAdmin = admin;
                            LoginSuccessAdmin?.Invoke(admin);
                            break;

                        case "customer":
                            Customer customer = new Customer(name, Email, phoneNumber, Password, role);
                            UserSession.CurrentCustomer = customer;
                            LoginSuccess?.Invoke(customer);
                            break;

                        default:
                            MessageBox.Show("Unknown role");
                            break;
                    }
                }
                else
                {
                    MessageBox.Show(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
