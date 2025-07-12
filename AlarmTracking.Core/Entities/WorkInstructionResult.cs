using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class WorkInstructionResult
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }
        public List<WorkInstruction> Instructions { get; private set; }

        private WorkInstructionResult(bool isSuccess, string errorMessage, List<WorkInstruction> instructions)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Instructions = instructions ?? new List<Entities.WorkInstruction>();
        }

        public static WorkInstructionResult CreateSuccess(List<WorkInstruction> instructions)
        {
            return new WorkInstructionResult(true, null, instructions);
        }

        public static WorkInstructionResult CreateFailure(string errorMessage)
        {
            return new WorkInstructionResult(false, errorMessage, null);
        }
    }
}
