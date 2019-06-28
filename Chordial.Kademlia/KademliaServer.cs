using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Ninject.Parameters;
using System.ServiceModel;

namespace Chordial.Kademlia
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class KademliaServer : IKadmeliaServer
    {
        // Network State
        private readonly IBucketList routingTable;
        private readonly IStorage storage;
        private readonly TimeSpan allowedClockSkew = new TimeSpan(0, 30, 0);
        public KademliaServer(IBucketList routingTable, IStorage storage)
        {
            this.routingTable = routingTable;
            this.storage = storage;
        }

        public SearchResult FindNode(Contact senderId, byte[] key)
        {
            routingTable.AddContact(NetworkContact.Parse(senderId));
            var result = routingTable.CloseContacts(new KadId(key), senderId.GetID());
            return new SearchResult() { Contacts = result.Select(x=>x.ToContact()).ToArray() };
        }

        public SearchResult FindValue(Contact senderId, byte[] key)
        {
            routingTable.AddContact(NetworkContact.Parse(senderId));
            var storageItems = storage.GetItems(key);
            if (storageItems != null)
                return new SearchResult() { Values = storageItems.Select(x => x.Value).ToArray() };

            return FindNode(senderId, key);
        }

        public bool StoreValue(Contact senderId, byte[] key, string data, DateTime published, DateTime expires)
        {
            //Publish date can't be older than expires
            if (published > expires)
                return false;

            //Published date shouldn't be too far in the future
            if (published > (DateTime.UtcNow + allowedClockSkew))
                return false;

            //Expire date shouldn't be too far into the past
            if ((expires - DateTime.UtcNow) > allowedClockSkew)
                return false;

            var hash = KadId.Hash(data);

            var item = new StorageItem()
            {
                Expires = expires,
                PublicationDate = published,
                Hash = hash.Data,
                Key = key,
                Value = data
            };


            storage.PutItem(item);

            return true;
        }

        public byte[] Ping(Contact senderId)
        {
            Log("I was pinged!");
            routingTable.AddContact(new NetworkContact(senderId));
            return this.routingTable.MySelf.Id.Data;
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
