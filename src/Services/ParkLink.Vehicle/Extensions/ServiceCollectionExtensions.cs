using ParkLink.Vehicle.Services;

namespace ParkLink.Vehicle.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVehicleServices(this IServiceCollection services)
        {
            services.AddScoped<IVehicleService, VehicleService>();

            return services;
        }
    }
}
