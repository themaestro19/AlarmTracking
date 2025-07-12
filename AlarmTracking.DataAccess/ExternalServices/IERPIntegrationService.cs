using AlarmTracking.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.ExternalServices
{
    public interface IERPIntegrationService
    {
        Task<ERPOrder> GetOrderFromERPAsync(string orderId);
        Task UpdateOrderStatusInERPAsync(string orderId, string status);
        Task<List<ERPOrder>> GetPendingOrdersFromERPAsync();
    }
}
