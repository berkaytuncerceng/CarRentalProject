using System.Data;
using CarRentalProject.Data.Abstract;
using CarRentalProject.Models;
using Dapper;

namespace CarRentalProject.Data.Repositories
{
    public class UserRepository : DapperRepositoryBase<User>, IUserService
    {
        public UserRepository(DbHelper dbHelper) : base(dbHelper)
        {
        }

        // Additional specific methods can be added here if needed
        public async Task<User> GetByUsernameAsync(string username)
        {
            using var connection = _dbHelper.CreateConnection();
            return await connection.QueryFirstAsync<User>(
                "SELECT * FROM Users WHERE Username = @Username",
                new { Username = username });
        }
    }
}
