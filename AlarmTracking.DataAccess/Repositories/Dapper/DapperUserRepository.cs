using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Repositories.Dapper
{
    public class DapperUserRepository : IUserReadRepository
    {
        private readonly string _connectionString;

        public DapperUserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<User?> GetByIdAsync(Guid id)
        {
            using var connection = CreateConnection();
            const string sql = @"
                SELECT Id, Username, Email, PasswordHash, FirstName, LastName, 
                       Department, CreatedAt, UpdatedAt
                FROM Users 
                WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = CreateConnection();
            const string sql = @"
                SELECT Id, Username, Email, PasswordHash, FirstName, LastName, 
                       Department, CreatedAt, UpdatedAt
                FROM Users 
                WHERE Email = @Email";

            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var connection = CreateConnection();
            const string sql = @"
                SELECT Id, Username, Email, PasswordHash, FirstName, LastName, 
                       Department, CreatedAt, UpdatedAt
                FROM Users 
                WHERE Username = @Username";

            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = CreateConnection();
            const string sql = @"
                SELECT Id, Username, Email, PasswordHash, FirstName, LastName, 
                       Department, CreatedAt, UpdatedAt
                FROM Users 
                ORDER BY Username";

            return await connection.QueryAsync<User>(sql);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            using var connection = CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Users WHERE Id = @Id";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public Task<User> AddAsync(User user) => throw new NotSupportedException("Use EF Core repository for write operations");
        public Task UpdateAsync(User user) => throw new NotSupportedException("Use EF Core repository for write operations");
        public Task DeleteAsync(Guid id) => throw new NotSupportedException("Use EF Core repository for write operations");
    }

    public interface IUserReadRepository : IUserRepository { }
}
