using Moq;
using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Test.Mocks
{
    public class MockInternalServiceSessionDAO : Mock<IInternalServiceSessionDAO>
    {
        public void MockGetSessionByToken(string token, string knownToken)
        {
            InternalServiceSession output = null;
            if (!string.IsNullOrWhiteSpace(token) && token == knownToken)
            {
                output = new InternalServiceSession
                {
                    AuthToken = token
                };
            }
            Setup(x => x.GetServiceSessionByTokenAsync(
                It.Is<string>(c => c == token)
                ))
                .Returns(Task.FromResult<InternalServiceSession>(output));
        }

        public void MockGetServiceSessionByAppKey(string appKey, string knownAppKey)
        {
            InternalServiceSession output = null;
            if (!string.IsNullOrWhiteSpace(appKey) && appKey == knownAppKey)
            {
                output = new InternalServiceSession
                {
                    AppKey = appKey
                };
            }
            Setup(x => x.GetServiceSessionByAppKeyAsync(
                It.Is<string>(c => c == appKey)
                ))
                .Returns(Task.FromResult<InternalServiceSession>(output));
        }
        public void MockUpdateSession()
        {          
            Setup(x => x.UpdateServiceSessionAsync(
                It.IsAny<InternalServiceSession>()
                )).Returns<InternalServiceSession>(s =>Task.FromResult<InternalServiceSession>(s));
        }
        public void MockCreateSession()
        {
            Setup(x => x.CreateServiceSessionAsync(
                It.IsAny<InternalServiceSession>()
                )).Returns<InternalServiceSession>(s => Task.FromResult<InternalServiceSession>(s));
        }
    }
}
