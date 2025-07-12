using AlarmTracking.Application.DTOs;

namespace AlarmTracking.WebService.DTOs
{
    public class MaterialReservationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<MaterialReservationDto> Reservations { get; set; }
    }
}
