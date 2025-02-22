using Microsoft.Data.SqlClient;
using System.Data;

namespace CarRentalProject.Data
{
    public class DbHelper(IConfiguration configuration)
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
