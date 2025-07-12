using AlarmTracking.Application.Common;
using AlarmTracking.Application.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class ProductionOrder : AggregateRoot
    {
        public string Id { get; private set; }
        public string ProductCode { get; private set; }
        public string TireSize { get; private set; }
        public string TirePattern { get; private set; }
        public int Quantity { get; private set; }
        public DateTime RequiredDate { get; private set; }
        public int Priority { get; private set; }
        public string CustomerCode { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedDateTime { get; private set; }

        private ProductionOrder() { } // For EF

        // Factory method for creating new orders
        public static ProductionOrder Create(string id, string productCode, string tireSize, string tirePattern,
            int quantity, DateTime requiredDate, int priority, string customerCode)
        {
            // Domain validation
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Order ID is required");
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");
            if (requiredDate <= DateTime.UtcNow)
                throw new ArgumentException("Required date must be in the future");

            var order = new ProductionOrder
            {
                Id = id,
                ProductCode = productCode,
                TireSize = tireSize,
                TirePattern = tirePattern,
                Quantity = quantity,
                RequiredDate = requiredDate,
                Priority = priority,
                CustomerCode = customerCode,
                Status = OrderStatus.Received,
                CreatedDateTime = DateTime.UtcNow
            };

            // Domain event
            order.AddDomainEvent(new Events.OrderCreatedEvent(id, productCode, quantity));

            return order;
        }

        // Factory method for reconstructing from database
        public static ProductionOrder CreateFromDatabase(string id, string productCode, string tireSize,
            string tirePattern, int quantity, DateTime requiredDate, int priority, string customerCode,
            OrderStatus status, DateTime createdDateTime)
        {
            return new ProductionOrder
            {
                Id = id,
                ProductCode = productCode,
                TireSize = tireSize,
                TirePattern = tirePattern,
                Quantity = quantity,
                RequiredDate = requiredDate,
                Priority = priority,
                CustomerCode = customerCode,
                Status = status,
                CreatedDateTime = createdDateTime
            };
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            if (Status != newStatus)
            {
                var oldStatus = Status;
                Status = newStatus;
                AddDomainEvent(new Events.OrderStatusChangedEvent(Id, oldStatus, newStatus));
            }
        }

        public bool CanBeScheduled()
        {
            return Status == OrderStatus.Received &&
                   RequiredDate > DateTime.UtcNow.AddDays(1);
        }

        public bool IsOverdue()
        {
            return RequiredDate < DateTime.UtcNow &&
                   Status != OrderStatus.Completed &&
                   Status != OrderStatus.Cancelled;
        }

        public void UpdatePriority(int newPriority)
        {
            if (newPriority < 1 || newPriority > 10)
                throw new ArgumentException("Priority must be between 1 and 10");

            Priority = newPriority;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed order");

            UpdateStatus(OrderStatus.Cancelled);
        }
    }
}
