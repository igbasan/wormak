using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Logic.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Logic.Implementations
{
    public class UserLogic : IUserLogic
    {
        protected readonly IUserDAO userDAO;
        public UserLogic(IUserDAO userDAO)
        {
            this.userDAO = userDAO ?? throw new ArgumentNullException("userDAO");
        }

        public Task<User> CreateUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return userDAO.CreateUserAsync(user);
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException("email");
            return userDAO.GetUserByEmailAsync(email);
        }
    }
}
