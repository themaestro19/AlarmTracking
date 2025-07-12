using AlarmTracking.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Domain.Repositories
{
    public interface IMaterialRepository
    {
        Task<List<Material>> GetAvailableMaterialsAsync(string materialId);
        Task<Material> GetByIdAsync(string id);
    }
}
