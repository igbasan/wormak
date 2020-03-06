using StackExchange.Redis;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Interfaces
{
    public interface IRedisConnection
    {
        IDatabase GetDatabase();
        Task<T> GetAsync<T>(string key);
        Task<bool> SetAsync<T>(string key, T item);
        Task<bool> SetAsync<T>(string key, T item, int expirationInDays);
    }
}
