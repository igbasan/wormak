using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Implementations.LinkedIn
{
    public class LinkedInAppAccessToken
    {
        //        {
        //    "access_token": "ya29.Il-5BzYf5tnxYK0_s80S-unnRFJ1kMgtMiB8S4p_oWw8yG_Cnc3D0O8MofLcTyD7jpRL2tFiq5LnG93n225RMjtu_foJTtZ_4AuUSFW-o76_s7eC4hg5IXkHZll-ACqaqw",
        //    "expires_in": 3600//}
        [Newtonsoft.Json.JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [Newtonsoft.Json.JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
