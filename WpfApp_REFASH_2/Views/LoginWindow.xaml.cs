﻿    using System;
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
    using WpfApp_REFASH.ViewModels;

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
        private LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            var dbManager = new DatabaseManager();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;
            _viewModel.LoginSuccess += OnLoginSuccess;
            _viewModel.LoginSuccessAdmin += OnLoginSuccessAdmin;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged; // Tambahkan ini
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoading")
            {
                LoadingOverlay.Visibility = _viewModel.IsLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void btn_register_click(object sender, RoutedEventArgs e)
            {
                RegisterWindow registerWindow = new RegisterWindow();
                registerWindow.Show();
                this.Close();
            }
            private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
            {
                if (DataContext != null)
                {
                    _viewModel.Password = ((PasswordBox)sender).Password;
                }
            }
            private void OnLoginSuccessAdmin(Admin admin)
            {
                if (admin == null)
                {
                    MessageBox.Show("Customer is missing in login");
                }
                Application.Current.Dispatcher.Invoke(() => {
                    var adminIntegratedWindow = new AdminIntegratedWindow();
                    adminIntegratedWindow.Show();
                    this.Close();
                });
            }
            private void OnLoginSuccess(Customer customer)
            {
                if(customer == null)
                {
                    MessageBox.Show("Customer is missing in login");
                }
                Application.Current.Dispatcher.Invoke(() => {
                    var integratedWindow = new IntegratedWindows();
                    integratedWindow.Show();
                    this.Close();
                });
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