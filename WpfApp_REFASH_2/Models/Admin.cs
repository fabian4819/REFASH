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
        private int TotalContent { get; set; }
        private DatabaseManager _dbManager = new DatabaseManager();
        private ObservableCollection<Content> ContentItem { get; set; }
        public ObservableCollection<Collection> Collections { get; set; }

        public Admin(string name, string email, string phoneNumber, string password, string role)
           : base(name, email, phoneNumber, password, role)
        {
            TotalContent = 0;
            GetData(Email);
        }

        protected override (bool, string, string, string, string, string) GetData(string email)
        {
            var (isFound, name, role, phoneNumber, dbPassword, address) = base.GetData(email);

            if (!isFound)
            {
                return (false, "Email not found in User table", null, null, null, null);
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
                                return (true, name, role, phoneNumber, dbPassword, null);
                            }
                            else
                            {
                                return (false, "Admin data not found", null, null, null, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during admin data retrieval: {ex.Message}");
                return (false, "Error during admin data retrieval", null, null, null, null);
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
                       c.image_path AS imagePath,
                       c.image_data AS imageData
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
        public void AddContent(Content content)
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
                            INSERT INTO contents (title, description, image_data, author_email) 
                            VALUES (@title, @description, @imageData, @authorEmail)";

                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@title", content.title ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@description", content.description ?? (object)DBNull.Value);
                                cmd.Parameters.Add("@imageData", NpgsqlTypes.NpgsqlDbType.Bytea).Value = content.imageData ?? (object)DBNull.Value;
                                cmd.Parameters.AddWithValue("@authorEmail", Email);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    throw new Exception("No rows were inserted. Check the input data.");
                                }

                                transaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error while adding content: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection or transaction error: " + ex.Message, "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
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
                            description = @description
                        WHERE id = @contentID";

                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@contentID", content.contentID);
                                cmd.Parameters.AddWithValue("@title", content.title ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@description", content.description ?? (object)DBNull.Value);
                                // cmd.Parameters.Add("@imageData", NpgsqlTypes.NpgsqlDbType.Bytea).Value = content.imageData ?? (object)DBNull.Value;
                                cmd.Parameters.AddWithValue("@authorEmail", Email);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    throw new Exception("No rows were updated. Check the input data.");
                                }

                                transaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error while updated content: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            throw;
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
        public void DeleteContent(Content content)
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
                        DELETE FROM contents
                        WHERE id = @contentID";

                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@contentID", content.contentID);

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

        public override ObservableCollection<Product> GetAllProductOffer()
        {
            var products = new ObservableCollection<Product>();
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT name, description, id AS productID, image_path AS image, price, category, size, stock 
                FROM products WHERE admin_email = @e";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@e", Email);
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
        public void AddProductOffer(Product product)
        {
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                INSERT INTO products 
                (name, description, image_path, price, category, size, stock, admin_email)
                VALUES 
                (@name, @description, @image, @price, @category, @size, @stock, @admin_email)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", product.Name);
                        cmd.Parameters.AddWithValue("@description", product.Description);
                        //cmd.Parameters.AddWithValue("@id", product.ProductID);
                        cmd.Parameters.AddWithValue("@image", product.Image ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@category", product.Category);
                        cmd.Parameters.AddWithValue("@size", product.Size);
                        cmd.Parameters.AddWithValue("@stock", product.Stock);
                        cmd.Parameters.AddWithValue("@admin_email", Email); // Assuming `Email` is the logged-in admin's email

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Product added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to add product. Please try again.", "Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product to the database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void EditProduct(Product product)
        {
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                UPDATE products
                SET name = @name,
                    description = @description,
                    image_path = @image,
                    price = @price,
                    category = @category,
                    size = @size,
                    stock = @stock
                WHERE id = @id AND admin_email = @admin_email";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", product.Name);
                        cmd.Parameters.AddWithValue("@description", product.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@image", product.Image ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@category", product.Category);
                        cmd.Parameters.AddWithValue("@size", product.Size ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@stock", product.Stock);
                        cmd.Parameters.AddWithValue("@id", product.ProductID);
                        cmd.Parameters.AddWithValue("@admin_email", Email);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Product updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to update product. Ensure the product exists and you have permission to update it.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product in the database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DeleteProduct(Product product)
        {
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                    DELETE FROM order_details WHERE product_id = @id;
                    DELETE FROM products WHERE id = @id AND admin_email = @admin_email;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", product.Name);
                        cmd.Parameters.AddWithValue("@description", product.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@image", product.Image ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@price", product.Price);
                        cmd.Parameters.AddWithValue("@category", product.Category);
                        cmd.Parameters.AddWithValue("@size", product.Size ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@stock", product.Stock);
                        cmd.Parameters.AddWithValue("@id", product.ProductID);
                        cmd.Parameters.AddWithValue("@admin_email", Email);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to deleted product. Ensure the product exists and you have permission to update it.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product in the database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public int GetSoldProductsCount()
        {
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                    SELECT COALESCE(SUM(od.quantity), 0)
                    FROM order_details od
                    JOIN products p ON od.product_id = p.id
                    WHERE p.admin_email = @email";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", Email);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting sold products count: {ex.Message}");
                return 0;
            }
        }

        public ObservableCollection<Order> GetAllOrders()
        {
            var orders = new ObservableCollection<Order>();
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                    SELECT DISTINCT 
                        o.id AS order_id,
                        o.customer_email,
                        o.create_at AS order_date,
                        o.status,
                        COALESCE(SUM(p.price * od.quantity) OVER (PARTITION BY o.id), 0) as total_amount
                    FROM orders o
                    JOIN order_details od ON o.id = od.order_id
                    JOIN products p ON od.product_id = p.id
                    WHERE p.admin_email = @email
                    ORDER BY o.create_at DESC";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", Email);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                orders.Add(new Order
                                {
                                    OrderId = reader.GetInt32(reader.GetOrdinal("order_id")),
                                    CustomerEmail = reader.GetString(reader.GetOrdinal("customer_email")),
                                    OrderDate = reader.GetDateTime(reader.GetOrdinal("order_date")),
                                    Status = reader.GetString(reader.GetOrdinal("status")),
                                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("total_amount"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching orders: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return orders;
        }


        public List<DailySales> GetDailySalesData()
        {
            var salesData = new List<DailySales>();
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                    SELECT 
                        DATE(o.create_at) as sale_date,
                        SUM(p.price * od.quantity) as daily_total
                    FROM orders o
                    JOIN order_details od ON o.id = od.order_id
                    JOIN products p ON od.product_id = p.id
                    WHERE p.admin_email = @email
                    AND o.create_at >= CURRENT_DATE - INTERVAL '30 days'
                    GROUP BY DATE(o.create_at)
                    ORDER BY sale_date";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", Email);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                salesData.Add(new DailySales
                                {
                                    Date = reader.GetDateTime(0),
                                    Amount = reader.GetDecimal(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching sales data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return salesData;
        }


        public void UpdateOrderStatus(int orderId, string newStatus)
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
                            UPDATE orders 
                            SET status = @status,
                                update_at = CURRENT_TIMESTAMP
                            WHERE id = @id";

                            using (var cmd = new NpgsqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@status", newStatus);
                                cmd.Parameters.AddWithValue("@id", orderId);

                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected == 0)
                                {
                                    throw new Exception("Order not found");
                                }
                            }
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update order status: {ex.Message}");
            }
        }
    }
}