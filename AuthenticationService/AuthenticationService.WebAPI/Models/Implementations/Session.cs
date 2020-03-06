using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AuthenticationService.WebAPI.Models.Implementations
{
    public abstract class Session
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AuthToken { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
