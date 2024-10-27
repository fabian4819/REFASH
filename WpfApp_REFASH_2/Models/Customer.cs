using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_REFASH
{
    // Inheritance (User)
    internal class Customer: User
    {
        public string CustomerID { get; private set; }
        public string Address { get; set; }
        public int LoyaltyPoints { get; set; }

        //Constructor for Customer
        public Customer(string name, string email, string phoneNumber, string password, string customerID, string address)
        : base(name, email, phoneNumber, password)
        {
            CustomerID = customerID;
            Address = address;
            LoyaltyPoints = 0;
        }

        // Override Method UpdateProfile for Customer (Polymorphism)
        public override void UpdateProfile()
        {
            Console.WriteLine($"Customer {Name}'s profile updated.");
        }

        // (Encapsulation) Check Loyalty Points
        public int CheckLoyaltyPoints()
        {
            return LoyaltyPoints;
        }
        // Add Loyalty Points
        public void AddLoyaltyPoints(int points)
        {
            LoyaltyPoints += points;
            Console.WriteLine($"{points} loyalty points added. Total now: {LoyaltyPoints}.");
        }
        // Add product to cart
        public void AddToCart(string productID)
        {
            Console.WriteLine($"Product {productID} added to cart.");
        }
        // Remove product from cart
        public void RemoveFromCart(string productID)
        {
            Console.WriteLine($"Product {productID} removed from cart.");
        }
        // See Product Detail
        public void SeeProductDetail(string productID)
        {
            Console.WriteLine($"Viewing details for product {productID}.");
        }
        // Checkout
        public void Checkout()
        {
            Console.WriteLine("Checkout completed.");
        }
        // Add to Collection
        public void AddToCollection()
        {
            Console.WriteLine($"Collection added.");
        }
        // Remove from Collection
        public void RemoveFromCollection(string collectionID)
        {
            Console.WriteLine($"Collection {collectionID} removed.");
        }
    }
}
