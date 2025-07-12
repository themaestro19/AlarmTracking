using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class MaterialReservationDto
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string MaterialId { get; set; }
        public string MaterialName { get; set; }
        public double ReservedQuantity { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public string AllocatedLocation { get; set; }
    }
}
