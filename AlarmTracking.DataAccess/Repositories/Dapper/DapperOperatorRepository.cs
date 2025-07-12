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
    public class DapperOperatorRepository : IOperatorRepository
    {
        private readonly string _connectionString;

        public DapperOperatorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Operator>> GetAvailableOperatorsAsync(string workStationId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
            SELECT Id, Name, Skills, CurrentWorkStation, IsAvailable
            FROM Operators 
            WHERE IsAvailable = 1 AND (CurrentWorkStation = @WorkStationId OR CurrentWorkStation IS NULL)";

            var results = await connection.QueryAsync<OperatorDto>(sql, new { WorkStationId = workStationId });
            return results.Select(MapToOperator).ToList();
        }

        public async Task<Operator> GetByIdAsync(string id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
            SELECT Id, Name, Skills, CurrentWorkStation, IsAvailable
            FROM Operators 
            WHERE Id = @Id";

            var result = await connection.QueryFirstOrDefaultAsync<OperatorDto>(sql, new { Id = id });
            return result != null ? MapToOperator(result) : null;
        }

        private Operator MapToOperator(OperatorDto dto)
        {
            return new Operator
            {
                Id = dto.Id,
                Name = dto.Name,
                Skills = string.IsNullOrEmpty(dto.Skills) ? new List<string>() : dto.Skills.Split(',').ToList(),
                CurrentWorkStation = dto.CurrentWorkStation,
                IsAvailable = dto.IsAvailable
            };
        }
    }

    public class OperatorDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Skills { get; set; } // Comma-separated
        public string CurrentWorkStation { get; set; }
        public bool IsAvailable { get; set; }
    }
}
