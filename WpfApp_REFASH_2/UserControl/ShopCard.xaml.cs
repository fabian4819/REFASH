using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for ShopCard.xaml
    /// </summary>
    
    public partial class ShopCard : UserControl
    {
        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity)); // Notify that property has changed if using INotifyPropertyChanged
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ShopCard()
        {
            InitializeComponent();
            btn_addToCart.Click += btn_addToCart_click;
        }

        private void btn_addToCart_click (object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Adding {Quantity} units to cart.");
        }
    }
}
