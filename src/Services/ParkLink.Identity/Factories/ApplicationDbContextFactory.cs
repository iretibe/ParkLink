using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ParkLink.Identity.Data;

namespace ParkLink.Identity.Factories
{
    public sealed class ApplicationDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .AddUserSecrets<ApplicationDbContextFactory>(optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                configuration.GetConnectionString(
                    "parklink-identitydb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'parklink-identitydb' was not found.");
            }

            var optionsBuilder =
                new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(
                        typeof(ApplicationDbContext)
                            .Assembly
                            .GetName()
                            .Name);
                });

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}