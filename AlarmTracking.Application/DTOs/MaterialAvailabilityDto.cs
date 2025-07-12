using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class MaterialAvailabilityDto
    {
        public string MaterialId { get; set; }
        public string MaterialName { get; set; }
        public double AvailableQuantity { get; set; }
        public string Unit { get; set; }
        public List<MaterialLotDto> Lots { get; set; }
    }
}
