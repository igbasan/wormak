using MongoDB.Driver;
using ProfileService.Data.Interfaces;
using ProfileService.Models;
using ProfileService.Models.Implementations;
using ProfileService.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Mongo
{
    public class ProfileDAO<T> : IProfileDAO<T> where T : Profile
    {
        protected IMongoCollection<T> _profiles;
        protected IMongoDatabase database;
        public ProfileDAO(IStoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);
        }

        public async Task<List<T>> GetAllProfilesAsync(string userId)
        {
            var profiles = await _profiles.FindAsync<T>(g => g.UserId == userId);
            return profiles.ToList() ?? new List<T>();
        }

        public async Task<List<T>> GetAllProfilesByInterestsAsync(List<Interest> interests)
        {
            var profiles = await _profiles.FindAsync<T>(g => g.Interests.Any(n => interests.Contains(n)));
            return profiles.ToList() ?? new List<T>();
        }

        public async Task<List<T>> GetAllProfilesByProfileIDsAsync(List<string> profileIds)
        {
            var profiles = await _profiles.FindAsync<T>(g => profileIds.Contains(g.Id));
            return profiles.ToList() ?? new List<T>();
        }

        public async Task<T> GetProfileByIDAsync(string id)
        {
            var profiles = await _profiles.FindAsync<T>(g => g.Id == id);
            return await profiles.FirstOrDefaultAsync();
        }

        public async Task<T> GetProfileByUserIDAndNameAsync(string userID, string name)
        {
            var profiles = await _profiles.FindAsync<T>(g => g.UserId == userID && g.Name == name);
            return await profiles.FirstOrDefaultAsync();
        }

        public async Task<T> GetProfileByUserIDAsync(string userID)
        {
            var profiles = await _profiles.FindAsync<T>(g => g.UserId == userID);
            return await profiles.FirstOrDefaultAsync();
        }

        public async Task<T> SaveProfileAsync(T profile)
        {
            await _profiles.InsertOneAsync(profile);
            return profile;
        }

        public async Task<T> UpdateProfileAsync(T profile)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, profile.Id);
            await _profiles.ReplaceOneAsync(filter, profile);
            return profile;
        }
    }
}
