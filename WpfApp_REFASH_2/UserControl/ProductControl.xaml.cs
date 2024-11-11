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

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for ProductControl.xaml
    /// </summary>
    public partial class ProductControl : UserControl
    {
        public event EventHandler<RoutedEventArgs> OnEdit;
        public event EventHandler<RoutedEventArgs> OnDelete;

        public ProductControl()
        {
            InitializeComponent();
        }

        private void btn_editProduct_Click(object sender, RoutedEventArgs e)
        {
            OnEdit?.Invoke(this, e);
        }

        private void btn_deleteProduct_Click(object sender, RoutedEventArgs e)
        {
            OnDelete?.Invoke(this, e);
        }
    }
}
