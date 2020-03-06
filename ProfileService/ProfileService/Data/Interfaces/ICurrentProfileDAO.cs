using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Interfaces
{
    public interface ICurrentProfileDAO
    {
        Task<CurrentProfile> GetCurrentProfileAsync(string userID);
        Task<CurrentProfile> SaveCurrentProfileAsync(CurrentProfile currentProfile);
        Task<CurrentProfile> UpdateCurrentProfileAsync(CurrentProfile currentProfile);
    }
}
