using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_REFASH
{
    public partial class AddCollectionDialog : UserControl
    {
        public event RoutedEventHandler OnAdd;
        public event RoutedEventHandler OnCancel;
        public string CollectionName => tb_NameTextBox.Text;
        public string CollectionDescription => tb_DescriptionTextBox.Text;
        public string CollectionCategory => (tb_CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        public string CollectionImagePath => tb_ImageURLTextBox.Text;
        public byte[] CollectionImageData;


        public AddCollectionDialog()
        {
            InitializeComponent();
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            OnAdd?.Invoke(this, e);
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke(this, e);
        }
        private void btn_browseImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Title = "Select Product Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tb_ImageURLTextBox.Text = openFileDialog.FileName;
                try
                {
                    CollectionImageData = File.ReadAllBytes(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to read the image file: {ex.Message}", "File Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
    }
}