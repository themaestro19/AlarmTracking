using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Entities;
using AlarmTracking.DataAccess.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Repositories.Dapper
{
    public class DapperWorkInstructionRepository : IWorkInstructionRepository
    {
        private readonly ApplicationDbContext _context;

        public DapperWorkInstructionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkInstruction>> GetByProductCodeAsync(string productCode)
        {
            return await _context.WorkInstructions
                .Where(wi => wi.ProductCode == productCode)
                .Include(wi => wi.Parameters)
                .Include(wi => wi.QualityChecks)
                .ToListAsync();
        }

        public async Task<WorkInstruction> GetByIdAsync(string id)
        {
            return await _context.WorkInstructions
                .Include(wi => wi.Parameters)
                .Include(wi => wi.QualityChecks)
                .FirstOrDefaultAsync(wi => wi.Id == id);
        }
    }
}
