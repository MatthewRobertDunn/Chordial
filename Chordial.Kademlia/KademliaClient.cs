using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Activation;
using Ninject.Parameters;

namespace Chordial.Kademlia
{
    public class KademliaClient : IKademilaClient
    {
        // Kademlia config
        private const int PARALELLISM = 3; // Number of requests to run at once for iterative operations.
        private const int NODES_TO_FIND = 20; // = k = bucket size


        // Network State
        private IBucketList routingTable;

        private Func<Uri, IKadmeliaServer> serverFactory;
        private IStorage dataStore;

        //Whoami
        private NetworkContact myself;

        /// <summary>
        /// Make a node on a specific port, with a specified ID
        /// </summary>
        /// <param name="port"></param>
        public KademliaClient(IBucketList cache, IStorage dataStore, Func<Uri, IKadmeliaServer> serverFactory)
        {
            this.routingTable = cache;
            this.dataStore = dataStore;
            this.serverFactory = serverFactory;
            myself = cache.MySelf;
        }

        /// <summary>
        /// Bootstrap this node from 1 or more known nodes
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool Booststrap(IList<string> bootstrapPeerUris)
        {

            //Ping as many remote peers as we can and if they're alive add them to our rating table
            foreach (var uri in bootstrapPeerUris)
            {
                var remotePeerUri = new Uri(uri);
                var bootstrapPeer = serverFactory(remotePeerUri);

                //Get the remote peers ID, pass it my own contact details
                var remotePeerID = bootstrapPeer.Ping(myself.ToContact());
                //Add this peer to my routing table if alive
                if (remotePeerID != null)
                    routingTable.AddContact(NetworkContact.Parse(remotePeerID, uri));
            }

            //Perform an iterative search for myself, this will populate my routing table further
            IterativeFindNode(myself.Id);
            return true;
        }


        public IterativeFindResult Put(byte[] key, string data, TimeSpan expires, int replicationFactor)
        {
            return IterativeStore(new KadId(key), data, DateTime.UtcNow, DateTime.UtcNow + expires, replicationFactor);
        }

        public IterativeFindResult Get(byte[] key)
        {
            var localStore = dataStore.GetItems(key);

            if (localStore != null)
                return new IterativeFindResult()
                {

                    Values = localStore.Select(x => x.Value).ToList(),
                    TargetPeer = this.routingTable.MySelf,
                };

            var result = IterativeFind(new KadId(key), true);
            return result;
        }


        /// <summary>
        /// Do an iterativeStore operation and publish the key/value pair on the network
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        private IterativeFindResult IterativeStore(KadId key, string val, DateTime originalInsertion, DateTime expires, int replicationFactor)
        {
            // Find the K closest nodes to the key
            var closest = IterativeFindNode(key);
            foreach (NetworkContact c in closest.ClosestPeers.Take(replicationFactor))
            {
                var peer = serverFactory(c.Uri);
                peer.StoreValue(myself.ToContact(), key.Data, val, originalInsertion, expires);
            }

            return closest;
        }


        /// <summary>
        /// Do an iterativeFindNode operation.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private IterativeFindResult IterativeFindNode(KadId target)
        {
            return IterativeFind(target, false);
        }


        /// <summary>
        /// Perform a Kademlia iterativeFind* operation.
        /// If getValue is true, it sends out a list of strings if values are found, or null none are.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="getValue">true for FindValue, false for FindNode</param>
        /// <param name="vals"></param>
        /// <returns></returns>
        private IterativeFindResult IterativeFind(KadId target, bool getValue)
        {
            IterativeFindResult result = new IterativeFindResult();

            // Log the lookup
            if (target != routingTable.MySelf.Id)
                routingTable.Touch(target);

            // Get the alpha closest nodes to the target
            var shortlist = new SortedList<KadId, HaveAsked>();

            foreach (NetworkContact c in routingTable.CloseContacts(20, target))
                shortlist.Add(c.Id ^ target, new HaveAsked() { Contact = c, Asked = false });

            // Until we run out of people to ask or we're done...
            bool peersLeftToAsk = true;
            while (peersLeftToAsk)
            {
                var closestPeerNotAsked = shortlist.Where(x => x.Value.Contact.Id != myself.Id && !x.Value.IsNotContactable)  //Don't ask myself, ignore not contactable nodes
                                                        .Take(3)    //only consider the first 3 closest nodes
                                                        .Where(x => x.Value.Asked == false) //That we haven't asked before
                                                        .FirstOrDefault(); //Get the first
                if (closestPeerNotAsked.Value == null)
                {
                    peersLeftToAsk = false;
                    continue;
                }

                result.NumberIterations += 1;


                closestPeerNotAsked.Value.Asked = true;
                var remotePeerUri = closestPeerNotAsked.Value.Contact.Uri;
                var peer = serverFactory(remotePeerUri);

                SearchResult searchResult;
                if (getValue)
                    searchResult = peer.FindValue(myself.ToContact(), target.Data);
                else
                    searchResult = peer.FindNode(myself.ToContact(), target.Data);

                //peer is down, ignore
                if (searchResult == null)
                {
                    closestPeerNotAsked.Value.IsNotContactable = true;
                    continue;
                }

                if (searchResult.Values != null)
                {
                    result.Values = searchResult.Values;
                    result.TargetPeer = closestPeerNotAsked.Value.Contact;
                    return result;
                }

                if (searchResult.Contacts != null)
                {
                    // Add suggestions to shortlist and check for closest
                    foreach (NetworkContact suggestion in searchResult.Contacts.Select(x=>new NetworkContact(x)))
                    {
                        var distance = suggestion.Id ^ target;
                        if (!shortlist.ContainsKey(distance))
                            shortlist.Add(distance, new HaveAsked() { Contact = suggestion });

                        //Add this guy to our contact cache
                        routingTable.AddContact(suggestion);
                    }
                }
            }

            result.ClosestPeers = shortlist.Values.Where(x => !x.IsNotContactable).Select(x => x.Contact).Take(20).ToList();
            return result;
        }


        /// <summary>
        /// Log debug messages, if debugging is enabled.
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message)
        {
            Console.WriteLine(message);
        }

    }
}
