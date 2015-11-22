using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chordial.Kademlia
{
    public static class Mappingcs
    {
        public static ID GetID(this Contact contact)
        {
            return new ID(contact.NodeId);
        }

    }
}
