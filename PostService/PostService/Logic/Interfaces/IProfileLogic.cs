using PostService.Models;
using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Logic.Interfaces
{
    public interface IProfileLogic
    {
        Task<List<Profile>> GetProfilesAsync(Dictionary<string, ProfileType> idTypePairs);
        Task<List<Profile>> GetProfilesByInterestsAsync(List<Interest> interests);
    }
}
