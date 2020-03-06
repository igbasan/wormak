using ProfileService.Data.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Redis
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

        public async Task<List<T>> GetAsync<T>(List<string> keys)
        {
            var itemList = await database.StringGetAsync(keys.Select(v => (RedisKey)v).ToArray());
            List<T> valueList = new List<T>();
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

        public async Task<bool> SetAsync<T>(Dictionary<string, T> keyValuePairs)
        {
            KeyValuePair<RedisKey, RedisValue>[] redisKeyValuePairs = keyValuePairs
                .Select(x => new KeyValuePair<RedisKey, RedisValue>(x.Key, Newtonsoft.Json.JsonConvert.SerializeObject(x.Value)))
                .ToArray();
            return await database.StringSetAsync(redisKeyValuePairs);
        }

        public async Task<long> SetAddListAsync(string key, List<string> itemIds)
        {
            RedisValue[] redisValues = itemIds
                .Select(x => (RedisValue)x)
                .ToArray();
            return await database.SetAddAsync(key, redisValues);
        }

        public async Task<bool> SetRemoveAsync(string key, string itemId)
        {
            return await database.SetRemoveAsync(key, itemId);
        }

        public async Task<bool> SetAddAsync(string key, string itemId)
        {
            return await database.SetAddAsync(key, itemId);
        }
        public async Task<List<string>> GetSetListAsync(string key)
        {
            var itemList = await database.SetMembersAsync(key);

            List<string> valueList = new List<string>();
            if (itemList == null && itemList.Length == 0) return valueList;

            //loop through the list and get the values
            foreach (var item in itemList)
            {
                if (!item.HasValue) continue;
                valueList.Add(item);
            }
            return valueList;
        }

        public async Task<bool> RemoveKeyAsync(string key)
        {
            return await database.KeyDeleteAsync(key);
        }
    }
}
