using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Interfaces
{
    public interface IUserDAO
    {
        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIDAsync(string id);
    }
}
