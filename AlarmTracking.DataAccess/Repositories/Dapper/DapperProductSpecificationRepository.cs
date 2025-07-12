using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Models;
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
    public class DapperProductSpecificationRepository : IProductSpecificationRepository
    {
        private readonly string _connectionString;

        public DapperProductSpecificationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ProductSpecification> GetByProductCodeAsync(string productCode)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
            SELECT ProductCode, BaseCycleTime, OptimalTemperature, OptimalPressure, OptimalRPM
            FROM ProductSpecifications 
            WHERE ProductCode = @ProductCode";

            return await connection.QueryFirstOrDefaultAsync<ProductSpecification>(sql, new { ProductCode = productCode });
        }

        public async Task<List<ProductSpecification>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
            SELECT ProductCode, BaseCycleTime, OptimalTemperature, OptimalPressure, OptimalRPM
            FROM ProductSpecifications";

            var results = await connection.QueryAsync<ProductSpecification>(sql);
            return results.ToList();
        }
    }
}
