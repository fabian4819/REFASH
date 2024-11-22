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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace WpfApp_REFASH
{

    public partial class AdminNewsWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private BlurEffect blurEffect;
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

            // Initialize blur effect
            blurEffect = new BlurEffect
            {
                Radius = 5
            };

            Admin = AdminSession.CurrentAdmin;
            if (Admin == null)
            {
                MessageBox.Show("No Admin is logged in.");
                Close();
                return;
            }

            ContentItem = Admin.GetAllContent();
            DataContext = this;
            upperBar.WelcomeName = Admin.Name;
        }

        public void ReloadContent()
        {
            ContentItem = Admin.GetAllContent();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ApplyBlurEffect()
        {
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 5,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            blurEffect.BeginAnimation(BlurEffect.RadiusProperty, animation);
            ContentContainer.Effect = blurEffect;
        }

        private void RemoveBlurEffect()
        {
            var animation = new DoubleAnimation
            {
                From = 5,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            animation.Completed += (s, e) => ContentContainer.Effect = null;
            blurEffect.BeginAnimation(BlurEffect.RadiusProperty, animation);
        }

        private void AddNewsButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyBlurEffect();
            ModalContainer.Visibility = Visibility.Visible;

            var addNewsCard = AddNewsCardModal as AddNewsCard;
            addNewsCard.OnSave += AddNewsCard_OnSave;
            addNewsCard.OnCancel += AddNewsCard_OnCancel;
        }

        private void AddNewsCard_OnSave(object sender, ProductEventArgs e)
        {
            RemoveBlurEffect();
            ModalContainer.Visibility = Visibility.Collapsed;
            ReloadContent();
        }

        private void AddNewsCard_OnCancel(object sender, RoutedEventArgs e)
        {
            RemoveBlurEffect();
            ModalContainer.Visibility = Visibility.Collapsed;
        }
    }
}
