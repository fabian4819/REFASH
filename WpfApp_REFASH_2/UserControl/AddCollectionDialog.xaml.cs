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
        //public VerificationStatus CollectionStatus
        //{
        //    get
        //    {
        //        switch (StatusComboBox.SelectedIndex)
        //        {
        //            case 0:
        //                return VerificationStatus.Verified;
        //            case 2:
        //                return VerificationStatus.Rejected;
        //            default:
        //                return VerificationStatus.InVerification;
        //        }
        //    }
        //}

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
    }
}