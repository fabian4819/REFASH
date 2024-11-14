using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public void AddCollection(Collection collection)
        {
            using (var conn = _dbManager.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand("INSERT INTO collections (name, description, category, image_path, status, customer_email) VALUES (@name, @desc, @cat, @path, 'In Review', @e)", conn);
                cmd.Parameters.AddWithValue("@name", collection.Name);
                cmd.Parameters.AddWithValue("@desc", collection.Description);
                cmd.Parameters.AddWithValue("@cat", collection.Category);
                cmd.Parameters.AddWithValue("@path", collection.ImagePath);
                cmd.Parameters.AddWithValue("@e", Email);
                cmd.ExecuteNonQuery();
            }
        }


        public ObservableCollection<Product> GetAllProductOffer()
        {
            var products = new ObservableCollection<Product>();
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT name, description, id AS productID, image_path AS image, price, category, size, stock 
                FROM products";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product(
                                    reader.GetString(reader.GetOrdinal("name")),
                                    reader.GetString(reader.GetOrdinal("description")),
                                    reader.GetInt32(reader.GetOrdinal("productID")),
                                    reader.GetString(reader.GetOrdinal("image")),
                                    reader.GetDecimal(reader.GetOrdinal("price")),
                                    reader.GetString(reader.GetOrdinal("category")),
                                    reader.GetString(reader.GetOrdinal("size")),
                                    reader.GetInt32(reader.GetOrdinal("stock"))
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching products from the database: " + ex.Message);
            }

            return products;
        }

        public ObservableCollection<Product> GetAllProductCart()
        {
            var products = new ObservableCollection<Product>();
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                select p.name as name, p.description as description, c.id as productID, p.image_path as image, p.price as price, p.category as category, p.size as size, c.quantity as stock from carts as c
	JOIN products as p ON c.product_id = p.id";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product(
                                    reader.GetString(reader.GetOrdinal("name")),
                                    reader.GetString(reader.GetOrdinal("description")),
                                    reader.GetInt32(reader.GetOrdinal("productID")),
                                    reader.GetString(reader.GetOrdinal("image")),
                                    reader.GetDecimal(reader.GetOrdinal("price")),
                                    reader.GetString(reader.GetOrdinal("category")),
                                    reader.GetString(reader.GetOrdinal("size")),
                                    reader.GetInt32(reader.GetOrdinal("stock"))
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching products from the database: " + ex.Message);
            }

            return products;
        }

        public void AddToCart(int product_id, int qty)
        {
            using (var conn = _dbManager.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // Ensure the function name and parameter placeholders are correct
                        var cmd = new NpgsqlCommand("SELECT add_to_cart(@customer_email, @qty, @product_id, @create_at, @update_at)", conn);
                        cmd.Transaction = trans;  // Associate the command with the transaction

                        // Make sure parameter names match those expected by the PostgreSQL function
                        cmd.Parameters.AddWithValue("@customer_email", NpgsqlTypes.NpgsqlDbType.Varchar, Email); // Assuming 'Email' is a class property of type string
                        cmd.Parameters.AddWithValue("@qty", NpgsqlTypes.NpgsqlDbType.Integer, qty);
                        cmd.Parameters.AddWithValue("@product_id", NpgsqlTypes.NpgsqlDbType.Integer, product_id);
                        cmd.Parameters.AddWithValue("@create_at", NpgsqlTypes.NpgsqlDbType.Date, DateTime.Now.Date);
                        cmd.Parameters.AddWithValue("@update_at", NpgsqlTypes.NpgsqlDbType.Date, DateTime.Now.Date);

                        // Execute the command and obtain the result
                        int result = (int)cmd.ExecuteScalar();

                        if (result == 1)
                        {
                            trans.Commit();
                            MessageBox.Show("Insert successful.");
                        }
                        else if (result == 0)
                        {
                            MessageBox.Show("Insert failed: Combination of email and product already exists.");
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                }
            }
        }



        public void DeleteFromCart(int cartId)
        {
            using (var conn = _dbManager.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // Correct command text to use parameter placeholders correctly
                        var cmd = new NpgsqlCommand("DELETE FROM carts WHERE id = @cart_id", conn);
                        cmd.Transaction = trans;  // Ensure the command is enrolled in the transaction

                        // Add the parameter for the cart ID correctly
                        cmd.Parameters.AddWithValue("@cart_id", NpgsqlTypes.NpgsqlDbType.Integer, cartId);

                        // Use ExecuteNonQuery for DELETE, UPDATE, and INSERT statements
                        int rowsAffected = cmd.ExecuteNonQuery();  // This returns the number of rows affected

                        if (rowsAffected > 0)
                        {
                            trans.Commit();  // Commit the transaction if the delete was successful
                            MessageBox.Show("Delete successful.");
                        }
                        else
                        {
                            MessageBox.Show("No records deleted.");  // Inform if no records were affected
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();  // Rollback the transaction on error
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                }
            }
        }

    }
}
