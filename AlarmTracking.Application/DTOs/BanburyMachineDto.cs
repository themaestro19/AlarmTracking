using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class BanburyMachineDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LineId { get; set; }
        public string Status { get; set; }
        public double MixingCapacity { get; set; }
        public string RecipeType { get; set; }
        public TimeSpan CycleTime { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
    }
}
