using Hive.Overlay.Api;

namespace Hive.Overlay.Kademlia
{
    public static class Mappingcs
    {
        public static KadId GetID(this Contact contact)
        {
            return new KadId(contact.Address);
        }

    }
}
