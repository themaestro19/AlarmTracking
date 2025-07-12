using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Repositories.Dapper
{
    public class DapperRefreshTokenRepository : IRefreshTokenReadRepository
    {
        private readonly string _connectionString;

        public DapperRefreshTokenRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            using var connection = CreateConnection();

            const string sql = @"
                SELECT rt.Id, rt.Token, rt.UserId, rt.ExpiresAt, rt.IsRevoked, 
                       rt.RevokedReason, rt.CreatedAt, rt.UpdatedAt,
                       u.Id, u.Username, u.Email, u.PasswordHash, u.FirstName, 
                       u.LastName, u.Department, u.Role, u.IsActive, u.LastLoginAt,
                       u.CreatedAt, u.UpdatedAt
                FROM RefreshTokens rt
                INNER JOIN Users u ON rt.UserId = u.Id
                WHERE rt.Token = @Token";

            var refreshTokenDictionary = new Dictionary<Guid, RefreshToken>();

            var result = await connection.QueryAsync<RefreshToken, User, RefreshToken>(
                sql,
                (refreshToken, user) =>
                {
                    if (!refreshTokenDictionary.TryGetValue(refreshToken.Id, out var existingToken))
                    {
                        existingToken = refreshToken;
                        refreshTokenDictionary.Add(refreshToken.Id, existingToken);
                    }

                    // Set user (this would need reflection or a public setter)
                    // For now, we'll return the token without the navigation property populated
                    return existingToken;
                },
                new { Token = token },
                splitOn: "Id");

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId)
        {
            using var connection = CreateConnection();

            const string sql = @"
                SELECT Id, Token, UserId, ExpiresAt, IsRevoked, 
                       RevokedReason, CreatedAt, UpdatedAt
                FROM RefreshTokens
                WHERE UserId = @UserId
                ORDER BY CreatedAt DESC";

            return await connection.QueryAsync<RefreshToken>(sql, new { UserId = userId });
        }

        // Write operations not supported in read repository
        public Task<RefreshToken> AddAsync(RefreshToken refreshToken) =>
            throw new NotSupportedException("Use EF Core repository for write operations");

        public Task UpdateAsync(RefreshToken refreshToken) =>
            throw new NotSupportedException("Use EF Core repository for write operations");

        public Task DeleteAsync(Guid id) =>
            throw new NotSupportedException("Use EF Core repository for write operations");

        public Task RevokeAllUserTokensAsync(Guid userId) =>
            throw new NotSupportedException("Use EF Core repository for write operations");
    }

    public interface IRefreshTokenReadRepository : IRefreshTokenRepository { }
}
