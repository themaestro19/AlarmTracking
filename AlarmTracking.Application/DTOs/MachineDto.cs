using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class MachineDto
    {
        public string WorkStationId { get; set; }
        public string WorkStationName { get; set; }
        public string Status { get; set; }
        public double MixingCapacity { get; set; }
        public string RecipeType { get; set; }
    }
}
