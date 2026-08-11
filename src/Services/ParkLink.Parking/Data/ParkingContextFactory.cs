using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ParkLink.Parking.Data
{
    public class ParkingContextFactory : IDesignTimeDbContextFactory<ParkingContext>
    {
        public ParkingContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                Environment.GetEnvironmentVariable("PARKLINK_PARKING_CONNECTION")
                ?? configuration.GetConnectionString("parklink-parkingdb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Parking database connection string was not found. " +
                    "Set PARKLINK_PARKING_CONNECTION or " +
                    "ConnectionStrings__parklink-parkingdb.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ParkingContext>();

            optionsBuilder.UseSqlServer(
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(
                        typeof(ParkingContext).Assembly.FullName);
                });

            return new ParkingContext(optionsBuilder.Options);
        }
    }
}
