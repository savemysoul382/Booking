using System.Diagnostics.CodeAnalysis;
using Booking.Application.DTOs;
using Booking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/user/bookings")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class UserBookingsController : ControllerBase
{
    private readonly IBookingService booking_service;

    public UserBookingsController(IBookingService booking_service)
    {
        this.booking_service = booking_service;
    }

    /// <summary>
    /// Получить бронирование по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetBooking(Int32 id)
    {
        BookingDto? booking = await this.booking_service.GetBookingByIdAsync(id: id);

        if (booking == null)
        {
            return NotFound($"Booking with id = {id} not found");
        }

        return Ok(value: booking);
    }

    /// <summary>
    /// Получить все бронирования пользователя
    /// </summary>
    [HttpGet("user/{id}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetUserBookings(Int32 id)
    {
        IEnumerable<BookingDto> bookings = await this.booking_service.GetUserBookingsAsync(id: id);
        return Ok(value: bookings);
    }

    /// <summary>
    /// Создать новое бронирование
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingDto create_booking_dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(modelState: ModelState);
        }

        try
        {
            BookingDto? booking = await this.booking_service.CreateBookingAsync(create_booking_dto: create_booking_dto);
            if (booking == null)
            {
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, "Error while creating a booking");
            }

            return CreatedAtAction(nameof(GetBooking), new {id = booking.Id}, value: booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(error: ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(error: ex.Message);
        }
    }
}