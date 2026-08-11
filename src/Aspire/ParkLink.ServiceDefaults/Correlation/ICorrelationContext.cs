namespace ParkLink.ServiceDefaults.Correlation
{
    public interface ICorrelationContext
    {
        string CorrelationId { get; }
        void Set(string correlationId);
    }
}
