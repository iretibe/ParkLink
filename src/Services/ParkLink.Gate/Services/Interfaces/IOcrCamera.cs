using ParkLink.Gate.Dtos;

namespace ParkLink.Gate.Services.Interfaces
{
    public interface IOcrCamera
    {
        string ProviderName { get; }

        Task<OcrResult?> RecognizeAsync(OcrRequest request,
            CancellationToken cancellationToken = default);
    }
}
