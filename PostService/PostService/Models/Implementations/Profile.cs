using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PostService.Models.Implementations
{
    public class Profile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("profileType")]
        public ProfileType ProfileType { get; set; }

        [JsonPropertyName("interests")]
        public List<Interest> Interests { get; set; }
    }

    public class RedisProfile
    {
        public string Id { get; set; }
        public List<Interest> Interests { get; set; }
        public List<string> ProfileIds { get; set; }
    }
}
