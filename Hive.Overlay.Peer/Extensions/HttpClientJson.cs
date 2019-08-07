using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hive.Overlay.Peer.Extensions
{
    public static class HttpClientJson
    {
        public static async Task<TResult> PostAsJsonAsync<TModel, TResult>(this HttpClient client, Uri requestUrl, TModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResult =  await client.PostAsync(requestUrl, stringContent);
            if (!httpResult.IsSuccessStatusCode)
                throw new Exception($"Error calling {requestUrl}");

            var stringResult = await httpResult.Content.ReadAsStringAsync();

            var objectResult = JsonConvert.DeserializeObject<TResult>(stringResult);
            return objectResult;
        }
    }
}
