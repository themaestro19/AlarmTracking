namespace AlarmTracking.WebService.DTOs
{
    public class ProcessOrderRequestDto
    {
        public string OrderId { get; set; }
        public string ProductCode { get; set; }
        public string TireSize { get; set; }
        public string TirePattern { get; set; }
        public int Quantity { get; set; }
        public DateTime RequiredDate { get; set; }
        public int Priority { get; set; }
        public string CustomerCode { get; set; }
    }
}
