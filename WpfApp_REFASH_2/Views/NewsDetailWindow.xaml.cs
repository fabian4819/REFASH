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

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for NewsDetailWindow.xaml
    /// </summary>
    public partial class NewsDetailWindow : Window
    {
        private Customer customer;
        public NewsDetailWindow(int id)
        {
            InitializeComponent();
        }

        private void btn_closeApp_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_minimizeApp_click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_back_click(object sender, RoutedEventArgs e)
        {
            NewsWindow newsWindow = new NewsWindow();
            newsWindow.Show();
            this.Close();
        }

        private void btn_navigateNews_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
