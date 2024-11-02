using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using WpfApp_REFASH.Utilities;

namespace WpfApp_REFASH.DataAccess
{
    public class UserRepository
    {
        private DatabaseManager _dbManager;

        public UserRepository(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        public bool RegisterUser(string name, string email, string phoneNumber, string password, string role)
        {
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        var cmd = new NpgsqlCommand("INSERT INTO users (name, email, password, phone_number, role) VALUES (@n, @e, @p, @ph, @r) RETURNING email", conn, transaction);
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.Parameters.AddWithValue("@p", SecurityUtils.HashPassword(password));
                        cmd.Parameters.AddWithValue("@ph", phoneNumber);
                        cmd.Parameters.AddWithValue("@r", role);

                        var result = cmd.ExecuteScalar()?.ToString(); // Use safe navigation to handle null

                        if (!string.IsNullOrEmpty(result))
                        {
                            // Handling roles in different tables
                            if (role == "Admin")
                            {
                                var adminCmd = new NpgsqlCommand("INSERT INTO admins (email) VALUES (@e)", conn, transaction);
                                adminCmd.Parameters.AddWithValue("@e", email);
                                adminCmd.ExecuteNonQuery();
                            }
                            else if (role == "Customer")
                            {
                                var customerCmd = new NpgsqlCommand("INSERT INTO customers (email) VALUES (@e)", conn, transaction);
                                customerCmd.Parameters.AddWithValue("@e", email);
                                customerCmd.ExecuteNonQuery();
                            }

                            transaction.Commit(); // Commit the transaction if all commands execute successfully
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, etc.)
                Console.WriteLine("Error: " + ex.Message);
                // Consider logging to a file or system rather than console for production environments
            }
            return false;
        }
        public (bool, string) AuthenticateUser(string email, string password)
        {
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("SELECT password, role FROM users WHERE email = @e", conn);
                    cmd.Parameters.AddWithValue("@e", email);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var dbPassword = reader.GetString(0);
                            var role = reader.GetString(1);

                            if (dbPassword == SecurityUtils.HashPassword(password))
                            {
                                return (true, role); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return (false, null); 
        }
    }
}
