using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp_REFASH.ViewModels
{
    public class CollectionViewModel : INotifyPropertyChanged
    {
        private readonly Customer _customer;
        private bool _isModalVisible;
        private AddCollectionDialog _addDialog;

        public ObservableCollection<CollectionItem> Collections { get; private set; }
        public int TotalCollections => Collections?.Count ?? 0;
        public bool IsModalVisible
        {
            get => _isModalVisible;
            set
            {
                _isModalVisible = value;
                OnPropertyChanged(nameof(IsModalVisible));
            }
        }

        public ICommand AddCollectionCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ConfirmAddCollectionCommand { get; }

        public CollectionViewModel(Customer customer)
        {
            _customer = customer;
            Collections = ConvertToCollectionItems(_customer.GetAllCollections());

            AddCollectionCommand = new RelayCommand(OpenAddCollectionDialog);
            CancelCommand = new RelayCommand(CloseAddCollectionDialog);
            ConfirmAddCollectionCommand = new RelayCommand(AddNewCollection);

            Collections.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalCollections));
        }

        private void OpenAddCollectionDialog()
        {
            _addDialog = new AddCollectionDialog();
            IsModalVisible = true;
        }

        private void CloseAddCollectionDialog()
        {
            IsModalVisible = false;
        }

        private void AddNewCollection()
        {
            var newCollection = new Collection(
                _addDialog.CollectionName,
                _addDialog.CollectionDescription,
                _addDialog.CollectionCategory,
                _addDialog.CollectionImagePath
            );

            _customer.AddCollection(newCollection);
            Collections.Add(new CollectionItem
            {
                Title = newCollection.Name,
                URL = newCollection.ImagePath,
                Status = VerificationStatus.InVerification
            });

            IsModalVisible = false;
            OnPropertyChanged(nameof(TotalCollections));
        }

        private ObservableCollection<CollectionItem> ConvertToCollectionItems(ObservableCollection<Collection> collections)
        {
            var collectionItems = new ObservableCollection<CollectionItem>();
            foreach (var collection in collections)
            {
                collectionItems.Add(new CollectionItem
                {
                    Title = collection.Name,
                    URL = collection.ImagePath,
                    Status = MapStatusToVerificationStatus(collection.Status)
                });
            }
            return collectionItems;
        }

        private VerificationStatus MapStatusToVerificationStatus(string status) =>
            status switch
            {
                "Verified" => VerificationStatus.Verified,
                "InVerification" => VerificationStatus.InVerification,
                "Rejected" => VerificationStatus.Rejected,
                _ => VerificationStatus.InVerification
            };

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
