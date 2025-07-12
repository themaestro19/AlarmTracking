using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class TaskDispatchResult
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }
        public List<ShopfloorTask> Tasks { get; private set; }

        private TaskDispatchResult(bool isSuccess, string errorMessage, List<ShopfloorTask> tasks)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Tasks = tasks ?? new List<ShopfloorTask>();
        }

        public static TaskDispatchResult CreateSuccess(List<ShopfloorTask> tasks)
        {
            return new TaskDispatchResult(true, null, tasks);
        }

        public static TaskDispatchResult CreateFailure(string errorMessage)
        {
            return new TaskDispatchResult(false, errorMessage, null);
        }
    }
}
