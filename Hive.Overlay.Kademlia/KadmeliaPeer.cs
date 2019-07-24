using System;
using Hive.Overlay.Api;

namespace Hive.Overlay.Kademlia
{
    public class KadmeliaPeer : IKadmeliaPeer
    {
        public NetworkContact Contact { get; }

        public KadmeliaPeer(Uri myServerUri, Func<Uri, IKadmeliaServer> serverFactory)
        {
            var id = KadId.HostID();
            Contact = new NetworkContact(id, new[] { myServerUri });
            var cache = new BucketList(Contact, serverFactory);
            this.client = new KademliaClient(cache, serverFactory);
            this.server = new KademliaServer(cache);
        }


        public void StartServer(Uri myServerUri)
        {

        }

        private IKademilaClient client;
        public IKademilaClient Client
        {
            get { return client; }
        }

        private IKadmeliaServer server;
        public IKadmeliaServer Server
        {
            get { return server; }
        }

    }
}
