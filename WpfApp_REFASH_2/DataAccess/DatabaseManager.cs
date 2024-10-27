using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace WpfApp_REFASH.DataAccess
{
    public class DatabaseManager
    {
        private const string ConnectionString = "Host=localhost;Username=postgres;Password=12345678;Database=junpro";

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}
