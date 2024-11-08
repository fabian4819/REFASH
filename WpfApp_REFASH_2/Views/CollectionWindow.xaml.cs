using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfApp_REFASH
{
    public enum VerificationStatus
    {
        Verified,
        InVerification,
        Rejected
    }
    public partial class CollectionWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<CollectionItem> _collections;
        public ObservableCollection<CollectionItem> Collections
        {
            get { return _collections; }
            set
            {
                _collections = value;
                OnPropertyChanged(nameof(Collections));
                OnPropertyChanged(nameof(TotalCollections));
            }
        }

        public int TotalCollections
        {
            get { return Collections?.Count ?? 0; }
        }

        public CollectionWindow()
        {
            InitializeComponent();
            // Initialize the collections list with sample data
            Collections = new ObservableCollection<CollectionItem>
{
    new CollectionItem { Title = "Item 1", URL = "../Assets/denimdummy.png", Status = VerificationStatus.Verified },
    new CollectionItem { Title = "Item 2", URL = "../Assets/denimdummy.png", Status = VerificationStatus.InVerification },
    new CollectionItem { Title = "Item 3", URL = "../Assets/denimdummy.png", Status = VerificationStatus.Rejected },
    new CollectionItem { Title = "Item 4", URL = "../Assets/denimdummy.png", Status = VerificationStatus.Verified },
    new CollectionItem { Title = "Item 5", URL = "../Assets/denimdummy.png", Status = VerificationStatus.InVerification },
    new CollectionItem { Title = "Item 6", URL = "../Assets/denimdummy.png", Status = VerificationStatus.Rejected },
    new CollectionItem { Title = "Item 7", URL = "../Assets/denimdummy.png", Status = VerificationStatus.Verified },
    new CollectionItem { Title = "Item 8", URL = "../Assets/denimdummy.png", Status = VerificationStatus.InVerification },
    new CollectionItem { Title = "Item 9", URL = "../Assets/denimdummy.png", Status = VerificationStatus.Rejected },
    new CollectionItem { Title = "Item 10", URL = "../Assets/denimdummy.png", Status = VerificationStatus.Verified }
};

            // Subscribe to CollectionChanged event
            Collections.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TotalCollections));
            };

            DataContext = this;
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Class to represent each collection item
    public class CollectionItem
    {
        public string Title { get; set; }
        public string URL { get; set; }
        public VerificationStatus Status { get; set; }
    }
}
