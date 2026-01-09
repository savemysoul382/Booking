using Booking.Api;
using Booking.Application.Interfaces;
using Booking.Application.Mappings;
using Booking.Application.Services;
using Booking.Infrastructure;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

#region Mapper

builder.Services.AddSingleton(provider =>
{
    TypeAdapterConfig config = new TypeAdapterConfig();
    config.Scan(typeof(BookingMapping).Assembly);
    return config;
});

//builder.Services.AddScoped<IMapper>(provider =>
//{
//    TypeAdapterConfig config = provider.GetRequiredService<TypeAdapterConfig>();
//    return new Mapper(config);
//});

builder.Services.AddScoped<IMapper, ServiceMapper>();

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

String connection_string = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(connectionString: connection_string));

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();

WebApplication app = builder.Build();

app.UseMiddleware<GlobalErrorHandler>();

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
    await db_context.Database.MigrateAsync();

    // заполним тестовыми данными
    DbInitializer.Initialize(context: db_context);
}
catch (Exception ex)
{
    Console.WriteLine($"Migration error.{Environment.NewLine}{ex.Message}");
}

app.Run();