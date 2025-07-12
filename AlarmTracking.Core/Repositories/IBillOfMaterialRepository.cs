using AlarmTracking.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Domain.Repositories
{
    public interface IBillOfMaterialRepository
    {
        Task<BillOfMaterial> GetByProductCodeAsync(string productCode);
    }
}
