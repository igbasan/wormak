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

namespace AuthenticationService.Test.ControllerTests
{
    public class InternalServiceControllerTest
    {



        [Fact]
        public async void HandleExternalLogin_KeyListisNull()
        {
            IInternalServiceKeys serviceKeys = new InternalServiceKeys { ServiceKeys = null };
            var internalServiceSessionLogic = new Mock<IInternalServiceSessionLogic>();

            var result = await new WebAPI.Controllers.InternalServiceController(serviceKeys, internalServiceSessionLogic.Object)
                .HandleExternalLogin("Key");

            Assert.IsType<UnauthorizedObjectResult>(result);

            internalServiceSessionLogic.VerifyAll();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void HandleExternalLogin_KeyisNullorEmpty(string key)
        {
            IInternalServiceKeys serviceKeys = new InternalServiceKeys { ServiceKeys = new Dictionary<string, string> { { "key", "value"} }  };
            var internalServiceSessionLogic = new Mock<IInternalServiceSessionLogic>();

            var result = await new WebAPI.Controllers.InternalServiceController(serviceKeys, internalServiceSessionLogic.Object)
                .HandleExternalLogin(key);

            Assert.IsType<UnauthorizedObjectResult>(result);

            internalServiceSessionLogic.VerifyAll();
        }

        [Theory]
        [InlineData("InvalidKey")]
        public async void HandleExternalLogin_InvalidKey(string key)
        {
            IInternalServiceKeys serviceKeys = new InternalServiceKeys { ServiceKeys = new Dictionary<string, string> { { "Key", "value" } } };
            var internalServiceSessionLogic = new Mock<IInternalServiceSessionLogic>();

            var result = await new WebAPI.Controllers.InternalServiceController(serviceKeys, internalServiceSessionLogic.Object)
                .HandleExternalLogin(key);

            Assert.IsType<UnauthorizedObjectResult>(result);

            internalServiceSessionLogic.VerifyAll();
        }
        
        [Fact]
        public async void HandleExternalLogin_Authenticated()
        {
            //Create key
            string key = Guid.NewGuid().ToString();
            IInternalServiceKeys serviceKeys = new InternalServiceKeys { ServiceKeys = new Dictionary<string, string> { { key, "TestService" } } };
            var internalServiceSessionLogic = new MockInternalServiceSessionLogic();

            //Setup Mock methods
            internalServiceSessionLogic.MockSetUpServiceSession("TestService",key);

            //Act
            IActionResult result = await new WebAPI.Controllers.InternalServiceController(serviceKeys, internalServiceSessionLogic.Object)
                .HandleExternalLogin(key);

            //Assert
            Assert.True(internalServiceSessionLogic.SessionCreated);

            JsonResult theResult = result as JsonResult;
            Assert.NotNull(theResult);

            AuthToken authToken = theResult.Value as AuthToken;
            Assert.NotNull(authToken.AccessToken);


            //Mock validations
            internalServiceSessionLogic.VerifyAll();

        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void VaidateServiceToken_NullOrEmpty(string token)
        {
            IInternalServiceKeys serviceKeys = new InternalServiceKeys ();
            var internalServiceSessionLogic = new MockInternalServiceSessionLogic();


            var result = await new WebAPI.Controllers.InternalServiceController(serviceKeys, internalServiceSessionLogic.Object).VaidateServiceToken(token);
            Assert.IsType<UnauthorizedObjectResult>(result);

        }

        [Theory]
        [InlineData("KnownToken")]
        [InlineData("UnknownToken")]
        public async void VaidateServiceToken_NotNull(string token)
        {
            IInternalServiceKeys serviceKeys = new InternalServiceKeys();
            var internalServiceSessionLogic = new MockInternalServiceSessionLogic();
            string knownToken = "KnownToken";

            internalServiceSessionLogic.MockGetServiceSessionByToken(token, knownToken, DateTime.Now.AddHours(24));

            var result = await new WebAPI.Controllers.InternalServiceController(serviceKeys, internalServiceSessionLogic.Object).VaidateServiceToken(token);

            //unknown Token
            if (token != knownToken)
            {
                Assert.IsType<UnauthorizedObjectResult>(result);
                return;
            }

            //known Token, validate response
            JsonResult theResult = result as JsonResult;
            Assert.NotNull(theResult);

            string appName = theResult.Value as string;
            Assert.NotNull(appName);

            internalServiceSessionLogic.VerifyAll();
        }

        [Fact]
        public async void VaidateServiceToken_ExpiredToken()
        {
            IInternalServiceKeys serviceKeys = new InternalServiceKeys();
            var internalServiceSessionLogic = new MockInternalServiceSessionLogic();
            string token = Guid.NewGuid().ToString();

            internalServiceSessionLogic.MockGetServiceSessionByToken(token, token, DateTime.Now.AddHours(-2));

            var result = await new WebAPI.Controllers.InternalServiceController(serviceKeys, internalServiceSessionLogic.Object).VaidateServiceToken(token);

            Assert.IsType<UnauthorizedObjectResult>(result);

            internalServiceSessionLogic.VerifyAll();
        }
    }
}
