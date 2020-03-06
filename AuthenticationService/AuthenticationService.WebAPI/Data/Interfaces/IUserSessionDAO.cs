using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Interfaces
{
    public interface IUserSessionDAO
    {
        Task<UserSession> GetUserSessionByUserIDAsync(string userID);
        Task<UserSession> GetUserSessionByTokenAsync(string token);
        Task<UserSession> UpdateUserSessionAsync(UserSession session);
        Task<UserSession> CreateUserSessionAsync(UserSession session);
    }
}
