using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfApp_REFASH.DataAccess;
using WpfApp_REFASH.Utilities;
using WpfApp_REFASH.ViewModels;

namespace WpfApp_REFASH
{
    public class User
    {
        public string Name { get; set; }
        protected string Email { get; set; }
        protected string PhoneNumber { get; set; }
        protected string Password { get; set; }
        protected string Role { get; set; }
        public string Address { get; set; }
        private DatabaseManager _dbManager = new DatabaseManager();

        // Modified Constructor to include address
        public User(string name, string email, string phoneNumber, string password, string role, string address = null)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Role = role;
            Address = address;
        }

        public User(string email, string password)
        {
            Email = email;
            Password = password;
        }

        // Modified GetData to include address
        protected virtual (bool, string, string, string, string, string) GetData(string email)
        {
            if (_dbManager == null)
            {
                return (false, "Database Manager not initialized", null, null, null, null);
            }
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT name, role, phone_number, password FROM users WHERE email = @e", conn))
                    {
                        cmd.Parameters.AddWithValue("@e", email);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var name = reader.GetString(0);
                                var role = reader.GetString(1);
                                var phoneNumber = reader.GetString(2);
                                var dbPassword = reader.GetString(3);
                                return (true, name, role, phoneNumber, dbPassword, null);
                            }
                            else
                            {
                                return (false, "Email not found", null, null, null, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during data retrieval: {ex.Message}");
                return (false, "Error during data retrieval", null, null, null, null);
            }
        }

        // Modified Login to include address
        public (bool, string, string, string, string) Login(string email, string password)
        {
            var (userFound, name, role, phoneNumber, dbPassword, address) = GetData(email);
            if (!userFound)
            {
                return (false, "User not found.", null, null, null);
            }
            var inputPasswordHash = SecurityUtils.HashPassword(password);
            if (SecurityUtils.PasswordComparison(inputPasswordHash, dbPassword))
            {
                Name = name;
                Role = role;
                PhoneNumber = phoneNumber;
                return (true, "Login successful", name, role, phoneNumber);
            }
            else
            {
                return (false, "Incorrect password", null, null, null);
            }
        }

        // Modified Register to include address
        public bool Register()
        {
            NpgsqlTransaction transaction = null;
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    // Modified INSERT query to include address
                    using (var cmd = new NpgsqlCommand("INSERT INTO users (name, email, phone_number, password, role) VALUES (@n, @e, @ph, @p, @r) RETURNING email", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@n", Name);
                        cmd.Parameters.AddWithValue("@e", Email);
                        cmd.Parameters.AddWithValue("@ph", PhoneNumber);
                        cmd.Parameters.AddWithValue("@p", SecurityUtils.HashPassword(Password));
                        cmd.Parameters.AddWithValue("@r", Role);
                        var userEmail = cmd.ExecuteScalar()?.ToString();
                        if (string.IsNullOrEmpty(userEmail))
                        {
                            throw new Exception("Failed to insert user data");
                        }
                    }
                    if (Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var cmd = new NpgsqlCommand("INSERT INTO admins (email) VALUES (@e)", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@e", Email);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (Role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var cmd = new NpgsqlCommand("INSERT INTO customers (email, address) VALUES (@e, @a)", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@e", Email);
                            cmd.Parameters.AddWithValue("@a", Address ?? (object)DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Console.WriteLine("Error during registration: " + ex.Message);
                return false;
            }
        }


        public virtual ObservableCollection<Content> GetAllContent()
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
                               c.image_path AS imagePath,
                               c.image_data AS imageData
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
                                            bitmapImage = reader.IsDBNull(reader.GetOrdinal("imageData")) ? null : ConvertToBitmapImage((byte[])reader["imageData"])
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
        public Content GetContentById(int contentID)
        {
            Content content = null;
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
                       c.image_data AS imageData
                FROM contents AS c 
                JOIN admins AS a ON c.author_email = a.email 
                JOIN users AS u ON a.email = u.email
                WHERE c.id = @contentID";

                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@contentID", contentID);

                                using (var reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        content = new Content
                                        {
                                            contentID = reader.GetInt32(reader.GetOrdinal("contentID")),
                                            title = reader.GetString(reader.GetOrdinal("title")),
                                            description = reader.GetString(reader.GetOrdinal("description")),
                                            writer = reader.GetString(reader.GetOrdinal("writer")),
                                            bitmapImage = reader.IsDBNull(reader.GetOrdinal("imageData")) ? null : ConvertToBitmapImage((byte[])reader["imageData"])
                                        };
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

            return content;
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
                               image_path,
                               image_data
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
                                            reader.IsDBNull(reader.GetOrdinal("image_path")) ? null : reader.GetString(reader.GetOrdinal("image_path")),
                                            reader.IsDBNull(reader.GetOrdinal("image_data")) ? null : ConvertToBitmapImage((byte[])reader["image_data"])
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

        public virtual ObservableCollection<Product> GetAllProductOffer()
        {
            var products = new ObservableCollection<Product>();
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT name, description, id AS productID, image_data AS imageData, price, category, size, stock 
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
                                    reader.IsDBNull(reader.GetOrdinal("imageData")) ? null : ConvertToBitmapImage((byte[])reader["imageData"]),
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

        //Logout
        public void Logout()
        {
            try
            {
                // Reset semua properti dasar User
                Name = null;
                Email = null;
                PhoneNumber = null;
                Password = null;
                Address = null;

                // Reset Session berdasarkan Role
                if (Role?.ToLower() == "admin")
                {
                    UserSession.CurrentAdmin = null;
                }
                else if (Role?.ToLower() == "customer")
                {
                    UserSession.CurrentCustomer = null;
                }

                // Reset Role setelah pengecekan
                Role = null;

                MessageBox.Show("Successfully logged out!", "Logout Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during logout: {ex.Message}", "Logout Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public BitmapImage ConvertToBitmapImage(byte[] imageData)
        {
            using (var ms = new MemoryStream(imageData))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze(); // Important for use in a WPF environment
                return image;
            }
        }
    }
}
