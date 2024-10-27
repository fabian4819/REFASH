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
                    var cmd = new NpgsqlCommand("INSERT INTO users (name, email, password, phone_number) VALUES (@n, @e, @p, @ph) RETURNING email", conn);
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", SecurityUtils.HashPassword(password));
                    cmd.Parameters.AddWithValue("@ph", phoneNumber);
                    var result = cmd.ExecuteScalar().ToString();

                    if (!string.IsNullOrEmpty(result))
                    {
                        // Optionally handle different roles
                        if (role == "Admin")
                        {
                            var adminCmd = new NpgsqlCommand("INSERT INTO admins (email) VALUES (@e)", conn);
                            adminCmd.Parameters.AddWithValue("@e", email);
                            adminCmd.ExecuteNonQuery();
                        }
                        else if (role == "Customer")
                        {
                            var customerCmd = new NpgsqlCommand("INSERT INTO customers (email) VALUES (@e)", conn);
                            customerCmd.Parameters.AddWithValue("@e", email);
                            customerCmd.ExecuteNonQuery();
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, etc.)
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
        }
    }
}
