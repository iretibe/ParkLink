namespace ParkLink.Notification.Services
{
    public interface INotificationDispatcher
    {
        Task DispatchAsync(Models.Notification notification,
            CancellationToken cancellationToken = default);
    }
}
