using Moq;
using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Test.Mocks
{
    public class MockUserSessionDAO : Mock<IUserSessionDAO>
    {
        public void MockGetSessionByToken(string token, string knownToken, string userID)
        {
            UserSession output = null;
            if (!string.IsNullOrWhiteSpace(token) && token == knownToken)
            {
                output = new UserSession
                {
                    AuthToken = token,
                    UserID = userID
                };
            }
            Setup(x => x.GetUserSessionByTokenAsync(
                It.Is<string>(c => c == token)
                ))
                .Returns(Task.FromResult<UserSession>(output));
        }

        public void MockGetUserSessionByUserID(string userID, string knownuserID)
        {
            UserSession output = null;
            if (!string.IsNullOrWhiteSpace(userID) && userID == knownuserID)
            {
                output = new UserSession
                {
                    UserID = userID
                };
            }
            Setup(x => x.GetUserSessionByUserIDAsync(
                It.Is<string>(c => c == userID)
                ))
                .Returns(Task.FromResult<UserSession>(output));
        }
        public void MockUpdateSession()
        {          
            Setup(x => x.UpdateUserSessionAsync(
                It.IsAny<UserSession>()
                )).Returns<UserSession>(s =>Task.FromResult<UserSession>(s));
        }
        public void MockCreateSession()
        {
            Setup(x => x.CreateUserSessionAsync(
                It.IsAny<UserSession>()
                )).Returns<UserSession>(s => Task.FromResult<UserSession>(s));
        }
    }
}
