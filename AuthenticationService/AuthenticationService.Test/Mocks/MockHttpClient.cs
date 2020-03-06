using AuthenticationService.WebAPI.Logic.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Test.Mocks
{
    public class MockHttpClient : Mock<IHttpHandler>
    {
        Dictionary<string, string> getResponses = new Dictionary<string, string> {
            { "https://www.googleapis.com/oauth2/v3/userinfo?access_token=ya29.Il-5BzYf5tnxYK0_s80S-unnRFJ1kMgtMiB8S4p_oWw8yG_Cnc3D0O8MofLcTyD7jpRL2tFiq5LnG93n225RMjtu_foJTtZ_4AuUSFW-o76_s7eC4hg5IXkHZll-ACqaqw",
              "{\"sub\": \"109545758890960601113\",\"name\": \"Adeoluwa Simeon\",\"given_name\": \"Adeoluwa\",\"family_name\": \"Simeon\",\"picture\": \"https:lh3.googleusercontent.com/-27qe30hGIlw/AAAAAAAAAAI/AAAAAAAAAAA/ACHi3reUjvopX7pxj5-Ah-z1VgleEb0QGg/photo.jpg\",\"email\": \"aide4th@gmail.com\",\"email_verified\": true,\"locale\": \"en\"}" },

            { "https://graph.facebook.com/oauth/access_token?client_id=957706237731885&client_secret=a33f41710d3ecef4bf5a9e0d58e1a253&grant_type=client_credentials",
              "{\"access_token\":\"957706237731885|mX4kv5n8RJE5wODNFfXvfRyje5I\",\"token_type\":\"bearer\"}"},

            { "https://graph.facebook.com/debug_token?input_token=EAANnB1wZAZAC0BAAdkek3ujY4LekFflMkX8AGakGe7tjwd8IlM31tFFmZBViYDKwVh7IT9WlP6zlc8OMGieOcIbZCXlYfCqRvN688DPtmZCZBGZCJOkBIfXaSnhoJZBskkH5eZAqEZBPj2Ys8Xbrb3cSUuzdJ8NqzNjlUZD&access_token=957706237731885|mX4kv5n8RJE5wODNFfXvfRyje5I",
              "{\"data\": {\"app_id\": \"957706237731885\",\"type\": \"USER\",\"application\": \"Adeoluwa's first app\",\"data_access_expires_at\": 1586022241,\"expires_at\": 1583429953,\"is_valid\": true,\"issued_at\": 1578245953,\"scopes\": [\"email\",\"public_profile\"],\"user_id\": \"10212880395859025\"}}" },

            { "https://graph.facebook.com/debug_token?input_token=InvalidToken&access_token=957706237731885|mX4kv5n8RJE5wODNFfXvfRyje5I",
              "{\"data\": {\"error\": {\"code\": 190,\"message\": \"Invalid OAuth access token.\"},\"is_valid\": false,\"scopes\": []}}" },

            { "https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token=EAANnB1wZAZAC0BAAdkek3ujY4LekFflMkX8AGakGe7tjwd8IlM31tFFmZBViYDKwVh7IT9WlP6zlc8OMGieOcIbZCXlYfCqRvN688DPtmZCZBGZCJOkBIfXaSnhoJZBskkH5eZAqEZBPj2Ys8Xbrb3cSUuzdJ8NqzNjlUZD",
              "{\"id\": \"10212880395859025\",\"email\": \"adeoluwasimeon\u0040rocketmail.com\",\"first_name\": \"Adeoluwa\",\"last_name\": \"Onigbinde\",\"name\": \"Adeoluwa Simeon Onigbinde\",\"picture\": {\"data\": {\"height\": 50,\"is_silhouette\": false,\"url\": \"https://platform-lookaside.fbsbx.com/platform/profilepic/?asid=10212880395859025&height=50&width=50&ext=1580853001&hash=AeS-Nsq3odmQm8v7\",\"width\": 50}}}" },

            { "https://api.linkedin.com/v2/me",
              "{\"localizedLastName\":\"ONIGBINDE\",\"lastName\":{\"localized\":{\"en_US\":\"ONIGBINDE\"},\"preferredLocale\":{\"country\":\"US\",\"language\":\"en\"}},\"firstName\":{\"localized\":{\"en_US\":\"ADEOLUWA\"},\"preferredLocale\":{\"country\":\"US\",\"language\":\"en\"}},\"profilePicture\":{\"displayImage\":\"urn:li:digitalmediaAsset:C4D03AQGOpN7vtf0P7Q\"},\"id\":\"AZclT9-33M\",\"localizedFirstName\":\"ADEOLUWA\"}"},

            { "https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~))",
              "{\"elements\":[{\"handle~\":{\"emailAddress\":\"aide4th@gmail.com\"},\"handle\":\"urn:li:emailAddress:1511898886\"}]}"}
        };
        Dictionary<string, string> postResponses = new Dictionary<string, string> {
            //Google Post Request
            { "https://oauth2.googleapis.com/token?code=4%2FvQF-WHpLDEhurZf5E5SoTiPrFFgZFI_c76TgyNploYSbfE_gcgEb5KP8fljuVDpFDhp7qWH4-tnXv45Bveyz-Mg&client_id=643056883737-g7l8hpridqbb6q2np2hmktvatv9ihvbq.apps.googleusercontent.com&client_secret=aVrTwGeUnmNblLYiC1c0myN3&redirect_uri=https://localhost:5001/api/account/GoogleCallBack&grant_type=authorization_code",
              "{\"access_token\": \"ya29.Il-5BzYf5tnxYK0_s80S-unnRFJ1kMgtMiB8S4p_oWw8yG_Cnc3D0O8MofLcTyD7jpRL2tFiq5LnG93n225RMjtu_foJTtZ_4AuUSFW-o76_s7eC4hg5IXkHZll-ACqaqw\",\"expires_in\": 3600,\"scope\": \"openid https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile\",\"token_type\": \"Bearer\",\"id_token\": \"eyJhbGciOiJSUzI1NiIsImtpZCI6ImNkMjM0OTg4ZTNhYWU2N2FmYmMwMmNiMWM0MTQwYjNjZjk2ODJjYWEiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI2NDMwNTY4ODM3MzctZzdsOGhwcmlkcWJiNnEybnAyaG1rdHZhdHY5aWh2YnEuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI2NDMwNTY4ODM3MzctZzdsOGhwcmlkcWJiNnEybnAyaG1rdHZhdHY5aWh2YnEuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMDk1NDU3NTg4OTA5NjA2MDExMTMiLCJlbWFpbCI6ImFpZGU0dGhAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImF0X2hhc2giOiJ1NEVyVFRFSHFOdUdWUVRuTHZSaDdBIiwibmFtZSI6IkFkZW9sdXdhIFNpbWVvbiIsInBpY3R1cmUiOiJodHRwczovL2xoMy5nb29nbGV1c2VyY29udGVudC5jb20vLTI3cWUzMGhHSWx3L0FBQUFBQUFBQUFJL0FBQUFBQUFBQUFBL0FDSGkzcmVVanZvcFg3cHhqNS1BaC16MVZnbGVFYjBRR2cvczk2LWMvcGhvdG8uanBnIiwiZ2l2ZW5fbmFtZSI6IkFkZW9sdXdhIiwiZmFtaWx5X25hbWUiOiJTaW1lb24iLCJsb2NhbGUiOiJlbiIsImlhdCI6MTU3ODYwNTEzMSwiZXhwIjoxNTc4NjA4NzMxfQ.TUo81fQXtq7h1ARHhhmJmF_BEq2LnBwqETng_ipNYdhWWC9MggQAkKb8uKqYue9-nUma56VsrOd_7lA7U6ZPgU4lHSNx3cOUs_qXSFaBrCaCjjyLEUu-AJMDbLRKhWdTrLa4aa0dzj-PH6s0OM6anKrLph7LtIjVTXhR0uTN-4I1NaMkT-C7GQB9uhAtHR3A2P5NfoTCBikqpy7vpJNM4GVBRpYufhcJueksyjDNquJmYRnv4Ltw3l93oJnovGjATbXnFuHk10S-U0Ce-8dqrsVuSgpoSGU3NH_V8KnfFtRdtwSHIPzc_I-c6Nc9WQk3krEYfLpGMN2X8vE9F1pE0A\"}" },

            { "https://www.linkedin.com/oauth/v2/accessToken?grant_type=authorization_code&code=AQQV79YX075fKSxTcDpyuPKlu8XLtQQmEy-vcuIlz6vjHPda1qvYk1EG7Vz4ayt_RC5NB1SbTeGZ7U01sleiZJdCk0KpuAiW7Cq41pMl5pTkkG11bu2BwlUX1Z5jU4IILcRVxslwj-MX-2ZarkrXGUHB8-qgcNo-u453lbWIDTpOY7wyrrdGKmZVKWejjQ&redirect_uri=https://localhost:5001/api/account/LinkedInCallBack&client_id=776nyv8ecq7e9t&client_secret=ACsCV2udJxEUWomT",
              "{\"access_token\":\"AQW0FGkKBBCMMOMH4-wG-M6esvFgyvmVOvK-FTfC8kI5_z3UDNaoDhWEc7jbZsOPAz7vAGJiJG0zdIeg4bgrvWgWRTrsjx57KRu6Hd_C-0C-U2Fd0Aj5OiLPaVtlhH92uSUHBBVrzIUiklqRsHnjUZYzaLkezGTpIkRJedUOhrCLkFP34dLnuXhV5L4gIxls7H3zlkgqCCa25WGLcS_ZgjXXzBuTQ4GcJYi0hM6BgZEdTGAVnmspyprJJarC1wq7wi2RqvHxYhu8K2l4o39zNI484PxpdvS9-wLk50INIIrwitJ2HgA_eeKae8pG0LJmGcn3ce9E-aBAyfFqgTXz0vl78j7P5A\",\"expires_in\":5183999}"}
             };

        public void MockGetStringAsync()
        {
            Setup(x => x.GetStringAsync(
                It.IsAny<string>()
                ))
                .Returns<string>(s => Task.FromResult<string>(getResponse(s)));
        }

        private string getResponse(string getRequest)
        {
            string response = string.Empty;
            getResponses.TryGetValue(getRequest, out response);
            return response;
        }

        public void MockPostAsync()
        {
            Setup(x => x.PostAsJsonAsync(
                It.IsAny<string>(),
                It.IsAny<object>()
                ))
                .Returns<string, object>((url,content) => Task.FromResult<HttpResponseMessage>(new HttpResponseMessage
                {
                    Content = new StringContent(postResponse(url) ?? "unauthorised response", Encoding.UTF8, "application/json"),
                    StatusCode = postResponse(url) == null? System.Net.HttpStatusCode.Unauthorized : System.Net.HttpStatusCode.OK
                }));
        }

        private string postResponse(string getRequest)
        {
            string response = string.Empty;
            postResponses.TryGetValue(getRequest, out response);
            return response;
        }
    }
}
