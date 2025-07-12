using AlarmTracking.Application.DTOs;
using AlarmTracking.Application.Entities;
using AlarmTracking.Application.Enums;
using AlarmTracking.Application.Repositories;
using AlarmTracking.DataAccess.Data.Context;
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
    public class DapperProductionOrderRepository : IProductionOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public DapperProductionOrderRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Write Operations (EF Core)

        public async Task AddAsync(ProductionOrder order)
        {
            await _context.ProductionOrders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductionOrder order)
        {
            _context.ProductionOrders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var order = await _context.ProductionOrders.FindAsync(id);
            if (order != null)
            {
                _context.ProductionOrders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Read Operations (Dapper)

        public async Task<ProductionOrder> GetByIdAsync(string id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, ProductCode, TireSize, TirePattern, Quantity, 
                       RequiredDate, Priority, CustomerCode, Status, CreatedDateTime
                FROM ProductionOrders 
                WHERE Id = @Id";

            var result = await connection.QueryFirstOrDefaultAsync<ProductionOrderDto>(sql, new { Id = id });

            return result != null ? MapToEntity(result) : null;
        }

        public async Task<List<ProductionOrder>> GetPendingOrdersAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, ProductCode, TireSize, TirePattern, Quantity, 
                       RequiredDate, Priority, CustomerCode, Status, CreatedDateTime
                FROM ProductionOrders 
                WHERE Status = @Status
                ORDER BY Priority DESC, RequiredDate ASC";

            var results = await connection.QueryAsync<ProductionOrderDto>(sql,
                new { Status = OrderStatus.Received.ToString() });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<ProductionOrder>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, ProductCode, TireSize, TirePattern, Quantity, 
                       RequiredDate, Priority, CustomerCode, Status, CreatedDateTime
                FROM ProductionOrders 
                ORDER BY CreatedDateTime DESC";

            var results = await connection.QueryAsync<ProductionOrderDto>(sql);

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<ProductionOrder>> GetByStatusAsync(OrderStatus status)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, ProductCode, TireSize, TirePattern, Quantity, 
                       RequiredDate, Priority, CustomerCode, Status, CreatedDateTime
                FROM ProductionOrders 
                WHERE Status = @Status
                ORDER BY Priority DESC, RequiredDate ASC";

            var results = await connection.QueryAsync<ProductionOrderDto>(sql,
                new { Status = status.ToString() });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<ProductionOrder>> GetByCustomerCodeAsync(string customerCode)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, ProductCode, TireSize, TirePattern, Quantity, 
                       RequiredDate, Priority, CustomerCode, Status, CreatedDateTime
                FROM ProductionOrders 
                WHERE CustomerCode = @CustomerCode
                ORDER BY CreatedDateTime DESC";

            var results = await connection.QueryAsync<ProductionOrderDto>(sql,
                new { CustomerCode = customerCode });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<ProductionOrder>> GetByProductCodeAsync(string productCode)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, ProductCode, TireSize, TirePattern, Quantity, 
                       RequiredDate, Priority, CustomerCode, Status, CreatedDateTime
                FROM ProductionOrders 
                WHERE ProductCode = @ProductCode
                ORDER BY CreatedDateTime DESC";

            var results = await connection.QueryAsync<ProductionOrderDto>(sql,
                new { ProductCode = productCode });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<ProductionOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, ProductCode, TireSize, TirePattern, Quantity, 
                       RequiredDate, Priority, CustomerCode, Status, CreatedDateTime
                FROM ProductionOrders 
                WHERE RequiredDate >= @StartDate AND RequiredDate <= @EndDate
                ORDER BY RequiredDate ASC";

            var results = await connection.QueryAsync<ProductionOrderDto>(sql,
                new { StartDate = startDate, EndDate = endDate });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<ProductionOrder>> GetOverdueOrdersAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, ProductCode, TireSize, TirePattern, Quantity, 
                       RequiredDate, Priority, CustomerCode, Status, CreatedDateTime
                FROM ProductionOrders 
                WHERE RequiredDate < @CurrentDate 
                  AND Status NOT IN (@Completed, @Cancelled)
                ORDER BY RequiredDate ASC";

            var results = await connection.QueryAsync<ProductionOrderDto>(sql,
                new
                {
                    CurrentDate = DateTime.UtcNow,
                    Completed = OrderStatus.Completed.ToString(),
                    Cancelled = OrderStatus.Cancelled.ToString()
                });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<ProductionOrder>> GetHighPriorityOrdersAsync(int minPriority)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, ProductCode, TireSize, TirePattern, Quantity, 
                       RequiredDate, Priority, CustomerCode, Status, CreatedDateTime
                FROM ProductionOrders 
                WHERE Priority >= @MinPriority 
                  AND Status = @Status
                ORDER BY Priority DESC, RequiredDate ASC";

            var results = await connection.QueryAsync<ProductionOrderDto>(sql,
                new
                {
                    MinPriority = minPriority,
                    Status = OrderStatus.Received.ToString()
                });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT CASE WHEN EXISTS(SELECT 1 FROM ProductionOrders WHERE Id = @Id) 
                       THEN 1 ELSE 0 END";

            return await connection.QueryFirstOrDefaultAsync<bool>(sql, new { Id = id });
        }

        public async Task<int> GetCountAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "SELECT COUNT(*) FROM ProductionOrders";

            return await connection.QueryFirstOrDefaultAsync<int>(sql);
        }

        public async Task<int> GetCountByStatusAsync(OrderStatus status)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT COUNT(*) 
                FROM ProductionOrders 
                WHERE Status = @Status";

            return await connection.QueryFirstOrDefaultAsync<int>(sql,
                new { Status = status.ToString() });
        }

        public async Task<double> GetTotalQuantityByStatusAsync(OrderStatus status)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT ISNULL(SUM(Quantity), 0) 
                FROM ProductionOrders 
                WHERE Status = @Status";

            return await connection.QueryFirstOrDefaultAsync<double>(sql,
                new { Status = status.ToString() });
        }

        #endregion

        #region Mapping Helper

        private ProductionOrder MapToEntity(ProductionOrderDto dto)
        {
            var status = Enum.TryParse<OrderStatus>(dto.Status, out var parsedStatus)
                ? parsedStatus
                : OrderStatus.Received;

            // You'll need to add this factory method to ProductionOrder entity
            return ProductionOrder.CreateFromDatabase(
                dto.Id,
                dto.ProductCode,
                dto.TireSize,
                dto.TirePattern,
                dto.Quantity,
                dto.RequiredDate,
                dto.Priority,
                dto.CustomerCode,
                status,
                dto.CreatedDateTime);
        }

        #endregion
    }
}
