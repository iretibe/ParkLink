using ParkLink.Shared.Models.Dashboard;

namespace ParkLink.Shared.Services.Api
{
    public interface IParkingService
    {
        Task<DashboardDataModel> GetDashboardDataAsync(
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ParkingModel>> GetParkingsAsync(
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ParkingSlotModel>> GetParkingSlotsAsync(
            Guid parkingId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ReservationModel>> GetRecentReservationsAsync(
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SystemNotificationModel>> GetNotificationsAsync(
            CancellationToken cancellationToken = default);
    }
}
