using MassTransit;
using ParkLink.SharedKernel.Events.Vehicle;
using ParkLink.Vehicle.Models;
using System.Text.Json;

namespace ParkLink.Vehicle.Messaging
{
    public class OutboxEventPublisher : IOutboxEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public OutboxEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            switch (message.EventType)
            {
                case nameof(VehicleCreatedIntegrationEvent):

                    var created =
                        JsonSerializer.Deserialize<VehicleCreatedIntegrationEvent>(
                            message.Payload)
                        ?? throw new InvalidOperationException(
                            "Invalid vehicle created event.");

                    await _publishEndpoint.Publish(created, cancellationToken);

                    break;


                    // other events
            }
        }
    }
}
