using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace WpfApp_REFASH
{
    internal class Admin
    {
        public string adminID { get; set; }
        public int totalContent { get; set; }
        public void addContent(string title, byte[] image, string description)
        {
            totalContent++;
        }   
        public void removeContent(string contentID)
        {
            totalContent--;
        }
        public void updateContent(string contentID, string title, byte[] image, string description)
        {
            // Update content
        }
        public void verifyCollection(string collectionID)
        {
            // Verify collection
        }
        public void addProduct(string name, byte[] image, string price, string category, string size, int stock, string description)
        {
            // Add product
        }
        public void removeProduct(string productID)
        {
            // Remove product
        }
        public void updateProduct(string productID, string name, byte[] image, string price, string category, string size, int stock, string decription) 
        {
            // Update product
        }
    }
}
