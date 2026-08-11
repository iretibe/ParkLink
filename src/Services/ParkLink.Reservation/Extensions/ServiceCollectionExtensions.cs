using ParkLink.Reservation.Services;

namespace ParkLink.Reservation.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddReservationServices(this IServiceCollection services)
        {
            services.AddScoped<IReservationService, ReservationService>();

            return services;
        }
    }
}
