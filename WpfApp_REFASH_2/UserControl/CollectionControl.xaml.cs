using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class CollectionControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(CollectionControl), new PropertyMetadata(null));

        public static readonly DependencyProperty URLProperty = DependencyProperty.Register(
            nameof(URL), typeof(string), typeof(CollectionControl), new PropertyMetadata(null));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string URL
        {
            get { return (string)GetValue(URLProperty); }
            set { SetValue(URLProperty, value); }
        }

        public CollectionControl()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                Debug.WriteLine($"Title: {Title}, URL: {URL}");
            };
        }

    }

}

