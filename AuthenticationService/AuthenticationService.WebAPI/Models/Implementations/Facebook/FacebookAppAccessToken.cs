using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Implementations.Facebook
{
    public class FacebookAppAccessToken
    {
        //{"access_token":"957706237731885|mX4kv5n8RJE5wODNFfXvfRyje5I","token_type":"bearer"}
        [Newtonsoft.Json.JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [Newtonsoft.Json.JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
