using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Implementations.LinkedIn
{
    public class LinkedInUserData
    {
        private readonly string serviceProvider = "LinkedIn";
        //        {
        //    "localizedLastName": "ONIGBINDE",
        //    "lastName": {
        //        "localized": {
        //            "en_US": "ONIGBINDE"
        //        },
        //        "preferredLocale": {
        //            "country": "US",
        //            "language": "en"
        //        }
        //    },
        //    "firstName": {
        //        "localized": {
        //            "en_US": "ADEOLUWA"
        //        },
        //        "preferredLocale": {
        //            "country": "US",
        //            "language": "en"
        //        }
        //    },
        //    "profilePicture": {
        //        "displayImage": "urn:li:digitalmediaAsset:C4D03AQGOpN7vtf0P7Q"
        //    },
        //    "id": "AZclT9-33M",
        //    "localizedFirstName": "ADEOLUWA"
        //}

        [Newtonsoft.Json.JsonProperty("id")]
        public string Id { get; set; }
        [Newtonsoft.Json.JsonProperty("localizedFirstName")]
        public string FirstName { get; set; }
        [Newtonsoft.Json.JsonProperty("localizedLastName")]
        public string LastName { get; set; }

        public User GetUserDetails()
        {
            return new User
            {
                FirstName = FirstName,
                LastName = LastName,
                LoginServiceProvider = serviceProvider
            };
        }
    }
}

