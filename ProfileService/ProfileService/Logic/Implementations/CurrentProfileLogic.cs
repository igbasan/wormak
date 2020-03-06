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
    public class CurrentProfileLogic : ICurrentProfileLogic
    {
        readonly ICompanyLogic companyLogic;
        readonly IGeneralUserLogic generalUserLogic;
        readonly IProfessionalLogic professionalLogic;
        readonly ICurrentProfileDAO currentProfileDAO;
        public CurrentProfileLogic(ICompanyLogic companyLogic, IGeneralUserLogic generalUserLogic, IProfessionalLogic professionalLogic, ICurrentProfileDAO currentProfileDAO)
        {
            this.companyLogic = companyLogic ?? throw new ArgumentNullException("companyLogic");
            this.generalUserLogic = generalUserLogic ?? throw new ArgumentNullException("generalUserLogic");
            this.professionalLogic = professionalLogic ?? throw new ArgumentNullException("professionalLogic");
            this.currentProfileDAO = currentProfileDAO ?? throw new ArgumentNullException("currentProfileDAO");
        }
        public async Task<CurrentProfile> GetCurrentProfileAsync(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID)) throw new ArgumentNullException("userID");
            CurrentProfile theCurrentProfile = await currentProfileDAO.GetCurrentProfileAsync(userID);
            if (theCurrentProfile != null)
            {
                Profile theProfile = null;
                switch (theCurrentProfile.ProfileType)
                {
                    case ProfileType.Company:
                        theProfile = await companyLogic.GetProfileByIDAsync(theCurrentProfile.ProfileID);
                        break;

                    case ProfileType.GeneralUser:
                        theProfile = await generalUserLogic.GetProfileByIDAsync(theCurrentProfile.ProfileID);
                        break;

                    case ProfileType.Professional:
                        theProfile = await professionalLogic.GetProfileByIDAsync(theCurrentProfile.ProfileID);
                        break;
                }
                if (theProfile != null && theProfile.UserId == userID)
                {
                    theCurrentProfile.Name = theProfile.Name;
                    theCurrentProfile.Interests = theProfile.Interests;
                    return theCurrentProfile;
                }
            }
            GeneralUser generalUser = (await generalUserLogic.GetAllProfilesAsync(userID))?.FirstOrDefault();
            if (generalUser == null) throw new ProfileServiceException("The User Profile for this account has not been set up yet");

            if (theCurrentProfile == null)
            {
                theCurrentProfile = new CurrentProfile
                {
                    LastSetDate = DateTime.Now,
                    Name = generalUser.Name,
                    Interests = generalUser.Interests,
                    ProfileID = generalUser.Id,
                    ProfileType = generalUser.ProfileType,
                    UserID = userID
                };
                await currentProfileDAO.SaveCurrentProfileAsync(theCurrentProfile);
            }
            else
            {
                theCurrentProfile.LastSetDate = DateTime.Now;
                theCurrentProfile.Name = generalUser.Name;
                theCurrentProfile.Interests = generalUser.Interests;
                theCurrentProfile.ProfileID = generalUser.Id;
                theCurrentProfile.ProfileType = generalUser.ProfileType;
                await currentProfileDAO.UpdateCurrentProfileAsync(theCurrentProfile);
            }

            return theCurrentProfile;
        }

        public async Task<CurrentProfile> SetCurrentProfileAsync(string userID, string profileID, ProfileType profileType)
        {
            if (string.IsNullOrWhiteSpace(userID)) throw new ArgumentNullException("userID");
            if (string.IsNullOrWhiteSpace(profileID)) throw new ArgumentNullException("profileID");

            Profile theProfile = null;
            switch (profileType)
            {
                case ProfileType.Company:
                    theProfile = await companyLogic.GetProfileByIDAsync(profileID);
                    break;

                case ProfileType.GeneralUser:
                    theProfile = await generalUserLogic.GetProfileByIDAsync(profileID);
                    break;

                case ProfileType.Professional:
                    theProfile = await professionalLogic.GetProfileByIDAsync(profileID);
                    break;
            }

            if (theProfile == null) throw new ProfileServiceException("The Profile with the provided profile Id cannot be found");
            if (theProfile.UserId != userID) throw new ProfileServiceException("The Profile with the provided profile Id does not belong to the current user");

            CurrentProfile theCurrentProfile = await currentProfileDAO.GetCurrentProfileAsync(userID);
            if (theCurrentProfile == null)
            {
                theCurrentProfile = new CurrentProfile
                {
                    LastSetDate = DateTime.Now,
                    Name = theProfile.Name,
                    ProfileID = theProfile.Id,
                    ProfileType = theProfile.ProfileType,
                    UserID = userID
                };
                await currentProfileDAO.SaveCurrentProfileAsync(theCurrentProfile);
            }
            else
            {
                theCurrentProfile.LastSetDate = DateTime.Now;
                theCurrentProfile.Name = theProfile.Name;
                theCurrentProfile.ProfileID = theProfile.Id;
                theCurrentProfile.ProfileType = theProfile.ProfileType;
                await currentProfileDAO.UpdateCurrentProfileAsync(theCurrentProfile);
            }

            return theCurrentProfile;
        }
    }
}
