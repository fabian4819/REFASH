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
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CollectionControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty BitmapImageProperty =
    DependencyProperty.Register(
        "BitmapImage",
        typeof(BitmapImage),
        typeof(CollectionControl),
        new PropertyMetadata(null));

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(VerificationStatus), typeof(CollectionControl), new PropertyMetadata(VerificationStatus.InVerification));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public BitmapImage BitmapImage
        {
            get { return (BitmapImage)GetValue(BitmapImageProperty); }
            set { SetValue(BitmapImageProperty, value); }
        }

        public VerificationStatus Status
        {
            get { return (VerificationStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public CollectionControl()
        {
            InitializeComponent();
        }
    }

}

