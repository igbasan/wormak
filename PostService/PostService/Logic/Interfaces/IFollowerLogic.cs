using PostService.Models;
using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Logic.Interfaces
{
    public interface IFollowerLogic
    {
        Task<List<Profile>> GetFollowerProfilesByProfileIDAsync(string profileId, ProfileType profileType);
        Task<List<Profile>> GetFollowingProfilesByProfileIDAsync(string profileId, ProfileType profileType);
    }
}
