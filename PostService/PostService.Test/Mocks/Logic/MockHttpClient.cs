using Moq;
using PostService.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PostService.Test.Mocks.Logic
{
    public class MockHttpClient : Mock<IHttpHandler>
    {
        Dictionary<string, string> getResponses = new Dictionary<string, string> {
            { "http://localhost:5002/api/Profile/GetCurrentInterestProfile",
              "{\"result\":{\"interests\":[\"Business\",\"Pleasure\"],\"id\":\"5e209dbe44dcb183fcecddcf\",\"name\":\"My FirstName My LastName\",\"profileType\":\"GeneralUser\"},\"isSuccessful\":true,\"message\":\"Successful\"}" },
            { "http://localhost:5000/api/internalService/VaidateServiceToken?token=4bc5055285404f60d8bdcfb2e16f6074e56a10d4f8db44accdc78996eda9ac83", "TestService" },
            { "http://localhost:5000/api/internalService/SignIn?appKey=AppKey", "{\"tokenType\":\"IntServ\",\"accessToken\":\"65d878eabcde837d50d8bf26cea025cdfacacc9659dce79548c0d7c61aa4e125\",\"expiresIn\":86400}" },
            { "http://localhost:5002/api/Profile/GetProfiles::[{\"id\":\"5e20ad1651c7c1df345d41a1\",\"name\":null,\"profileType\":\"Professional\",\"interests\":null},{\"id\":\"5e209dbe44dcb183fcecddcf\",\"name\":null,\"profileType\":\"GeneralUser\",\"interests\":null}]", "{\"result\":[{\"id\":\"5e209dbe44dcb183fcecddcf\",\"name\":\"My FirstName My LastName\",\"profileType\":\"GeneralUser\"},{\"id\":\"5e20ad1651c7c1df345d41a1\",\"name\":\"name\",\"profileType\":\"Professional\"}],\"isSuccessful\":true,\"message\":\"Successful\"}" },
            { "http://localhost:5004/api/Follower/GetAllFollowers?profileId=5e20ad1651c7c1df345d41a1&profileType=Professional", "{\"result\":[{\"id\":\"5e209dbe44dcb183fcecddcf\",\"name\":\"My FirstName My LastName\",\"profileType\":\"GeneralUser\"}],\"totalCount\":1,\"isSuccessful\":true,\"message\":\"Successful\"}" },
            { "http://localhost:5002/api/Profile/GetInterests::[\"Art\",\"Science\",\"Business\",\"Pleasure\"]", "{\"result\":[{\"interests\":[\"Pleasure\",\"Art\"],\"id\":\"5e359aea29c953e43403b847\",\"name\":\"New LastName\",\"profileType\":\"Company\"},{\"interests\":[\"Business\",\"Pleasure\"],\"id\":\"5e209dbe44dcb183fcecddcf\",\"name\":\"My FirstName My LastName\",\"profileType\":\"GeneralUser\"},{\"interests\":[\"Science\",\"Art\"],\"id\":\"5e356a8890bc24d11834b273\",\"name\":\"My LastName\",\"profileType\":\"Professional\"},{\"interests\":[\"Science\",\"Art\"],\"id\":\"5e356b4e9d8a25647ccebc82\",\"name\":\"New LastName\",\"profileType\":\"Professional\"},{\"interests\":[\"Business\",\"Pleasure\"],\"id\":\"5e3559289b5e7460f0a7f39d\",\"name\":\"new name\",\"profileType\":\"Professional\"}],\"isSuccessful\":true,\"message\":\"Successful\"}"},
            { "http://localhost:5004/api/Follower/GetAllFollowing?profileId=5e209dbe44dcb183fcecddcf&profileType=GeneralUser","{\"result\":[{\"id\":\"5e20ad1651c7c1df345d41a1\",\"name\":\"name\",\"profileType\":\"Professional\"}],\"totalCount\":1,\"isSuccessful\":true,\"message\":\"Successful\"}" }

        };
        private Dictionary<string, string> headers = new Dictionary<string, string>();
        private string getResponse(string getRequest)
        {
            string response = string.Empty;
            getResponses.TryGetValue(getRequest, out response);
            return response;
        }
        public void MockGetAsync()
        {
            Setup(x => x.GetAsync(
                It.IsAny<string>()
                ))
                .Returns<string>((url) => Task.FromResult<HttpResponseMessage>(new HttpResponseMessage
                {
                    Content = new StringContent(getResponse(url) ?? "unauthorised response", Encoding.UTF8, "application/json"),
                    StatusCode = getResponse(url) == null ? System.Net.HttpStatusCode.Unauthorized : System.Net.HttpStatusCode.OK
                }));
        }

        public void MockGetAsyncWithAuthHeader(string validToken)
        {
            Setup(x => x.GetAsync(
                It.IsAny<string>()
                ))
                .Returns<string>((url) =>
                {
                    string authToken = string.Empty;
                    headers.TryGetValue("Authorization", out authToken);

                    string contentText = "unauthorised response";
                    System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.Unauthorized;

                    if (authToken == validToken)
                    {
                        contentText = getResponse(url) ?? "unauthorised response";
                        statusCode = getResponse(url) == null ? System.Net.HttpStatusCode.Unauthorized : System.Net.HttpStatusCode.OK;
                    }
                    return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage
                    {
                        Content = new StringContent(contentText, Encoding.UTF8, "application/json"),
                        StatusCode = statusCode
                    });
                });
        }

        public void MockPostAsyncWithAuthHeader(string validToken)
        {
            Setup(x => x.PostAsJSONAsync(
                It.IsAny<string>(),
                It.IsAny<object>()
                ))
                .Returns<string, object>((url, body) =>
                {
                    string authToken = string.Empty;
                    headers.TryGetValue("Authorization", out authToken);

                    string contentText = "unauthorised response";
                    System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.Unauthorized;

                    if (authToken == validToken)
                    {
                        string request = $"{url}::{JsonSerializer.Serialize(body)}";
                        contentText = getResponse(request) ?? "unauthorised response";
                        statusCode = getResponse(request) == null ? System.Net.HttpStatusCode.Unauthorized : System.Net.HttpStatusCode.OK;
                    }
                    return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage
                    {
                        Content = new StringContent(contentText, Encoding.UTF8, "application/json"),
                        StatusCode = statusCode
                    });
                });
        }

        public void MockAddDefaultRequestHeaders()
        {
            Setup(x => x.AddDefaultRequestHeaders(
                It.IsAny<string>(),
                It.IsAny<string>()
                )).Callback<string, string>((header, value) => { headers.Add(header, value); });
        }
        public void MockRemoveDefaultRequestHeaders()
        {
            Setup(x => x.RemoveDefaultRequestHeaders(
                It.IsAny<string>()
                )).Callback<string, string>((header, value) => { if (headers.ContainsKey(header)) headers.Remove(header); });
        }
    }
}
