namespace AlarmTracking.WebService.DTOs
{
    public class MaterialReservationRequestDto
    {
        public string OrderId { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public string ScheduleId { get; set; }
    }
}
