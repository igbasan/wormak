using InterUserService.Data.Interfaces;
using InterUserService.Logic.Interfaces;
using InterUserService.Models;
using InterUserService.Models.Exceptions;
using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Logic.Implementation
{
    public class InterUserLogic<T> : IInterUserLogic<T> where T : InterUser
    {
        readonly IInterUserDAO<T> interUserDAO;
        readonly IProfileLogic profileLogic;
        public InterUserLogic(IInterUserDAO<T> interUserDAO, IProfileLogic profileLogic)
        {
            this.interUserDAO = interUserDAO ?? throw new ArgumentNullException("interUserDAO");
            this.profileLogic = profileLogic ?? throw new ArgumentNullException("profileLogic");
        }
        public async Task<CountList<Profile>> GetAllByActiveProfileIDAsync(string profileID, ProfileType profileType, int skip, int take)
        {
            if (string.IsNullOrWhiteSpace(profileID)) throw new ArgumentNullException("profileID");
            CountList<T> interUsers = await interUserDAO.GetAllByActiveProfileIDAsync($"{profileType}_{profileID}", skip, take);

            CountList<Profile> profiles = new CountList<Profile>();
            profiles.TotalCount = interUsers?.TotalCount ?? 0;

            if (interUsers == null || interUsers.Count == 0) return profiles;

            Dictionary<string, ProfileType> profilesDict = new Dictionary<string, ProfileType>();
            foreach (var item in interUsers)
            {
                profilesDict.Add(item.PassiveProfileIDRaw, item.PassiveProfileType);
            }

            List<Profile> profilesWithNames = await profileLogic.GetProfilesAsync(profilesDict);
            if (profilesWithNames == null || profilesWithNames.Count != profilesDict.Count) throw new InterUserException("Profile Mismatch Error occured");

            profiles.AddRange(profilesWithNames);
            return profiles;
        }

        public async Task<CountList<Profile>> GetAllByPassiveProfileIDAsync(string profileID, ProfileType profileType, int skip, int take)
        {
            if (string.IsNullOrWhiteSpace(profileID)) throw new ArgumentNullException("profileID");
            CountList<T> interUsers = await interUserDAO.GetAllByPassiveProfileIDAsync($"{profileType}_{profileID}", skip, take);

            CountList<Profile> profiles = new CountList<Profile>();
            profiles.TotalCount = interUsers?.TotalCount ?? 0;

            if (interUsers == null || interUsers.Count == 0) return profiles;

            Dictionary<string, ProfileType> profilesDict = new Dictionary<string, ProfileType>();
            foreach (var item in interUsers)
            {
                profilesDict.Add(item.ActiveProfileIDRaw, item.ActiveProfileType);
            }

            List<Profile> profilesWithNames = await profileLogic.GetProfilesAsync(profilesDict);
            if (profilesWithNames == null || profilesWithNames.Count != profilesDict.Count) throw new InterUserException("Profile Mismatch Error occured");

            profiles.AddRange(profilesWithNames);
            return profiles;
        }

        public async Task<T> GetByActiveProfileIDandPassiveProfileIDAsync(string activeProfileID, ProfileType activeProfileType, string passiveProfileID, ProfileType passiveProfileType)
        {
            if (string.IsNullOrWhiteSpace(activeProfileID)) throw new ArgumentNullException("activeProfileID");
            if (string.IsNullOrWhiteSpace(passiveProfileID)) throw new ArgumentNullException("passiveProfileID");
            return await interUserDAO.GetByActiveProfileIDandPassiveProfileIDAsync($"{activeProfileType}_{activeProfileID}", $"{passiveProfileType}_{passiveProfileID}");
        }

        public async Task<T> SetUpRelationshipAsync(T interUser, bool shouldDeactivate = false)
        {
            if (interUser == null) throw new ArgumentNullException("interUser");
            if (interUser.PassiveProfileID == interUser.ActiveProfileID) throw new InterUserException("Cannot set up profile relationship with self");

            //validate profile
            Dictionary<string, ProfileType> profilesDict = new Dictionary<string, ProfileType>() { { interUser.PassiveProfileIDRaw, interUser.PassiveProfileType } };

            Profile profileWithName = (await profileLogic.GetProfilesAsync(profilesDict))?.FirstOrDefault();
            if (profileWithName == null) throw new InterUserException("The specified profile can not be found");

            T existingRecord = await interUserDAO.GetByActiveProfileIDandPassiveProfileIDAsync(interUser.ActiveProfileID, interUser.PassiveProfileID);

            if(existingRecord == null)
            {
                //no relationship to deactivate, do nothing
                if (shouldDeactivate) return interUser;

                interUser.DateCreated = DateTime.Now;
                interUser.IsActive = true;
                existingRecord = await interUserDAO.CreateAsync(interUser);
            }
            else if(existingRecord.IsActive && shouldDeactivate) //deactivate existing relationship
            {
                existingRecord.DateUpdated = DateTime.Now;
                existingRecord.IsActive = false;
                existingRecord = await interUserDAO.UpdateAsync(existingRecord);
            }
            else if(!existingRecord.IsActive && !shouldDeactivate) //activate inactive relationship{
            {
                existingRecord.DateUpdated = DateTime.Now;
                existingRecord.IsActive = true;
                existingRecord = await interUserDAO.UpdateAsync(existingRecord);
            }

            return existingRecord;

        }
    }
}
