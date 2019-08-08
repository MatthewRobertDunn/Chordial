using Hive.Overlay.Api;
using Hive.Overlay.Peer.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Hive.Overlay.Peer
{
    public class RestClient : IKadmeliaServer
    {
        private Uri baseUri;
        public RestClient(Uri remoteAddress)
        {
            this.baseUri = new Uri(remoteAddress, "/v1/hive/");
        }

        public byte[] Address(Contact senderId = null)
        {
            return PostAsync<Contact, byte[]>("address", senderId);
        }

        public Contact[] CloseContacts(byte[] key, Contact senderId = null)
        {
            return PostAsync<Contact, Contact[]>("closecontacts", senderId);
        }

        private TResult PostAsync<TModel, TResult>(string restPath, TModel model)
        {
            var client = new HttpClient();
            return client.PostAsJsonAsync<TModel, TResult>(new Uri(baseUri, restPath), model).Result;

        }
    }
}
