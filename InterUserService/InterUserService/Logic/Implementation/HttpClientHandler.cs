using InterUserService.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InterUserService.Logic.Implementation
{
    public class HttpClientHandler : IHttpHandler
    {
        private readonly HttpClient client;
        public HttpClientHandler(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException("client");
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await client.GetAsync(url);
        }
        public void AddDefaultRequestHeaders(string name, string value)
        {
            client.DefaultRequestHeaders.Add(name, value);
        }

        public void RemoveDefaultRequestHeaders(string name)
        {
            client.DefaultRequestHeaders.Remove(name);
        }

        public async Task<HttpResponseMessage> PostAsJSONAsync(string url,  object body)
        {
            return await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));
        }
    }
}
