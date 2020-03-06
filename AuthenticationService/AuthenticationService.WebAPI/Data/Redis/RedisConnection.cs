using AuthenticationService.WebAPI.Data.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Redis
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
            var item= await database.StringGetAsync(key);
            T value = default(T);
              if(item.HasValue) value = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(item);
            return value;
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
    }
}
