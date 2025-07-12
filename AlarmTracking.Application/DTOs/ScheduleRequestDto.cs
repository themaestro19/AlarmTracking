namespace AlarmTracking.WebService.DTOs
{
    public class ScheduleRequestDto
    {
        public string OrderId { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public DateTime RequiredDate { get; set; }
    }
}
