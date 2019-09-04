using Hive.Overlay.Api;
using Hive.Overlay.Peer.Dto;
using Hive.Overlay.Peer.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Hive.Overlay.Peer
{
    public class KadmeliaServerClient :RestClients.RestClient, IKadmeliaServer
    {
        public KadmeliaServerClient(Uri remoteAddress)
        {
            this.BaseUri = new Uri(remoteAddress, "/hive/v1/route/");
        }

        public byte[] Address(Contact senderId = null)
        {
            return PostAsync<Contact, byte[]>("address", senderId);
        }

        public Contact[] CloseContacts(byte[] key, Contact senderId = null)
        {
            var dto = new ClosestNodeSearch()
            {
                Address = key,
                RequestedBy = senderId
            };
            return PostAsync<ClosestNodeSearch, Contact[]>("closecontacts", dto);
        }


    }
}
