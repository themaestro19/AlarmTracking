using AlarmTracking.Application.DTOs;

namespace AlarmTracking.WebService.DTOs
{
    public class ProcessOrderResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string OrderId { get; set; }
        public ScheduleDto Schedule { get; set; }
        public List<MaterialReservationDto> Reservations { get; set; }
        public List<WorkInstructionDto> Instructions { get; set; }
        public List<TaskDto> Tasks { get; set; }
    }
}
