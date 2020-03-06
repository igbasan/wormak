using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AuthenticationService.WebAPI.Models.Implementations
{
    public class LoginAttempt
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserID { get; set; }
        public string AuthToken { get; set; }
        public bool SuccessfulAttempt { get; set; }
        public DateTime AttemptDate { get; set; }
        public DateTime SessionExpirationDate { get; set; }
        public string LoginServiceProvider { get; set; }
        public string ProviderAuthCode { get; set; }
    }
}
