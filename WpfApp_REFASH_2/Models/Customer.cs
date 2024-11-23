using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        // Remove Address property since it's now in base class
        private int LoyaltyPoints { get; set; }
        private DatabaseManager _dbManager = new DatabaseManager();
        private ObservableCollection<Content> ContentItem { get; set; }
        public ObservableCollection<Collection> collections { get; set; }

        public Customer(string name, string email, string phoneNumber, string password, string role, string address = null)
        : base(name, email, phoneNumber, password, role)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Role = role;
            GetData(Email);
            // Initialize cart items
            CartItems = GetAllProductCart();
        }
        // Add CartItems property
        public ObservableCollection<Product> CartItems
        {
            get
            {
                // Lazy loading of cart items
                if (_cartItems == null)
                {
                    _cartItems = GetAllProductCart();
                }
                return _cartItems;
            }
            private set
            {
                _cartItems = value;
            }
        }
        private ObservableCollection<Product> _cartItems;

        public int GetCartItemCount()
        {
            return CartItems?.Count ?? 0;
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
                    using (var cmd = new NpgsqlCommand("SELECT loyalty_point, address FROM customers WHERE email = @e", conn))
                    {
                        cmd.Parameters.AddWithValue("@e", email);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var loyaltyPoints = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                Address = reader.IsDBNull(1) ? null : reader.GetString(1);
                                LoyaltyPoints = loyaltyPoints;

                                return (true, name, role, phoneNumber, dbPassword, address);
                            }
                            else
                            {
                                return (false, "Customer data not found", null, null, null, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during customer data retrieval: {ex.Message}");
                return (false, "Error during customer data retrieval", null, null, null, null);
            }
        }
        public void AddCollection(Collection collection)
        {
            using (var conn = _dbManager.GetConnection())
            {
                conn.Open();
                var cmd = new NpgsqlCommand("INSERT INTO collections (name, description, category, image_data, status, customer_email) VALUES (@name, @desc, @cat, @data, 'In Review', @e)", conn);
                cmd.Parameters.AddWithValue("@name", collection.Name);
                cmd.Parameters.AddWithValue("@desc", collection.Description);
                cmd.Parameters.AddWithValue("@cat", collection.Category);
                cmd.Parameters.AddWithValue("@data", collection.ImageData);
                cmd.Parameters.AddWithValue("@e", Email);
                cmd.ExecuteNonQuery();
            }
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
                select p.name as name, p.description as description, c.id as productID, p.image_data as imageData, p.price as price, p.category as category, p.size as size, c.quantity as stock from carts as c
	JOIN products as p ON c.product_id = p.id WHERE c.customer_email = @e";
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
        public void Checkout(ObservableCollection<Product> CartItems)
        {
            using (var conn = _dbManager.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // Membuat order pada tabel order id
                        string insertOrderQUery = "INSERT INTO orders (customer_email, create_at, status) VALUES (@customerEmail, @createAt, 'Packaging') RETURNING id;";
                        int orderId;
                        using (var cmd = new NpgsqlCommand(insertOrderQUery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@customerEmail", Email);
                            cmd.Parameters.AddWithValue("@createAt", DateTime.Now);
                            orderId = (int)cmd.ExecuteScalar();
                        }
                        MessageBox.Show("Create Order Successfully");
                        // Memasukkan setiap product yang ada di cart ke order detail dan update stock
                        foreach (var product in CartItems)
                        {
                            string checkStockQuery = "SELECT stock FROM products AS p JOIN carts AS c ON p.id = c.product_id WHERE c.id = @cart_id;";
                            int currentStock;
                            using ( var cmd = new NpgsqlCommand(checkStockQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@cart_id", product.ProductID);
                                currentStock = (int)cmd.ExecuteScalar();
                            }
                            if (currentStock < product.Stock)
                            {
                                throw new Exception($"Insufficient stock for product: {product.Name}. Avaliable: {currentStock}, Requested: {product.Stock}");
                            }
                            // Memasukkan ke order detail
                            string insertOrderDetailQuery = "INSERT INTO order_details (order_id, product_id, quantity) SELECT @orderId, product_id, @quantity FROM carts WHERE id = @cartId;";
                            using (var cmd = new NpgsqlCommand(insertOrderDetailQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@orderId", orderId);
                                cmd.Parameters.AddWithValue("@cartId", product.ProductID);
                                cmd.Parameters.AddWithValue("@quantity", product.Stock);
                                cmd.ExecuteNonQuery();
                            }
                            //MessageBox.Show("insert order detail successfully");
                            // Memperbarui product stock
                            string updateStockQuery = "UPDATE products SET stock = stock - @quantity WHERE id = (SELECT product_id FROM carts WHERE carts.id = @cartId);";
                            using (var cmd = new NpgsqlCommand(updateStockQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@quantity", product.Stock);
                                cmd.Parameters.AddWithValue("@cartId", product.ProductID);
                                cmd.ExecuteNonQuery();
                            }
                            //MessageBox.Show("Update product stock successfully");
                            // Hapus cart
                            string deleteFromCartQuery = "DELETE FROM carts WHERE customer_email = @customerEmail AND id = @cartId;";
                            using (var cmd = new NpgsqlCommand(deleteFromCartQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@customerEmail", Email);
                                cmd.Parameters.AddWithValue("@cartId", product.ProductID);
                                cmd.ExecuteNonQuery();
                            }
                            //MessageBox.Show("delete cart successfully");
                        }
                        // commit the transaction
                        trans.Commit();
                        //MessageBox.Show("Checkout successfull");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show($"Checkout failed: {ex.Message}");
                        throw;
                    }
                }
            }
        }
        public ObservableCollection<Transaction> GetAllTransactions()
        {
            var transactions = new ObservableCollection<Transaction>();
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT o.id AS OrderID, 
                       o.create_at AS OrderDate, 
                       p.name AS ProductName, 
                       p.description AS Description, 
                       p.image_data AS ImageData, 
                       p.price AS Price, 
                       p.category AS Category, 
                       p.size AS Size, 
                       od.quantity AS Quantity, 
                       (od.quantity * p.price) AS TotalPricePerItem, 
                       SUM(od.quantity * p.price) OVER (PARTITION BY o.id) AS TotalOrderPrice,
                       o.status AS Status
                FROM orders AS o
                JOIN order_details AS od ON o.id = od.order_id
                JOIN products AS p ON p.id = od.product_id
                WHERE o.customer_email = @e";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@e", Email);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                transactions.Add(new Transaction(
                                    orderID: reader.GetInt32(reader.GetOrdinal("OrderID")),
                                    orderDate: reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                    productName: reader.GetString(reader.GetOrdinal("ProductName")),
                                    description: reader.GetString(reader.GetOrdinal("Description")),
                                    bitmapImage: reader.IsDBNull(reader.GetOrdinal("ImageData")) ? null : ConvertToBitmapImage((byte[])reader["ImageData"]),
                                    price: reader.GetDecimal(reader.GetOrdinal("Price")),
                                    category: reader.GetString(reader.GetOrdinal("Category")),
                                    size: reader.GetString(reader.GetOrdinal("Size")),
                                    stock: 0, // Stock is not part of this query
                                    quantity: reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    totalPricePerItem: reader.GetDecimal(reader.GetOrdinal("TotalPricePerItem")),
                                    totalOrderPrice: reader.GetDecimal(reader.GetOrdinal("TotalOrderPrice")),
                                    status: reader.GetString(reader.GetOrdinal("Status"))
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching transactions from the database: " + ex.Message);
            }

            return transactions;
        }

    }
}
