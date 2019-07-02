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

        public static bool TryParse(Contact contact, out NetworkContact result)
        {
            try
            {
                result = NetworkContact.Parse(contact);
                return true;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                result = null;
                return false;
            }
        }

        public NetworkContact(KadId nodeId, Uri uri)
        {
            this.Id = nodeId;
            this.Uri = uri;
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
