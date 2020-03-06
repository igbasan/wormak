using InterUserService.Data.Interfaces;
using InterUserService.Models.Implemetations;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Data.Redis
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
            if (item.HasValue) value = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(item);
            return value;
        }

        public async Task<CountList<T>> GetSortedSetAsync<T>(string key, int skip, int take)
        {
            var itemList = await database.SortedSetRangeByScoreAsync(key, double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Ascending, skip, take);
            long itemCount = await database.SortedSetLengthAsync(key);
            CountList<T> valueList = new CountList<T>();
            valueList.TotalCount = itemCount;

            if (itemList == null && itemList.Length == 0) return valueList;

            //loop through the list and get the values
            foreach (var item in itemList)
            {
                if (!item.HasValue) continue;
                valueList.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(item));
            }
            return valueList;
        }

        public async Task<bool> SetAsync<T>(string key, T item)
        {
            string stringValue = Newtonsoft.Json.JsonConvert.SerializeObject(item);
            return await database.StringSetAsync(key, stringValue);
        }
        public async Task<bool> SetAsync<T>(string key, T item, int expirationInDays)
        {
            string stringValue = Newtonsoft.Json.JsonConvert.SerializeObject(item);
            return await database.StringSetAsync(key, stringValue, TimeSpan.FromDays(expirationInDays));
        }

        public async Task<long> SetSortedSetAsync<T>(string key, Dictionary<double, T> valueScorePairs)
        {
            SortedSetEntry[] redisKeyValuePairs = valueScorePairs
                .Select(x => new SortedSetEntry(Newtonsoft.Json.JsonConvert.SerializeObject(x.Value), x.Key))
                .ToArray();
            return await database.SortedSetAddAsync(key, redisKeyValuePairs);
        }

        public async Task<long> RemoveSortedSetAsync(string key)
        {
            return await database.SortedSetRemoveRangeByScoreAsync(key, double.NegativeInfinity, double.PositiveInfinity);
        }
    }
}
