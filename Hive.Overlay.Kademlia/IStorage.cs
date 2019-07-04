using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.Overlay.Kademlia
{
    public interface IStorage
    {
        IEnumerable<StorageItem> GetItems(byte[] key);
        bool PutItem(StorageItem item);
    }
}
