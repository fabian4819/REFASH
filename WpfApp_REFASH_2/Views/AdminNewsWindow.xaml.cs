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
    /// Interaction logic for AdminNewsWindow.xaml
    /// </summary>
    public partial class AdminNewsWindow : Window, INotifyPropertyChanged
    {
        // Implement INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

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

        public AdminNewsWindow()
        {
            InitializeComponent();

            if (AdminSession.CurrentAdmin == null)
            {
                MessageBox.Show("No Admin is logged in.");
                Close();
                return;
            }

            // Load initial data
            ContentItem = AdminSession.CurrentAdmin.GetAllContent();

            // Set the DataContext for binding
            DataContext = this;
        }
        public void ReloadContent()
        {
            ContentItem = AdminSession.CurrentAdmin.GetAllContent();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddNewsButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic for adding news
            MessageBox.Show("Add News functionality is not yet implemented.");
        }
    }
}
