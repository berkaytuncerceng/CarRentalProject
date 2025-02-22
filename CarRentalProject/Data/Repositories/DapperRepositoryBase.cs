using System.Data;
using CarRentalProject.Data.Abstract;
using Dapper;

namespace CarRentalProject.Data.Repositories
{
    public class DapperRepositoryBase<T> : IRepositoryBase<T> where T : class, new()
    {
        protected readonly DbHelper _dbHelper;
        private readonly IDbConnection _connection;

        protected DapperRepositoryBase(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
            _connection = _dbHelper.CreateConnection();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var storedProcedure = $"GetAll{typeof(T).Name}s"; 
            return await _connection.QueryAsync<T>(storedProcedure, commandType: CommandType.StoredProcedure);
        }
        public async Task<T> GetByIdAsync(int id)
        {
            var storedProcedure = $"Get{typeof(T).Name}ById"; 
            return await _connection.QueryFirstAsync<T>(storedProcedure, new { Id = id }, commandType: CommandType.StoredProcedure);
        }
        public async Task AddAsync(T entity)
        {
            var storedProcedure = $"Insert{typeof(T).Name}";
            var parameters = new DynamicParameters();

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.Name != "Id")
                {
                    parameters.Add($"@{property.Name}", property.GetValue(entity));
                }
            }

            await _connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task UpdateAsync(T entity)
        {
            var storedProcedure = $"Update{typeof(T).Name}";
            var parameters = new DynamicParameters();

            foreach (var property in typeof(T).GetProperties())
            {
                parameters.Add($"@{property.Name}", property.GetValue(entity));
            }

            await _connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task DeleteAsync(int id)
        {
            var storedProcedure = $"Delete{typeof(T).Name}"; 
            await _connection.ExecuteAsync(storedProcedure, new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}
