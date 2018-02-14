using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CucaBot.Services
{
    [Serializable]
    public class HttpService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public HttpService()
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
        }

        public async Task<HttpResponseMessage> Send(HttpMethod method, string url)
        {
            return await Send(method, url, null);
        }

        public async Task<HttpResponseMessage> Send(HttpRequestMessage request, object body)
        {
            if (body != null)
            {
                var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var json = JsonConvert.SerializeObject(body, jsonSerializerSettings);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> Send(HttpMethod method, string url, object body)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            return await Send(request, body);
        }

        public async Task<T> Send<T>(HttpMethod method, string url)
        {
            return await Send<T>(method, url, null);
        }

        public async Task<T> Send<T>(HttpRequestMessage request, object body)
        {
            if (body != null)
            {
                var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var json = JsonConvert.SerializeObject(body, jsonSerializerSettings);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<T> Send<T>(HttpMethod method, string url, object body)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            return await Send<T>(request, body);
        }
    }
}
