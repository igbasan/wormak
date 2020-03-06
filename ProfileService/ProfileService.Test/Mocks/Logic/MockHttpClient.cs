using Moq;
using ProfileService.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProfileService.Test.Mocks
{
    public class MockHttpClient : Mock<IHttpHandler>
    {
        Dictionary<string, string> getResponses = new Dictionary<string, string> {
            { "http://localhost:5000/api/account/GetUserByToken?token=cc1922988c973d2e54d72b70da167410435b4fc6114a78bf35f000fd6bbb5ada",
              "{\"id\":\"5e121f45556c1e229074de29\",\"userName\":\"Adeoluwa Simeon Onigbinde\",\"firstName\":\"Adeoluwa\",\"lastName\":\"Onigbinde\",\"email\":\"adeoluwasimeon@rocketmail.com\",\"loginServiceProvider\":null}" },
            { "http://localhost:5000/api/internalService/VaidateServiceToken?token=4bc5055285404f60d8bdcfb2e16f6074e56a10d4f8db44accdc78996eda9ac83", "TestService" }
        };
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
    }
}
