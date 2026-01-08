using Booking.Api;
using Booking.Application.Interfaces;
using Booking.Application.Services;
using Booking.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

String connection_string = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(connectionString: connection_string));

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(error_app =>
{
    error_app.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        IExceptionHandlerFeature? error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            Console.WriteLine($"Unhandled exception{error.Error.Message}");

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new {error = "Internal error occurred"}));
        }
    });
});

app.UseMiddleware<GlobalErrorHandler>();

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