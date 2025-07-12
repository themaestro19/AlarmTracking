using AlarmTracking.Application.Entities;
using AlarmTracking.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Domain.Repositories
{
    public interface IBanburyMachineRepository
    {
        Task<List<BanburyMachine>> GetAvailableMachinesAsync();
        Task<BanburyMachine> GetByIdAsync(string id);
        Task<List<BanburyMachine>> GetByLineIdAsync(string lineId);
        Task<List<BanburyMachine>> GetByStatusAsync(MachineStatus status);
        Task<bool> ExistsAsync(string id);
        Task<int> GetCountAsync();
        Task<List<BanburyMachine>> GetMachinesByCapacityAsync(double minCapacity);
    }
}
