using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Implementations
{
    public class InternalServiceSession : Session
    {
        public string AppKey { get; set; }
        public string AppName { get; set; }
        public DateTime FirstKeyExchangeDate { get; set; }
        public DateTime LastExchangeDate { get; set; }
    }
}
