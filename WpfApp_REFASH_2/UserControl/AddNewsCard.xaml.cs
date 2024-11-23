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
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for AddNewsCard.xaml
    /// </summary>

    public partial class AddNewsCard : UserControl
    {
        public event EventHandler<ProductEventArgs> OnSave;
        public event RoutedEventHandler OnCancel;

        // Remove the local variable definition here since we are already using this field.
        public Content CurrentContent { get; private set; }

        private bool IsNewContent;
        private byte[] currentImageData;

        public AddNewsCard()
        {
            InitializeComponent();
            IsNewContent = true; // Default is new content
        }

        // Method to initialize the control when editing an existing content
        public void InitializeForEdit(Content contentToEdit)
        {
            IsNewContent = false;
            CurrentContent = contentToEdit;
            tb_title.Text = contentToEdit.title;
            tb_description.Text = contentToEdit.description;
            tb_imageUrl.Text = contentToEdit.imagePath;
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                // Use CurrentContent rather than declaring a new variable
                CurrentContent = new Content
                {
                    title = tb_title.Text,
                    description = tb_description.Text,
                    imagePath = tb_imageUrl.Text,
                    imageData = currentImageData
                };

                // AdminSession and AddContent should handle saving or updating
                if (IsNewContent)
                {
                    AdminSession.CurrentAdmin.AddContent(CurrentContent);
                }
                else
                {
                    AdminSession.CurrentAdmin.EditContent(CurrentContent);
                }

                OnSave?.Invoke(this, new ProductEventArgs
                {
                    Content = CurrentContent,
                    IsNewContent = IsNewContent
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving content: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            // Form validation checks
            if (string.IsNullOrWhiteSpace(tb_title.Text))
            {
                MessageBox.Show("Title is required", "Validation Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(tb_description.Text))
            {
                MessageBox.Show("Description is required", "Validation Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(tb_imageUrl.Text))
            {
                MessageBox.Show("Image URL is required", "Validation Error");
                return false;
            }
            return true;
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke(this, new RoutedEventArgs());
        }

        private void btn_browseImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Title = "Select News Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tb_imageUrl.Text = openFileDialog.FileName;
                try
                {
                    currentImageData = File.ReadAllBytes(openFileDialog.FileName);
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
