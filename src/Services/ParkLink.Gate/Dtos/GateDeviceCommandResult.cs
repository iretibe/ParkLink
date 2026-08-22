namespace ParkLink.Gate.Dtos
{
    public sealed record GateDeviceCommandResult(
        bool Success, Guid CommandId,
        string Command, string? ErrorMessage = null,
        DateTime? CompletedAtUtc = null
    );
}
