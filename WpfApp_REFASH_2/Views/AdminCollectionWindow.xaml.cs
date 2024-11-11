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
using System.Windows.Shapes;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for AdminCollectionWindow.xaml
    /// </summary>
    public partial class AdminCollectionWindow : Window, INotifyPropertyChanged
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

        public AdminCollectionWindow()
        {
            InitializeComponent();
            LoadCollections();
            DataContext = this;
            StatusFilter.SelectedIndex = 0; // Select "All" by default
        }

        private void LoadCollections()
        {
            // TODO: Load collections from database
            _allCollections = new ObservableCollection<Collection>();
            // Temporary sample data
            _allCollections.Add(new Collection("Collection 1", "Description 1", "Category A", "../Assets/Logo.png") { Status = "InVerification" });
            _allCollections.Add(new Collection("Collection 2", "Description 2", "Category B", "../Assets/Logo.png") { Status = "Verified" });
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
            collection.Status = newStatus;

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
}
