using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WpfApp_REFASH.DataAccess;

namespace WpfApp_REFASH
{
    // Inheritance (User)
    internal class Customer: User
    {
        private string CustomerID { get; set; }
        private string Address { get; set; }
        private int LoyaltyPoints { get; set; }
        private DatabaseManager _dbManager = new DatabaseManager();

        public Customer(string name, string email, string phoneNumber, string password, string role)
        : base(name, email, phoneNumber, password, role)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Role = role;
            GetData(Email);
        }

        protected override (bool, string, string, string, string) GetData(string email)
        {
            var (isFound, name, role, phoneNumber, dbPassword) = base.GetData(email);

            if (!isFound)
            {
                return (false, "Email not found in User table", null, null, null);
            }

            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT address, loyalty_points, id FROM customers WHERE email = @e", conn))
                    {
                        cmd.Parameters.AddWithValue("@e", email);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var address = reader.IsDBNull(0) ? null : reader.GetString(0);
                                var loyaltyPoints = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                var customerId = reader.IsDBNull(2) ? null : reader.GetString(2);
                                Address = address;
                                LoyaltyPoints = loyaltyPoints;
                                CustomerID = customerId;

                                return (true, name, role, phoneNumber, dbPassword);
                            }
                            else
                            {
                                return (false, "Customer data not found", null, null, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during customer data retrieval: {ex.Message}");
                return (false, "Error during customer data retrieval", null, null, null);
            }
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
