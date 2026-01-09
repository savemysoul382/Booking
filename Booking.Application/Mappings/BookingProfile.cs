using Booking.Application.DTOs;
using Booking.Domain.Entities;
using Mapster;

namespace Booking.Application.Mappings
{
    public class BookingMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Domain.Entities.Booking, BookingDto>()
                .Map(d => d.UserName, s => s.User.Name);

            config.NewConfig<Room, RoomDto>()
                .Map(d => d.Class, s => s.Class.ToString());

            config.NewConfig<User, UserDto>();
        }
    }

}
