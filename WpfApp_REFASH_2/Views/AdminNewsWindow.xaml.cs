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
    
    public partial class AdminNewsWindow : Window, INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        private Grid modalBackground;
        private AddNewsCard addNewsCard;

        private ObservableCollection<Content> _contentItem;
        public ObservableCollection<Content> ContentItem
        {
            get => _contentItem;
            set
            {
                if (_contentItem != value)
                {
                    _contentItem = value;
                    OnPropertyChanged(nameof(ContentItem));
                }
            }
        }
        private Admin Admin { get; set; }
        public AdminNewsWindow()
        {
            InitializeComponent();
            Admin = AdminSession.CurrentAdmin;

            if (Admin == null)
            {
                MessageBox.Show("No Admin is logged in.");
                Close();
                return;
            }

            ContentItem = Admin.GetAllContent();
            DataContext = this;
        }

        public void ReloadContent()
        {
            ContentItem = Admin.GetAllContent();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddNewsButton_Click(object sender, RoutedEventArgs e)
        {
            ModalContainer.Visibility = Visibility.Visible;
            //MainGrid.Effect = (Effect)FindResource("BackgroundBlurEffect");
            var addNewsCard = AddNewsCardModal as AddNewsCard;
            addNewsCard.OnSave += AddNewsCard_OnSave;
            addNewsCard.OnCancel += AddNewsCard_OnCancel;
        }
        private void AddNewsCard_OnSave(object sender, ProductEventArgs e)
        {
            ModalContainer.Visibility = Visibility.Collapsed;
            //MainGrid.Effect = null;

            // Optionally, reload content or add the new content to your list
            ReloadContent();
        }
        private void AddNewsCard_OnCancel(object sender, RoutedEventArgs e)
        {
            ModalContainer.Visibility = Visibility.Collapsed;
            //MainGrid.Effect = null;
        }
    }
}
