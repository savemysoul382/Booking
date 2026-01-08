namespace Booking.Application.DTOs;

public class CreateRoomDto
{
    public String Class { get; set; } = String.Empty;
    public Decimal Price { get; set; }
    public String Description { get; set; } = String.Empty;
}
