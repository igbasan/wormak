using ProfileService.Models;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Interfaces
{
    public interface IProfileDAO<T> where T : Profile
    {
        Task<T> SaveProfileAsync(T profile);
        Task<T> UpdateProfileAsync(T profile);
        Task<T> GetProfileByIDAsync(string id);
        Task<T> GetProfileByUserIDAsync(string userID);
        Task<T> GetProfileByUserIDAndNameAsync(string userID, string name);
        Task<List<T>> GetAllProfilesAsync(string userId);
        Task<List<T>> GetAllProfilesByProfileIDsAsync(List<string> profileIds);
        Task<List<T>> GetAllProfilesByInterestsAsync(List<Interest> interests);
    }
}
