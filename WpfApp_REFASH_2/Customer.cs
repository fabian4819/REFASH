using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    internal class Customer: User
    {
        private string customerID;
        private string address;
        private int loyaltyPoint;

        public int checkLoyaltyPoint()
        {
            return loyaltyPoint;
        }
        public void addChart(string productID)
        {
            Console.WriteLine($"Product {productID} added to cart.");
        }
        public void removeChart(string productID)
        {
            Console.WriteLine($"Product {productID} removed from cart.");
        }
        public void seeProductDetail(string productID)
        {
            Console.WriteLine($"Viewing details for product {productID}.");
        }
        public void checkout()
        {
            Console.WriteLine("Checkout completed.");
        }
        public void addCollection()
        {
            Console.WriteLine($"Collection added.");
        }
        public void removeCollection(string collectionID)
        {
            Console.WriteLine($"Collection {collectionID} removed.");
        }
    }
}
