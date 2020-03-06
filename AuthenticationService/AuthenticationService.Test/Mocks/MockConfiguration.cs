using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationService.Test.Mocks
{
    public class MockConfiguration : Mock<IConfiguration>
    {
        Dictionary<string, string> config = new Dictionary<string, string>
        {
            {"Authentication_Google_ClientSecret", "aVrTwGeUnmNblLYiC1c0myN3"},
            {"Authentication_Google_CallbackUrl", "https://localhost:5001/api/account/GoogleCallBack"},
            {"Authentication_Google_ClientId", "643056883737-g7l8hpridqbb6q2np2hmktvatv9ihvbq.apps.googleusercontent.com"},

            {"Authentication_Facebook_AppSecret", "a33f41710d3ecef4bf5a9e0d58e1a253"},
            {"Authentication_Facebook_CallbackUrl", "https://localhost:5001/api/account/FbCallBack"},
            {"Authentication_Facebook_AppId", "957706237731885"},

            {"Authentication_LinkedIn_ClientId", "776nyv8ecq7e9t"},
            {"Authentication_LinkedIn_CallbackUrl", "https://localhost:5001/api/account/LinkedInCallBack"},
            {"Authentication_LinkedIn_ClientSecret", "ACsCV2udJxEUWomT"}
        };

        public void MockSetUpIndexer()
        {
            Setup(x => x[
                It.IsAny<string>()
                ]).Returns((string key) => config[key]);
        }
    }
}
