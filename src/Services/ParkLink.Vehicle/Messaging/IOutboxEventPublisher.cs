using ParkLink.Vehicle.Models;

namespace ParkLink.Vehicle.Messaging
{
    public interface IOutboxEventPublisher
    {
        Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    }
}
