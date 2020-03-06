using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models.Implementations
{
    public class Post: ProfileBased
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [MaxLength(500, ErrorMessage = "Message should not exceed 500 characters")]
        public string Message { get; set; }
        public DateTime DatePosted { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
        [Required]
        public List<Interest> Tags { get; set; }
        public bool IsEdited { get; set; }
    }
}
