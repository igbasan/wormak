using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Implementations.Google
{
    public class GoogleAppAccessToken
    {
        //        {
        //    "access_token": "ya29.Il-5BzYf5tnxYK0_s80S-unnRFJ1kMgtMiB8S4p_oWw8yG_Cnc3D0O8MofLcTyD7jpRL2tFiq5LnG93n225RMjtu_foJTtZ_4AuUSFW-o76_s7eC4hg5IXkHZll-ACqaqw",
        //    "expires_in": 3600,
        //    "scope": "openid https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile",
        //    "token_type": "Bearer",
        //    "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6ImNkMjM0OTg4ZTNhYWU2N2FmYmMwMmNiMWM0MTQwYjNjZjk2ODJjYWEiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI2NDMwNTY4ODM3MzctZzdsOGhwcmlkcWJiNnEybnAyaG1rdHZhdHY5aWh2YnEuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI2NDMwNTY4ODM3MzctZzdsOGhwcmlkcWJiNnEybnAyaG1rdHZhdHY5aWh2YnEuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMDk1NDU3NTg4OTA5NjA2MDExMTMiLCJlbWFpbCI6ImFpZGU0dGhAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImF0X2hhc2giOiJ1NEVyVFRFSHFOdUdWUVRuTHZSaDdBIiwibmFtZSI6IkFkZW9sdXdhIFNpbWVvbiIsInBpY3R1cmUiOiJodHRwczovL2xoMy5nb29nbGV1c2VyY29udGVudC5jb20vLTI3cWUzMGhHSWx3L0FBQUFBQUFBQUFJL0FBQUFBQUFBQUFBL0FDSGkzcmVVanZvcFg3cHhqNS1BaC16MVZnbGVFYjBRR2cvczk2LWMvcGhvdG8uanBnIiwiZ2l2ZW5fbmFtZSI6IkFkZW9sdXdhIiwiZmFtaWx5X25hbWUiOiJTaW1lb24iLCJsb2NhbGUiOiJlbiIsImlhdCI6MTU3ODYwNTEzMSwiZXhwIjoxNTc4NjA4NzMxfQ.TUo81fQXtq7h1ARHhhmJmF_BEq2LnBwqETng_ipNYdhWWC9MggQAkKb8uKqYue9-nUma56VsrOd_7lA7U6ZPgU4lHSNx3cOUs_qXSFaBrCaCjjyLEUu-AJMDbLRKhWdTrLa4aa0dzj-PH6s0OM6anKrLph7LtIjVTXhR0uTN-4I1NaMkT-C7GQB9uhAtHR3A2P5NfoTCBikqpy7vpJNM4GVBRpYufhcJueksyjDNquJmYRnv4Ltw3l93oJnovGjATbXnFuHk10S-U0Ce-8dqrsVuSgpoSGU3NH_V8KnfFtRdtwSHIPzc_I-c6Nc9WQk3krEYfLpGMN2X8vE9F1pE0A"
        //}
        [Newtonsoft.Json.JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [Newtonsoft.Json.JsonProperty("token_type")]
        public string TokenType { get; set; }
        [Newtonsoft.Json.JsonProperty("scope")]
        public string Scope { get; set; }
        [Newtonsoft.Json.JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [Newtonsoft.Json.JsonProperty("id_token")]
        public string IdToken { get; set; }
    }
}
