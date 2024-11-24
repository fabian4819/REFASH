﻿using System;
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
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for IntegratedWindows.xaml
    /// </summary>
    public partial class IntegratedWindows : Window
    {
        private Customer Customer { get; set; }
        public IntegratedWindows()
        {
            InitializeComponent();
            Customer = UserSession.CurrentCustomer;
            SetMainContent(new CustomerNewsView());
            upperbar.WelcomeName = Customer.Name;
        }
        public void SetMainContent(UserControl control)
        {
            MainContent.Content = control;
        }
    }
}
