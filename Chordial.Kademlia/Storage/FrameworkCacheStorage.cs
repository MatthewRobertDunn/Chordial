using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chordial.Kademlia.Storage
{
    public class FrameworkCacheStorage : IStorage
    {
        MemoryCache cache = new MemoryCache("storage");

        public IEnumerable<StorageItem> GetItems(byte[] key)
        {
            var stringKey = BitConverter.ToString(key);
            var storage = cache.Get(stringKey) as List<StorageItem>;

            if (storage != null)
                lock (storage)
                {
                    var now = DateTime.UtcNow;
                    storage.RemoveAll(x => now > x.Expires);
                    //Get max expires
                    if (storage.Count > 0)
                    {
                        var maxExpires = storage.Max(x => x.Expires);
                        cache.Set(stringKey, storage, maxExpires);
                    }
                    else
                    {
                        cache.Remove(stringKey);
                    }
                }

            if (storage?.Count == 0)
                return null;

            return storage;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool PutItem(StorageItem item)
        {
            var stringKey = BitConverter.ToString(item.Key);
            var storage = new List<StorageItem>();
            storage = cache.AddOrGetExisting(stringKey, storage, item.Expires) as List<StorageItem>;
            lock (storage)
            {
                storage.Add(item);
                if (storage.Count > 1)
                {
                    var maxExpires = storage.Max(x => x.Expires);
                    cache.Add(stringKey, storage, maxExpires);
                }
            }

            return true;
        }
    }
}
