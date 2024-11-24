using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
    /// <summary>
    /// Interaction logic for AdminCollectionView.xaml
    /// </summary>
    public partial class AdminCollectionView : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Collection> _allCollections;
        private ObservableCollection<Collection> _filteredCollections;
        public ObservableCollection<Collection> FilteredCollections
        {
            get { return _filteredCollections; }
            set
            {
                _filteredCollections = value;
                OnPropertyChanged(nameof(FilteredCollections));
            }
        }
        private Admin Admin { get; set; }
        public AdminCollectionView()
        {
            InitializeComponent();
            Admin = AdminSession.CurrentAdmin;
            LoadCollections();
            DataContext = this;
            StatusFilter.SelectedIndex = 0;
        }
        private void LoadCollections()
        {
            // TODO: Load collections from database
            _allCollections = Admin.GetAllCollections();
            FilteredCollections = new ObservableCollection<Collection>(_allCollections);
        }
        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusFilter.SelectedItem == null) return;

            string selectedStatus = ((ComboBoxItem)StatusFilter.SelectedItem).Content.ToString();
            if (selectedStatus == "All")
            {
                FilteredCollections = new ObservableCollection<Collection>(_allCollections);
            }
            else
            {
                FilteredCollections = new ObservableCollection<Collection>(
                    _allCollections.Where(c => c.Status == selectedStatus.Replace(" ", ""))
                );
            }
        }
        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.DataContext is Collection collection)
            {
                string newStatus = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString().Replace(" ", "");
                UpdateCollectionStatus(collection, newStatus);
            }
        }
        private void UpdateCollectionStatus(Collection collection, string newStatus)
        {
            // TODO: Update status in database
            Admin.UpdateCollectionStatus(collection, newStatus);
            // Optional: Show notification
            MessageBox.Show($"Collection status updated to {newStatus}", "Status Updated",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                switch (status.ToLower())
                {
                    case "inreview":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F39C12"));
                    case "verified":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"));
                    case "rejected":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                    default:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A5A6"));
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
