using Hive.Overlay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.Overlay.Kademlia
{
    public class KademliaClient : IKademilaClient
    {
        // Kademlia config
        private const int PARALELLISM = 3; // Number of requests to run at once for iterative operations.
        private const int NODES_TO_FIND = 20; // = k = bucket size


        // Network State
        private IRoutingTable routingTable;

        private Func<Uri, IKadmeliaServer> serverFactory;
        //Whoami
        private NetworkContact myself;

        /// <summary>
        /// Make a node on a specific port, with a specified ID
        /// </summary>
        /// <param name="port"></param>
        public KademliaClient(IRoutingTable cache, Func<Uri, IKadmeliaServer> serverFactory)
        {
            this.routingTable = cache;
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
                var remotePeerID = bootstrapPeer.Address(myself.ToContact());
                //Add this peer to my routing table if alive
                if (remotePeerID != null)
                    routingTable.AddContact(NetworkContact.Parse(remotePeerID, new[] { uri }));
            }

            //Perform an iterative search for myself, this will populate my routing table further
            IterativeFindNode(myself.Address);
            return true;
        }

        /// <summary>
        /// Do an iterativeFindNode operation.
        /// </summary>f
        /// <param name="target"></param>
        /// <returns></returns>
        private IterativeFindResult IterativeFindNode(KadId target)
        {
            return IterativeFind(target);
        }


        /// <summary>
        /// Perform a Kademlia iterativeFind* operation.
        /// If getValue is true, it sends out a list of strings if values are found, or null none are.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="getValue">true for FindValue, false for FindNode</param>
        /// <param name="vals"></param>
        /// <returns></returns>
        private IterativeFindResult IterativeFind(KadId target)
        {
            IterativeFindResult result = new IterativeFindResult();

            // Get the alpha closest nodes to the target
            var shortlist = new SortedList<KadId, HaveAsked>();

            foreach (NetworkContact c in routingTable.CloseContacts(20, target))
                shortlist.Add(c.Address ^ target, new HaveAsked() { Contact = c, Asked = false });

            // Until we run out of people to ask or we're done...
            bool peersLeftToAsk = true;
            while (peersLeftToAsk)
            {
                var closestPeerNotAsked = shortlist.Where(x => x.Value.Contact.Address != myself.Address && !x.Value.IsNotContactable)  //Don't ask myself, ignore not contactable nodes
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
                var remotePeerUri = closestPeerNotAsked.Value.Contact.UriDefault;

                SearchResult searchResult = null;
                try
                {
                    var peer = serverFactory(remotePeerUri);
                    searchResult = peer.CloseContacts(target.Data, myself.ToContact());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError("Could not contact peer {0} {1}", remotePeerUri, ex.Message);
                    closestPeerNotAsked.Value.IsNotContactable = true;
                    //If we can't contact this peer, why have them in our routing table?
                    routingTable.Remove(closestPeerNotAsked.Key);
                    continue;
                }

                if (searchResult.Contacts != null)
                {
                    // Add suggestions to shortlist and check for closest
                    foreach (NetworkContact suggestion in searchResult.Contacts.Select(x => NetworkContact.Parse(x)))
                    {
                        var distance = suggestion.Address ^ target;
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

        public IterativeFindResult ClosestContacts(byte[] key)
        {
            return IterativeFindNode(new KadId(key));
        }
    }
}
