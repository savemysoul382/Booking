using Booking.Application.DTOs;
using Booking.Application.Enums;

namespace Booking.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
    Task<RoomDto?> GetRoomByIdAsync(Int32 id);
    Task<RoomDto> CreateRoomAsync(CreateRoomDto create_room_dto);
    Task<DeleteRoomResult> DeleteRoomAsync(Int32 id);
}
