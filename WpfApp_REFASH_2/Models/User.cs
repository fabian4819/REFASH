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
        //Encapsulation (protected access modifier)
        protected string Name { get; set; }
        protected string Email { get; set; }
        protected string PhoneNumber { get; set; }
        protected string Password { get; set; }
        protected string Role { get; set; }
        private static DatabaseManager _dbManager = new DatabaseManager();

        

        //Constructor
        public User(string name, string email, string phoneNumber, string password, string role)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Role = role;
        }
        //Login
        public static (bool, string) Login(string email, string password)
        {
            if (_dbManager == null)
            {
                return (false, "Database Manager not initialized");
            }

            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT password, role FROM users WHERE email = @e", conn))
                    {
                        cmd.Parameters.AddWithValue("@e", email);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var dbPassword = reader.GetString(0);
                                var role = reader.GetString(1);
                                reader.Close();

                                if (SecurityUtils.HashPassword(password) == dbPassword)
                                {
                                    return (true, role);
                                }
                                else
                                {
                                    return (false, "Incorrect password");
                                }
                            }
                            else
                            {
                                return (false, "Email not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return (false, "Error during login");
            }
        }



        //Register
        public bool Register()
        {
            try
            {
                using (var conn = _dbManager.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        var cmd = new NpgsqlCommand("INSERT INTO users (name, email, password, phone_number, role) VALUES (@n, @e, @p, @ph, @r) RETURNING email", conn, transaction);
                        cmd.Parameters.AddWithValue("@n", Name);
                        cmd.Parameters.AddWithValue("@e", Email);
                        cmd.Parameters.AddWithValue("@p", SecurityUtils.HashPassword(Password));
                        cmd.Parameters.AddWithValue("@ph", PhoneNumber);
                        cmd.Parameters.AddWithValue("@r", Role);
                        var result = cmd.ExecuteScalar()?.ToString(); 
                        if (!string.IsNullOrEmpty(result))
                        {
                            if (Role == "Admin")
                            {
                                var adminCmd = new NpgsqlCommand("INSERT INTO admins (email) VALUES (@e)", conn, transaction);
                                adminCmd.Parameters.AddWithValue("@e", Email);
                                adminCmd.ExecuteNonQuery();
                            }
                            else if (Role == "Customer")
                            {
                                var customerCmd = new NpgsqlCommand("INSERT INTO customers (email) VALUES (@e)", conn, transaction);
                                customerCmd.Parameters.AddWithValue("@e", Email);
                                customerCmd.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
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
