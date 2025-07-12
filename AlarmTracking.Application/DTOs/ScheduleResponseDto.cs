using AlarmTracking.Application.DTOs;

namespace AlarmTracking.WebService.DTOs
{
    public class ScheduleResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ScheduleDto Schedule { get; set; }
    }
}
