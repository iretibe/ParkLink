namespace ParkLink.ServiceDefaults.Correlation
{
    public sealed class CorrelationContext : ICorrelationContext
    {
        private string _correlationId = Guid.NewGuid().ToString("N");
        public string CorrelationId => _correlationId;

        public void Set(string correlationId)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                return;
            }

            _correlationId = correlationId;
        }
    }
}
