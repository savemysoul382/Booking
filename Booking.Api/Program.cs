using Booking.Application.Services;
using Booking.Infrastructure;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

String connection_string = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(connectionString: connection_string));

builder.Services.AddScoped<IRoomService, RoomService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

try
{
    using IServiceScope scope = app.Services.CreateScope();
    BookingDbContext db_context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    db_context.Database.Migrate();

    // заполним тестовыми данными
    DbInitializer.Initialize(context: db_context);
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при миграции.{Environment.NewLine}{ex.Message}");
}

app.Run();