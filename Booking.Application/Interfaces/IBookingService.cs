using BookingRoom.Application.DTOs;

namespace BookingRoom.Application.Services;

public interface IBookingService
{
    Task<BookingDto?> GetBookingByIdAsync(Int32 id);
    Task<IEnumerable<BookingDto>> GetUserBookingsAsync(String userName);
    Task<BookingDto?> CreateBookingAsync(CreateBookingDto createBookingDto);
}