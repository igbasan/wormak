using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using AuthenticationService.Test.Mocks;
using AuthenticationService.WebAPI.Logic.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using AuthenticationService.WebAPI.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xunit;
using System.Net.Http;
using AuthenticationService.WebAPI.Utilities;
using Microsoft.Extensions.Logging;
using AuthenticationService.WebAPI.Controllers;

namespace AuthenticationService.Test.ControllerTests
{
    public class AccountControllerTest
    {
        //used to instantiate AccountController
        readonly IConfiguration config = new Mock<IConfiguration>().Object;

        //Eventually, when I know what to test for, but for now, I want to be sure it runs
        //[Fact]
        //public void SignInWithGoogle()
        //{
        //    var userLogic = new Mock<IUserLogic>();
        //    var httpClient = new Mock<HttpClient>();
        //    var userSessionLogic = new Mock<IUserSessionLogic>();
        //    var result = new WebAPI.Controllers.AccountController(config, userLogic.Object, userSessionLogic.Object, httpClient.Object).SignInWithGoogle();
        //    Assert.IsType<ChallengeResult>(result);
        //}

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("InvalidToken")]
        [InlineData("EAANnB1wZAZAC0BAAdkek3ujY4LekFflMkX8AGakGe7tjwd8IlM31tFFmZBViYDKwVh7IT9WlP6zlc8OMGieOcIbZCXlYfCqRvN688DPtmZCZBGZCJOkBIfXaSnhoJZBskkH5eZAqEZBPj2Ys8Xbrb3cSUuzdJ8NqzNjlUZD")]
        public async void HandleExternalLoginWithFacebook(string token)
        {
            var config = new MockConfiguration();
            var userLogic = new Mock<IUserLogic>();
            var httpClient = new MockHttpClient();
            var userSessionLogic = new MockUserSessionLogic();
            var logger = new Mock<ILogger<AccountController>>();

            httpClient.MockGetStringAsync();
            config.MockSetUpIndexer();

            var result = await new Wrappers.AccountControllerWrapper(config.Object, userLogic.Object, userSessionLogic.Object, httpClient.Object, logger.Object)
                .HandleExternalLoginWithFacebook(token);


            if (string.IsNullOrWhiteSpace(token) || token == "InvalidToken")
            {
                Assert.IsType<UnauthorizedObjectResult>(result);
            }
            else
            {
                Assert.IsType<JsonResult>(result);
                httpClient.VerifyAll();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("InvalidCode")]
        [InlineData("4%2FvQF-WHpLDEhurZf5E5SoTiPrFFgZFI_c76TgyNploYSbfE_gcgEb5KP8fljuVDpFDhp7qWH4-tnXv45Bveyz-Mg")]
        public async void HandleExternalLoginWithGoogle(string authorizationCode)
        {
            var config = new MockConfiguration();
            var userLogic = new Mock<IUserLogic>();
            var httpClient = new MockHttpClient();
            var userSessionLogic = new MockUserSessionLogic();
            var logger = new Mock<ILogger<AccountController>>();

            httpClient.MockGetStringAsync();
            httpClient.MockPostAsync();
            config.MockSetUpIndexer();

            var result = await new Wrappers.AccountControllerWrapper(config.Object, userLogic.Object, userSessionLogic.Object, httpClient.Object, logger.Object)
                .HandleExternalLoginWithGoogle(authorizationCode);

            if (string.IsNullOrWhiteSpace(authorizationCode) || authorizationCode == "InvalidCode")
            {
                Assert.IsType<UnauthorizedObjectResult>(result);
            }
            else
            {
                Assert.IsType<JsonResult>(result);
                httpClient.VerifyAll();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("InvalidCode")]
        [InlineData("AQQV79YX075fKSxTcDpyuPKlu8XLtQQmEy-vcuIlz6vjHPda1qvYk1EG7Vz4ayt_RC5NB1SbTeGZ7U01sleiZJdCk0KpuAiW7Cq41pMl5pTkkG11bu2BwlUX1Z5jU4IILcRVxslwj-MX-2ZarkrXGUHB8-qgcNo-u453lbWIDTpOY7wyrrdGKmZVKWejjQ")]
        public async void HandleExternalLoginWithLinkedIn(string authorizationCode)
        {
            var config = new MockConfiguration();
            var userLogic = new Mock<IUserLogic>();
            var httpClient = new MockHttpClient();
            var userSessionLogic = new MockUserSessionLogic();
            var logger = new Mock<ILogger<AccountController>>();

            httpClient.MockGetStringAsync();
            httpClient.MockPostAsync();
            config.MockSetUpIndexer();

            var result = await new Wrappers.AccountControllerWrapper(config.Object, userLogic.Object, userSessionLogic.Object, httpClient.Object, logger.Object)
                .HandleExternalLoginWithLinkedIn(authorizationCode);

            if (string.IsNullOrWhiteSpace(authorizationCode) || authorizationCode == "InvalidCode")
            {
                Assert.IsType<UnauthorizedObjectResult>(result);
            }
            else
            {
                Assert.IsType<JsonResult>(result);
                httpClient.VerifyAll();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void HandleExternalLogin_TokenisNullorEmpty(string token)
        {
            var userLogic = new Mock<IUserLogic>();
            var httpClient = new Mock<IHttpHandler>();
            var userSessionLogic = new MockUserSessionLogic();
            var logger = new Mock<ILogger<AccountController>>();

            var result = await new WebAPI.Controllers.AccountController(config, userLogic.Object, userSessionLogic.Object, httpClient.Object, logger.Object)
                .HandleExternalLogin(new User(), token);

            Assert.IsType<UnauthorizedObjectResult>(result);

            userLogic.VerifyAll();
        }

        [Fact]
        public async void HandleExternalLogin_NullUser()
        {
            var userLogic = new Mock<IUserLogic>();
            var httpClient = new Mock<IHttpHandler>();
            var userSessionLogic = new MockUserSessionLogic();
            var logger = new Mock<ILogger<AccountController>>();

            var result = await new WebAPI.Controllers.AccountController(config, userLogic.Object, userSessionLogic.Object, httpClient.Object, logger.Object)
                .HandleExternalLogin(null, "Token");

            Assert.IsType<UnauthorizedObjectResult>(result);

            userLogic.VerifyAll();
        }

        [Theory]
        [InlineData("AdeKnows@gmail.com", "Ade", "Knows", "AdeKnows@gmail.com")]
        [InlineData("UnKnown@gmail.com", "Ade", "Knows", "AdeKnows@gmail.com")]
        public async void HandleExternalLogin_Authenticated(string email, string givenName, string surname, string knownEmail)
        {
            //Arrange
            //Create Authentication Ticket
            var authenticationTicket = new AuthenticationTicket(
                new ClaimsPrincipal(
                    new List<ClaimsIdentity> {
                        new ClaimsIdentity(
                            new List<Claim> {
                                new Claim(ClaimTypes.Name, $"{givenName} {surname}"),
                                new Claim(ClaimTypes.GivenName, givenName),
                                new Claim(ClaimTypes.Surname, surname),
                                new Claim(ClaimTypes.Email, email),
                            })
                    }), "Google");

            //Create user
            var user = new User
            {
                Email = email,
                FirstName = givenName,
                LastName = surname,
                UserName = $"{givenName} {surname}",
                LoginServiceProvider = "Mine"
            };

            //Create token
            string token = Guid.NewGuid().ToString();

            //declare mocks
            MockUserLogic userLogic = new MockUserLogic();
            var httpClient = new Mock<IHttpHandler>();
            MockUserSessionLogic userSessionLogic = new MockUserSessionLogic();
            var logger = new Mock<ILogger<AccountController>>();

            //Setup Mock methods
            if (email != knownEmail) userLogic.MockCreateUser(user);

            userLogic.MockGetUserByEmail(email, knownEmail, user);

            userSessionLogic.MockSetUpUserSession(token, user);

            //Act
            IActionResult result = await new WebAPI.Controllers.AccountController(config, userLogic.Object, userSessionLogic.Object, httpClient.Object, logger.Object)
                .HandleExternalLogin(user, token);

            //Assert
            if (email != knownEmail) Assert.True(userLogic.AccountCreated);
            else Assert.False(userLogic.AccountCreated);

            Assert.True(userSessionLogic.SessionCreated);

            JsonResult theResult = result as JsonResult;
            Assert.NotNull(theResult);

            AuthToken authToken = theResult.Value as AuthToken;
            Assert.Equal(ShaHashTool.Hash($"{user.LoginServiceProvider}-{token}"), authToken.AccessToken);


            //Mock validations
            userLogic.VerifyAll();
            userSessionLogic.VerifyAll();

        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void GetUserByToken_NullOrEmpty(string token)
        {
            var userLogic = new Mock<IUserLogic>();
            var httpClient = new Mock<IHttpHandler>();
            var userSessionLogic = new MockUserSessionLogic();
            var logger = new Mock<ILogger<AccountController>>();


            var result = await new WebAPI.Controllers.AccountController(config, userLogic.Object, userSessionLogic.Object, httpClient.Object, logger.Object).GetUserByToken(token);
            Assert.IsType<UnauthorizedObjectResult>(result);

        }

        [Theory]
        [InlineData("KnownToken")]
        [InlineData("UnknownToken")]
        public async void GetUserByToken_NotNull(string token)
        {
            var userLogic = new Mock<IUserLogic>();
            var httpClient = new Mock<IHttpHandler>();
            var userSessionLogic = new MockUserSessionLogic();
            var logger = new Mock<ILogger<AccountController>>();
            String knownToken = "KnownToken";

            userSessionLogic.MockGetUserSessionByToken(token, knownToken, DateTime.Now.AddHours(24));

            var result = await new WebAPI.Controllers.AccountController(config, userLogic.Object, userSessionLogic.Object, httpClient.Object, logger.Object).GetUserByToken(token);

            //unknown Token
            if (token != knownToken)
            {
                Assert.IsType<UnauthorizedObjectResult>(result);
                return;
            }

            //known Token, validate response
            JsonResult theResult = result as JsonResult;
            Assert.NotNull(theResult);

            User user = theResult.Value as User;
            Assert.NotNull(user);
            Assert.NotNull(user.Email);
            Assert.NotNull(user.Id);
            Assert.NotNull(user.FirstName);
            Assert.NotNull(user.LastName);

            userSessionLogic.VerifyAll();
        }

        [Fact]
        public async void GetUserByToken_ExpiredToken()
        {
            var userLogic = new Mock<IUserLogic>();
            var httpClient = new Mock<IHttpHandler>();
            var userSessionLogic = new MockUserSessionLogic();
            var logger = new Mock<ILogger<AccountController>>();
            String token = Guid.NewGuid().ToString();

            userSessionLogic.MockGetUserSessionByToken(token, token, DateTime.Now.AddHours(-2));

            var result = await new WebAPI.Controllers.AccountController(config, userLogic.Object, userSessionLogic.Object, httpClient.Object, logger.Object).GetUserByToken(token);

            Assert.IsType<UnauthorizedObjectResult>(result);

            userSessionLogic.VerifyAll();
        }
    }
}
