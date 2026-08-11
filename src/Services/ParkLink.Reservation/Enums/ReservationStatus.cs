namespace ParkLink.Reservation.Enums
{
    public enum ReservationStatus
    {
        Pending = 0,

        // Slot is temporarily held while waiting for payment.
        Held = 1,

        // Payment completed and reservation confirmed.
        Confirmed = 2,

        // Vehicle has entered the parking lot.
        Active = 3,

        // Parking session completed.
        Completed = 4,

        // Cancelled by user, administrator, or system.
        Cancelled = 5,

        // Reservation expired before confirmation.
        Expired = 6,

        // User failed to arrive within the allowed time.
        NoShow = 7
    }
}
