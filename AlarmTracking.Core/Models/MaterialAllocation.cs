using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Models
{
    public class MaterialAllocation
    {
        public string MaterialId { get; set; }
        public string MaterialName { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
    }
}
