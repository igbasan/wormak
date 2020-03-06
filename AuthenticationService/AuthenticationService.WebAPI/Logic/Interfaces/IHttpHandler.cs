using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Logic.Interfaces
{
    public interface IHttpHandler
    {
        Task<string> GetStringAsync(string url);
        Task<HttpResponseMessage> PostAsJsonAsync(string url, object content);
        void AddDefaultRequestHeaders(string name, string value);
        void RemoveDefaultRequestHeaders(string name);

    }
}
