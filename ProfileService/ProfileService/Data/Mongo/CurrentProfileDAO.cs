using MongoDB.Driver;
using ProfileService.Data.Interfaces;
using ProfileService.Models.Implementations;
using ProfileService.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Mongo
{
    public class CurrentProfileDAO : ICurrentProfileDAO
    {
        protected IMongoCollection<CurrentProfile> _currentProfiles;
        protected IMongoDatabase database;
        public CurrentProfileDAO(IStoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);

            _currentProfiles = database.GetCollection<CurrentProfile>(settings.CurrentProfilesCollectionName);
        }
        public async Task<CurrentProfile> GetCurrentProfileAsync(string userID)
        {
            var currentProfiles = await _currentProfiles.FindAsync<CurrentProfile>(g => g.UserID == userID);
            return await currentProfiles.FirstOrDefaultAsync();
        }

        public async Task<CurrentProfile> SaveCurrentProfileAsync(CurrentProfile currentProfile)
        {
            await _currentProfiles.InsertOneAsync(currentProfile);
            return currentProfile;
        }

        public async Task<CurrentProfile> UpdateCurrentProfileAsync(CurrentProfile currentProfile)
        {
            var filter = Builders<CurrentProfile>.Filter.Eq(s => s.Id, currentProfile.Id);
            await _currentProfiles.ReplaceOneAsync(filter, currentProfile);
            return currentProfile;
        }
    }
}
