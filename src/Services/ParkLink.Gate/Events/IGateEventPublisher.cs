using ParkLink.SharedKernel.Events.Gate;

namespace ParkLink.Gate.Events
{
    public interface IGateEventPublisher
    {
        Task PublishAccessRequestedAsync(
            GateAccessRequestedIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishAccessGrantedAsync(
            GateAccessGrantedIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishAccessDeniedAsync(
            GateAccessDeniedIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishAccessCompletedAsync(
            GateAccessCompletedIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishGateOpenedAsync(
            GateOpenedIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishGateClosedAsync(
            GateClosedIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishVehicleDetectedAsync(
            VehicleDetectedAtGateIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishLicensePlateRecognizedAsync(
            LicensePlateRecognizedIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishRfidDetectedAsync(
            RfidTagDetectedIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishDeviceOnlineAsync(
            GateDeviceOnlineIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishDeviceOfflineAsync(
            GateDeviceOfflineIntegrationEvent message,
            CancellationToken cancellationToken = default);

        Task PublishDeviceErrorAsync(
            GateDeviceErrorIntegrationEvent message,
            CancellationToken cancellationToken = default);
    }
}
