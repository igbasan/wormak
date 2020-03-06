using InterUserService.Models;
using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Logic.Interfaces
{
    public interface IInterUserLogic<T> where T : InterUser
    {
        Task<CountList<Profile>> GetAllByActiveProfileIDAsync(string profileID, ProfileType profileType, int skip, int take);
        Task<CountList<Profile>> GetAllByPassiveProfileIDAsync(string profileID, ProfileType profileType, int skip, int take);
        Task<T> SetUpRelationshipAsync(T interUser, bool shouldDeactivate);
        Task<T> GetByActiveProfileIDandPassiveProfileIDAsync(string activeProfileID, ProfileType activeProfileType, string passiveProfileID, ProfileType passiveProfileType);
    }
}
