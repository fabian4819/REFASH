using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
    public partial class ShopWindow : Window, INotifyPropertyChanged
    {
        public Customer Customer { get; private set; }
        public ObservableCollection<Product> ProductItem { get; set; }
        private ObservableCollection<Product> AllProducts { get; set; }
        private string currentSearchText = string.Empty;
        private int _cartItemCount;
        public int CartItemCount
        {
            get => _cartItemCount;
            set
            {
                if (_cartItemCount != value)
                {
                    _cartItemCount = value;
                    OnPropertyChanged(nameof(CartItemCount));
                }
            }
        }

        // Kembalikan parameter customer di constructor
        public ShopWindow(Customer customer)
        {
            InitializeComponent();
            Customer = UserSession.CurrentCustomer;
            if (Customer == null)
            {
                MessageBox.Show("Customer data is missing.");
                return;
            }

            // Initialize products
            AllProducts = Customer.GetAllProductOffer();
            ProductItem = new ObservableCollection<Product>(AllProducts);

            // Set initial values
            DataContext = this;
            upperBar.WelcomeName = Customer.Name;

            // Initialize filters
            PriceRangeFilter.SelectedIndex = 0;
            SortByFilter.SelectedIndex = 0;

            // Initialize cart count
            UpdateCartCount();
        }

        private void UpdateCartCount()
        {
            CartItemCount = Customer.GetCartItemCount();
            OnPropertyChanged(nameof(CartItemCount));
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearchText = SearchBox.Text;
            ApplyFilters();
        }

        private void PriceRangeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PriceRangeFilter.SelectedItem != null)
            {
                ApplyFilters();
            }
        }

        private void SortByFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortByFilter.SelectedItem != null)
            {
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (AllProducts == null) return;

            var filteredProducts = AllProducts.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(currentSearchText))
            {
                filteredProducts = filteredProducts.Where(p =>
                    p.Name.ToLower().Contains(currentSearchText.ToLower()) ||
                    p.Description.ToLower().Contains(currentSearchText.ToLower()));
            }

            // Apply price range filter
            switch (PriceRangeFilter.SelectedIndex)
            {
                case 1: // Under 300k
                    filteredProducts = filteredProducts.Where(p => p.Price < 300000);
                    break;
                case 2: // 300k - 500k
                    filteredProducts = filteredProducts.Where(p => p.Price >= 300000 && p.Price <= 500000);
                    break;
                case 3: // 500k - 1M
                    filteredProducts = filteredProducts.Where(p => p.Price > 500000 && p.Price <= 1000000);
                    break;
                case 4: // Above 1M
                    filteredProducts = filteredProducts.Where(p => p.Price > 1000000);
                    break;
            }

            // Apply sorting
            switch (SortByFilter.SelectedIndex)
            {
                case 1: // Low to High
                    filteredProducts = filteredProducts.OrderBy(p => p.Price);
                    break;
                case 2: // High to Low
                    filteredProducts = filteredProducts.OrderByDescending(p => p.Price);
                    break;
            }

            // Update the ProductItem collection
            ProductItem.Clear();
            foreach (var product in filteredProducts.ToList())
            {
                ProductItem.Add(product);
            }

            // Update UI if no results
            if (!ProductItem.Any())
            {
                MessageBox.Show("No products found matching your criteria.", "Search Results",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btn_cart_click(object sender, RoutedEventArgs e)
        {
            ShopCartWindow shopCartWindow = new ShopCartWindow();
            shopCartWindow.Show();
            this.Close();
        }

        private void btn_transaction_click(object sender, RoutedEventArgs e)
        {
            ShopTransactionWindow shopTransactionWindow = new ShopTransactionWindow();
            shopTransactionWindow.Show();
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}