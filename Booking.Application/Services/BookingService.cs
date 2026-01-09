using Booking.Application.DTOs;
using Booking.Application.Interfaces;
using Booking.Domain.Entities;
using Booking.Infrastructure;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext context;
    private readonly IMapper mapper;

    public BookingService(BookingDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
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

        BookingDto booking_dto = this.mapper.Map<Booking.Domain.Entities.Booking, BookingDto>(source: booking);
        return booking_dto;
    }

    public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(Int32 id)
    {
        List<Domain.Entities.Booking> bookings = await this.context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .Where(b => b.User.Id == id)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        IEnumerable<BookingDto> booking_dtos = this.mapper.Map<IEnumerable<BookingDto>>(source: bookings);
        return booking_dtos;
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

        Domain.Entities.Booking booking = new()
        {
            Room = room,
            User = user,
            CheckInDate = create_booking_dto.CheckInDate,
            CheckOutDate = create_booking_dto.CheckOutDate,
            CreatedAt = DateTime.UtcNow
        };

        this.context.Bookings.Add(entity: booking);
        await this.context.SaveChangesAsync();

        BookingDto booking_dto = this.mapper.Map<Booking.Domain.Entities.Booking, BookingDto>(source: booking);
        return booking_dto;
    }
}