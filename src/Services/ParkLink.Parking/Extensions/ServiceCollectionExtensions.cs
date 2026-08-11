using ParkLink.Parking.Services;

namespace ParkLink.Parking.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddParkingServices(this IServiceCollection services)
        {
            services.AddScoped<IParkingLotService, ParkingLotService>();
            services.AddScoped<IParkingZoneService, ParkingZoneService>();
            services.AddScoped<IParkingSlotService, ParkingSlotService>();
            services.AddScoped<IParkingGateService, ParkingGateService>();

            return services;
        }
    }
}
