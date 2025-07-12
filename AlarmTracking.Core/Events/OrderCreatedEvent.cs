using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Events
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public string OrderId { get; private set; }
        public string ProductCode { get; private set; }
        public int Quantity { get; private set; }
        public DateTime OccurredOn { get; private set; }

        public OrderCreatedEvent(string orderId, string productCode, int quantity)
        {
            OrderId = orderId;
            ProductCode = productCode;
            Quantity = quantity;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
