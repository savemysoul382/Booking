namespace Booking.Application.DTOs;

public class RoomDto
{
    public Int32 Id { get; set; }
    public String Class { get; set; } = String.Empty;
    public Decimal Price { get; set; }
    public String Description { get; set; } = String.Empty;
    public DateTime CreatedAt { get; set; }
}
