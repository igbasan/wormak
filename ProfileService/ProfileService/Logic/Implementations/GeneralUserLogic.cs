using ProfileService.Data.Interfaces;
using ProfileService.Logic.Interfaces;
using ProfileService.Models.Exceptions;
using ProfileService.Models.Implementations;
using System;
using System.Threading.Tasks;

namespace ProfileService.Logic.Implementations
{
    public class GeneralUserLogic : ProfileLogic<GeneralUser>, IGeneralUserLogic
    {
        public GeneralUserLogic(IGeneralUserDAO generalUserDAO) : base(generalUserDAO)
        {
        }
        public override async Task<GeneralUser> SetupProfileAsync(GeneralUser profile)
        {
            if (profile == null) throw new ArgumentNullException("profile");

            GeneralUser existingProfile = await profileDAO.GetProfileByUserIDAsync(profile.UserId);
            //profile with name already exists for the user
            if (existingProfile != null) throw new ProfileServiceException($"You already have a user profile");

            return await profileDAO.SaveProfileAsync(profile);
        }

        public override async Task<GeneralUser> UpdateProfileAsync(GeneralUser profile)
        {
            if (profile == null) throw new ArgumentNullException("profile");

            //check if the profile to be updated has the same userId
            GeneralUser existingProfile = await profileDAO.GetProfileByIDAsync(profile.Id);

            if (existingProfile == null) throw new ProfileServiceException($"Invalid profile id");
            if (existingProfile.UserId != profile.UserId) throw new ProfileServiceException($"You do not have the access to update this user profile");

            //this needs to be set for cache
            profile.InterestsBeforeUpdate = existingProfile.Interests;

            return await profileDAO.UpdateProfileAsync(profile);
        }
    }
}

