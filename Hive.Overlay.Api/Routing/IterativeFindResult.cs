using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.Overlay.Kademlia
{
    public class IterativeFindResult
    {
        public int NumberIterations { get; set; }
        public NetworkContact TargetPeer { get; set; }
        public List<NetworkContact> ClosestPeers { get; set; }
    }
}
