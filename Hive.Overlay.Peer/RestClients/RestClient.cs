using Hive.Overlay.Peer.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Hive.Overlay.Peer.RestClients
{
    public abstract class RestClient
    {

        protected Uri BaseUri { get; set; }
        protected TResult PostAsync<TModel, TResult>(string restPath, TModel model)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = ValidateCert;
                using (var client = new HttpClient(httpClientHandler))
                {
                    return client.PostAsJsonAsync<TModel, TResult>(new Uri(BaseUri, restPath), model).Result;
                }
            }

        }

        private static bool ValidateCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
