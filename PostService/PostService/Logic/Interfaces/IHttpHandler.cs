using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PostService.Logic.Interfaces
{
    public interface IHttpHandler
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsJSONAsync(string url, object body);
        void AddDefaultRequestHeaders(string name, string value);
        void RemoveDefaultRequestHeaders(string name);
    }
}
