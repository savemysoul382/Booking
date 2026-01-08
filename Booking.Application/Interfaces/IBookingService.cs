using Booking.Application.DTOs;

namespace Booking.Application.Interfaces;

public interface IBookingService
{
    Task<BookingDto?> GetBookingByIdAsync(Int32 id);
    Task<IEnumerable<BookingDto>> GetUserBookingsAsync(Int32 id);
    Task<BookingDto?> CreateBookingAsync(CreateBookingDto create_booking_dto);
}