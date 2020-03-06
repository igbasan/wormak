using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Logic.Interfaces
{
    public interface IInternalServiceSessionLogic
    {
        Task<InternalServiceSession> GetServiceSessionByTokenAsync(string token);
        Task<InternalServiceSession> SetUpServiceSessionAsync(string appName, string appID);
    }
}
