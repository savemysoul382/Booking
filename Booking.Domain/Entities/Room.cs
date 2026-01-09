using Booking.Domain.Enum;

namespace Booking.Domain.Entities;
public record Room
{
    public Int32 Id { get; set; }
    public RoomClass Class { get; set; }
    public Decimal Price { get; set; }
    public String Description { get; set; } = String.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационное свойство для бронирований
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}