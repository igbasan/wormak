using InterUserService.Models;
using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Logic.Interfaces
{
    public interface IProfileLogic
    {
        Task<List<Profile>> GetProfilesAsync(Dictionary<string, ProfileType> idTypePairs);       
    }
}
