using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public interface IWorkInstructionDomainService
    {
        Task<WorkInstructionResult> LoadWorkInstructionsAsync(Entities.ProductionOrder order, Entities.ProductionSchedule schedule);
        Task ClearWorkInstructionsAsync(string workStationId);
    }
}
