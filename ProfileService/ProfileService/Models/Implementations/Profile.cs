using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProfileService.Models.Implementations
{
    public abstract class Profile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [RegularExpression(@"^[0-9a-fA-F]+$",ErrorMessage ="Invalid Id")]
        public string Id { get; set; }
        public string UserId { get; set; }
        public virtual string Name { get; set; }
        [Required]
        [StringLength(200)]
        public string Address { get; set; }
        [Required]
        [StringLength(50)]
        public string City { get; set; }
        [Required]
        [StringLength(50)]
        public string State { get; set; }
        [Required]
        [StringLength(50)]
        public string Country { get; set; }
        [BsonIgnore]
        public ProfileType ProfileType { get; set; }
        public List<Interest> Interests { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public List<Interest> InterestsBeforeUpdate { get; set; }

    }
}
