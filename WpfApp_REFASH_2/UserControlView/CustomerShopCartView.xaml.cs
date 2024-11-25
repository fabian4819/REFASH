using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfApp_REFASH.ViewModels;
using System.IO;
using System.Diagnostics;
using QRCoder;
using System.Drawing;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for CustomerShopCartView.xaml
    /// </summary>
    public partial class CustomerShopCartView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Customer _customer;
        private ObservableCollection<CartItem> _cartItems;
        private decimal _totalPrice;
        private int _totalSelectedItems;
        private string _whatsAppLink; // Tambahkan field untuk WhatsAppLink

        // Tambahkan property WhatsAppLink
        public string WhatsAppLink
        {
            get => _whatsAppLink;
            set
            {
                if (_whatsAppLink != value)
                {
                    _whatsAppLink = value;
                    OnPropertyChanged(nameof(WhatsAppLink));
                }
            }
        }

        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
            }
        }

        public decimal TotalPrice
        {
            get => _totalPrice;
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public int TotalSelectedItems
        {
            get => _totalSelectedItems;
            set
            {
                if (_totalSelectedItems != value)
                {
                    _totalSelectedItems = value;
                    OnPropertyChanged(nameof(TotalSelectedItems));
                    OnPropertyChanged(nameof(HasSelectedItems));
                }
            }
        }

        public bool HasSelectedItems => TotalSelectedItems > 0;

        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set
            {
                _cartItems = value;
                OnPropertyChanged(nameof(CartItems));
                UpdateTotalPrice();
                UpdateSelectedItemsCount();
            }
        }

        public CustomerShopCartView()
        {
            InitializeComponent();
            Customer = UserSession.CurrentCustomer;
            if (Customer == null)
            {
                MessageBox.Show("Customer data is missing.");
                return;
            }
            LoadCartItems();
            DataContext = this;
            WhatsAppLink = "https://wa.me/6282232018289"; // Initialize default WhatsApp link
        }

        private void LoadCartItems()
        {
            var products = Customer.GetAllProductCart();
            CartItems = new ObservableCollection<CartItem>(
                products.Select(p => new CartItem(p)));

            foreach (var item in CartItems)
            {
                item.PropertyChanged += CartItem_PropertyChanged;
            }
        }

        private void CartItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItem.IsSelected))
            {
                UpdateTotalPrice();
                UpdateSelectedItemsCount();
            }
        }

        private void UpdateTotalPrice()
        {
            TotalPrice = CartItems?.Where(item => item.IsSelected)
                                 .Sum(item => item.Price * item.Stock) ?? 0;
        }

        private void UpdateSelectedItemsCount()
        {
            TotalSelectedItems = CartItems?.Count(item => item.IsSelected) ?? 0;
        }

        private void btn_back_click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is IntegratedWindows integratedWindows)
            {
                integratedWindows.SetMainContent(new CustomerShopView());
            }
            else
            {
                MessageBox.Show("This Window is not of type IntegratedWindows.");
            }
        }

        private void btn_deleteFromCart_click(object sender, RoutedEventArgs e)
        {
            var cartItem = ((FrameworkElement)sender).DataContext as CartItem;
            if (cartItem != null)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to remove this item from your cart?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _customer.DeleteFromCart(cartItem.ProductID);
                    LoadCartItems();
                }
            }
        }

        private void btn_checkout_click(object sender, RoutedEventArgs e)
        {
            if (!HasSelectedItems)
            {
                MessageBox.Show("Please select at least one item to checkout.",
                    "No Items Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            try
            {
                var selectedItems = CartItems.Where(item => item.IsSelected)
                                          .Select(item => item.ToProduct())
                                          .ToList();

                var totalAmount = selectedItems.Sum(item => item.Price * item.Stock);

                var result = MessageBox.Show(
                    $"Total amount: Rp. {totalAmount:N0}\nProceed with checkout?",
                    "Confirm Checkout",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _customer.Checkout(new ObservableCollection<Product>(selectedItems));
                    ShowPaymentModal(totalAmount); // Show payment modal
                    LoadCartItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Checkout failed: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowPaymentModal(decimal totalAmount)
        {
            TotalAmountText.Text = $"Rp. {totalAmount:N0}";

            // Create product details string
            var productDetails = string.Join("%0A", CartItems
                .Where(item => item.IsSelected)
                .Select(item => $"- {item.ProductName} (x{item.Stock})"));

            // Create WhatsApp message template with current date and time
            var message = $"Halo Admin REFASH, saya ingin mengkonfirmasi pembayaran:%0A" +
                         $"Tanggal: {DateTime.Now:dd MMMM yyyy}%0A" +
                         $"Pukul: {DateTime.Now:HH:mm} WIB%0A" +
                         $"Nama: {Customer.Name}%0A" +
                         $"%0ADetail Produk:%0A{productDetails}%0A" +
                         $"%0ATotal Pembayaran: Rp. {totalAmount:N0}";

            // Update WhatsApp link with message template
            WhatsAppLink = $"https://wa.me/6282232018289?text={message}";

            // Generate QR Code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(totalAmount.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            // Convert to Bitmap
            System.Drawing.Bitmap qrCodeImage = qrCode.GetGraphic(20);

            // Convert to BitmapImage for WPF
            using (MemoryStream memory = new MemoryStream())
            {
                qrCodeImage.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                QRCodeImage.Source = bitmapImage;
            }

            PaymentModal.Visibility = Visibility.Visible;
        }

        private void ClosePaymentModal_Click(object sender, RoutedEventArgs e)
        {
            PaymentModal.Visibility = Visibility.Collapsed;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open WhatsApp: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            e.Handled = true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class CartItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        public BitmapImage BitmapImage { get; set; }

        public CartItem(Product product)
        {
            ProductID = product.ProductID;
            ProductName = product.ProductName;
            Description = product.Description;
            BitmapImage = product.BitmapImage;
            Price = product.Price;
            Category = product.Category;
            Size = product.Size;
            Stock = product.Stock;
            IsSelected = false;
        }

        public Product ToProduct()
        {
            return new Product(
                ProductName,
                Description,
                ProductID,
                Image,
                Price,
                Category,
                Size,
                Stock
            );
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
