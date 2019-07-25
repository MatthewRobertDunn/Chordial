using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.Overlay.Kademlia
{
    public class HaveAsked
    {
        public NetworkContact Contact { get; set; }
        public bool Asked { get; set; }
        public bool IsNotContactable { get; internal set; }
    }

}
