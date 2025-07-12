using AlarmTracking.Application.Models;
using AlarmTracking.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Repositories.Stubs
{
    public class StubMaterialRepository : IMaterialRepository
    {
        public Task<List<Material>> GetAvailableMaterialsAsync(string materialId)
        {
            // Return dummy materials
            return Task.FromResult(new List<Material>
            {
                new Material
                {
                    Id = materialId,
                    Name = "Dummy Material",
                    AvailableQuantity = 1000,
                    Unit = "kg",
                    ExpiryDate = DateTime.Now.AddYears(1),
                    LotNumber = "LOT001"
                }
            });
        }

        public Task<Material> GetByIdAsync(string id)
        {
            return Task.FromResult(new Material
            {
                Id = id,
                Name = "Dummy Material",
                AvailableQuantity = 1000,
                Unit = "kg",
                ExpiryDate = DateTime.Now.AddYears(1),
                LotNumber = "LOT001"
            });
        }
    }
}
