using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public interface ITaskDispatchDomainService
    {
        Task<TaskDispatchResult> DispatchTasksAsync(ProductionOrder order, ProductionSchedule schedule,
            List<MaterialReservation> reservations, List<WorkInstruction> instructions);
    }
}
