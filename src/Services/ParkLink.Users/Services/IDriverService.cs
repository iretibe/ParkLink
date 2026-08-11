using ParkLink.SharedKernel.Pagination;
using ParkLink.Users.Dtos.Drivers;

namespace ParkLink.Users.Services
{
    public interface IDriverService
    {
        Task<PagedResult<DriverListItemDto>> GetDriversAsync(
            DriverSearchRequest request, CancellationToken cancellationToken = default);
        Task<DriverDetailsDto?> GetDriverByIdAsync(string driverId,
            CancellationToken cancellationToken = default);
        Task ApproveDriverAsync(string driverId, DriverActionRequest? request = null,
            CancellationToken cancellationToken = default);
        Task RejectDriverAsync(string driverId, DriverActionRequest? request = null,
            CancellationToken cancellationToken = default);
        Task SuspendDriverAsync(string driverId, DriverActionRequest? request = null,
            CancellationToken cancellationToken = default);
    }
}
