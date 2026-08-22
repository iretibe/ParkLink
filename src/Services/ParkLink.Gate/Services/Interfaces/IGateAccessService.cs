using ParkLink.Gate.Dtos;

namespace ParkLink.Gate.Services.Interfaces
{
    public interface IGateAccessService
    {
        Task<AccessDecisionResult> ProcessAccessAsync(
            AccessRequest request,
            CancellationToken cancellationToken = default);
    }
}
