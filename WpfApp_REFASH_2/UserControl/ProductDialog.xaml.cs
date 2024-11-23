using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
    public class ProductEventArgs : EventArgs
    {
        public Product Product { get; set; }
        public bool IsNewProduct { get; set; }
        public Content Content { get; set; }
        public bool IsNewContent { get; set; }
    }

    public partial class ProductDialog : UserControl
    {
        public event EventHandler<ProductEventArgs> OnSave;
        public event RoutedEventHandler OnCancel;

        private Product currentProduct;
        private bool isNewProduct;
        private Admin Admin;
        private bool editingMode = false;
        private byte[] currentImageData;

        public ProductDialog()
        {
            InitializeComponent();
            Admin = AdminSession.CurrentAdmin;
        }

        public void LoadProduct(Product product)
        {
            editingMode = true;
            currentProduct = product;
            isNewProduct = false;
            DialogTitle.Text = "Edit Product";

            // Fill form fields
            tb_productName.Text = product.ProductName;
            tb_price.Text = product.Price.ToString();
            tb_stock.Text = product.Stock.ToString();
            tb_description.Text = product.Description;
            tb_imageUrl.Text = product.Image;

            // Set ComboBox selections
            SetComboBoxSelection(cb_category, product.Category);
            SetComboBoxSelection(cb_size, product.Size);
        }

        private void SetComboBoxSelection(ComboBox comboBox, string value)
        {
            var item = comboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == value);
            if (item != null)
                comboBox.SelectedItem = item;
        }

        public void ClearForm()
        {
            editingMode = false;
            currentProduct = null;
            isNewProduct = true;
            DialogTitle.Text = "Add New Product";

            tb_productName.Clear();
            tb_price.Clear();
            tb_stock.Clear();
            tb_description.Clear();
            tb_imageUrl.Clear();
            cb_category.SelectedIndex = 0;
            cb_size.SelectedIndex = 2; // M is default
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                var productName = tb_productName.Text;
                var description = tb_description.Text;
                var productId = currentProduct?.ProductID ?? 0; // Use 0 or generate new ID for new products
                var image = tb_imageUrl.Text;
                var price = decimal.Parse(tb_price.Text);
                var category = ((ComboBoxItem)cb_category.SelectedItem).Content.ToString();
                var size = ((ComboBoxItem)cb_size.SelectedItem).Content.ToString();
                var stock = int.Parse(tb_stock.Text);

                var product = new Product(
                    productName,
                    description,
                    productId,
                    currentImageData,
                    price,
                    category,
                    size,
                    stock
                );
                
                if(editingMode == false)
                {
                    Admin.AddProductOffer(product);
                }
                if (editingMode == true)
                {
                    Admin.EditProduct(product);
                }

                OnSave?.Invoke(this, new ProductEventArgs
                {
                    Product = product,
                    IsNewProduct = isNewProduct
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(tb_productName.Text))
            {
                MessageBox.Show("Product name is required", "Validation Error");
                return false;
            }

            if (!decimal.TryParse(tb_price.Text, out _))
            {
                MessageBox.Show("Please enter a valid price", "Validation Error");
                return false;
            }

            if (!int.TryParse(tb_stock.Text, out _))
            {
                MessageBox.Show("Please enter a valid stock number", "Validation Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(tb_imageUrl.Text))
            {
                MessageBox.Show("Image URL is required", "Validation Error");
                return false;
            }

            return true;
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke(this, new RoutedEventArgs());
        }

        private void btn_browseImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Title = "Select Product Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tb_imageUrl.Text = openFileDialog.FileName;
                try
                {
                    currentImageData = File.ReadAllBytes(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to read the image file: {ex.Message}", "File Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
    }
}
