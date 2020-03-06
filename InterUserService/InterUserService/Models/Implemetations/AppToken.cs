using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Models.Implemetations
{
    public class AppToken
    {
        //{\"tokenType\":\"IntServ\",\"accessToken\":\"65d878eabcde837d50d8bf26cea025cdfacacc9659dce79548c0d7c61aa4e125\",\"expiresIn\":86400}
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public long ExpiresIn { get; set; }
    }
}
