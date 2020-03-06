using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProfileService.Models.Implementations
{
    public class CurrentProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserID { get; set; }

        [RegularExpression(@"^[0-9a-fA-F]+$", ErrorMessage = "Invalid Id")]
        [Required]
        public string ProfileID { get; set; }
        [BsonRepresentation(BsonType.Int32)]
        [Required]
        public ProfileType ProfileType { get; set; }
        public DateTime LastSetDate { get; set; }

        [BsonIgnore]
        public string Name { get; set; }
        [BsonIgnore]
        public List<Interest> Interests { get; set; }
    }
}
