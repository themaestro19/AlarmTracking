using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class ProductionOrderDto
    {
        public string Id { get; set; }
        public string ProductCode { get; set; }
        public string TireSize { get; set; }
        public string TirePattern { get; set; }
        public int Quantity { get; set; }
        public DateTime RequiredDate { get; set; }
        public int Priority { get; set; }
        public string CustomerCode { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
