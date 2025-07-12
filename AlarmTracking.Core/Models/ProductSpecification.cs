using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Models
{
    public class ProductSpecification
    {
        public string ProductCode { get; set; }
        public TimeSpan BaseCycleTime { get; set; }
        public double OptimalTemperature { get; set; }
        public double OptimalPressure { get; set; }
        public int OptimalRPM { get; set; }
        public List<string> QualityRequirements { get; set; } = new List<string>();
    }
}
