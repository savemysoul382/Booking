namespace Booking.Domain;

public class Booking
{
    public Int32 Id { get; set; }
    public Int32 RoomId { get; set; }
    public Int32 UserId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
    public Room Room { get; set; } = null!;
    public User User { get; set; } = null!;

}
