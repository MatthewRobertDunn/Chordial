﻿using Hive.Overlay.Api;
using System;
using System.Diagnostics;
using System.Linq;

namespace Hive.Overlay.Kademlia
{
    public class KademliaServer : IKadmeliaServer
    {
        // Network State
        private readonly IRoutingTable routingTable;
        private readonly TimeSpan allowedClockSkew = new TimeSpan(0, 30, 0);
        public KademliaServer(IRoutingTable routingTable)
        {
            this.routingTable = routingTable;
        }

        public Contact[] CloseContacts(byte[] key, Contact senderId)
        {
            if (senderId != null)
                routingTable.AddContact(NetworkContact.Parse(senderId));

            var result = routingTable.CloseContacts(new KadId(key), senderId.GetID());
            return result.Select(x => x.ToContact()).ToArray();
        }

        public byte[] Address(Contact senderId)
        {
            Log("I was pinged!");
            if (senderId != null)
                routingTable.AddContact(NetworkContact.Parse(senderId));

            return this.routingTable.MySelf.Address.Data;
        }

        /// <summary>
        /// Log debug messages, if debugging is enabled.
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message)
        {
            Trace.WriteLine(message);
        }

    }



}
