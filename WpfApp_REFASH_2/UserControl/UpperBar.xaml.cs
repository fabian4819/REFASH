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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
    public partial class UpperBar : UserControl
    {
        public static readonly DependencyProperty WelcomeNameProperty =
            DependencyProperty.Register("WelcomeName", typeof(string), typeof(UpperBar),
                new PropertyMetadata("Guest"));

        public string WelcomeName
        {
            get => (string)GetValue(WelcomeNameProperty);
            set => SetValue(WelcomeNameProperty, value);
        }

        public UpperBar()
        {
            InitializeComponent();
        }

        private void btn_minimizeApp_click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            parentWindow.WindowState = WindowState.Minimized;
        }

        private void btn_closeApp_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}