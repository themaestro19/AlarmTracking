using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Repositories
{
    public interface IProductionOrderRepository
    {
        Task<ProductionOrder> GetByIdAsync(string id);
        Task AddAsync(ProductionOrder order);
        Task UpdateAsync(ProductionOrder order);
        Task<List<ProductionOrder>> GetPendingOrdersAsync();
    }
}
