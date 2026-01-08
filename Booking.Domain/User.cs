namespace Booking.Domain;

public class User
{
    public Int32 Id { get; set; }
    public String Name { get; set; } = String.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}