using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Npgsql;
using WpfApp_REFASH.DataAccess;

namespace WpfApp_REFASH
{
    // Inheritance (User)
    public class Admin : User
    {
        // Properties for Admin
        private int TotalContent { get; set; }
        private DatabaseManager _dbManager = new DatabaseManager();
        private ObservableCollection<Content> ContentItem { get; set; }
        public ObservableCollection<Collection> Collections { get; set; }

        // Constructor for Admin
        public Admin(string name, string email, string phoneNumber, string password, string role)
           : base(name, email, phoneNumber, password, role)
        {
            TotalContent = 0;
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
                    using (var cmd = new NpgsqlCommand("SELECT email FROM admins WHERE email = @e", conn))
                    {
                        cmd.Parameters.AddWithValue("@e", email);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var adminId = reader.IsDBNull(0) ? null : reader.GetString(0);

                                return (true, name, role, phoneNumber, dbPassword);
                            }
                            else
                            {
                                return (false, "Admin data not found", null, null, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during admin data retrieval: {ex.Message}");
                return (false, "Error during admin data retrieval", null, null, null);
            }
        }

        public override ObservableCollection<Content> GetAllContent()
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
                JOIN users AS u ON a.email = u.email
                WHERE c.author_email = @email";

                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@email", Email);
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
        public void EditContent(Content content)
        {
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
                        UPDATE contents
                        SET title = @title,
                            description = @description,
                            image_path = @imagePath
                        WHERE id = @contentID";

                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@contentID", content.contentID);
                                cmd.Parameters.AddWithValue("@title", content.title);
                                cmd.Parameters.AddWithValue("@description", content.description);
                                cmd.Parameters.AddWithValue("@imagePath", content.imagePath ?? (object)DBNull.Value);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    throw new Exception("No rows were updated. The content may not exist.");
                                }
                            }

                            transaction.Commit(); // Commit transaction after successful execution
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // Rollback transaction on error
                            throw new Exception("Error while updating content: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or display exception for connection issues
                Console.WriteLine("Database connection or transaction error: " + ex.Message);
                throw new Exception("Failed to edit content.", ex);
            }
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
                        FROM collections";

                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
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

        public void UpdateCollectionStatus(Collection collection, string newStatus)
        {
            using (var conn = _dbManager.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    "UPDATE collections SET status = @status WHERE id = @id", conn);

                // Bind parameters
                cmd.Parameters.AddWithValue("@status", newStatus);
                cmd.Parameters.AddWithValue("@id", collection.CollectionID);

                // Execute the update
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Update the in-memory collection status
                    collection.Status = newStatus;

                    // Optional: Notify success
                    MessageBox.Show($"Collection status updated to '{newStatus}'.", "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Notify failure if no rows were affected
                    MessageBox.Show("No rows were updated. Ensure the collection exists.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}