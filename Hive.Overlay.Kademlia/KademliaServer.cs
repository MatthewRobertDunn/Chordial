using Hive.Overlay.Api;
using System;
using System.Linq;

namespace Hive.Overlay.Kademlia
{
    public class KademliaServer : IKadmeliaServer
    {
        // Network State
        private readonly IBucketList routingTable;
        private readonly TimeSpan allowedClockSkew = new TimeSpan(0, 30, 0);
        public KademliaServer(IBucketList routingTable)
        {
            this.routingTable = routingTable;
        }

        public SearchResult CloseContacts(Contact senderId, byte[] key)
        {
            routingTable.AddContact(NetworkContact.Parse(senderId));
            var result = routingTable.CloseContacts(new KadId(key), senderId.GetID());
            return new SearchResult() { Contacts = result.Select(x => x.ToContact()).ToArray() };
        }

        public byte[] Ping(Contact senderId)
        {
            Log("I was pinged!");
            routingTable.AddContact(NetworkContact.Parse(senderId));
            return this.routingTable.MySelf.Address.Data;
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
