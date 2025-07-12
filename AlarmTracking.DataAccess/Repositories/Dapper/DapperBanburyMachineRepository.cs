using AlarmTracking.Application.DTOs;
using AlarmTracking.Application.Entities;
using AlarmTracking.Application.Enums;
using AlarmTracking.DataAccess.Data.Context;
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
    public class DapperBanburyMachineRepository : IBanburyMachineRepository
    {
        private readonly string _connectionString;

        public DapperBanburyMachineRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<BanburyMachine>> GetAvailableMachinesAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, Name, LineId, Status, MixingCapacity, RecipeType, 
                       CycleTime, Temperature, Pressure, LastMaintenanceDate
                FROM BanburyMachines 
                WHERE Status = @Status";

            var results = await connection.QueryAsync<BanburyMachineDto>(sql,
                new { Status = MachineStatus.Available.ToString() });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<BanburyMachine> GetByIdAsync(string id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, Name, LineId, Status, MixingCapacity, RecipeType, 
                       CycleTime, Temperature, Pressure, LastMaintenanceDate
                FROM BanburyMachines 
                WHERE Id = @Id";

            var result = await connection.QueryFirstOrDefaultAsync<BanburyMachineDto>(sql, new { Id = id });

            return result != null ? MapToEntity(result) : null;
        }

        public async Task<List<BanburyMachine>> GetByLineIdAsync(string lineId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, Name, LineId, Status, MixingCapacity, RecipeType, 
                       CycleTime, Temperature, Pressure, LastMaintenanceDate
                FROM BanburyMachines 
                WHERE LineId = @LineId";

            var results = await connection.QueryAsync<BanburyMachineDto>(sql, new { LineId = lineId });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<List<BanburyMachine>> GetByStatusAsync(MachineStatus status)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, Name, LineId, Status, MixingCapacity, RecipeType, 
                       CycleTime, Temperature, Pressure, LastMaintenanceDate
                FROM BanburyMachines 
                WHERE Status = @Status";

            var results = await connection.QueryAsync<BanburyMachineDto>(sql,
                new { Status = status.ToString() });

            return results.Select(MapToEntity).ToList();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT CASE WHEN EXISTS(SELECT 1 FROM BanburyMachines WHERE Id = @Id) 
                       THEN 1 ELSE 0 END";

            return await connection.QueryFirstOrDefaultAsync<bool>(sql, new { Id = id });
        }

        public async Task<int> GetCountAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "SELECT COUNT(*) FROM BanburyMachines";

            return await connection.QueryFirstOrDefaultAsync<int>(sql);
        }

        public async Task<List<BanburyMachine>> GetMachinesByCapacityAsync(double minCapacity)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT Id, Name, LineId, Status, MixingCapacity, RecipeType, 
                       CycleTime, Temperature, Pressure, LastMaintenanceDate
                FROM BanburyMachines 
                WHERE MixingCapacity >= @MinCapacity";

            var results = await connection.QueryAsync<BanburyMachineDto>(sql,
                new { MinCapacity = minCapacity });

            return results.Select(MapToEntity).ToList();
        }

        #region Mapping Helper

        private BanburyMachine MapToEntity(BanburyMachineDto dto)
        {
            var status = Enum.TryParse<MachineStatus>(dto.Status, out var parsedStatus)
                ? parsedStatus
                : MachineStatus.Available;

            // You'll need to create a factory method in BanburyMachine entity
            // or use reflection to set private properties
            return BanburyMachine.CreateFromDatabase(
                dto.Id,
                dto.Name,
                dto.LineId,
                status,
                dto.MixingCapacity,
                dto.RecipeType,
                dto.CycleTime,
                dto.Temperature,
                dto.Pressure,
                dto.LastMaintenanceDate);
        }
        #endregion
    }
}
