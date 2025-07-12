using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.DTOs;
using AlarmTracking.Application.Entities;
using AlarmTracking.Application.Enums;
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
    public class DapperMaterialReservationRepository : IMaterialReservationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public DapperMaterialReservationRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Write Operations (EF Core)

        public async Task AddAsync(MaterialReservation reservation)
        {
            await _context.MaterialReservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MaterialReservation reservation)
        {
            _context.MaterialReservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var reservation = await _context.MaterialReservations.FindAsync(id);
            if (reservation != null)
            {
                _context.MaterialReservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Read Operations (Dapper)

        public async Task<MaterialReservation> GetByIdAsync(string id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, MaterialId, MaterialName, ReservedQuantity, 
                       Unit, Status, ReservationDateTime, AllocatedLocation
                FROM MaterialReservations 
                WHERE Id = @Id";

            var result = await connection.QueryFirstOrDefaultAsync<MaterialReservationDto>(sql, new { Id = id });

            return result != null ? MapToEntity(result) : null;
        }

        public async Task<List<MaterialReservation>> GetByOrderIdAsync(string orderId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, MaterialId, MaterialName, ReservedQuantity, 
                       Unit, Status, ReservationDateTime, AllocatedLocation
                FROM MaterialReservations 
                WHERE OrderId = @OrderId
                ORDER BY ReservationDateTime";

            var results = await connection.QueryAsync<MaterialReservationDto>(sql, new { OrderId = orderId });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<MaterialReservation>> GetByMaterialIdAsync(string materialId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, MaterialId, MaterialName, ReservedQuantity, 
                       Unit, Status, ReservationDateTime, AllocatedLocation
                FROM MaterialReservations 
                WHERE MaterialId = @MaterialId
                ORDER BY ReservationDateTime";

            var results = await connection.QueryAsync<MaterialReservationDto>(sql, new { MaterialId = materialId });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<MaterialReservation>> GetByStatusAsync(ReservationStatus status)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, MaterialId, MaterialName, ReservedQuantity, 
                       Unit, Status, ReservationDateTime, AllocatedLocation
                FROM MaterialReservations 
                WHERE Status = @Status
                ORDER BY ReservationDateTime";

            var results = await connection.QueryAsync<MaterialReservationDto>(sql,
                new { Status = status.ToString() });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<MaterialReservation>> GetActiveReservationsAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, MaterialId, MaterialName, ReservedQuantity, 
                       Unit, Status, ReservationDateTime, AllocatedLocation
                FROM MaterialReservations 
                WHERE Status IN (@Reserved, @Allocated)
                ORDER BY ReservationDateTime";

            var results = await connection.QueryAsync<MaterialReservationDto>(sql,
                new
                {
                    Reserved = ReservationStatus.Reserved.ToString(),
                    Allocated = ReservationStatus.Allocated.ToString()
                });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<double> GetTotalReservedQuantityAsync(string materialId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT ISNULL(SUM(ReservedQuantity), 0)
                FROM MaterialReservations 
                WHERE MaterialId = @MaterialId 
                  AND Status IN (@Reserved, @Allocated)";

            return await connection.QueryFirstOrDefaultAsync<double>(sql,
                new
                {
                    MaterialId = materialId,
                    Reserved = ReservationStatus.Reserved.ToString(),
                    Allocated = ReservationStatus.Allocated.ToString()
                });
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT CASE WHEN EXISTS(SELECT 1 FROM MaterialReservations WHERE Id = @Id) 
                       THEN 1 ELSE 0 END";

            return await connection.QueryFirstOrDefaultAsync<bool>(sql, new { Id = id });
        }

        public async Task<int> GetCountAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "SELECT COUNT(*) FROM MaterialReservations";

            return await connection.QueryFirstOrDefaultAsync<int>(sql);
        }

        public async Task<List<MaterialReservation>> GetExpiredReservationsAsync(DateTime cutoffDate)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, MaterialId, MaterialName, ReservedQuantity, 
                       Unit, Status, ReservationDateTime, AllocatedLocation
                FROM MaterialReservations 
                WHERE Status = @Status 
                  AND ReservationDateTime < @CutoffDate
                ORDER BY ReservationDateTime";

            var results = await connection.QueryAsync<MaterialReservationDto>(sql,
                new
                {
                    Status = ReservationStatus.Reserved.ToString(),
                    CutoffDate = cutoffDate
                });

            return results.Select(MapToEntity).ToList();
        }

        #endregion

        #region Mapping Helper

        private MaterialReservation MapToEntity(MaterialReservationDto dto)
        {
            var status = Enum.TryParse<ReservationStatus>(dto.Status, out var parsedStatus)
                ? parsedStatus
                : ReservationStatus.Reserved;

            // You'll need to add this factory method to MaterialReservation entity
            return MaterialReservation.CreateFromDatabase(
                dto.Id,
                dto.OrderId,
                dto.MaterialId,
                dto.MaterialName,
                dto.ReservedQuantity,
                dto.Unit,
                status,
                dto.ReservationDateTime,
                dto.AllocatedLocation);
        }

        #endregion
    }
}
