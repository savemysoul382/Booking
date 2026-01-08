using Booking.Application.DTOs;
using Booking.Domain.Entities;
using Booking.Domain.Enum;
using Booking.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Services;

public class RoomService : IRoomService
{
    private readonly BookingDbContext context;

    public RoomService(BookingDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
    {
        List<Room> rooms = await this.context.Rooms
            .OrderBy(r => r.Id)
            .ToListAsync();

        return rooms.Select(r => new RoomDto
        {
            Id = r.Id,
            Class = r.Class.ToString(),
            Price = r.Price,
            Description = r.Description,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<RoomDto?> GetRoomByIdAsync(Int32 id)
    {
        Room? room = await this.context.Rooms.FindAsync(id);

        if (room == null)
        {
            return null;
        }

        return new RoomDto
        {
            Id = room.Id,
            Class = room.Class.ToString(),
            Price = room.Price,
            Description = room.Description,
            CreatedAt = room.CreatedAt
        };
    }

    public async Task<RoomDto> CreateRoomAsync(CreateRoomDto create_room_dto)
    {
        if (!Enum.TryParse<RoomClass>(value: create_room_dto.Class, ignoreCase: true, out RoomClass room_class))
        {
            throw new ArgumentException($"Invalid room class: {create_room_dto.Class}. Valid values are: {String.Join(", ", Enum.GetNames<RoomClass>())}");
        }

        Room room = new Room
        {
            Class = room_class,
            Price = create_room_dto.Price,
            Description = create_room_dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        this.context.Rooms.Add(entity: room);
        await this.context.SaveChangesAsync();

        return new RoomDto
        {
            Id = room.Id,
            Class = room.Class.ToString(),
            Price = room.Price,
            Description = room.Description,
            CreatedAt = room.CreatedAt
        };
    }

    public async Task<Boolean> DeleteRoomAsync(Int32 id)
    {
        Room? room = await this.context.Rooms.FindAsync(id);

        if (room == null)
        {
            return false;
        }

        // Проверяем, есть ли активные бронирования
        Boolean has_active_bookings = await this.context.Bookings
            .AnyAsync(b => b.RoomId == id && b.CheckOutDate > DateTime.UtcNow);

        if (has_active_bookings)
        {
            return false;
        }

        this.context.Rooms.Remove(entity: room);
        await this.context.SaveChangesAsync();

        return true;
    }
}