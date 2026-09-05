using ParkLink.Shared.Enums;

namespace ParkLink.Shared.Clients
{
    public interface IClientContext
    {
        ParkLinkClientType ClientType { get; }
        bool IsMobile { get; }
        bool IsWeb { get; }
    }
}
