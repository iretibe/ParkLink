using ParkLink.Gate.Hardware;
using ParkLink.Gate.Services.Implementations;
using ParkLink.Gate.Services.Interfaces;

namespace ParkLink.Gate.Extensions
{
    public static class GateApplicationExtensions
    {
        public static IServiceCollection AddGateApplication(this IServiceCollection services)
        {
            services.AddScoped<IGateService, GateService>();
            services.AddScoped<IGateDeviceService, GateDeviceService>();
            services.AddScoped<IGateAccessService, GateAccessService>();
            services.AddScoped<IGateDeviceCommandService, GateDeviceCommandService>();

            services.AddScoped<IGateHardwareClient, SimulatedGateHardwareClient>();

            return services;
        }
    }
}
