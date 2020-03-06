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
    public class UserSessionLogicTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async void SetUpUserSession_NullOrEmptyToken(string token)
        {
            //Arrange
            var mockUserSessionDAO = new MockUserSessionDAO();
            var mockLoginAttemptDAO = new MockLoginAttemptDAO();
            var mockUserDAO = new MockUserDAO();
            User user = new User();

            //Act
            var exception = await Record.ExceptionAsync(() => new UserSessionLogic(mockUserSessionDAO.Object, mockLoginAttemptDAO.Object, mockUserDAO.Object).SetUpUserSessionAsync(token, user));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async void SetUpUserSession_NullUser()
        {
            //Arrange
            var mockUserSessionDAO = new MockUserSessionDAO();
            var mockLoginAttemptDAO = new MockLoginAttemptDAO();
            var mockUserDAO = new MockUserDAO();
            string token = Guid.NewGuid().ToString();
            User user = null;

            //Act
            var exception = await Record.ExceptionAsync(() => new UserSessionLogic(mockUserSessionDAO.Object, mockLoginAttemptDAO.Object, mockUserDAO.Object).SetUpUserSessionAsync(token, user));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("knownUserID")]
        [InlineData("unKnownUserID")]
        public async void SetUpUserSession_NotNull(string userID)
        {
            //Arrange
            var mockUserSessionDAO = new MockUserSessionDAO();
            var mockLoginAttemptDAO = new MockLoginAttemptDAO();
            var mockUserDAO = new MockUserDAO();
            string token = Guid.NewGuid().ToString();
            string knownUserID = "knownUserID";

            User user = new User
            {
                Email = "aide4th@gmail.com",
                FirstName = "Ade",
                LastName = "Knows",
                UserName = "Aide4th",
                LoginServiceProvider= "Mine",
                Id = userID
            };

            mockUserSessionDAO.MockGetUserSessionByUserID(userID, knownUserID);
            //Create or update new session
            if (knownUserID == userID)
            {
                mockUserSessionDAO.MockUpdateSession();
            }
            else
            {
                mockUserSessionDAO.MockCreateSession();
            }

            mockLoginAttemptDAO.MockSaveLoginAttempt();

            //Act
            var userSession = await new UserSessionLogic(mockUserSessionDAO.Object, mockLoginAttemptDAO.Object, mockUserDAO.Object).SetUpUserSessionAsync(token, user);

            //Assert
            Assert.NotNull(userSession);
            Assert.Equal(user, userSession.User);
            Assert.Equal(token, userSession.ProviderAuthCode);
            Assert.Equal(ShaHashTool.Hash($"{user.LoginServiceProvider}-{token}"), userSession.AuthToken);

            Assert.NotEqual(DateTime.MinValue, userSession.ExpirationDate);
            Assert.InRange(userSession.LastloginDate, DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));

            if (knownUserID != userID) Assert.InRange(userSession.FirstloginDate,DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));

            mockUserSessionDAO.VerifyAll();
            mockLoginAttemptDAO.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async void GetUserSessionByToken_NullOrEmpty(string token)
        {
            //Arrange
            var mockUserSessionDAO = new MockUserSessionDAO();
            var mockLoginAttemptDAO = new MockLoginAttemptDAO();
            var mockUserDAO = new MockUserDAO();

            //Act
            var exception = await Record.ExceptionAsync(() => new UserSessionLogic(mockUserSessionDAO.Object, mockLoginAttemptDAO.Object, mockUserDAO.Object).GetUserSessionByTokenAsync(token));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("KnownToken")]
        [InlineData("UnknownToken")]
        public async void GetUserSessionByToken_NotNull(string token)
        {
            //Arrange
            var mockUserSessionDAO = new MockUserSessionDAO();
            var mockLoginAttemptDAO = new MockLoginAttemptDAO();
            var mockUserDAO = new MockUserDAO();
            string knownToken = "KnownToken";
            string userID = "userID";

            mockUserSessionDAO.MockGetSessionByToken(token, knownToken, userID);

            mockUserDAO.MockGetUserByID(userID);

            //Act
            var userSession = await new UserSessionLogic(mockUserSessionDAO.Object, mockLoginAttemptDAO.Object, mockUserDAO.Object).GetUserSessionByTokenAsync(token);


            //Assert
            if (token == knownToken)
            {
                Assert.NotNull(userSession);
                Assert.NotNull(userSession.User);
                Assert.Equal(token, userSession.AuthToken);
                mockUserDAO.VerifyAll();
            }
            else
            {
                Assert.Null(userSession);
            }

            mockUserSessionDAO.VerifyAll();
        }
    }
}
