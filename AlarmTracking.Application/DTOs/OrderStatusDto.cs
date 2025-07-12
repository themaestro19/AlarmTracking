using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class OrderStatusDto
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string CurrentStep { get; set; }
        public double ProgressPercentage { get; set; }
    }
}
