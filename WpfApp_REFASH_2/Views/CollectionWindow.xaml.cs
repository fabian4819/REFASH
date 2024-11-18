﻿using System;
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
using WpfApp_REFASH.ViewModels;

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
        public Customer Customer { get; private set; }
        private Grid modalBackground;
        private AddCollectionDialog addDialog;
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

        public CollectionWindow(Customer customer)
        {
            InitializeComponent();
            InitializeModalLayer();
            Customer = UserSession.CurrentCustomer;

            // Initialize collections
            RefreshCollections();

            // Subscribe to CollectionChanged event
            Collections.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TotalCollections));
            };

            DataContext = this;

            Customer = UserSession.CurrentCustomer;
            upperBar.WelcomeName = Customer.Name;
        }

        private void RefreshCollections()
        {
            var collections = Customer.GetAllCollections();
            Collections = ConvertToCollectionItems(collections);
        }

        private VerificationStatus MapStatusToVerificationStatus(string status)
        {
            return status switch
            {
                "Verified" => VerificationStatus.Verified,
                "InVerification" => VerificationStatus.InVerification,
                "Rejected" => VerificationStatus.Rejected,
                _ => VerificationStatus.InVerification // Default case
            };
        }

        public ObservableCollection<CollectionItem> ConvertToCollectionItems(ObservableCollection<Collection> collections)
        {
            ObservableCollection<CollectionItem> collectionItems = new ObservableCollection<CollectionItem>();
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

        private void InitializeModalLayer()
        {
            modalBackground = new Grid
            {
                Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
                Visibility = Visibility.Collapsed,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var blurEffect = new System.Windows.Media.Effects.BlurEffect
            {
                Radius = 5,
                KernelType = System.Windows.Media.Effects.KernelType.Gaussian
            };

            addDialog = new AddCollectionDialog();
            addDialog.OnAdd += AddDialog_OnAdd;
            addDialog.OnCancel += AddDialog_OnCancel;
            addDialog.HorizontalAlignment = HorizontalAlignment.Center;
            addDialog.VerticalAlignment = VerticalAlignment.Center;

            modalBackground.IsVisibleChanged += (s, e) =>
            {
                if (modalBackground.Visibility == Visibility.Visible)
                    MainGrid.Children[1].Effect = blurEffect;
                else
                    MainGrid.Children[1].Effect = null;
            };

            Grid.SetColumn(modalBackground, 1); // Set modal ke kolom kedua (content area)
            Grid.SetZIndex(modalBackground, 999);
            modalBackground.Children.Add(addDialog);
            MainGrid.Children.Add(modalBackground);
        }

        private void btn_addCollection_Click(object sender, RoutedEventArgs e)
        {
            modalBackground.Visibility = Visibility.Visible;
        }

        private void AddDialog_OnAdd(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(addDialog.CollectionName))
            {
                MessageBox.Show("Collection name is required", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Create and add new collection
                var newCollection = new Collection(
                    addDialog.CollectionName,
                    addDialog.CollectionDescription,
                    addDialog.CollectionCategory,
                    addDialog.CollectionImagePath
                );

                Customer.AddCollection(newCollection);

                // Refresh the collections list
                RefreshCollections();

                // Close dialog
                modalBackground.Visibility = Visibility.Collapsed;

                // Optional: Show success message
                MessageBox.Show("Collection added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding collection: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddDialog_OnCancel(object sender, RoutedEventArgs e)
        {
            modalBackground.Visibility = Visibility.Collapsed;
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
