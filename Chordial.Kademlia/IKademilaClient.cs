﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Parameters;

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
        private IKernel kernel;
        private Contact myself;
        public KadmeliaPeer(IStorage storage, Uri myServerUri, IKernel kernel)
        {
            this.kernel = kernel;

            var id = ID.RandomID();
            myself = new Contact() { NodeId = id.Data, Uri = myServerUri.ToString() };
            var cache = new BucketList(myself);
            this.client = new KademliaClient(cache, storage, kernel);
            this.server = new KademliaServer(cache, storage, kernel);
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
