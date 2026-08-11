using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ParkLink.Reservation.Data
{
    public class ReservationContextFactory : IDesignTimeDbContextFactory<ReservationContext>
    {
        public ReservationContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                Environment.GetEnvironmentVariable("PARKLINK_RESERVATION_CONNECTION")
                ?? configuration.GetConnectionString("parklink-reservationdb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Reservation database connection string was not found. " +
                    "Set PARKLINK_RESERVATION_CONNECTION or " +
                    "ConnectionStrings__parklink-reservationdb.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ReservationContext>();

            optionsBuilder.UseSqlServer(
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(
                        typeof(ReservationContext).Assembly.FullName);
                });

            return new ReservationContext(optionsBuilder.Options);
        }
    }
}
