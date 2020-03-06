using MongoDB.Driver;
using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using AuthenticationService.WebAPI.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Mongo
{
    public class UserSessionDAO : IUserSessionDAO
    {
        protected readonly IMongoCollection<UserSession> _sessions;
        public UserSessionDAO(IStoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _sessions = database.GetCollection<UserSession>(settings.UserSessionsCollectionName);
        }
        public async Task<UserSession> CreateUserSessionAsync(UserSession session)
        {
            await _sessions.InsertOneAsync(session);
            return session;
        }
        public async Task<UserSession> GetUserSessionByTokenAsync(string token)
        {
            var sessions = await _sessions.FindAsync<UserSession>(g => g.AuthToken == token);
            return await sessions.FirstOrDefaultAsync();
        }

        public async Task<UserSession> UpdateUserSessionAsync(UserSession session)
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.Id, session.Id);
            await _sessions.ReplaceOneAsync(filter, session);
            return session;
        }

        public async Task<UserSession> GetUserSessionByUserIDAsync(string userID)
        {
            var sessions = await _sessions.FindAsync<UserSession>(g => g.UserID == userID);
            return await sessions.FirstOrDefaultAsync();
        }
    }
}
