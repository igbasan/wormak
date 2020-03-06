using ProfileService.Logic.Implementations;
using ProfileService.Test.Mocks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProfileService.Test.Tests
{
    public class TokenValidatorTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ValidateTokenAsync_nullToken(string token)
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();

            //Act
            var exception = await Record.ExceptionAsync(() => new TokenValidator(mockHttpClient.Object, string.Empty).ValidateTokenAsync(token));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("InvalidToken")]
        [InlineData("cc1922988c973d2e54d72b70da167410435b4fc6114a78bf35f000fd6bbb5ada")]
        public async Task ValidateTokenAsync_TokenNotNull(string token)
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            mockHttpClient.MockGetAsync();
            string authenticationUrl = "http://localhost:5000/api";

            //Act
            var userDetails = await new TokenValidator(mockHttpClient.Object, authenticationUrl).ValidateTokenAsync(token);

            //Assert
            if (token == "InvalidToken")
            {
                Assert.Null(userDetails);
            }
            else
            {
                Assert.NotNull(userDetails);
            }

            mockHttpClient.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ValidateAppTokenAsync_nullToken(string token)
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();

            //Act
            var exception = await Record.ExceptionAsync(() => new TokenValidator(mockHttpClient.Object, string.Empty).ValidateAppTokenAsync(token));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("InvalidToken")]
        [InlineData("4bc5055285404f60d8bdcfb2e16f6074e56a10d4f8db44accdc78996eda9ac83")]
        public async Task ValidateAppTokenAsync_TokenNotNull(string token)
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            mockHttpClient.MockGetAsync();
            string authenticationUrl = "http://localhost:5000/api";

            //Act
            string appName = await new TokenValidator(mockHttpClient.Object, authenticationUrl).ValidateAppTokenAsync(token);

            //Assert
            if (token == "InvalidToken")
            {
                Assert.Null(appName);
            }
            else
            {
                Assert.Equal("TestService", appName);
            }

            mockHttpClient.VerifyAll();
        }
    }
}
