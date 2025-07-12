using AlarmTracking.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Contracts.Persistence
{
    public interface IProductSpecificationRepository
    {
        Task<ProductSpecification> GetByProductCodeAsync(string productCode);
    }
}
