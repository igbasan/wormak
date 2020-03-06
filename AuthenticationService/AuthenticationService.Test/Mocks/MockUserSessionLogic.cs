using Moq;
using AuthenticationService.WebAPI.Logic.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AuthenticationService.WebAPI.Utilities;

namespace AuthenticationService.Test.Mocks
{
    public class MockUserSessionLogic : Mock<IUserSessionLogic>
    {
        public bool SessionCreated { get; set; }

        public void MockSetUpUserSession(string token, User user)
        {
            UserSession session = new UserSession
            {
                AuthToken = ShaHashTool.Hash($"{user.LoginServiceProvider}-{token}"),
                ExpirationDate = DateTime.Now.AddHours(24),
                FirstloginDate = DateTime.Now,
                LastloginDate = DateTime.Now,
                User = user,
                UserID = user.Id,
                ProviderAuthCode = token,
                LoginServiceProvider = user.LoginServiceProvider
            };
            Setup(x => x.SetUpUserSessionAsync(
                It.Is<string>(c => c == token), It.IsAny<User>()
                )).Callback(() => SessionCreated = true)
                .Returns(Task.FromResult<UserSession>(session));
        }
        public void MockGetUserSessionByToken(string token, string knownToken, DateTime expirationDate)
        {
            UserSession session = null;
            if (!string.IsNullOrWhiteSpace(token) && token == knownToken)
            {
                session = new UserSession
                {
                    AuthToken = token,
                    ExpirationDate = expirationDate,
                    FirstloginDate = DateTime.Now,
                    LastloginDate = DateTime.Now,
                    User = new User
                    {
                        Email = "aideknows@gmail.com",
                        FirstName = "Aide",
                        LastName = "Knows",
                        UserName = $"aide4th",
                        Id = Guid.NewGuid().ToString()
                    },
                    UserID = "userID"
                };
            }

            Setup(x => x.GetUserSessionByTokenAsync(
                It.Is<string>(c => c == token)
                )).Returns(Task.FromResult<UserSession>(session));
        }
    }
}
