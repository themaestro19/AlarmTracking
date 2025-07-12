using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public interface IOrderSchedulingDomainService
    {
        Task<SchedulingResult> ScheduleOrderAsync(ProductionOrder order);
        Task CancelScheduleAsync(string scheduleId);
        Task<List<BanburyMachine>> GetAvailableMachinesAsync();
    }
}
