using System;
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

namespace WpfApp_REFASH
{
    public partial class CollectionWindow : Window
    {
        public ObservableCollection<CollectionItem> Collections { get; set; }

        public CollectionWindow()
        {
            InitializeComponent();

            // Initialize the collections list with sample data
            Collections = new ObservableCollection<CollectionItem>
        {
            new CollectionItem { Title = "Item 1", URL = "../Assets/denimdummy.png" },
            new CollectionItem { Title = "Item 2", URL = "../Assets/denimdummy.png" },
            new CollectionItem { Title = "Item 3", URL = "../Assets/denimdummy.png" },
            new CollectionItem { Title = "Item 4", URL = "../Assets/denimdummy.png" },
            new CollectionItem { Title = "Item 5", URL = "../Assets/denimdummy.png" },
            new CollectionItem { Title = "Item 6", URL = "../Assets/denimdummy.png" },
            new CollectionItem { Title = "Item 7", URL = "../Assets/denimdummy.png" },
            new CollectionItem { Title = "Item 8", URL = "../Assets/denimdummy.png" },
            new CollectionItem { Title = "Item 9", URL = "../Assets/denimdummy.png" }
        };

            DataContext = this;
        }
    }

    // Class to represent each collection item
    public class CollectionItem
    {
        public string Title { get; set; }
        public string URL { get; set; }
    }
}
