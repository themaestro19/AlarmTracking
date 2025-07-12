using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class ScheduleDto
    {
        public string ScheduleId { get; set; }
        public string OrderId { get; set; }
        public string LineId { get; set; }
        public string WorkStationId { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; }
        public string Status { get; set; }
    }
}
