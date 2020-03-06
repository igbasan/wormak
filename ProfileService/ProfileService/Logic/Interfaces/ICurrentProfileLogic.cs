using ProfileService.Models;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Logic.Interfaces
{
    public interface ICurrentProfileLogic
    {
        Task<CurrentProfile> SetCurrentProfileAsync(string userID, string profileID, ProfileType profileType);
        Task<CurrentProfile> GetCurrentProfileAsync(string userID);
    }
}
