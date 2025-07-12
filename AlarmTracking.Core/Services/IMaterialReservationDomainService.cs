using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public interface IMaterialReservationDomainService
    {
        Task<MaterialReservationResult> ReserveMaterialsAsync(ProductionOrder order, ProductionSchedule schedule);
        Task ReleaseReservationsAsync(List<MaterialReservation> reservations);
    }
}
