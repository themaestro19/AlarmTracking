using AlarmTracking.Application.Entities;
using AlarmTracking.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Contracts.Persistence
{
    public interface IProductionScheduleRepository
    {
        #region Write Operations
        Task AddAsync(ProductionSchedule schedule);
        Task UpdateAsync(ProductionSchedule schedule);
        Task DeleteAsync(string id);
        Task<bool> SaveChangesAsync();
        #endregion

        #region Read Operations
        Task<ProductionSchedule> GetByIdAsync(string id);
        Task<List<ProductionSchedule>> GetAllAsync();
        Task<List<ProductionSchedule>> GetByOrderIdAsync(string orderId);
        Task<List<ProductionSchedule>> GetByWorkStationIdAsync(string workStationId);
        Task<ProductionSchedule> GetLastScheduleForMachineAsync(string machineId);
        Task<int> GetLastSequenceNumberAsync(string machineId);
        Task<List<ProductionSchedule>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<ProductionSchedule>> GetSchedulesByStatusAsync(ScheduleStatus status);
        Task<bool> ExistsAsync(string id);
        Task<int> GetCountAsync();
        Task<List<ProductionSchedule>> GetScheduleConflictsAsync(string workStationId, DateTime startTime, DateTime endTime);
        #endregion
    }
}
