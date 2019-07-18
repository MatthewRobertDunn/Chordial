using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.Overlay.Kademlia
{
    public interface IKademilaClient
    {
        bool Booststrap(IList<string> bootstrapPeerUris);
        IterativeFindResult Put(byte[] key, string data, TimeSpan expires, int replicationFactor);
        IterativeFindResult Get(byte[] key);
    }
}
