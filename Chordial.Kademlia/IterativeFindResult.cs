using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chordial.Kademlia
{
    public class IterativeFindResult
    {
        public IList<string> Values { get; set; }
        public int NumberIterations { get; set; }
        public NetworkContact TargetPeer { get; set; }
        public List<NetworkContact> ClosestPeers { get; set; }
    }
}
