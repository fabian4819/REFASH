using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class AdminNewsCard : UserControl
    {
        public AdminNewsCard()
        {
            InitializeComponent();
        }

        private void btn_edit_click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Content content)
            {
                try
                {
                    content.title = tb_title.Text?.Trim() ?? string.Empty;
                    content.description = tb_description.Text?.Trim() ?? string.Empty;

                    AdminSession.CurrentAdmin?.EditContent(content);
                    MessageBox.Show("Content updated successfully.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reload parent window's data
                    if (Window.GetWindow(this) is AdminNewsWindow parentWindow)
                    {
                        parentWindow.ReloadContent();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to update content: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btn_delete_click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Content content)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to delete \"{content.title}\"?",
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        AdminSession.CurrentAdmin?.DeleteContent(content);

                        // Reload parent window's data
                        if (Window.GetWindow(this) is AdminNewsWindow parentWindow)
                        {
                            parentWindow.ReloadContent();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete content: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}