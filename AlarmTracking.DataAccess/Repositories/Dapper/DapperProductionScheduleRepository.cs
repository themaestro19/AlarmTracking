using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.DTOs;
using AlarmTracking.Application.Entities;
using AlarmTracking.Application.Enums;
using AlarmTracking.DataAccess.Data.Context;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AlarmTracking.DataAccess.Repositories.Dapper
{
    public class DapperProductionScheduleRepository : IProductionScheduleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public DapperProductionScheduleRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Write Operations (EF Core)

        public async Task AddAsync(ProductionSchedule schedule)
        {
            await _context.ProductionSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductionSchedule schedule)
        {
            _context.ProductionSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var schedule = await _context.ProductionSchedules.FindAsync(id);
            if (schedule != null)
            {
                _context.ProductionSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        public async Task<ProductionSchedule> GetByIdAsync(string id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, LineId, WorkStationId, ScheduledStartTime, 
                       ScheduledEndTime, Sequence, Status
                FROM ProductionSchedules 
                WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<ProductionSchedule>(sql, new { Id = id });
        }

        public async Task<List<ProductionSchedule>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, LineId, WorkStationId, ScheduledStartTime, 
                       ScheduledEndTime, Sequence, Status
                FROM ProductionSchedules 
                ORDER BY ScheduledStartTime";

            var results = await connection.QueryAsync<ProductionScheduleDto>(sql);
            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<ProductionSchedule>> GetByOrderIdAsync(string orderId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, LineId, WorkStationId, ScheduledStartTime, 
                       ScheduledEndTime, Sequence, Status
                FROM ProductionSchedules 
                WHERE OrderId = @OrderId
                ORDER BY Sequence";

            var results = await connection.QueryAsync<ProductionScheduleDto>(sql, new { OrderId = orderId });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<ProductionSchedule>> GetByWorkStationIdAsync(string workStationId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, LineId, WorkStationId, ScheduledStartTime, 
                       ScheduledEndTime, Sequence, Status
                FROM ProductionSchedules 
                WHERE WorkStationId = @WorkStationId
                ORDER BY ScheduledStartTime";

            var results = await connection.QueryAsync<ProductionScheduleDto>(sql, new { WorkStationId = workStationId });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<ProductionSchedule> GetLastScheduleForMachineAsync(string machineId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT TOP 1 Id, OrderId, LineId, WorkStationId, ScheduledStartTime, 
                             ScheduledEndTime, Sequence, Status
                FROM ProductionSchedules 
                WHERE WorkStationId = @MachineId
                ORDER BY ScheduledEndTime DESC";

            return await connection.QueryFirstOrDefaultAsync<ProductionSchedule>(sql, new { MachineId = machineId });
        }

        public async Task<int> GetLastSequenceNumberAsync(string machineId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT ISNULL(MAX(Sequence), 0) 
                FROM ProductionSchedules 
                WHERE WorkStationId = @MachineId";

            return await connection.QueryFirstOrDefaultAsync<int>(sql, new { MachineId = machineId });
        }

        public async Task<List<ProductionSchedule>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, LineId, WorkStationId, ScheduledStartTime, 
                       ScheduledEndTime, Sequence, Status
                FROM ProductionSchedules 
                WHERE ScheduledStartTime >= @StartDate AND ScheduledStartTime <= @EndDate
                ORDER BY ScheduledStartTime";

            var results = await connection.QueryAsync<ProductionScheduleDto>(sql,
                new { StartDate = startDate, EndDate = endDate });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<ProductionSchedule>> GetSchedulesByStatusAsync(ScheduleStatus status)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, LineId, WorkStationId, ScheduledStartTime, 
                       ScheduledEndTime, Sequence, Status
                FROM ProductionSchedules 
                WHERE Status = @Status
                ORDER BY ScheduledStartTime";

            var results = await connection.QueryAsync<ProductionScheduleDto>(sql,
                new { Status = status.ToString() });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT CASE WHEN EXISTS(SELECT 1 FROM ProductionSchedules WHERE Id = @Id) 
                       THEN 1 ELSE 0 END";

            return await connection.QueryFirstOrDefaultAsync<bool>(sql, new { Id = id });
        }

        public async Task<int> GetCountAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "SELECT COUNT(*) FROM ProductionSchedules";

            return await connection.QueryFirstOrDefaultAsync<int>(sql);
        }

        public async Task<List<ProductionSchedule>> GetScheduleConflictsAsync(string workStationId, DateTime startTime, DateTime endTime)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, OrderId, LineId, WorkStationId, ScheduledStartTime, 
                       ScheduledEndTime, Sequence, Status
                FROM ProductionSchedules 
                WHERE WorkStationId = @WorkStationId 
                  AND (
                    (ScheduledStartTime <= @StartTime AND ScheduledEndTime > @StartTime) OR
                    (ScheduledStartTime < @EndTime AND ScheduledEndTime >= @EndTime) OR
                    (ScheduledStartTime >= @StartTime AND ScheduledEndTime <= @EndTime)
                  )
                ORDER BY ScheduledStartTime";

            var results = await connection.QueryAsync<ProductionScheduleDto>(sql,
                new { WorkStationId = workStationId, StartTime = startTime, EndTime = endTime });

            return results.Select(MapToEntity).ToList();
        }

        private ProductionSchedule MapToEntity(ProductionScheduleDto dto)
        {
            // Create entity using factory method
            var schedule = ProductionSchedule.Create(
                dto.OrderId,
                dto.LineId,
                dto.WorkStationId,
                dto.ScheduledStartTime,
                dto.ScheduledEndTime,
                dto.Sequence);

            // Set the ID using reflection since it has private setter
            var idProperty = typeof(ProductionSchedule).GetProperty("Id");
            idProperty?.SetValue(schedule, dto.Id);

            // Set status if different from default
            if (Enum.TryParse<ScheduleStatus>(dto.Status, out var status))
            {
                if (status == ScheduleStatus.Confirmed)
                    schedule.Confirm();
                else if (status == ScheduleStatus.Cancelled)
                    schedule.Cancel();
            }

            return schedule;
        }
    }
}
