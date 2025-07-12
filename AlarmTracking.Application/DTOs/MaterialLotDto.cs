using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class MaterialLotDto
    {
        public string LotNumber { get; set; }
        public double Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string StorageLocation { get; set; }
    }
}
