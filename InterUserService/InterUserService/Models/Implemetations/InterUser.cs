using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Models.Implemetations
{
    public class InterUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        /// <summary>
        /// The User on the active part of the relationship, the follower, the Client or Giving the testimonial
        /// </summary>
        public string ActiveProfileID
        {
            get
            {
                return $"{ActiveProfileType}_{ActiveProfileIDRaw}";
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains("_"))
                {
                    ActiveProfileIDRaw = value;
                }
                else
                {
                    string[] values = value.Split("_");
                    if (values.Length == 2)
                    {
                        ActiveProfileType = (ProfileType)Enum.Parse(typeof(ProfileType), values[0]);
                        ActiveProfileIDRaw = values[1];
                    }
                }
            }
        }
        [BsonIgnore]
        public string ActiveProfileIDRaw { get; private set; }
        [BsonIgnore]
        public ProfileType ActiveProfileType { get; set; }
        /// <summary>
        /// The User on the passive part of the relationship, the business being followed or with clients or testimonials
        /// </summary>
        public string PassiveProfileID {
            get
            {
                return $"{PassiveProfileType}_{PassiveProfileIDRaw}";
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains("_"))
                {
                    PassiveProfileIDRaw = value;
                }
                else
                {
                    string[] values = value.Split("_");
                    if (values.Length == 2)
                    {
                        PassiveProfileType = (ProfileType)Enum.Parse(typeof(ProfileType), values[0]);
                        PassiveProfileIDRaw = values[1];
                    }
                }
            }
        }
        [BsonIgnore]
        public string PassiveProfileIDRaw { get; private set; }
        [BsonIgnore]
        public ProfileType PassiveProfileType { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsActive { get; set; }
    }
}
