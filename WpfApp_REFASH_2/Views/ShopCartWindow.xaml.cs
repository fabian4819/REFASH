using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
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

        public CartItem(Product product)
        {
            ProductID = product.ProductID;
            ProductName = product.ProductName;
            Description = product.Description;
            Image = product.Image;
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

    public partial class ShopCartWindow : Window, INotifyPropertyChanged
    {
        private Customer _customer;
        private ObservableCollection<CartItem> _cartItems;
        private decimal _totalPrice;
        private int _totalSelectedItems;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public ShopCartWindow()
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
            upperBar.WelcomeName = Customer.Name;
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
                    MessageBox.Show("Checkout successful!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCartItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Checkout failed: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_back_click(object sender, RoutedEventArgs e)
        {
            var shopWindow = new ShopWindow(_customer);
            shopWindow.Show();
            Close();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}