using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using PostService.Test.Mocks.Logic;
using PostService.Test.Wrappers;
using PostService.Utility;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PostService.Test.Tests.Logic
{
    public class TokenHandlerTest
    {

        [Theory]
        [InlineData("InvalidToken")]
        [InlineData("Token")]
        public async Task HandleRequirementAsync_Bearer(string token)
        {
            //Arrange
            string knownToken = "Token";
            var mockTokenValidator = new MockTokenValidator();
            mockTokenValidator.MockValidateTokenAsync(token, knownToken);

            //set up context
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            httpContext.User = new ClaimsPrincipal();

            var authorizationHandlerContext = new AuthorizationHandlerContext(new List<IAuthorizationRequirement>(), null, null);

            //Act
            await new TokenHandlerWrapper(mockTokenValidator.Object, mockHttpContextAccessor.Object).HandleRequirementAsyncPublic(authorizationHandlerContext, new TokenRequirement());

            //Assert
            if (token == knownToken)
            {
                Assert.True(authorizationHandlerContext.HasSucceeded);
                Assert.NotNull(httpContext.User.FindFirst(ClaimTypes.Name));
                Assert.NotNull(httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId"));
                Assert.NotNull(httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType"));
                Assert.NotNull(httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/interests"));
            }
            else
            {
                Assert.True(authorizationHandlerContext.HasFailed);
            }

            mockTokenValidator.VerifyAll();
        }

        [Theory]
        [InlineData("InvalidToken")]
        [InlineData("Token")]
        public async Task HandleRequirementAsync_App(string token)
        {
            //Arrange
            string knownToken = "Token";
            var mockTokenValidator = new MockTokenValidator();
            mockTokenValidator.MockValidateAppTokenAsync(token, knownToken);

            //set up context
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"IntApp {token}";
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            var authorizationHandlerContext = new AuthorizationHandlerContext(new List<IAuthorizationRequirement>(), null, null);

            //Act
            await new TokenHandlerWrapper(mockTokenValidator.Object, mockHttpContextAccessor.Object).HandleRequirementAsyncPublic(authorizationHandlerContext, new TokenRequirement());

            //Assert
            if (token == knownToken)
            {
                Assert.True(authorizationHandlerContext.HasSucceeded);
            }
            else
            {
                Assert.True(authorizationHandlerContext.HasFailed);
            }

            mockTokenValidator.VerifyAll();
        }

        [Theory]
        [InlineData("")]
        [InlineData("Invalid ")]
        public async Task HandleRequirementAsync_InavlidAuthScheme(string authScheme)
        {
            //Arrange
            string knownToken = "Token";
            var mockTokenValidator = new MockTokenValidator();
            mockTokenValidator.MockValidateAppTokenAsync(knownToken, knownToken);

            //set up context
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"{authScheme}{knownToken}";

            var authorizationHandlerContext = new AuthorizationHandlerContext(new List<IAuthorizationRequirement>(), null, null);

            //Act
            await new TokenHandlerWrapper(mockTokenValidator.Object, mockHttpContextAccessor.Object).HandleRequirementAsyncPublic(authorizationHandlerContext, new TokenRequirement());

            //Assert
            Assert.True(authorizationHandlerContext.HasFailed);
        }
    }
}
