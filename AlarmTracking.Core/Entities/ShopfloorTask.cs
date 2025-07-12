using AlarmTracking.Application.Common;
using AlarmTracking.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = AlarmTracking.Application.Enums.TaskStatus;

namespace AlarmTracking.Application.Entities
{
    public class ShopfloorTask : Entity
    {
        public string Id { get; private set; }
        public string OrderId { get; private set; }
        public string WorkStationId { get; private set; }
        public string OperatorId { get; private set; }
        public TaskType TaskType { get; private set; }
        public string Instructions { get; private set; }
        public TaskStatus Status { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public DateTime? StartedDateTime { get; private set; }
        public DateTime? CompletedDateTime { get; private set; }

        public static ShopfloorTask Create(string orderId, string workStationId, TaskType taskType, string instructions)
        {
            return new ShopfloorTask
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = orderId,
                WorkStationId = workStationId,
                TaskType = taskType,
                Instructions = instructions,
                Status = TaskStatus.Created,
                CreatedDateTime = DateTime.UtcNow
            };
        }

        public void AssignToOperator(string operatorId)
        {
            OperatorId = operatorId;
            Status = TaskStatus.Assigned;
        }

        public void Start()
        {
            Status = TaskStatus.InProgress;
            StartedDateTime = DateTime.UtcNow;
        }

        public void Complete()
        {
            Status = TaskStatus.Completed;
            CompletedDateTime = DateTime.UtcNow;
        }
    }
}
