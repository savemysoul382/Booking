namespace Booking.Application.DTOs;

public record UserDto
{
    public Int32 Id { get; set; }
    public String Name { get; set; } = String.Empty;
    public DateTime CreatedAt { get; set; }
}
