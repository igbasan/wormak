using ProfileService.Data.Interfaces;
using ProfileService.Models;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Redis
{
    public class ProfileRDAO<T> : IProfileDAO<T> where T : Profile
    {
        readonly IRedisConnection connection;
        readonly IProfileDAO<T> profileDao;
        protected readonly string Indentifier;
        public ProfileRDAO(IProfileDAO<T> profileDao, IRedisConnection connection, string indentifier)
        {
            this.connection = connection ?? throw new ArgumentNullException("connection");
            this.profileDao = profileDao ?? throw new ArgumentNullException("profileDao");
            this.Indentifier = indentifier ?? throw new ArgumentNullException("indentifier");
        }

        public async Task<List<T>> GetAllProfilesAsync(string userId)
        {
            //get from cache
            List<T> theProfiles = await connection.GetAsync<List<T>>($"{Indentifier}_AllUserID_{userId}");
            if (theProfiles != null && theProfiles.Count > 0) return theProfiles;

            //get if cache doesn't have the value
            theProfiles = await profileDao.GetAllProfilesAsync(userId);

            //return user after cache
            if (theProfiles != null && theProfiles.Count > 0) await connection.SetAsync($"{Indentifier}_AllUserID_{userId}", theProfiles, 1);
            return theProfiles;
        }
        public async Task<List<T>> GetAllProfilesByProfileIDsAsync(List<string> profileIds)
        {
            profileIds = profileIds.Distinct().ToList();
            //get from cache
            List<string> profileKeys = profileIds.Select(c => $"{Indentifier}_ID_{c}").ToList();
            List<T> theProfiles = await connection.GetAsync<T>(profileKeys);

            //check list to confirm if all keys were gotten
            if (theProfiles != null && profileKeys.Count == theProfiles.Count) return theProfiles;

            //only get what we need to from the Db
            var gottenKeys = theProfiles?.Select(c => c.Id);
            List<string> notGottenKeys = profileKeys.Where(v => !gottenKeys?.Contains(v) ?? true).Select(v => v.Replace($"{Indentifier}_ID_", string.Empty)).ToList();

            //get if cache doesn't have the value
            var newProfiles = await profileDao.GetAllProfilesByProfileIDsAsync(notGottenKeys);

            //return profiles after cache
            if (newProfiles != null && newProfiles.Count > 0)
            {
                Dictionary<string, T> newProfileDict = new Dictionary<string, T>();
                newProfiles.ForEach(b => newProfileDict.Add($"{Indentifier}_ID_{b.Id}", b));
                await connection.SetAsync(newProfileDict);

                // add new proflies
                theProfiles.AddRange(newProfiles);
            }
            return theProfiles;
        }


        public async Task<T> GetProfileByIDAsync(string id)
        {
            //get from cache
            T theProfile = await connection.GetAsync<T>($"{Indentifier}_ID_{id}");
            if (theProfile != null) return theProfile;

            //get if cache doesn't have the value
            theProfile = await profileDao.GetProfileByIDAsync(id);

            //return user after cache
            if (theProfile != null) await connection.SetAsync($"{Indentifier}_ID_{id}", theProfile, 1);
            return theProfile;
        }

        public async Task<T> GetProfileByUserIDAndNameAsync(string userID, string name)
        {
            T theProfile = await profileDao.GetProfileByUserIDAndNameAsync(userID, name);
            return theProfile;
        }

        public async Task<T> GetProfileByUserIDAsync(string userID)
        {
            //get from cache
            T theProfile = await connection.GetAsync<T>($"{Indentifier}_UserID_{userID}");
            if (theProfile != null) return theProfile;

            //get if cache doesn't have the value
            theProfile = await profileDao.GetProfileByUserIDAsync(userID);

            //return user after cache
            if (theProfile != null) await connection.SetAsync($"{Indentifier}_UserID_{userID}", theProfile, 1);
            return theProfile;
        }

        public async Task<T> SaveProfileAsync(T profile)
        {
            profile = await profileDao.SaveProfileAsync(profile);

            //return user after cache
            if (profile == null) return profile;

            //update cached values too
            await connection.SetAsync($"{Indentifier}_ID_{profile.Id}", profile, 1);
            await connection.SetAsync($"{Indentifier}_UserID_{profile.UserId}", profile, 1);
            await connection.RemoveKeyAsync($"{Indentifier}_AllUserID_{profile.UserId}");

            //update interests
            if (profile.Interests == null)
                return profile;

            //add to redis list
            if (profile.Interests.Count > 0)
            {
                foreach (var interestToAdd in profile.Interests)
                {
                    await connection.SetAddAsync($"{Indentifier}_Interest_{interestToAdd}", profile.Id);
                }
            }

            return profile;
        }

        public async Task<T> UpdateProfileAsync(T profile)
        {
            profile = await profileDao.UpdateProfileAsync(profile);

            //return user after cache
            if (profile == null) return profile;

            //update cached values too
            await connection.SetAsync($"{Indentifier}_ID_{profile.Id}", profile, 1);
            await connection.SetAsync($"{Indentifier}_UserID_{profile.UserId}", profile, 1);
            await connection.RemoveKeyAsync($"{Indentifier}_AllUserID_{profile.UserId}");

            //update interests
            if (profile.Interests == null)
                return profile;

            List<Interest> interestsToAdd = profile.Interests.Where(v => !profile.InterestsBeforeUpdate?.Contains(v) ?? false).ToList();
            List<Interest> interestsToRemove = profile.InterestsBeforeUpdate?.Where(v => !profile.Interests.Contains(v)).ToList();

            //add to redis list
            if (interestsToAdd.Count > 0)
            {
                foreach (var interestToAdd in interestsToAdd)
                {
                    await connection.SetAddAsync($"{Indentifier}_Interest_{interestToAdd}", profile.Id);
                }
            }

            //remove to redis list
            if (interestsToRemove?.Count > 0)
            {
                foreach (var interestToRemove in interestsToRemove)
                {
                    await connection.SetRemoveAsync($"{Indentifier}_Interest_{interestToRemove}", profile.Id);
                }
            }
            return profile;
        }

        public async Task<List<T>> GetAllProfilesByInterestsAsync(List<Interest> interests)
        { //get from cache
            List<string> profileIds = new List<string>();
            if (interests == null) return new List<T>();

            List<Interest> interestsToSearch = new List<Interest>();

            foreach (var interest in interests)
            {
                var ids = await connection.GetSetListAsync($"{Indentifier}_Interest_{interest}");
                
                if (ids == null || ids.Count == 0) interestsToSearch.Add(interest);
                else profileIds.AddRange(ids);
            }

            //get all by profile Ids
            List<T> theProfiles = await GetAllProfilesByProfileIDsAsync(profileIds);

            if (interestsToSearch.Count == 0) return theProfiles;

            //get if cache doesn't have the value
            List<T> newProfiles = await profileDao.GetAllProfilesByInterestsAsync(interestsToSearch);

            //if new list is empty retun the already found profiles
            if (newProfiles == null || newProfiles.Count == 0) return theProfiles;

            theProfiles.AddRange(newProfiles);
            //return user after cache
            foreach (var interest in interestsToSearch)
            {
                var profiles = newProfiles?.Where(c => c.Interests?.Contains(interest) ?? false);
                if (profiles == null) continue;

                //this is what works for redis 2.4 and below
                // foreach(var profile in profiles) { await connection.SetAddAsync($"{Indentifier}_Interest_{interest}", profile.Id); }

                var profileIdsToSet = profiles.Select(v => v.Id).ToList();
                await connection.SetAddListAsync($"{Indentifier}_Interest_{interest}", profileIdsToSet);
            }
            return theProfiles;
        }
    }
}
