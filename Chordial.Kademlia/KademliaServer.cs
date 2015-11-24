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
        private readonly IStorage _storage;
        private TimeSpan _allowedClockSkew = new TimeSpan(0, 30, 0);
        private Func<Uri, IKadmeliaServer> serverFactory;
        private Contact myself;
        public KademliaServer(IBucketList cache, IStorage storage, Func<Uri, IKadmeliaServer> serverFactory)
        {
            this.serverFactory = this.serverFactory;
            this.routingTable = cache;
            this._storage = storage;
            this.myself = cache.OurContact;
        }

        public SearchResult FindNode(Contact senderId, byte[] key)
        {
            routingTable.AddContact(senderId);
            var result = routingTable.CloseContacts(new ID(key), new ID(senderId.NodeId));
            return new SearchResult() { Contacts = result.ToArray() };
        }

        public SearchResult FindValue(Contact senderId, byte[] key)
        {
            routingTable.AddContact(senderId);
            var storageItems = _storage.GetItems(key);
            if (storageItems != null)
                return new SearchResult() { Values = storageItems.Select(x => x.Value).ToArray() };

            return FindNode(senderId, key);
        }

        public bool? StoreValue(Contact senderId, byte[] key, string data, DateTime published, DateTime expires)
        {
            //If published is older than expires that's silly
            if (published > expires)
                return false;

            //Published date shouldn't be too far in the past
            if (published > (DateTime.UtcNow + _allowedClockSkew))
                return false;

            //Expire date shouldn't be too far into the future
            if ((expires - DateTime.UtcNow) > _allowedClockSkew)
                return false;

            var hash = ID.Hash(data);

            var item = new StorageItem()
            {
                Expires = expires,
                PublicationDate = published,
                Hash = hash.Data,
                Key = key,
                Value = data
            };


            _storage.PutItem(item);

            return true;
        }



        public byte[] Ping(Contact senderId)
        {
            Log("I was pinged!");
            routingTable.AddContact(senderId);
            return this.myself.NodeId;
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
