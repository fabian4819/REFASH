using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    namespace WpfApp_REFASH.ViewModel
    {
        public class SideBarViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private Customer _customer;
            public Customer Customer
            {
                get => _customer;
                set
                {
                    if (_customer != value)
                    {
                        _customer = value;
                        OnPropertyChanged(nameof(Customer));
                    }
                }
            }

            public ICommand NavigateNewsCommand { get; }
            public ICommand NavigateCollectionCommand { get; }
            public ICommand NavigateShopCommand { get; }

            public SideBarViewModel(Customer customer)
            {
                Customer = customer ?? throw new ArgumentNullException(nameof(customer));

                NavigateNewsCommand = new RelayCommand(OpenNewsWindow);
                NavigateCollectionCommand = new RelayCommand(OpenCollectionWindow);
                NavigateShopCommand = new RelayCommand(OpenShopWindow);
            }

            private void OpenNewsWindow()
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Customer == null)
                    {
                        MessageBox.Show("Customer data is missing. Cannot navigate to news");
                        return;
                    }
                    var newsWindow = new NewsWindow();
                    newsWindow.Show();
                    CloseCurrentWindow();
                });
            }

            private void OpenCollectionWindow()
            {
                if (Customer == null)
                {
                    MessageBox.Show("Customer data is missing.Cannot navigate to collection");
                    return;
                }
                var collectionWindow = new CollectionWindow(Customer);
                collectionWindow.Show();
                CloseCurrentWindow();
            }

            private void OpenShopWindow()
            {
                if (Customer == null)
                {
                    MessageBox.Show("Customer data is missing. Cannot navigate to shop");
                    return;
                }
                var shopWindow = new ShopWindow(Customer);
                shopWindow.Show();
                CloseCurrentWindow();
            }

            private void CloseCurrentWindow()
            {
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
