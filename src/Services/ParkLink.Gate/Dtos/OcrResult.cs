namespace ParkLink.Gate.Dtos
{
    public sealed record OcrResult(
        bool Success, string? LicensePlate, 
        decimal Confidence, DateTime RecognizedAtUtc,
        string? ErrorMessage = null
    );
}
