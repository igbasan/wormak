using System.Threading.Tasks;
using MongoDB.Driver;
using AuthenticationService.WebAPI.Models.Interfaces;
using System.Linq;
using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;

namespace AuthenticationService.WebAPI.Data.Mongo
{
    public class UserDAO : IUserDAO
    {
        private readonly IMongoCollection<User> _users;

        public UserDAO(IStoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var books = await _users.FindAsync<User>(user => user.Email == email);
            return await books.FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByIDAsync(string id)
        {
            var books = await _users.FindAsync<User>(user => user.Id == id);
            return await books.FirstOrDefaultAsync();
        }
    }
}
