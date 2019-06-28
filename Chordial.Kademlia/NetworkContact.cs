using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chordial.Kademlia
{
    public class NetworkContact
    {

        public static NetworkContact Parse(byte[] nodeId, string uriString)
        {
            return new NetworkContact(new KadId(nodeId), new Uri(uriString));
        }

        public static NetworkContact Parse(Contact contact)
        {
            return Parse(contact.NodeId, contact.Uri);
        }

        public NetworkContact(KadId nodeId, Uri uri)
        {
            this.Id = nodeId;
            this.Uri = Uri;
        }
        public NetworkContact(Contact contact)
        {
            this.Id = contact.GetID();
            this.Uri = new Uri(contact.Uri);
        }

        public KadId Id { get; }

        public Uri Uri { get; }

        public Contact ToContact()
        {
            return new Contact() { NodeId = Id.Data, Uri = this.Uri.ToString() };
        }
    }
}
