using Hive.Overlay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hive.Overlay.Kademlia
{
    public class NetworkContact
    {

        public static NetworkContact Parse(byte[] nodeId, string[] uriString)
        {
            var uris = uriString.Select(x => new Uri(x)).ToList();
            return new NetworkContact(new KadId(nodeId), uris);
        }

        public static NetworkContact Parse(Contact contact)
        {
            return Parse(contact.Address, contact.Uri);
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

        public NetworkContact(KadId nodeId, IList<Uri> uri)
        {
            this.Id = nodeId;
            this.Uri = uri;
        }

        public KadId Id { get; }

        public IList<Uri> Uri { get; }

        /// <summary>
        /// In the future this will return the preferred URI for contacting an overlay node
        /// For now we just pick the first on the list
        /// </summary>
        public Uri UriDefault
        {
            get
            {
                return Uri.First();
            }
        }

        public Contact ToContact()
        {
            return new Contact()
            {
                Address = Id.Data,
                Uri = this.Uri.Select(x => x.ToString()).ToArray()
            };
        }
    }
}
