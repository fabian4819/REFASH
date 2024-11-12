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

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for AdminShopWindow.xaml
    /// </summary>
    public partial class AdminShopWindow : Window
    {
        private Grid modalBackground;
        private ProductDialog productDialog;
        public ObservableCollection<Product> Products { get; set; }

        public AdminShopWindow()
        {
            InitializeComponent();
            InitializeModalLayer();
            LoadProducts();
            DataContext = this;
        }

        private void InitializeModalLayer()
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
            Products = new ObservableCollection<Product>();
            //Add sample products here
            var sample = new Product(
                name: "Sample Product",
                description: "Sample Description",
                productID: 1,
                image: "../Assets/Logo.png",
                price: 100000M,
                category: "Clothing",
                size: "M",
                stock: 10
            );
            Products.Add(sample);
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
                    DeleteProduct(product);
                }
            }
        }

        private void DeleteProduct(Product product)
        {
            // TODO: Delete from database
            Products.Remove(product);
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
