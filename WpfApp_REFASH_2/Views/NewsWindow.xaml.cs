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

namespace WpfApp_REFASH
{
    /// <summary>
    /// Interaction logic for NewsWindow.xaml
    /// </summary>
    public partial class NewsWindow : Window
    {
        public ObservableCollection<Content> ContentItem { get; set; }
        public NewsWindow()
        {
            InitializeComponent();

            ContentItem = new ObservableCollection<Content>
        {
            new Content
            {
                contentID = 0,
                title = "Lorem ipsum dolor sit amet",
                description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                writer = "Author 1",
                imagePath = "../Assets/Logo.png"
            },
            new Content
            {
                contentID = 1,
                title = "Another News Title",
                description = "This is another sample description for the news content.",
                writer = "Author 2",
                imagePath = "../Assets/NewsImage.png"
            }
        };

            DataContext = this;
        }
    }
}
