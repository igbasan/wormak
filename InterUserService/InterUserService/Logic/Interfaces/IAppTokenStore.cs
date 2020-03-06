using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Logic.Interfaces
{
    public interface IAppTokenStore
    {
        Task<string> GetAppTokenAsync();
    }
}
