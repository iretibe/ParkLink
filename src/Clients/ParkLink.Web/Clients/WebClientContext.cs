using ParkLink.Shared.Clients;
using ParkLink.Shared.Enums;

namespace ParkLink.Web.Clients
{
    public class WebClientContext : IClientContext
    {
        public ParkLinkClientType ClientType => ParkLinkClientType.Web;
        public bool IsMobile => false;
        public bool IsWeb => true;
    }
}
