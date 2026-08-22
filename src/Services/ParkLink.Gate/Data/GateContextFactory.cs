using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ParkLink.Gate.Data
{
    public class GateContextFactory : IDesignTimeDbContextFactory<GateContext>
    {
        public GateContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                Environment.GetEnvironmentVariable("PARKLINK_GATE_CONNECTION")
                ?? configuration.GetConnectionString("parklink-gatedb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Gate database connection string was not found. " +
                    "Set PARKLINK_GATE_CONNECTION or " +
                    "ConnectionStrings__parklink-gatedb.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<GateContext>();

            optionsBuilder.UseSqlServer(
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(
                        typeof(GateContext).Assembly.FullName);
                });

            return new GateContext(optionsBuilder.Options);
        }
    }
}
