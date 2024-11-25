using ScottPlot;
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

namespace WpfApp_REFASH
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int count)
            {
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Interaction logic for AdminDashboardView.xaml
    /// </summary>
    public partial class AdminDashboardView : UserControl, INotifyPropertyChanged
    {
        private Admin Admin {  get; set; }
        private ObservableCollection<Order> _allOrders;
        private ObservableCollection<Order> _filteredOrders;
        private WpfPlot _salesPlot;
        public List<string> StatusOptions { get; } = new List<string>
    {
        "Packaging",
        "Shipped",
        "Delivered"
    };
        public ObservableCollection<Order> FilteredOrders
        {
            get { return _filteredOrders; }
            set
            {
                _filteredOrders = value;
                OnPropertyChanged(nameof(FilteredOrders));
            }
        }
        public AdminDashboardView()
        {
            InitializeComponent();
            DataContext = this;
            InitializeData();
            InitializeSalesChart();
        }
        private void InitializeData()
        {
            try
            {
                Admin = AdminSession.CurrentAdmin;

                // Set statistics
                int newsCount = Admin.GetAllContent().Count;
                int collectionCount = Admin.GetAllCollections().Count;
                int productCount = Admin.GetAllProductOffer().Count;
                int soldCount = Admin.GetSoldProductsCount();

                NewsTotalText.Text = newsCount.ToString();
                CollectionTotalText.Text = collectionCount.ToString();
                TotalProductsText.Text = productCount.ToString();
                SoldProductsText.Text = soldCount.ToString();

                // Load orders with extra logging
                _allOrders = Admin.GetAllOrders();
                Console.WriteLine($"Loaded {_allOrders?.Count ?? 0} orders");

                if (_allOrders == null)
                {
                    _allOrders = new ObservableCollection<Order>();
                }

                FilteredOrders = new ObservableCollection<Order>(_allOrders);
                Console.WriteLine($"Initialized FilteredOrders with {FilteredOrders.Count} items");

                // Force UI refresh
                OrdersListView.ItemsSource = null;
                OrdersListView.ItemsSource = FilteredOrders;

                if (OrderStatusFilter != null)
                {
                    OrderStatusFilter.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing data: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void InitializeSalesChart()
        {
            var salesData = Admin.GetDailySalesData();

            // Create WPF Plot
            _salesPlot = new WpfPlot();

            // Prepare data for plotting
            double[] xData = salesData.Select((s, i) => (double)i).ToArray();
            double[] yData = salesData.Select(s => (double)s.Amount).ToArray();
            string[] xLabels = salesData.Select(s => s.Date.ToString("MMM dd")).ToArray();

            // Configure plot
            var plot = _salesPlot.Plot;

            // Create scatter plot with lines
            plot.AddScatter(
                xs: xData,
                ys: yData,
                color: System.Drawing.Color.FromArgb(233, 69, 96),
                lineWidth: 2,
                markerSize: 8);

            // Configure axes
            plot.Title("Daily Sales Overview");
            plot.XLabel("Date");
            plot.YLabel("Sales Amount (Rp)"); // Ubah label Y-axis

            // Set axis limits
            plot.SetAxisLimits(
                xMin: -0.5,
                xMax: xData.Length - 0.5,
                yMin: yData.Min() * 0.9,
                yMax: yData.Max() * 1.1);

            // Add custom X axis labels
            plot.XAxis.ManualTickPositions(xData, xLabels);
            plot.XAxis.TickLabelStyle(rotation: 45);

            // Format Y axis ticks to show Rupiah format
            plot.YAxis.TickLabelFormat((y) => $"Rp {y:N0}");

            // Configure grid and style
            plot.Grid(enable: true, lineStyle: ScottPlot.LineStyle.Dot);

            // Set plot style
            plot.Style(figureBackground: System.Drawing.Color.White);

            // Add the plot to the container
            ChartContainer.Child = _salesPlot;

            // Force render
            _salesPlot.Refresh();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_salesPlot != null)
            {
                _salesPlot.Refresh();
            }
        }
        private void OrderStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrderStatusFilter?.SelectedItem == null || _allOrders == null) return;

            try
            {
                string selectedStatus = ((ComboBoxItem)OrderStatusFilter.SelectedItem).Content.ToString();

                if (selectedStatus == "All Orders")
                {
                    FilteredOrders = new ObservableCollection<Order>(_allOrders);
                }
                else
                {
                    FilteredOrders = new ObservableCollection<Order>(
                        _allOrders.Where(o => string.Equals(o.Status, selectedStatus,
                            StringComparison.OrdinalIgnoreCase))
                    );
                }

                // Force UI refresh
                OrdersListView.ItemsSource = null;
                OrdersListView.ItemsSource = FilteredOrders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering orders: {ex.Message}",
                    "Filter Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ComboBox comboBox && comboBox.DataContext is Order order)
                {
                    string newStatus = comboBox.SelectedItem as string;
                    if (string.IsNullOrEmpty(newStatus) || newStatus == order.Status) return;

                    var result = MessageBox.Show(
                        $"Are you sure you want to update Order #{order.OrderId} status to '{newStatus}'?",
                        "Confirm Status Update",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Admin?.UpdateOrderStatus(order.OrderId, newStatus);

                        // Update UI
                        order.Status = newStatus;

                        // Optional: Refresh data untuk memastikan konsistensi
                        InitializeData();
                    }
                    else
                    {
                        // Revert selection jika user membatalkan
                        comboBox.SelectedItem = order.Status;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating order status: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Log untuk debugging
                Console.WriteLine($"Error in OrderStatus_SelectionChanged: {ex}");
            }
        }

        private void UpdateOrderStatus(Order order, string newStatus)
        {
            try
            {
                Admin?.UpdateOrderStatus(order.OrderId, newStatus);
                order.Status = newStatus; // Update status di UI

                MessageBox.Show($"Order #{order.OrderId} status has been updated to '{newStatus}'",
                    "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update order status: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Order : INotifyPropertyChanged
    {
        private string _status = "Packaging"; // Default value
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // DailySales class (unchanged)
    public class DailySales
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
