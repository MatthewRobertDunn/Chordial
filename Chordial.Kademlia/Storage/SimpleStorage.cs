using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chordial.Kademlia.Storage
{
    public class SimpleStorage : IStorage
    {
        Dictionary<string, List<StorageItem>> cache = new Dictionary<string, List<StorageItem>>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<StorageItem> GetItems(byte[] key)
        {
            var stringKey = BitConverter.ToString(key);
            List<StorageItem> result = null;
            cache.TryGetValue(stringKey, out result);
            return result;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool PutItem(StorageItem item)
        {
            var stringKey = BitConverter.ToString(item.Key);

            List<StorageItem> result = null;
            cache.TryGetValue(stringKey, out result);

            if (result == null)
            {
                result = new List<StorageItem>();
                cache[stringKey] = result;
            }

            result.Add(item);

            return true;
        }
    }

}
