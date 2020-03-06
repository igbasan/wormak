using InterUserService.Models.Implemetations;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Data.Interfaces
{
    public interface IRedisConnection
    {
        IDatabase GetDatabase();
        Task<T> GetAsync<T>(string key);
        Task<CountList<T>> GetSortedSetAsync<T>(string key, int skip, int take);
        Task<bool> SetAsync<T>(string key, T item);
        Task<bool> SetAsync<T>(string key, T item, int expirationInDays);
        Task<long> SetSortedSetAsync<T>(string key, Dictionary<double, T> keyValuePairs);
        Task<long> RemoveSortedSetAsync(string key);
    }
}
