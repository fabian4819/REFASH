using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class AdminNewsWindow : Window
    {
        public ObservableCollection<Content> ContentItem { get; set; }
        public AdminNewsWindow()
        {
            InitializeComponent();
            if (AdminSession.CurrentAdmin == null)
            {
                MessageBox.Show("No Admin is logged in.");
                Close();
                return;
            }
            ContentItem = AdminSession.CurrentAdmin.GetAllContent();
        }

        private void AddNewsButton_Click(object sender, RoutedEventArgs e)
        {
            // Add your logic to handle the "Add News" button click event
        }

        private void EditNewsButton_Click(object sender, RoutedEventArgs e)
        {
            // Add your logic to handle the "Edit News" button click event
        }
    }
}
