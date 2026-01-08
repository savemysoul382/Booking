using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Booking.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
{
    public BookingDbContext CreateDbContext(String[] args)
    {
        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        IConfigurationRoot config = builder.Build();

        String? connection_string = config.GetConnectionString("DefaultConnection");

        DbContextOptionsBuilder<BookingDbContext> options_builder = new DbContextOptionsBuilder<BookingDbContext>();
        options_builder.UseNpgsql(connectionString: connection_string);

        return new BookingDbContext(options_builder.Options);
    }
}