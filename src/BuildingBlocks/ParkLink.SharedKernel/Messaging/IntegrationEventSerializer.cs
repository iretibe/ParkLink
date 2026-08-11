using ParkLink.SharedKernel.Events.Vehicle;
using System.Text.Json;

namespace ParkLink.SharedKernel.Messaging
{
    public static class IntegrationEventSerializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static object Deserialize(string eventType, string payload)
        {
            return eventType switch
            {
                nameof(VehicleCreatedIntegrationEvent)
                    => Deserialize<VehicleCreatedIntegrationEvent>(payload),

                nameof(VehicleUpdatedIntegrationEvent)
                    => Deserialize<VehicleUpdatedIntegrationEvent>(payload),

                nameof(VehicleVerifiedIntegrationEvent)
                    => Deserialize<VehicleVerifiedIntegrationEvent>(payload),

                nameof(VehicleSuspendedIntegrationEvent)
                    => Deserialize<VehicleSuspendedIntegrationEvent>(payload),

                nameof(VehicleActivatedIntegrationEvent)
                    => Deserialize<VehicleActivatedIntegrationEvent>(payload),

                nameof(VehicleDeletedIntegrationEvent)
                    => Deserialize<VehicleDeletedIntegrationEvent>(payload),

                _ => throw new InvalidOperationException(
                    $"Unknown integration event type '{eventType}'.")
            };
        }

        private static T Deserialize<T>(string payload)
        {
            return JsonSerializer.Deserialize<T>(payload, Options)
                ?? throw new InvalidOperationException(
                    $"Unable to deserialize integration event '{typeof(T).Name}'.");
        }
    }
}