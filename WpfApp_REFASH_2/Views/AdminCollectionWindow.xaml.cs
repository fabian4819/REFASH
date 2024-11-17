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
using WpfApp_REFASH.ViewModels;

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
        private Admin Admin {  get; set; }
        public AdminCollectionWindow()
        {
            InitializeComponent();
            Admin = AdminSession.CurrentAdmin;
            LoadCollections();
            DataContext = this;
            StatusFilter.SelectedIndex = 0; // Select "All" by default
            upperBar.WelcomeName = Admin.Name;
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
}
