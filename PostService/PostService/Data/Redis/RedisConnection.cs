using PostService.Data.Interfaces;
using PostService.Models.Implementations;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PostService.Data.Redis
{
    public class RedisConnection : IRedisConnection
    {
        IDatabase database;
        public RedisConnection(string configuration)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configuration);
            database = redis.GetDatabase();
        }

        public IDatabase GetDatabase()
        {
            return database;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var item = await database.StringGetAsync(key);
            T value = default(T);
            if (item.HasValue) value = JsonSerializer.Deserialize<T>(item);
            return value;
        }
        public async Task<List<T>> GetAsync<T>(List<string> keys)
        {
            var itemList = await database.StringGetAsync(keys.Select(v => (RedisKey)v).ToArray());
            List<T> valueList = new List<T>();
            if (itemList == null && itemList.Length == 0) return valueList;

            //loop through the list and get the values
            foreach (var item in itemList)
            {
                if (!item.HasValue) continue;
                valueList.Add(JsonSerializer.Deserialize<T>(item));
            }
            return valueList;
        }

        public async Task<CountList<string>> GetSortedSetAsync(string key, int skip, int take, Order order)
        {
            var itemList = await database.SortedSetRangeByScoreAsync(key, double.NegativeInfinity, double.PositiveInfinity, Exclude.None, order, skip, take);
            long itemCount = await database.SortedSetLengthAsync(key);
            CountList<string> valueList = new CountList<string>();
            valueList.TotalCount = itemCount;

            if (itemList == null && itemList.Length == 0) return valueList;

            //loop through the list and get the values
            foreach (var item in itemList)
            {
                if (!item.HasValue) continue;
                valueList.Add(item);
            }
            return valueList;
        }

        public async Task<List<SortedSetEntry>> GetSortedSetWithScoresAsync(string key, int skip, int take, Order order)
        {
            var itemList = await database.SortedSetRangeByRankWithScoresAsync(key, skip, skip + take, order);

            return itemList?.ToList() ?? new List<SortedSetEntry>();
        }

        public async Task<bool> SetAsync<T>(string key, T item)
        {
            string stringValue = JsonSerializer.Serialize(item);
            return await database.StringSetAsync(key, stringValue);
        }
        public async Task<bool> SetAsync<T>(string key, T item, int expirationInDays)
        {
            string stringValue = JsonSerializer.Serialize(item);
            return await database.StringSetAsync(key, stringValue, TimeSpan.FromDays(expirationInDays));
        }
        public async Task<bool> SetAsync<T>(Dictionary<string, T> keyValuePairs)
        {
            KeyValuePair<RedisKey, RedisValue>[] redisKeyValuePairs = keyValuePairs
                .Select(x => new KeyValuePair<RedisKey, RedisValue>(x.Key, JsonSerializer.Serialize(x.Value)))
                .ToArray();
            return await database.StringSetAsync(redisKeyValuePairs);
        }

        public async Task<bool> SetSortedSetAsync(string key, double score, string value)
        {
            return await database.SortedSetAddAsync(key, value, score);
        }

        public async Task<long> SetSortedSetAsync<T>(string key, Dictionary<double, T> valueScorePairs)
        {
            SortedSetEntry[] redisKeyValuePairs = valueScorePairs
                .Select(x => new SortedSetEntry(JsonSerializer.Serialize(x.Value), x.Key))
                .ToArray();
            return await database.SortedSetAddAsync(key, redisKeyValuePairs);
        }

        public async Task<long> SetSortedSetAsync(string key, List<SortedSetEntry> sortedSetEntries)
        {
            return await database.SortedSetAddAsync(key, sortedSetEntries.ToArray());
        }

        public async Task<long> RemoveSortedSetAsync(string key)
        {
            return await database.SortedSetRemoveRangeByScoreAsync(key, double.NegativeInfinity, double.PositiveInfinity);
        }

        public async Task<bool> RemoveSortedSetAsync(string key, string value)
        {
            return await database.SortedSetRemoveAsync(key, value);
        }
    }

}
