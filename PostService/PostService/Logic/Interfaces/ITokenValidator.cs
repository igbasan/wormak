using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Logic.Interfaces
{
    public interface ITokenValidator
    {
        Task<Profile> ValidateTokenAsync(string token);
        Task<string> ValidateAppTokenAsync(string token);
    }
}
