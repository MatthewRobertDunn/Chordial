using System;
using Hive.Overlay.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hive.Overlay.Kademlia
{
    public class KadmeliaPeer : IKadmeliaPeer
    {
        public NetworkContact Contact { get; }

        public KadmeliaPeer(Uri myServerUri, Func<Uri, IKadmeliaServer> serverFactory)
        {
            var logger = NullLoggerFactory.Instance;
            var id = KadId.HostID();
            Contact = new NetworkContact(id, new[] { myServerUri });
            var cache = new RoutingTable(Contact, serverFactory, logger.CreateLogger<RoutingTable>());
            this.client = new KademliaClient(cache, serverFactory, logger.CreateLogger<KademliaClient>());
            this.server = new KademliaServer(cache, logger.CreateLogger<KademliaServer>());
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
