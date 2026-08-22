namespace ParkLink.Gate.Dtos
{
    public sealed class UpdateGateRequest
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
    }
}
