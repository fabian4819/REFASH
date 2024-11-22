using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using ScottPlot;
using ScottPlot.WPF;
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
    public partial class AdminDashboardWindow : Window, INotifyPropertyChanged
    {
        private Admin Admin;
        private ObservableCollection<Order> _allOrders;
        private ObservableCollection<Order> _filteredOrders;
        private WpfPlot _salesPlot;

        public ObservableCollection<Order> FilteredOrders
        {
            get { return _filteredOrders; }
            set
            {
                _filteredOrders = value;
                OnPropertyChanged(nameof(FilteredOrders));
            }
        }

        public AdminDashboardWindow()
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

                // Set welcome name
                upperBar.WelcomeName = Admin.Name;

                // Load orders
                _allOrders = Admin.GetAllOrders();
                if (_allOrders == null)
                {
                    _allOrders = new ObservableCollection<Order>();
                }
                FilteredOrders = new ObservableCollection<Order>(_allOrders);

                // Ensure the ListView updates
                if (OrdersListView != null)
                {
                    OrdersListView.ItemsSource = FilteredOrders;
                }

                // Set default order filter
                if (OrderStatusFilter != null)
                {
                    OrderStatusFilter.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
            plot.YLabel("Sales Amount ($)");

            // Set axis limits
            plot.SetAxisLimits(
                xMin: -0.5,
                xMax: xData.Length - 0.5,
                yMin: yData.Min() * 0.9,
                yMax: yData.Max() * 1.1);

            // Add custom X axis labels
            plot.XAxis.ManualTickPositions(xData, xLabels);
            plot.XAxis.TickLabelStyle(rotation: 45);

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
            if (OrderStatusFilter.SelectedItem == null) return;

            string selectedStatus = ((ComboBoxItem)OrderStatusFilter.SelectedItem).Content.ToString();

            if (selectedStatus == "All Orders")
            {
                FilteredOrders = new ObservableCollection<Order>(_allOrders);
            }
            else
            {
                FilteredOrders = new ObservableCollection<Order>(
                    _allOrders.Where(o => o.Status == selectedStatus)
                );
            }
        }

        private void OrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.DataContext is Order order)
            {
                string newStatus = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();
                UpdateOrderStatus(order, newStatus);
            }
        }

        private void UpdateOrderStatus(Order order, string newStatus)
        {
            try
            {
                Admin.UpdateOrderStatus(order.OrderId, newStatus);
                order.Status = newStatus;
                MessageBox.Show($"Order status updated to {newStatus}", "Status Updated",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update order status: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Order class (unchanged)
    public class Order : INotifyPropertyChanged
    {
        private string _status;
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
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