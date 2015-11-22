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
        public Contact TargetPeer { get; set; }
        public List<Contact> ClosestPeers { get; set; }
    }
}
