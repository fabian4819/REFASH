using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        private ObservableCollection<Content> ContentItem { get; set; }
        public ObservableCollection<Collection> collections { get; set; }

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
            ObservableCollection<Content> contents = new ObservableCollection<Content>();
            
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string query = @"
                        SELECT c.id AS contentID, 
                               c.title AS title, 
                               c.description AS description, 
                               u.name AS writer, 
                               c.image_path AS imagePath 
                        FROM contents AS c 
                        JOIN admins AS a ON c.author_email = a.email 
                        JOIN users AS u ON a.email = u.email";

                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var content = new Content
                                        {
                                            contentID = reader.GetInt32(reader.GetOrdinal("contentID")),
                                            title = reader.GetString(reader.GetOrdinal("title")),
                                            description = reader.GetString(reader.GetOrdinal("description")),
                                            writer = reader.GetString(reader.GetOrdinal("writer")),
                                            imagePath = reader.GetString(reader.GetOrdinal("imagePath"))
                                        };
                                        contents.Add(content);
                                    }
                                }
                            }

                            transaction.Commit(); // Commit transaction after successful execution
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // Rollback transaction on error
                            throw; // Re-throw the exception to handle it outside or log it
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log exception related to connection issues
                Console.WriteLine("Database connection or transaction error: " + ex.Message);
            }

            return contents;
        }
        public ObservableCollection<Collection> GetAllCollections()
        {
            ObservableCollection<Collection> collections = new ObservableCollection<Collection>();

            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string query = @"
                        SELECT id AS collectionID, 
                               name, 
                               description, 
                               status, 
                               category,
                               image_path
                        FROM collections 
                        WHERE customer_email = @Email";

                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Email", Email);

                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var collection = new Collection(
                                            reader.GetString(reader.GetOrdinal("name")),
                                            reader.GetString(reader.GetOrdinal("description")),
                                            reader.GetInt32(reader.GetOrdinal("collectionID")),
                                            reader.IsDBNull(reader.GetOrdinal("status")) ? null : reader.GetString(reader.GetOrdinal("status")),
                                            reader.IsDBNull(reader.GetOrdinal("category")) ? null : reader.GetString(reader.GetOrdinal("category")),
                                            reader.IsDBNull(reader.GetOrdinal("image_path")) ? null : reader.GetString(reader.GetOrdinal("image_path"))
                                        );
                                        collections.Add(collection);
                                    }
                                }
                            }

                            transaction.Commit(); // Commit the transaction after successful execution
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // Rollback the transaction on error
                            throw; // Optionally re-throw to handle higher up
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database connection or transaction error: " + ex.Message);
            }

            return collections;
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
