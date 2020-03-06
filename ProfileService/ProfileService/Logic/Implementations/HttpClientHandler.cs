using ProfileService.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProfileService.Logic.Implementations
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
    }

}
