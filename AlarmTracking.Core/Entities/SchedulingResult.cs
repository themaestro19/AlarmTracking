using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class SchedulingResult
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }
        public ProductionSchedule Schedule { get; private set; }

        private SchedulingResult(bool isSuccess, string errorMessage, ProductionSchedule schedule)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Schedule = schedule;
        }

        public static SchedulingResult CreateSuccess(ProductionSchedule schedule)
        {
            return new SchedulingResult(true, null, schedule);
        }

        public static SchedulingResult CreateFailure(string errorMessage)
        {
            return new SchedulingResult(false, errorMessage, null);
        }
    }
}
