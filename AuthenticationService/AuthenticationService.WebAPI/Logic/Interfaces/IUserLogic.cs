using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Logic.Interfaces
{
    public interface IUserLogic
    {
        Task<User> CreateUserAsync(User proflie);
        Task<User> GetUserByEmailAsync(string email);
    }
}
