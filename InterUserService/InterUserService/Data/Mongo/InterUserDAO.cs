using InterUserService.Data.Interfaces;
using InterUserService.Models.Implemetations;
using InterUserService.Models.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Data.Mongo
{
    public class InterUserDAO<T> : IInterUserDAO<T> where T : InterUser
    {
        protected IMongoCollection<T> _interUsers;
        protected IMongoDatabase database;
        public InterUserDAO(IStoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);
        }

        public async Task<T> CreateAsync(T interUser)
        {
            await _interUsers.InsertOneAsync(interUser);
            return interUser;
        }

        public async Task<CountList<T>> GetAllByActiveProfileIDAsync(string profileID, int skip, int take)
        {
            var fluentFinder = _interUsers.Find<T>(g => g.ActiveProfileID == profileID && g.IsActive).Skip(skip);

            if (take >= 0) fluentFinder.Limit(take);

            var interUsers = await fluentFinder.SortBy(x => x.Id).ToCursorAsync();

            List<T> interUsersList = await interUsers.ToListAsync();

            CountList<T> result = new CountList<T>();
            if (interUsersList != null) result.AddRange(interUsersList);

            return result;
        }

        public async Task<CountList<T>> GetAllByPassiveProfileIDAsync(string profileID, int skip, int take)
        {
            var fluentFinder = _interUsers.Find<T>(g => g.PassiveProfileID == profileID && g.IsActive).Skip(skip);

            if (take >= 0) fluentFinder.Limit(take);

            var interUsers = await fluentFinder.SortBy(x => x.Id).ToCursorAsync();

            List<T> interUsersList = await interUsers.ToListAsync();

            CountList<T> result = new CountList<T>();
            if (interUsersList != null) result.AddRange(interUsersList);

            return result;
        }

        public async Task<T> GetByActiveProfileIDandPassiveProfileIDAsync(string activeProfileID, string passiveProfileID)
        {
            var interUsers = await _interUsers.FindAsync<T>(g => g.ActiveProfileID == activeProfileID && g.PassiveProfileID == passiveProfileID);
            return await interUsers.FirstOrDefaultAsync();
        }

        public async Task<T> UpdateAsync(T interUser)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, interUser.Id);
            await _interUsers.ReplaceOneAsync(filter, interUser);
            return interUser;
        }
    }
}
