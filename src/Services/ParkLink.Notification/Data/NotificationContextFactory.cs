using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ParkLink.Notification.Data
{
    public class NotificationContextFactory : IDesignTimeDbContextFactory<NotificationContext>
    {
        public NotificationContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                Environment.GetEnvironmentVariable("PARKLINK_NOTIFICATION_CONNECTION")
                ?? configuration.GetConnectionString("parklink-notificationdb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Notification database connection string was not found. " +
                    "Set PARKLINK_NOTIFICATION_CONNECTION or " +
                    "ConnectionStrings__parklink-notificationdb.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<NotificationContext>();

            optionsBuilder.UseSqlServer(
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(
                        typeof(NotificationContext).Assembly.FullName);
                });

            return new NotificationContext(optionsBuilder.Options);
        }
    }
}
