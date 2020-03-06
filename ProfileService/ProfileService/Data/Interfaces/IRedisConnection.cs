using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Interfaces
{
    public interface IRedisConnection
    {
        IDatabase GetDatabase();
        Task<T> GetAsync<T>(string key);
        Task<List<T>> GetAsync<T>(List<string> keys);
        Task<bool> SetAsync<T>(string key, T item);
        Task<bool> SetAsync<T>(string key, T item, int expirationInDays);
        Task<bool> SetAsync<T>(Dictionary<string, T> keyValuePairs);
        Task<long> SetAddListAsync(string key, List<string> itemIds);
        Task<bool> SetRemoveAsync(string key, string itemId);
        Task<bool> SetAddAsync(string key, string itemId);
        Task<bool> RemoveKeyAsync(string key);
        Task<List<string>> GetSetListAsync(string key);
    }
}
