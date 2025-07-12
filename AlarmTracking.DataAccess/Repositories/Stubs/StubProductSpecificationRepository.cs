using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Repositories.Stubs
{
    public class StubProductSpecificationRepository : IProductSpecificationRepository
    {
        public Task<ProductSpecification> GetByProductCodeAsync(string productCode)
        {
            // Return a dummy specification
            return Task.FromResult(new ProductSpecification
            {
                ProductCode = productCode,
                BaseCycleTime = TimeSpan.FromHours(2),
                OptimalTemperature = 180,
                OptimalPressure = 45,
                OptimalRPM = 80
            });
        }

        public Task<List<ProductSpecification>> GetAllAsync()
        {
            return Task.FromResult(new List<ProductSpecification>());
        }
    }
}
