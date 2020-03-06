using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProfileService.Logic.Interfaces
{
    public interface IHttpHandler
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }
}
