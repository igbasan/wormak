using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using AuthenticationService.WebAPI.Models.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Mongo
{
    public class InternalServiceSessionDAO :  IInternalServiceSessionDAO
    {
        protected readonly IMongoCollection<InternalServiceSession> _sessions;
        public InternalServiceSessionDAO(IStoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _sessions = database.GetCollection<InternalServiceSession>(settings.InternalServiceSessionsCollectionName);
        }
        public async Task<InternalServiceSession> CreateServiceSessionAsync(InternalServiceSession session)
        {
            await _sessions.InsertOneAsync(session);
            return session;
        }
        public async Task<InternalServiceSession> GetServiceSessionByTokenAsync(string token)
        {
            var sessions = await _sessions.FindAsync<InternalServiceSession>(g => g.AuthToken == token);
            return await sessions.FirstOrDefaultAsync();
        }

        public async Task<InternalServiceSession> UpdateServiceSessionAsync(InternalServiceSession session)
        {
            var filter = Builders<InternalServiceSession>.Filter.Eq(s => s.Id, session.Id);
            await _sessions.ReplaceOneAsync(filter, session);
            return session;
        }
        public async Task<InternalServiceSession> GetServiceSessionByAppKeyAsync(string appKey)
        {
            var sessions = await _sessions.FindAsync(g => g.AppKey == appKey);
            return await sessions.FirstOrDefaultAsync();
        }
    }
}
