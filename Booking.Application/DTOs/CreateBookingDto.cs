namespace Booking.Application.DTOs;

public record CreateBookingDto
{
    public Int32 RoomId { get; set; }
    public String UserName { get; set; } = String.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}
