using AuthenticationService.WebAPI.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Logic.Implementations
{
    public class HttpClientHandler : IHttpHandler
    {
        private readonly HttpClient client;


        public HttpClientHandler(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException("client");
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync(string url, object content)
        {
            return await client.PostAsync(url, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
        }

        public async Task<string> GetStringAsync(string url)
        {
            return await client.GetStringAsync(url);
        }

        public void AddDefaultRequestHeaders(string name, string value)
        {
            client.DefaultRequestHeaders.Add(name, value);
        }

        public void RemoveDefaultRequestHeaders(string name)
        {
            client.DefaultRequestHeaders.Remove(name);
        }
    }
}
