﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using WpfApp_REFASH.DataAccess;

namespace WpfApp_REFASH
{
    // Inheritance (User)
    internal class Admin : User
    {
        // Properties for Admin
        public string AdminID { get; private set; }
        private int TotalContent { get; set; }
        private DatabaseManager _dbManager = new DatabaseManager();
        private ObservableCollection<Content> ContentItem { get; set; }
        public ObservableCollection<Collection> Collections { get; set; }

        // Constructor for Admin
        public Admin(string name, string email, string phoneNumber, string password, string role, string adminID)
           : base(name, email, phoneNumber, password, role)
        {
            AdminID = adminID;
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
                    using (var cmd = new NpgsqlCommand("SELECT id FROM admins WHERE email = @e", conn))
                    {
                        cmd.Parameters.AddWithValue("@e", email);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var adminId = reader.IsDBNull(0) ? null : reader.GetString(0);
                                AdminID = adminId;

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
    }
}