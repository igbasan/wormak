using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PostService.Models.Implementations
{
    public class AppToken
    {
        //{\"tokenType\":\"IntServ\",\"accessToken\":\"65d878eabcde837d50d8bf26cea025cdfacacc9659dce79548c0d7c61aa4e125\",\"expiresIn\":86400}

        [JsonPropertyName("tokenType")]
        public string TokenType { get; set; }
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expiresIn")]
        public long ExpiresIn { get; set; }
    }
}
