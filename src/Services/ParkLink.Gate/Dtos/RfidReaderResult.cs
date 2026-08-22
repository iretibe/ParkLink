namespace ParkLink.Gate.Dtos
{
    public sealed record RfidReaderResult(
        string TagIdentifier,
        DateTime ReadAtUtc,
        double? SignalStrength = null
    );
}
