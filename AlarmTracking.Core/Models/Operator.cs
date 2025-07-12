using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Models
{
    public class Operator
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public string CurrentWorkStation { get; set; }
        public bool IsAvailable { get; set; }
    }
}
