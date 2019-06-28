using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Parameters;
using System.ServiceModel;

namespace Chordial.Kademlia
{
    public interface IKademilaClient
    {
        bool Booststrap(IList<string> bootstrapPeerUris);
        IterativeFindResult Put(byte[] key, string data, TimeSpan expires, int replicationFactor);
        IterativeFindResult Get(byte[] key);
    }

    public interface IKadmeliaPeer
    {
        void StartServer(Uri myServerUri);
        IKademilaClient Client { get; }
        IKadmeliaServer Server { get; }
        Contact Myself { get; }
    }

    public class KadmeliaPeer : IKadmeliaPeer
    {
        private Contact myself;
        public KadmeliaPeer(IStorage storage, Uri myServerUri, Func<Uri, IKadmeliaServer> serverFactory)
        {
            var id = ID.HostID();
            myself = new Contact() { NodeId = id.Data, Uri = myServerUri.ToString() };
            var cache = new BucketList(myself, serverFactory);
            this.client = new KademliaClient(cache, storage, serverFactory);
            this.server = new KademliaServer(cache, storage);
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

        public Contact Myself
        {
            get
            {
                return myself;
            }
        }
    }
}
