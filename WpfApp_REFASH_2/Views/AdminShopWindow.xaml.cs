using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AdminShopWindow.xaml
    /// </summary>
    public partial class AdminShopWindow : Window
    {
        private Grid modalBackground;
        private ProductDialog productDialog, editProductDialog;
        public ObservableCollection<Product> Products { get; set; }
        private Admin Admin { get; set; }

        public AdminShopWindow()
        {
            InitializeComponent();
            Admin = AdminSession.CurrentAdmin;
            InitializeModals();
            LoadProducts();
            DataContext = this;
        }

        private void InitializeModals()
        {
            modalBackground = new Grid
            {
                Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
                Visibility = Visibility.Collapsed,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var blurEffect = new System.Windows.Media.Effects.BlurEffect
            {
                Radius = 5,
                KernelType = System.Windows.Media.Effects.KernelType.Gaussian
            };

            productDialog = new ProductDialog();
            productDialog.OnSave += ProductDialog_OnSave;
            productDialog.OnCancel += ProductDialog_OnCancel;
            productDialog.HorizontalAlignment = HorizontalAlignment.Center;
            productDialog.VerticalAlignment = VerticalAlignment.Center;


            modalBackground.IsVisibleChanged += (s, e) =>
            {
                if (modalBackground.Visibility == Visibility.Visible)
                    ContentGrid.Effect = blurEffect;
                else
                    ContentGrid.Effect = null;
            };

            modalBackground.Children.Add(productDialog);
            MainGrid.Children.Add(modalBackground);
            Panel.SetZIndex(modalBackground, 999);
        }

        private void LoadProducts()
        {
            Products = Admin.GetAllProductOffer();
        }
        
        private void ProductControl_OnEdit(object sender, RoutedEventArgs e)
        {
            var productControl = sender as ProductControl;
            if (productControl != null)
            {
                var product = productControl.DataContext as Product;
                if (product != null)
                {
                    // You can now perform the deletion logic, such as:
                    // Remove the product from the collection
                    Products.Remove(product);
                    MessageBox.Show($"Product {product.ProductName} deleted.");
                }
            }
        }

        private void btn_addProduct_Click(object sender, RoutedEventArgs e)
        {
            productDialog.ClearForm();
            modalBackground.Visibility = Visibility.Visible;
        }

        private void btn_editProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = ((FrameworkElement)sender).DataContext as Product;
            if (product != null)
            {
                productDialog.LoadProduct(product);
                modalBackground.Visibility = Visibility.Visible;
            }
        }

        private void btn_deleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = ((FrameworkElement)sender).DataContext as Product;
            if (product != null)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete {product.ProductName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    Admin.DeleteProduct(product);
                    Products.Remove(product );
                    LoadProducts();
                }
            }
        }

        private void ProductDialog_OnSave(object sender, ProductEventArgs e)
        {
            if (e.IsNewProduct)
            {
                Products.Add(e.Product);
            }
            else
            {
                var index = Products.IndexOf(Products.First(p => p.ProductID == e.Product.ProductID));
                Products[index] = e.Product;
            }

            modalBackground.Visibility = Visibility.Collapsed;
        }

        private void ProductDialog_OnCancel(object sender, RoutedEventArgs e)
        {
            modalBackground.Visibility = Visibility.Collapsed;
        }
    }
}
