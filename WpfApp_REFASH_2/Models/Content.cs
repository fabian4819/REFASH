using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfApp_REFASH
{
    public class Content
    {
        public int contentID { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string writer { get; set; }
        public string imagePath { get; set; }
        public Customer Customer { get; set; }
        public BitmapImage bitmapImage { get; set; }
        public byte[] imageData { get; set; }
    }
}
