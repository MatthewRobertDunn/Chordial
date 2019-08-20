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
    public class RestClient : IKadmeliaServer
    {
        private Uri baseUri;
        public RestClient(Uri remoteAddress)
        {
            this.baseUri = new Uri(remoteAddress, "/hive/v1/route/");
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

        private TResult PostAsync<TModel, TResult>(string restPath, TModel model)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = ValidateCert;
                using (var client = new HttpClient(httpClientHandler))
                {
                    return client.PostAsJsonAsync<TModel, TResult>(new Uri(baseUri, restPath), model).Result;
                }
            }

        }

        public static bool ValidateCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
