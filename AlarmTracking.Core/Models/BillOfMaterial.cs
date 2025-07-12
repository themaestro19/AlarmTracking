using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Models
{
    public class BillOfMaterial
    {
        public string ProductCode { get; set; }
        public string Version { get; set; }
        public List<BOMItem> Items { get; set; } = new List<BOMItem>();
    }
}
