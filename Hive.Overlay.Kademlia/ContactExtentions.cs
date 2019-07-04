using Hive.Overlay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.Overlay.Kademlia
{
    public static class ContactExtentions
    {
        public static Uri ToUri(this Contact contact)
        {
            return new Uri(contact.Uri);
        }
    }
}
