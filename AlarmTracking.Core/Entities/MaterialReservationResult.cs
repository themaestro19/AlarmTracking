using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class MaterialReservationResult
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }
        public List<MaterialReservation> Reservations { get; private set; }

        private MaterialReservationResult(bool isSuccess, string errorMessage, List<MaterialReservation> reservations)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Reservations = reservations ?? new List<MaterialReservation>();
        }

        public static MaterialReservationResult CreateSuccess(List<MaterialReservation> reservations)
        {
            return new MaterialReservationResult(true, null, reservations);
        }

        public static MaterialReservationResult CreateFailure(string errorMessage)
        {
            return new MaterialReservationResult(false, errorMessage, null);
        }
    }
}
