using AlarmTracking.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.ExternalServices
{
    public interface IMaterialManagementService
    {
        Task<List<Material>> GetAvailableMaterialsAsync(string materialId);
        Task<BillOfMaterial> GetBillOfMaterialsAsync(string productCode);
        Task ReserveMaterialAsync(string materialId, double quantity, string orderId);
        Task ReleaseMaterialReservationAsync(string reservationId);
    }
}
