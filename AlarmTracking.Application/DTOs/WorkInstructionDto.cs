using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class WorkInstructionDto
    {
        public string InstructionId { get; set; }
        public string ProcessStep { get; set; }
        public string Instructions { get; set; }
        public List<ProcessParameterDto> Parameters { get; set; }
    }
}
