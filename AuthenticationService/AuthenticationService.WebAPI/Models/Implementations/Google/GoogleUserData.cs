using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Implementations.Google
{
    public class GoogleUserData
    {
        private readonly string serviceProvider = "Google";
        //       {
        //  "sub": "109545758890960601113",
        //  "name": "Adeoluwa Simeon",
        //  "given_name": "Adeoluwa",
        //  "family_name": "Simeon",
        //  "picture": "https://lh3.googleusercontent.com/-27qe30hGIlw/AAAAAAAAAAI/AAAAAAAAAAA/ACHi3reUjvopX7pxj5-Ah-z1VgleEb0QGg/photo.jpg",
        //  "email": "aide4th@gmail.com",
        //  "email_verified": true,
        //  "locale": "en"
        //}

        [Newtonsoft.Json.JsonProperty("sub")]
        public string Sub { get; set; }
        [Newtonsoft.Json.JsonProperty("email")]
        public string Email { get; set; }
        [Newtonsoft.Json.JsonProperty("given_name")]
        public string FirstName { get; set; }
        [Newtonsoft.Json.JsonProperty("family_name")]
        public string LastName { get; set; }
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name { get; set; }
        [Newtonsoft.Json.JsonProperty("picture")]
        public string Picture { get; set; }

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

