using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Implementations.Facebook
{
    public class FacebookUserData
    {
        private readonly string serviceProvider = "Facebook";
        //     {
        //"id": "10212880395859025",
        //"email": "adeoluwasimeon\u0040rocketmail.com",
        //"first_name": "Adeoluwa",
        //"last_name": "Onigbinde",
        //"name": "Adeoluwa Simeon Onigbinde",
        //"picture": {
        //   "data": {
        //      "height": 50,
        //      "is_silhouette": false,
        //      "url": "https://platform-lookaside.fbsbx.com/platform/profilepic/?asid=10212880395859025&height=50&width=50&ext=1580853001&hash=AeS-Nsq3odmQm8v7",
        //      "width": 50
        //   }
        //}

        [Newtonsoft.Json.JsonProperty("id")]
        public string Id { get; set; }
        [Newtonsoft.Json.JsonProperty("email")]
        public string Email { get; set; }
        [Newtonsoft.Json.JsonProperty("first_name")]
        public string FirstName { get; set; }
        [Newtonsoft.Json.JsonProperty("last_name")]
        public string LastName { get; set; }
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name { get; set; }

        public User GetUserDetails()
        {
            return new User
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                LoginServiceProvider = serviceProvider
            };
        }
    }
}

