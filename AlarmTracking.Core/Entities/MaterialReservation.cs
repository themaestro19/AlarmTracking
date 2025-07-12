using AlarmTracking.Application.Common;
using AlarmTracking.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class MaterialReservation : Entity
    {
        public string Id { get; private set; }
        public string OrderId { get; private set; }
        public string MaterialId { get; private set; }
        public string MaterialName { get; private set; }
        public double ReservedQuantity { get; private set; }
        public string Unit { get; private set; }
        public ReservationStatus Status { get; private set; }
        public DateTime ReservationDateTime { get; private set; }
        public string AllocatedLocation { get; private set; }

        private MaterialReservation() { } // For EF

        // Factory method for creating new reservations
        public static MaterialReservation Create(string orderId, string materialId, string materialName,
            double quantity, string unit)
        {
            if (quantity <= 0)
                throw new ArgumentException("Reservation quantity must be greater than zero");

            return new MaterialReservation
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = orderId,
                MaterialId = materialId,
                MaterialName = materialName,
                ReservedQuantity = quantity,
                Unit = unit,
                Status = ReservationStatus.Reserved,
                ReservationDateTime = DateTime.UtcNow,
                AllocatedLocation = string.Empty
            };
        }

        // Factory method for reconstructing from database
        public static MaterialReservation CreateFromDatabase(string id, string orderId, string materialId,
            string materialName, double quantity, string unit, ReservationStatus status,
            DateTime reservationDateTime, string allocatedLocation)
        {
            return new MaterialReservation
            {
                Id = id,
                OrderId = orderId,
                MaterialId = materialId,
                MaterialName = materialName,
                ReservedQuantity = quantity,
                Unit = unit,
                Status = status,
                ReservationDateTime = reservationDateTime,
                AllocatedLocation = allocatedLocation ?? string.Empty
            };
        }

        public void Allocate(string location)
        {
            Status = ReservationStatus.Allocated;
            AllocatedLocation = location;
        }

        public void Release()
        {
            Status = ReservationStatus.Released;
        }

        public void Consume()
        {
            Status = ReservationStatus.Consumed;
        }

        public bool IsActive()
        {
            return Status == ReservationStatus.Reserved || Status == ReservationStatus.Allocated;
        }

        public bool IsExpired(TimeSpan maxAge)
        {
            return DateTime.UtcNow - ReservationDateTime > maxAge && Status == ReservationStatus.Reserved;
        }
    }
}
