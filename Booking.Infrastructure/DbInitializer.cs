using Booking.Domain.Entities;
using Booking.Domain.Enum;

namespace Booking.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(BookingDbContext context)
        {
            if (context.Users.Any() || context.Rooms.Any())
            {
                return;
            }

            // Создаем пользователей
            List<User> users = new()
            {
                new User {Name = "Тест Иванов", CreatedAt = DateTime.UtcNow},
                new User {Name = "Тест Петров", CreatedAt = DateTime.UtcNow},
                new User {Name = "Тест Сидоров", CreatedAt = DateTime.UtcNow},
                new User {Name = "Тест Козлов", CreatedAt = DateTime.UtcNow}
            };

            context.Users.AddRange(entities: users);
            context.SaveChanges();

            // Создаем комнаты
            List<Room> rooms = new List<Room>
            {
                new()
                {
                    Class = RoomClass.Economy,
                    Price = 2200.00m,
                    Description = "Эконом",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Class = RoomClass.Standard,
                    Price = 2500.00m,
                    Description = "Стандартный номер",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Class = RoomClass.Deluxe,
                    Price = 4500.00m,
                    Description = "Номер люкс",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Class = RoomClass.Suite,
                    Price = 7500.00m,
                    Description = "Люкс со всеми удобствами",
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Rooms.AddRange(entities: rooms);
            context.SaveChanges();

            DateTime now = DateTime.UtcNow;
            List<Domain.Entities.Booking> bookings = new List<Domain.Entities.Booking>
            {
                // Бронирование 1: Иван Иванов - Standard комната
                new()
                {
                    RoomId = rooms[0].Id,
                    UserId = users[0].Id,
                    CheckInDate = now.AddDays(1),
                    CheckOutDate = now.AddDays(5),
                    CreatedAt = now
                },
                // Бронирование 2: Мария Петрова - Deluxe комната
                new()
                {
                    RoomId = rooms[1].Id,
                    UserId = users[1].Id,
                    CheckInDate = now.AddDays(5),
                    CheckOutDate = now.AddDays(10),
                    CreatedAt = now
                },
                // Бронирование 3: Алексей Сидоров - Suite комната
                new()
                {
                    RoomId = rooms[2].Id,
                    UserId = users[2].Id,
                    CheckInDate = now.AddDays(10),
                    CheckOutDate = now.AddDays(15),
                    CreatedAt = now
                },
                new()
                {
                    RoomId = rooms[3].Id,
                    UserId = users[3].Id,
                    CheckInDate = now.AddDays(3),
                    CheckOutDate = now.AddDays(9),
                    CreatedAt = now
                },
                new()
                {
                    RoomId = rooms[1].Id,
                    UserId = users[0].Id,
                    CheckInDate = now.AddDays(30),
                    CheckOutDate = now.AddDays(35),
                    CreatedAt = now
                },
                new()
                {
                    RoomId = rooms[0].Id,
                    UserId = users[1].Id,
                    CheckInDate = now.AddDays(25),
                    CheckOutDate = now.AddDays(30),
                    CreatedAt = now
                }
            };

            context.Bookings.AddRange(entities: bookings);
            context.SaveChanges();
        }
    }
}