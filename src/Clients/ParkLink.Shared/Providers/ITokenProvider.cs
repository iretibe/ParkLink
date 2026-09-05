namespace ParkLink.Shared.Providers
{
    public interface ITokenProvider
    {
        Task<string?> GetAccessTokenAsync();
    }
}
