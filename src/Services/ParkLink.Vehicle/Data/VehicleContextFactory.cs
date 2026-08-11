using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ParkLink.Vehicle.Data
{
    public class VehicleContextFactory : IDesignTimeDbContextFactory<VehicleContext>
    {
        public VehicleContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                Environment.GetEnvironmentVariable("PARKLINK_VEHICLE_CONNECTION")
                ?? configuration.GetConnectionString("parklink-vehicledb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Vehicle database connection string was not found. " +
                    "Set PARKLINK_VEHICLE_CONNECTION or " +
                    "ConnectionStrings__parklink-vehicledb.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<VehicleContext>();

            optionsBuilder.UseSqlServer(
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(
                        typeof(VehicleContext).Assembly.FullName);
                });

            return new VehicleContext(optionsBuilder.Options);
        }
    }
}
