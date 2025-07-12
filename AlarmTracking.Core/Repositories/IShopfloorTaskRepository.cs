using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Contracts.Persistence
{
    public interface IShopfloorTaskRepository
    {
        Task AddAsync(ShopfloorTask task);
        Task UpdateAsync(ShopfloorTask task);
        Task<List<ShopfloorTask>> GetByOrderIdAsync(string orderId);
        Task<List<ShopfloorTask>> GetByOperatorIdAsync(string operatorId);
    }
}
