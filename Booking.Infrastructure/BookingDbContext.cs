using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options: options)
    {
    }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Domain.Entities.Booking> Bookings { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder model_builder)
    {
        base.OnModelCreating(modelBuilder: model_builder);

        // Настройка Room
        model_builder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Class)
                .IsRequired()
                .HasConversion<Int32>();
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description);
        });

        // Настройка User
        model_builder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });

        // Настройка Booking
        model_builder.Entity<Domain.Entities.Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CheckInDate).IsRequired();
            entity.Property(e => e.CheckOutDate).IsRequired();

            entity.HasOne(e => e.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(deleteBehavior: DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(deleteBehavior: DeleteBehavior.Restrict);
        });
    }
}