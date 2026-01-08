using Booking.Application.DTOs;
using Booking.Application.Interfaces;
using Booking.Domain.Entities;
using Booking.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext context;

    public BookingService(BookingDbContext context)
    {
        this.context = context;
    }

    public async Task<BookingDto?> GetBookingByIdAsync(Int32 id)
    {
        Booking.Domain.Entities.Booking? booking = await this.context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return null;
        }

        return new BookingDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            UserId = booking.UserId,
            UserName = booking.User.Name,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            CreatedAt = booking.CreatedAt,
            Room = new RoomDto
            {
                Id = booking.Room.Id,
                Class = booking.Room.Class.ToString(),
                Price = booking.Room.Price,
                Description = booking.Room.Description,
                CreatedAt = booking.Room.CreatedAt
            },
            User = new UserDto
            {
                Id = booking.User.Id,
                Name = booking.User.Name,
                CreatedAt = booking.User.CreatedAt
            }
        };
    }

    public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(Int32 id)
    {
        List<Domain.Entities.Booking> bookings = await this.context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Where(b => b.User.Id == id)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            RoomId = b.RoomId,
            UserId = b.UserId,
            UserName = b.User.Name,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            CreatedAt = b.CreatedAt,
            Room = new RoomDto
            {
                Id = b.Room.Id,
                Class = b.Room.Class.ToString(),
                Price = b.Room.Price,
                Description = b.Room.Description,
                CreatedAt = b.Room.CreatedAt
            },
            User = new UserDto
            {
                Id = b.User.Id,
                Name = b.User.Name,
                CreatedAt = b.User.CreatedAt
            }
        });
    }

    public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto create_booking_dto)
    {
        if (create_booking_dto.CheckInDate >= create_booking_dto.CheckOutDate)
        {
            throw new ArgumentException("Check out date must be after check in date");
        }

        if (create_booking_dto.CheckInDate < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Check in date cannot be in the past");
        }

        Room? room = await this.context.Rooms.FindAsync(create_booking_dto.RoomId);
        if (room == null)
        {
            throw new ArgumentException("Room not found");
        }

        //тут мы создадим нового пользователя, если не найдём.
        User? user = await this.context.Users
            .FirstOrDefaultAsync(u => u.Name == create_booking_dto.UserName);

        if (user == null)
        {
            user = new User
            {
                Name = create_booking_dto.UserName,
                CreatedAt = DateTime.UtcNow
            };
            this.context.Users.Add(entity: user);
            await this.context.SaveChangesAsync();
        }

        Boolean conflicting_booking = await this.context.Bookings
            .AnyAsync(b => b.RoomId == create_booking_dto.RoomId &&
                           b.CheckInDate < create_booking_dto.CheckOutDate &&
                           b.CheckOutDate > create_booking_dto.CheckInDate);

        if (conflicting_booking)
        {
            throw new InvalidOperationException("Room is booked for the selected dates");
        }

        Domain.Entities.Booking booking = new Domain.Entities.Booking
        {
            Room = room,
            User = user,
            CheckInDate = create_booking_dto.CheckInDate,
            CheckOutDate = create_booking_dto.CheckOutDate,
            CreatedAt = DateTime.UtcNow
        };

        this.context.Bookings.Add(entity: booking);
        await this.context.SaveChangesAsync();
       

        return new BookingDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            UserId = booking.UserId,
            UserName = booking.User.Name,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            CreatedAt = booking.CreatedAt,
            Room = new RoomDto
            {
                Id = booking.Room.Id,
                Class = booking.Room.Class.ToString(),
                Price = booking.Room.Price,
                Description = booking.Room.Description,
                CreatedAt = booking.Room.CreatedAt
            },
            User = new UserDto
            {
                Id = booking.User.Id,
                Name = booking.User.Name,
                CreatedAt = booking.User.CreatedAt
            }
        };
    }
}