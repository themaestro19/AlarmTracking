using AlarmTracking.Application.Models;
using AlarmTracking.Domain.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Repositories.Dapper
{
    public class DapperMaterialRepository : IMaterialRepository
    {
        private readonly string _connectionString;

        public DapperMaterialRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Material>> GetAvailableMaterialsAsync(string materialId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
            SELECT Id, Name, Type, AvailableQuantity, Unit, StorageLocation, ExpiryDate, LotNumber
            FROM Materials 
            WHERE Id = @MaterialId AND AvailableQuantity > 0";

            var results = await connection.QueryAsync<Material>(sql, new { MaterialId = materialId });
            return results.ToList();
        }

        public async Task<Material> GetByIdAsync(string id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
            SELECT Id, Name, Type, AvailableQuantity, Unit, StorageLocation, ExpiryDate, LotNumber
            FROM Materials 
            WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<Material>(sql, new { Id = id });
        }
    }
}
