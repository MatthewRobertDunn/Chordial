using Hive.Overlay.Api;

namespace Hive.Overlay.Kademlia
{
    public interface IKadmeliaPeer
    {
        IKademilaClient Client { get; }
        IKadmeliaServer Server { get; }
        NetworkContact Myself { get; }
    }
}
