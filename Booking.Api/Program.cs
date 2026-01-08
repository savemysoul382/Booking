using Booking.Infrastructure;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

String connection_string = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(connectionString: connection_string));


WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseAuthorization();
app.MapControllers();


app.Run();