using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Models
{
    public class Material
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double AvailableQuantity { get; set; }
        public string Unit { get; set; }
        public string StorageLocation { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string LotNumber { get; set; }
    }
}
