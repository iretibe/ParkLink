using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ParkLink.Payment.Data
{
    public class PaymentContextFactory : IDesignTimeDbContextFactory<PaymentContext>
    {
        public PaymentContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                Environment.GetEnvironmentVariable("PARKLINK_PAYMENT_CONNECTION")
                ?? configuration.GetConnectionString("parklink-paymentdb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Payment database connection string was not found. " +
                    "Set PARKLINK_PAYMENT_CONNECTION or " +
                    "ConnectionStrings__parklink-paymentdb.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<PaymentContext>();

            optionsBuilder.UseSqlServer(
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(
                        typeof(PaymentContext).Assembly.FullName);
                });

            return new PaymentContext(optionsBuilder.Options);
        }
    }
}
