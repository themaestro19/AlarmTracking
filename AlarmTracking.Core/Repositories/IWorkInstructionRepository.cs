using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Contracts.Persistence
{
    public interface IWorkInstructionRepository
    {
        Task<List<WorkInstruction>> GetByProductCodeAsync(string productCode);
        Task<WorkInstruction> GetByIdAsync(string id);
    }
}
