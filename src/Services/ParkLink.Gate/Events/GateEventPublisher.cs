using MassTransit;
using ParkLink.SharedKernel.Events.Gate;

namespace ParkLink.Gate.Events
{
    public sealed class GateEventPublisher : IGateEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public GateEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishAccessCompletedAsync(
            GateAccessCompletedIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishAccessRequestedAsync(
            GateAccessRequestedIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishAccessGrantedAsync(
            GateAccessGrantedIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishAccessDeniedAsync(
            GateAccessDeniedIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishDeviceErrorAsync(
            GateDeviceErrorIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishGateOpenedAsync(
            GateOpenedIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishGateClosedAsync(
            GateClosedIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishVehicleDetectedAsync(
            VehicleDetectedAtGateIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishLicensePlateRecognizedAsync(
            LicensePlateRecognizedIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishRfidDetectedAsync(
            RfidTagDetectedIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishDeviceOnlineAsync(
            GateDeviceOnlineIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);

        public Task PublishDeviceOfflineAsync(
            GateDeviceOfflineIntegrationEvent message,
            CancellationToken cancellationToken = default)
            => _publishEndpoint.Publish(message, cancellationToken);
    }
}