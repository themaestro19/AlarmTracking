using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Contracts.Persistence
{
    public interface IMaterialReservationRepository
    {
        Task AddAsync(MaterialReservation reservation);
        Task UpdateAsync(MaterialReservation reservation);
        Task<List<MaterialReservation>> GetByOrderIdAsync(string orderId);
    }
}
