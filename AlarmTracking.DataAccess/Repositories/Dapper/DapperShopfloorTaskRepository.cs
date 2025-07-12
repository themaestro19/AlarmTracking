using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.DataAccess.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlarmTracking.DataAccess.Repositories.Dapper
{
    public class DapperShopfloorTaskRepository : IShopfloorTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public DapperShopfloorTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Application.Entities.ShopfloorTask task)
        {
            await _context.ShopfloorTasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Application.Entities.ShopfloorTask task)
        {
            _context.ShopfloorTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Application.Entities.ShopfloorTask>> GetByOrderIdAsync(string orderId)
        {
            return await _context.ShopfloorTasks
                .Where(t => t.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<List<Application.Entities.ShopfloorTask>> GetByOperatorIdAsync(string operatorId)
        {
            return await _context.ShopfloorTasks
                .Where(t => t.OperatorId == operatorId &&
                           t.Status != Application.Enums.TaskStatus.Completed)
                .ToListAsync();
        }
    }
}
