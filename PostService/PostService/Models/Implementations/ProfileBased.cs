using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models.Implementations
{
    public class ProfileBased
    {
        public string ProfileID
        {
            get
            {
                return $"{ProfileType}_{ProfileIDRaw}";
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains("_"))
                {
                    ProfileIDRaw = value;
                }
                else
                {
                    string[] values = value.Split("_");
                    if (values.Length == 2)
                    {
                        ProfileType = (ProfileType)Enum.Parse(typeof(ProfileType), values[0]);
                        ProfileIDRaw = values[1];
                    }
                }
            }
        }
        [BsonIgnore]
        public string ProfileIDRaw { get; private set; }
        [BsonIgnore]
        public ProfileType ProfileType { get; set; }
        [BsonIgnore]
        public string ProfileName { get; set; }
    }
}
