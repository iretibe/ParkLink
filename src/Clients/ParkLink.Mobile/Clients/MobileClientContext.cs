using ParkLink.Shared.Clients;
using ParkLink.Shared.Enums;

namespace ParkLink.Mobile.Clients
{
    public sealed class MobileClientContext : IClientContext
    {
        public ParkLinkClientType ClientType => ParkLinkClientType.Mobile;
        public bool IsMobile => true;
        public bool IsWeb => false;
    }
}
