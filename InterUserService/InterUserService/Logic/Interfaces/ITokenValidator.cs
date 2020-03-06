using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Logic.Interfaces
{
    public interface ITokenValidator
    {
        Task<Profile> ValidateTokenAsync(string token);
        Task<string> ValidateAppTokenAsync(string token);
    }
}
