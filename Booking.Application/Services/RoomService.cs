using Booking.Application.DTOs;
using Booking.Application.Enums;
using Booking.Application.Interfaces;
using Booking.Domain.Entities;
using Booking.Domain.Enum;
using Booking.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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

        if (create_room_dto.Price <= 0)
        {
           throw new ArgumentException("Price must be greater than 0");
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

    public async Task<DeleteRoomResult> DeleteRoomAsync(Int32 id)
    {
        if (id <= 0)
        {
            return DeleteRoomResult.NotFound;
        }

        await using IDbContextTransaction transaction = await this.context.Database.BeginTransactionAsync();

        try
        {
            // Проверяем, существует ли комната И нет ли активных бронирований
            Room? room = await this.context.Rooms
                .Where(r => r.Id == id)
                .Where(r => !this.context.Bookings
                    .Any(b => b.RoomId == id && b.CheckOutDate > DateTime.UtcNow))
                .FirstOrDefaultAsync();

            if (room == null)
            {
                Boolean exists = await this.context.Rooms.AnyAsync(r => r.Id == id);
                return exists
                    ? DeleteRoomResult.HasActiveBookings
                    : DeleteRoomResult.NotFound;
            }

            // Удаляем найденную комнату (без повторного запроса)
            this.context.Rooms.Remove(room);
            await this.context.SaveChangesAsync();

            await transaction.CommitAsync();
            return DeleteRoomResult.Success;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw; 
        }
    }
}