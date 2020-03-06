using PostService.Test.Mocks.Logic;
using PostService.Test.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PostService.Test.Tests.Logic
{
    public class AppTokenStoreTest
    {
        [Fact]
        public async Task GetAppTokenAsync_TokenNotNull()
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            mockHttpClient.MockGetAsync();

            string authUrl = "http://localhost:5000/api";
            string appID = "AppKey";

            //Act
            var appTokenStore = new AppTokenStoreWrapper(mockHttpClient.Object, authUrl, appID);
            appTokenStore.UpdateExpirationTime(DateTime.Now.AddSeconds(-1));

            var token = await appTokenStore.GetAppTokenAsync();

            //Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Matches(token, "65d878eabcde837d50d8bf26cea025cdfacacc9659dce79548c0d7c61aa4e125");

            mockHttpClient.VerifyAll();
        }

    }
}
