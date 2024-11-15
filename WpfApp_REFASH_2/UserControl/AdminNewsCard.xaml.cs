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
    /// <summary>
    /// Interaction logic for AdminNewsCard.xaml
    /// </summary>
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
                    // Simulate content editing
                    content.title = tb_title.Text.Trim(); // Replace with actual editing logic
                    content.description = tb_description.Text.Trim();

                    // Update the content in the database
                    AdminSession.CurrentAdmin.EditContent(content);

                    MessageBox.Show("Content updated successfully.");

                    // Reload the parent window's data
                    var parentWindow = Window.GetWindow(this) as AdminNewsWindow;
                    parentWindow?.ReloadContent();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete content: {ex.Message}");
                }
            }
        }

        private void btn_delete_click(object sender, RoutedEventArgs e)
        {
            if(DataContext is Content content)
            {
                try
                {
                    var result = MessageBox.Show(
                    $"Are you sure you want to delete {content.title}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        AdminSession.CurrentAdmin.DeleteContent(content);
                        var parentWindow = Window.GetWindow(this) as AdminNewsWindow;
                        parentWindow?.ReloadContent();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete content: {ex.Message}");
                }
            }
        }
    }
}
