using AlarmTracking.Application.Models;
using AlarmTracking.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Repositories.Stubs
{
    public class StubBillOfMaterialRepository : IBillOfMaterialRepository
    {
        public Task<BillOfMaterial> GetByProductCodeAsync(string productCode)
        {
            return Task.FromResult(new BillOfMaterial
            {
                ProductCode = productCode,
                Version = "1.0",
                Items = new List<BOMItem>
                {
                    new BOMItem
                    {
                        MaterialId = "MAT001",
                        MaterialName = "Rubber",
                        Quantity = 10,
                        Unit = "kg"
                    }
                }
            });
        }
    }
}
