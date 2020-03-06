using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Logic.Interfaces
{
    public interface ITokenValidator
    {
        Task<User> ValidateTokenAsync(string token);
        Task<string> ValidateAppTokenAsync(string token);
    }
}
