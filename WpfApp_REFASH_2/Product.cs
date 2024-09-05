using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    internal class Product
    {
        public string productID { get; set; }
        public string name { get; set; }
        public byte[] image { get; set; }
        public string price { get; set; }
        public string category { get; set; }
        public string size { get; set; }
        public string stock { get; set; }
        public string description { get; set; }
    }
}
