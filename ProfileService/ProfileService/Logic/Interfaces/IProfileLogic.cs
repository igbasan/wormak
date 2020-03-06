using ProfileService.Models;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Logic.Interfaces
{
    public interface IProfileLogic<T> where T : Profile
    {
        Task<T> SetupProfileAsync(T profile);
        Task<T> UpdateProfileAsync(T profile);
        Task<T> GetProfileByIDAsync(string id);
        Task<List<T>> GetAllProfilesAsync(string userId);
        Task<List<T>> GetAllProfilesByIDsAsync(List<string> ids);
        Task<List<T>> GetAllProfilesByInterestsAsync(List<Interest> interests);
    }
}
