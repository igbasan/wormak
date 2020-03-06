using ProfileService.Data.Interfaces;
using ProfileService.Logic.Interfaces;
using ProfileService.Models;
using ProfileService.Models.Exceptions;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Logic.Implementations
{
    public class ProfileLogic<T> : IProfileLogic<T> where T : Profile
    {
        protected readonly IProfileDAO<T> profileDAO;
        public ProfileLogic(IProfileDAO<T> profileDAO)
        {
            this.profileDAO = profileDAO ?? throw new ArgumentNullException("profileDAO");
        }
        public async Task<List<T>> GetAllProfilesAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("userId");

            //always ensure an empty list is sent to avoid nulls
            return await profileDAO.GetAllProfilesAsync(userId) ?? new List<T>();
        }

        public async Task<List<T>> GetAllProfilesByIDsAsync(List<string> ids)
        {
            if (ids == null) throw new ArgumentNullException("ids");

            //always ensure an empty list is sent to avoid nulls
            return await profileDAO.GetAllProfilesByProfileIDsAsync(ids) ?? new List<T>();
        }

        public async Task<List<T>> GetAllProfilesByInterestsAsync(List<Interest> interests)
        {
            if (interests == null) throw new ArgumentNullException("interests");

            //always ensure an empty list is sent to avoid nulls
            return await profileDAO.GetAllProfilesByInterestsAsync(interests) ?? new List<T>();
        }

        public async Task<T> GetProfileByIDAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException("id");
            return await profileDAO.GetProfileByIDAsync(id);
        }

        public virtual async Task<T> SetupProfileAsync(T profile)
        {
            if (profile == null) throw new ArgumentNullException("profile");

            T existingProfile = await profileDAO.GetProfileByUserIDAndNameAsync(profile.UserId, profile.Name);
            //profile with name already exists for the user
            if (existingProfile != null) throw new ProfileServiceException($"You already have a {typeof(T).Name.ToLower()} profile with this name");

            return await profileDAO.SaveProfileAsync(profile);
        }

        public virtual async Task<T> UpdateProfileAsync(T profile)
        {
            if (profile == null) throw new ArgumentNullException("profile");

            //check if the profile to be updated has the same userId
            T existingProfile = await profileDAO.GetProfileByIDAsync(profile.Id);

            if (existingProfile == null) throw new ProfileServiceException($"Invalid profile id");
            if (existingProfile.UserId != profile.UserId) throw new ProfileServiceException($"You do not have the access to update this {typeof(T).Name.ToLower()} profile");

            //profile with name already exists for the user
            T existingProfileName = await profileDAO.GetProfileByUserIDAndNameAsync(profile.UserId, profile.Name);
            if (existingProfileName !=null && existingProfileName.Id != profile.Id) throw new ProfileServiceException($"You already have a {typeof(T).Name.ToLower()} profile with this name");
            
            //this needs to be set for cache
            profile.InterestsBeforeUpdate = existingProfile.Interests;

            return await profileDAO.UpdateProfileAsync(profile);
        }
    }
}
