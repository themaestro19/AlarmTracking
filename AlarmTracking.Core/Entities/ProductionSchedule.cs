using AlarmTracking.Application.Common;
using AlarmTracking.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class ProductionSchedule : Entity
    {
        public string Id { get; private set; }
        public string OrderId { get; private set; }
        public string LineId { get; private set; }
        public string WorkStationId { get; private set; }
        public DateTime ScheduledStartTime { get; private set; }
        public DateTime ScheduledEndTime { get; private set; }
        public int Sequence { get; private set; }
        public ScheduleStatus Status { get; private set; }

        private ProductionSchedule() { } // For EF

        public static ProductionSchedule Create(string orderId, string lineId, string workStationId,
            DateTime startTime, DateTime endTime, int sequence)
        {
            if (endTime <= startTime)
                throw new ArgumentException("End time must be after start time");

            return new ProductionSchedule
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = orderId,
                LineId = lineId,
                WorkStationId = workStationId,
                ScheduledStartTime = startTime,
                ScheduledEndTime = endTime,
                Sequence = sequence,
                Status = ScheduleStatus.Planned
            };
        }

        // Add this factory method for reconstructing from database
        public static ProductionSchedule CreateFromDatabase(string id, string orderId, string lineId,
            string workStationId, DateTime startTime, DateTime endTime, int sequence, ScheduleStatus status)
        {
            return new ProductionSchedule
            {
                Id = id,
                OrderId = orderId,
                LineId = lineId,
                WorkStationId = workStationId,
                ScheduledStartTime = startTime,
                ScheduledEndTime = endTime,
                Sequence = sequence,
                Status = status
            };
        }

        public void Confirm()
        {
            Status = ScheduleStatus.Confirmed;
        }

        public void Cancel()
        {
            Status = ScheduleStatus.Cancelled;
        }
    }
}

