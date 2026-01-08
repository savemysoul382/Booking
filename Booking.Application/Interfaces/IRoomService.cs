using Booking.Application.DTOs;

namespace Booking.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
    Task<RoomDto?> GetRoomByIdAsync(Int32 id);
    Task<RoomDto> CreateRoomAsync(CreateRoomDto create_room_dto);
    Task<Boolean> DeleteRoomAsync(Int32 id);
}
