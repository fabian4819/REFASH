using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace WpfApp_REFASH.DataAccess
{
    public class DatabaseManager
{
    private readonly string ConnectionString;

    public DatabaseManager()
    {
        ConnectionString = Environment.GetEnvironmentVariable("REFASH_CONNECTION_STRING")
                           ?? throw new InvalidOperationException("Connection string not found in environment variables.");
    }

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