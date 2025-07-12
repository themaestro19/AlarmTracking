using AlarmTracking.Application.Services;
using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Contracts.Persistence
{
    public interface IProductionTrackingRepository
    {
        Task<User?> GetByIdAsync(Guid id);
    }
}
