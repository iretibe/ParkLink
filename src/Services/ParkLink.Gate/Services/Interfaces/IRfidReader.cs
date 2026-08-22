using ParkLink.Gate.Dtos;

namespace ParkLink.Gate.Services.Interfaces
{
    public interface IRfidReader
    {
        string ProviderName { get; }

        Task<RfidReaderResult?> ReadAsync(Guid deviceId,
            CancellationToken cancellationToken = default);
    }
}
