using Hive.Overlay.Api;
using Hive.Cryptography.Primitives;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;

namespace Hive.Overlay.Kademlia
{
    public class KademliaServer : IKadmeliaServer
    {
        // Network State
        private readonly IRoutingTable routingTable;
        private readonly ILogger<KademliaServer> log;
        private readonly TimeSpan allowedClockSkew = new TimeSpan(0, 30, 0);
        public KademliaServer(IRoutingTable routingTable,  ILogger<KademliaServer> log)
        {
            this.routingTable = routingTable;
            this.log = log;
            log.LogInformation("KademliaServer starting");
        }

        public Contact[] CloseContacts(byte[] key, Contact senderId)
        {
            log.LogInformation($"Close contacts for {key?.ToBase64()} requested by {senderId?.ToString()}");
            if (senderId != null)
                routingTable.AddContact(NetworkContact.Parse(senderId));

            var result = routingTable.CloseContacts(new KadId(key), senderId.GetID());
            return result.Select(x => x.ToContact()).ToArray();
        }

        public byte[] Address(Contact senderId)
        {
            log.LogInformation($"My address was requested by {senderId?.ToString()}");
            if (senderId != null)
                routingTable.AddContact(NetworkContact.Parse(senderId));

            return this.routingTable.MySelf.Address.Data;
        }

    }



}
