namespace ParkLink.ServiceDefaults.Correlation
{
    public class CorrelationContext : ICorrelationContext
    {
        private string _correlationId = Guid.NewGuid().ToString();
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
