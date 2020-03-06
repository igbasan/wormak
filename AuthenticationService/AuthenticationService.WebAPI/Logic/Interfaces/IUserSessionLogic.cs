using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Logic.Interfaces
{
    public interface IUserSessionLogic
    {
        Task<UserSession> GetUserSessionByTokenAsync(string token);
        Task<UserSession> SetUpUserSessionAsync(string token, User user);
    }
}
