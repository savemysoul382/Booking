using System.Diagnostics.CodeAnalysis;
using Booking.Application.DTOs;
using Booking.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/admin/rooms")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class AdminRoomsController : ControllerBase
{
    private readonly IRoomService room_service;

    public AdminRoomsController(IRoomService roomService)
    {
        this.room_service = roomService;
    }

    /// <summary>
    /// Получить все комнаты
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAllRooms()
    {
        IEnumerable<RoomDto> rooms = await this.room_service.GetAllRoomsAsync();
        return Ok(value: rooms);
    }

    /// <summary>
    /// Получить комнату по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetRoom(Int32 id)
    {
        RoomDto? room = await this.room_service.GetRoomByIdAsync(id: id);

        if (room == null)
            return NotFound($"Room id = {id} not found");

        return Ok(value: room);
    }

    /// <summary>
    /// Создать новую комнату
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] CreateRoomDto createRoomDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(modelState: ModelState);
        }

        try
        {
            RoomDto room = await this.room_service.CreateRoomAsync(create_room_dto: createRoomDto);
            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }
        catch (Exception)
        {
            return StatusCode(500, "Creating room error");
        }
    }

    /// <summary>
    /// Удалить комнату
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(Int32 id)
    {
        Boolean deleted = await this.room_service.DeleteRoomAsync(id: id);

        if (!deleted)
        {
            return NotFound($"Room with id {id} not found or has active bookings");
        }

        return NoContent();
    }
}