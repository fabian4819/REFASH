using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using WpfApp_REFASH.DataAccess;

namespace WpfApp_REFASH
{
    // Inheritance (User)
    internal class Admin : User
    {
        //Property for Admin
        public string AdminID { get; private set; }
        private int TotalContent { get; set; }
        //Constructor for Admin
        public Admin(string name, string email, string phoneNumber, string password, string role, string adminID)
           : base(name, email, phoneNumber, password, role)
        {
            AdminID = adminID;
            TotalContent = 0;
        }
        // Override Method UpdateProfile for Admin (Polymorphism)
        public override void UpdateProfile()
        {
            Console.WriteLine($"Admin {Name}'s profile updated with admin ID: {AdminID}.");
        }
        // Add Content
        public void AddContent(string title, byte[] image, string description)
        {
            TotalContent++;
            Console.WriteLine($"Content '{title}' added by {Name}. Total content now: {TotalContent}.");
        }
        // Remove Content
        public void RemoveContent(string contentID)
        {
            TotalContent--;
            Console.WriteLine($"Content {contentID} removed by {Name}. Total content now: {TotalContent}.");
        }
        // Update Content
        public void UpdateContent(string contentID, string title, byte[] image, string description)
        {
            Console.WriteLine($"Content {contentID} updated by {Name}.");   
        }
        // Verify Collection
        public void VerifyCollection(string collectionID)
        {
            Console.WriteLine($"Collection {collectionID} verified by Admin {Name}.");
        }
        // Add Product
        public void AddProduct(string name, byte[] image, string price, string category, string size, int stock, string description)
        {
            Console.WriteLine($"Product {name} added by {Name}.");  
        }
        // Remove Product
        public void RemoveProduct(string productID)
        {
            Console.WriteLine($"Product {productID} removed by {Name}.");
        }
        // Update Product
        public void UpdateProduct(string productID, string name, byte[] image, string price, string category, string size, int stock, string decription) 
        {
            Console.WriteLine($"Product {productID} updated by {Name}.");
        }
    }
}
