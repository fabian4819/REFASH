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
    public class Customer : User
    {
        private string CustomerID { get; set; }
        private string Address { get; set; }
        private int LoyaltyPoints { get; set; }
        private DatabaseManager _dbManager = new DatabaseManager();
        public ObservableCollection<Content> ContentItem { get; set; }

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
        public ObservableCollection<Content> GetAllContent()
        {
            ObservableCollection<Content> list = new ObservableCollection<Content>
            {
                new Content
                {
                    contentID = 0,
                    title = "Lorem ipsum dolor sit amet",
                    description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                    writer = "Author 1",
                    imagePath = "../Assets/Logo.png"
                },
                new Content
                {
                    contentID = 1,
                    title = "Another News Title",
                    description = "This is another sample description for the news content.",
                    writer = "Author 2",
                    imagePath = "../Assets/Logo-Black-Transparant.png"
                }
            };

            return list;
        }
        public ObservableCollection<Product> GetAllProductOffer()
        {
            ObservableCollection<Product> list = new ObservableCollection<Product>
            {
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 2", "Description 2", "P002", "../Assets/Logo.png", "Rp150.000", "Category B", "M", 5)
            };
            return list;
        }
        public ObservableCollection<Product> GetAllCart()
        {
            ObservableCollection<Product> list = new ObservableCollection<Product>
            {
                new Product("Product 1", "Description 1", "P001", "../Assets/Logo.png", "Rp100.000", "Category A", "L", 10),
                new Product("Product 2", "Description 2", "P002", "../Assets/Logo.png", "Rp150.000", "Category B", "M", 5)
            };
            return list;
        }
        public void AddToCart(string ProductID)
        {

        }
        public void DeleteFromCart(string ProductID)
        {

        }
    }
}
