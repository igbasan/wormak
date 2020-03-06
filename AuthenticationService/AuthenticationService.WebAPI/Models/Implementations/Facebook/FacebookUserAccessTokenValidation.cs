using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Implementations.Facebook
{
    public class FacebookUserAccessTokenValidation
    {
        //     {
        //"data": {
        //   "app_id": "957706237731885",
        //   "type": "USER",
        //   "application": "Adeoluwa's first app",
        //   "data_access_expires_at": 1586022241,
        //   "expires_at": 1583429953,
        //   "is_valid": true,
        //   "issued_at": 1578245953,
        //   "scopes": [
        //      "email",
        //      "public_profile"
        //   ],
        //   "user_id": "10212880395859025"
        //}
        [Newtonsoft.Json.JsonProperty("data")]
       public FacebookUserAccessTokenData Data { get; set; }
    }

    public class FacebookUserAccessTokenData
    {
        [Newtonsoft.Json.JsonProperty("is_valid")]
        public bool IsValid { get; set; }
    }
}
