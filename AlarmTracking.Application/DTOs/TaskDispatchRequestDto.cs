namespace AlarmTracking.WebService.DTOs
{
    public class TaskDispatchRequestDto
    {
        public string OrderId { get; set; }
        public string ScheduleId { get; set; }
        public List<string> MaterialReservationIds { get; set; }
        public List<string> WorkInstructionIds { get; set; }
    }
}
