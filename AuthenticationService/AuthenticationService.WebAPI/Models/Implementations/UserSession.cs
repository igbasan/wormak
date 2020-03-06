using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AuthenticationService.WebAPI.Models.Implementations
{
    public class UserSession : Session
    {
        [BsonIgnore]
        public User User { get; set; }
        public string UserID { get; set; }
        public DateTime FirstloginDate { get; set; }
        public DateTime LastloginDate { get; set; }
        public string LoginServiceProvider { get; set; }
        public string ProviderAuthCode { get; set; }
    }
}
