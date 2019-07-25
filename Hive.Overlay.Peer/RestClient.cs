//using Hive.Overlay.Api;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text;

//namespace Hive.Overlay.Peer
//{
//    public class RestClient : IKadmeliaServer
//    {
//        private Uri baseUri;
//        public RestClient (Uri remoteAddress)
//        {
//            this.baseUri = new Uri(remoteAddress, "/v1/hive/");
//        }

//        public byte[] Address(Contact senderId = null)
//        {
//            var client = new HttpClient();
//            var body = new StringContent(JsonConvert.SerializeObject(senderId));
//            client.PostAsJsonAsync(new Uri(baseUri,"address"),body).Result.Content.
//        }

//        public Contact[] CloseContacts(byte[] key, Contact senderId = null)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
