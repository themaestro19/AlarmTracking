using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.DTOs
{
    public class TaskDto
    {
        public string TaskId { get; set; }
        public string OrderId { get; set; }
        public string WorkStationId { get; set; }
        public string OperatorId { get; set; }
        public string TaskType { get; set; }
        public string Instructions { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
