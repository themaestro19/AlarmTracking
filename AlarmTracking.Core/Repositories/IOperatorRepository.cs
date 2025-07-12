using AlarmTracking.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Contracts.Persistence
{
    public interface IOperatorRepository
    {
        Task<List<Operator>> GetAvailableOperatorsAsync(string workStationId);
        Task<Operator> GetByIdAsync(string id);
    }
}
