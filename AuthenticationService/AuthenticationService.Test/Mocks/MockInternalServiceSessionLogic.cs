using AuthenticationService.WebAPI.Logic.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using AuthenticationService.WebAPI.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Test.Mocks
{
   public class MockInternalServiceSessionLogic : Mock<IInternalServiceSessionLogic>
    {
        public bool SessionCreated { get; set; }

        public void MockSetUpServiceSession(string appName, string appKey)
        {
            InternalServiceSession session = new InternalServiceSession
            {
                AuthToken = ShaHashTool.Hash($"{appName}-{Guid.NewGuid().ToString()}"),
                ExpirationDate = DateTime.Now.AddHours(24),
                FirstKeyExchangeDate = DateTime.Now,
                LastExchangeDate = DateTime.Now,
                AppKey = appKey,
                AppName = appName
            };
            Setup(x => x.SetUpServiceSessionAsync(
                It.Is<string>(c => c == appName),
                It.Is<string>(c => c == appKey)
                )).Callback(() => SessionCreated = true)
                .Returns(Task.FromResult<InternalServiceSession>(session));
        }
        public void MockGetServiceSessionByToken(string token, string knownToken, DateTime expirationDate)
        {
            InternalServiceSession session = null;
            if (!string.IsNullOrWhiteSpace(token) && token == knownToken)
            {
                session = new InternalServiceSession
                {
                    AuthToken = ShaHashTool.Hash($"AppName-{Guid.NewGuid().ToString()}"),
                    ExpirationDate = expirationDate,
                    FirstKeyExchangeDate = DateTime.Now,
                    LastExchangeDate = DateTime.Now,
                    AppKey = "Key",
                    AppName = "AppName"
                };
            }

            Setup(x => x.GetServiceSessionByTokenAsync(
                It.Is<string>(c => c == token)
                )).Returns(Task.FromResult<InternalServiceSession>(session));
        }
    }
}