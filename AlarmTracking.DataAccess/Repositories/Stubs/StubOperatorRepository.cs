using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Repositories.Stubs
{
    public class StubOperatorRepository : IOperatorRepository
    {
        public Task<List<Operator>> GetAvailableOperatorsAsync(string workStationId)
        {
            return Task.FromResult(new List<Operator>
            {
                new Operator
                {
                    Id = "OP001",
                    Name = "John Doe",
                    Skills = new List<string> { "BanburyOperation" },
                    IsAvailable = true
                }
            });
        }

        public Task<Operator> GetByIdAsync(string id)
        {
            return Task.FromResult(new Operator
            {
                Id = id,
                Name = "John Doe",
                Skills = new List<string> { "BanburyOperation" },
                IsAvailable = true
            });
        }
    }
}
