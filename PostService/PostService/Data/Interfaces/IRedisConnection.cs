using PostService.Models.Implementations;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Data.Interfaces
{
    public interface IRedisConnection
    {
        IDatabase GetDatabase();
        Task<T> GetAsync<T>(string key);
        Task<List<T>> GetAsync<T>(List<string> keys);
        Task<CountList<string>> GetSortedSetAsync(string key, int skip, int take, Order order);
        Task<List<SortedSetEntry>> GetSortedSetWithScoresAsync(string key, int skip, int take, Order order);
        Task<bool> SetAsync<T>(string key, T item);
        Task<bool> SetAsync<T>(string key, T item, int expirationInDays);
        Task<bool> SetAsync<T>(Dictionary<string, T> keyValuePairs);
        Task<long> SetSortedSetAsync<T>(string key, Dictionary<double, T> keyValuePairs);
        Task<long> SetSortedSetAsync(string key, List<SortedSetEntry> sortedSetEntries);
        Task<bool> SetSortedSetAsync(string key, double score, string value);
        Task<long> RemoveSortedSetAsync(string key);
        Task<bool> RemoveSortedSetAsync(string key, string value);
    }
}
