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
    public class DapperBillOfMaterialRepository : IBillOfMaterialRepository
    {
        private readonly string _connectionString;

        public DapperBillOfMaterialRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<BillOfMaterial> GetByProductCodeAsync(string productCode)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
            SELECT ProductCode, Version
            FROM BillOfMaterials 
            WHERE ProductCode = @ProductCode";

            var bom = await connection.QueryFirstOrDefaultAsync<BillOfMaterial>(sql, new { ProductCode = productCode });

            if (bom != null)
            {
                const string itemsSql = @"
                SELECT MaterialId, MaterialName, Quantity, Unit
                FROM BillOfMaterialItems 
                WHERE ProductCode = @ProductCode";

                var items = await connection.QueryAsync<BOMItem>(itemsSql, new { ProductCode = productCode });
                bom.Items = items.ToList();
            }

            return bom;
        }
    }
}
