using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp_REFASH.DataAccess;
using WpfApp_REFASH.Utilities;

namespace WpfApp_REFASH
{
    public class User
    {
        public string Name { get; set; }
        protected string Email { get; set; }
        protected string PhoneNumber { get; set; }
        protected string Password { get; set; }
        protected string Role { get; set; }
        private DatabaseManager _dbManager = new DatabaseManager();



        //Constructor
        public User(string name, string email, string phoneNumber, string password, string role)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Role = role;
        }
        public User(string email, string password)
        {
            Email = email;
            Password = password;
        }

        // GetData
        protected virtual (bool, string, string, string, string) GetData(string email)
        {
            if (_dbManager == null)
            {
                return (false, "Database Manager not initialized", null, null, null);
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

                                return (true, name, role, phoneNumber, dbPassword);
                            }
                            else
                            {
                                return (false, "Email not found", null, null, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during data retrieval: {ex.Message}");
                return (false, "Error during data retrieval", null, null, null);
            }
        }

        //Login
        public (bool, string, string, string, string) Login(string email, string password)
        {
            var (userFound, name, role, phoneNumber, dbPassword) = GetData(email);

            if (!userFound)
            {
                return (false, "User not found.", null, null, null);
            }

            // Hash the input password
            var inputPasswordHash = SecurityUtils.HashPassword(password);

            // Compare hashes using the constant time string comparison
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





        public bool Register()
        {
            NpgsqlTransaction transaction = null;
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

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
                        using (var cmd = new NpgsqlCommand("INSERT INTO customers (email) VALUES (@e)", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@e", Email);
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



        //Logout
        public virtual void Logout()
        {
            Console.WriteLine($"{Name} logged out.");
        }
        //Change Password
        public void ChangePassword()
        {
            Console.WriteLine("Password changed.");
        }
        //Polymorphism (updateProfile method)
        public virtual void UpdateProfile()
        {
            Console.WriteLine("Profile updated.");
        }
    }
}
