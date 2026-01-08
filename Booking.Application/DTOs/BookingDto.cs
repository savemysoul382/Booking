namespace Booking.Application.DTOs;

public class BookingDto
{
    public Int32 Id { get; set; }
    public Int32 RoomId { get; set; }
    public Int32 UserId { get; set; }
    public String UserName { get; set; } = String.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public RoomDto? Room { get; set; }
    public UserDto? User { get; set; }
}