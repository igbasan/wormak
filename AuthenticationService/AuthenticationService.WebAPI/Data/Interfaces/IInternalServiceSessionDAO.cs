using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Interfaces
{
   public interface IInternalServiceSessionDAO
    {
        Task<InternalServiceSession> GetServiceSessionByAppKeyAsync(string appKey);
        Task<InternalServiceSession> GetServiceSessionByTokenAsync(string token);
        Task<InternalServiceSession> UpdateServiceSessionAsync(InternalServiceSession session);
        Task<InternalServiceSession> CreateServiceSessionAsync(InternalServiceSession session);
    }
}
