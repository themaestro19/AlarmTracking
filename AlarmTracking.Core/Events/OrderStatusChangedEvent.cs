using AlarmTracking.Application.Common;
using AlarmTracking.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Events
{
    public class OrderStatusChangedEvent : IDomainEvent
    {
        public string OrderId { get; private set; }
        public OrderStatus OldStatus { get; private set; }
        public OrderStatus NewStatus { get; private set; }
        public DateTime OccurredOn { get; private set; }

        public OrderStatusChangedEvent(string orderId, OrderStatus oldStatus, OrderStatus newStatus)
        {
            OrderId = orderId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
