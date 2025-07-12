using AlarmTracking.Application.DTOs;

namespace AlarmTracking.WebService.DTOs
{
    public class TaskDispatchResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<TaskDto> Tasks { get; set; }
    }
}
