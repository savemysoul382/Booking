using BookingRoom.Application.DTOs;
using BookingRoom.Data;
using BookingRoom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Application.Services;

public class BookingService : IBookingService
{
    private readonly BookingRoomDbContext context;

    public BookingService(BookingRoomDbContext context)
    {
        this.context = context;
    }

    public async Task<BookingDto?> GetBookingByIdAsync(Int32 id)
    {
        Booking? booking = await this.context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return null;

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

    public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(String userName)
    {
        List<Booking> bookings = await this.context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Where(b => b.User.Name == userName)
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

    public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto createBookingDto)
    {
        // Валидация дат
        if (createBookingDto.CheckInDate >= createBookingDto.CheckOutDate)
        {
            throw new ArgumentException("Check-out date must be after check-in date");
        }

        if (createBookingDto.CheckInDate < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Check-in date cannot be in the past");
        }

        // Проверяем существование комнаты
        Room? room = await this.context.Rooms.FindAsync(createBookingDto.RoomId);
        if (room == null)
        {
            throw new ArgumentException("Room not found");
        }

        // Ищем пользователя по имени, если не найден - создаем нового
        User? user = await this.context.Users
            .FirstOrDefaultAsync(u => u.Name == createBookingDto.UserName);

        if (user == null)
        {
            user = new User
            {
                Name = createBookingDto.UserName,
                CreatedAt = DateTime.UtcNow
            };
            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();
        }

        // Защита от двойного бронирования
        // Проверяем, нет ли пересекающихся бронирований для этой комнаты
        Boolean conflictingBooking = await this.context.Bookings
            .AnyAsync(b => b.RoomId == createBookingDto.RoomId &&
                b.CheckInDate < createBookingDto.CheckOutDate &&
                b.CheckOutDate > createBookingDto.CheckInDate);

        if (conflictingBooking)
        {
            throw new InvalidOperationException("Room is already booked for the selected dates");
        }

        Booking booking = new Booking
        {
            RoomId = createBookingDto.RoomId,
            UserId = user.Id,
            CheckInDate = createBookingDto.CheckInDate,
            CheckOutDate = createBookingDto.CheckOutDate,
            CreatedAt = DateTime.UtcNow
        };

        this.context.Bookings.Add(booking);
        await this.context.SaveChangesAsync();

        // Загружаем связанные сущности для возврата
        await this.context.Entry(booking).Reference(b => b.Room).LoadAsync();
        await this.context.Entry(booking).Reference(b => b.User).LoadAsync();

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
