using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class ProductionScheduleDto
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string LineId { get; set; }
        public string WorkStationId { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; }
        public int Sequence { get; set; }
        public string Status { get; set; }
    }
}
