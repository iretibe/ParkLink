namespace ParkLink.Parking.Exceptions
{
    public sealed class ParkingConcurrencyException : Exception
    {
        public ParkingConcurrencyException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
