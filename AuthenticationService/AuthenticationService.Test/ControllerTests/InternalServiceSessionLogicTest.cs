using Moq;
using AuthenticationService.Test.Mocks;
using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Logic.Implementations;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using AuthenticationService.WebAPI.Utilities;

namespace AuthenticationService.Test.ControllerTests
{
    public class InternalServiceSessionLogicTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async void SetUpServiceSession_NullOrEmptyAppName(string appName)
        {
            //Arrange
            var mockInternalServiceSessionDAO = new MockInternalServiceSessionDAO();
            User user = new User();

            //Act
            var exception = await Record.ExceptionAsync(() => new InternalServiceSessionLogic(mockInternalServiceSessionDAO.Object).SetUpServiceSessionAsync(appName, "AppKey"));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async void SetUpServiceSession_NullOrEmptyAppKey(string appKey)
        {
            //Arrange
            var mockInternalServiceSessionDAO = new MockInternalServiceSessionDAO();
            User user = new User();

            //Act
            var exception = await Record.ExceptionAsync(() => new InternalServiceSessionLogic(mockInternalServiceSessionDAO.Object).SetUpServiceSessionAsync("AppName", appKey));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("knownAppID")]
        [InlineData("unKnownAppID")]
        public async void SetUpServiceSession_NotNull(string appKey)
        {
            //Arrange
            var mockInternalServiceSessionDAO = new MockInternalServiceSessionDAO();
            string token = Guid.NewGuid().ToString();
            string knownAppKey = "knownAppID";
            string appName = "AppName";

            mockInternalServiceSessionDAO.MockGetServiceSessionByAppKey(appKey, knownAppKey);
            //Create or update new session
            if (knownAppKey == appKey)
            {
                mockInternalServiceSessionDAO.MockUpdateSession();
            }
            else
            {
                mockInternalServiceSessionDAO.MockCreateSession();
            }

            //Act
            var serviceSession = await new InternalServiceSessionLogic(mockInternalServiceSessionDAO.Object).SetUpServiceSessionAsync(appName, appKey);

            //Assert
            Assert.NotNull(serviceSession);
            Assert.Equal(appName, serviceSession.AppName);
            Assert.Equal(appKey, serviceSession.AppKey);
            Assert.NotNull(serviceSession.AuthToken);

            Assert.NotEqual(DateTime.MinValue, serviceSession.ExpirationDate);
            Assert.InRange(serviceSession.LastExchangeDate, DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));

            //check for newly created record
            if (knownAppKey != appKey) Assert.InRange(serviceSession.FirstKeyExchangeDate,DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));

            mockInternalServiceSessionDAO.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async void GetServiceSessionByToken_NullOrEmpty(string token)
        {
            //Arrange
            var mockInternalServiceSessionDAO = new MockInternalServiceSessionDAO();

            //Act
            var exception = await Record.ExceptionAsync(() => new InternalServiceSessionLogic(mockInternalServiceSessionDAO.Object).GetServiceSessionByTokenAsync(token));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("KnownToken")]
        [InlineData("UnknownToken")]
        public async void GetServiceSessionByToken_NotNull(string token)
        {
            //Arrange
            var mockInternalServiceSessionDAO = new MockInternalServiceSessionDAO();
            string knownToken = "KnownToken";

            mockInternalServiceSessionDAO.MockGetSessionByToken(token, knownToken);

            //Act
            var serviceSession = await new InternalServiceSessionLogic(mockInternalServiceSessionDAO.Object).GetServiceSessionByTokenAsync(token);


            //Assert
            if (token == knownToken)
            {
                Assert.NotNull(serviceSession);
                Assert.Equal(token, serviceSession.AuthToken);
            }
            else
            {
                Assert.Null(serviceSession);
            }

            mockInternalServiceSessionDAO.VerifyAll();
        }
    }
}
