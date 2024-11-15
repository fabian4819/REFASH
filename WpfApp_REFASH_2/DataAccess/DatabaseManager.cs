using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace WpfApp_REFASH.DataAccess
{
    public class DatabaseManager
    {
        private readonly string ConnectionString = "Host=localhost;Username=postgres;Password=12345678;Database=junpro100";

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        public bool CheckConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open(); 
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        Console.WriteLine("Connection successful.");
                        return true;  
                    }
                    else
                    {
                        Console.WriteLine("Connection failed.");
                        return false;  
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed with error: {ex.Message}");
                return false;
            }
        }
    }
}
