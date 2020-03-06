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
    public class LoginAttemptDAO : ILoginAttemptDAO
    {
        private readonly IMongoCollection<LoginAttempt> _loginAttempts;
        public LoginAttemptDAO(IStoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _loginAttempts = database.GetCollection<LoginAttempt>(settings.LoginAttemptsCollectionName);
        }

        public async Task<LoginAttempt> SaveLoginAttemptAsync(LoginAttempt attempt)
        {
            await _loginAttempts.InsertOneAsync(attempt);
            return attempt;
        }
    }
}
