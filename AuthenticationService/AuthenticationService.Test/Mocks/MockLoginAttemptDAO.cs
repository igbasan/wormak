using Moq;
using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Test.Mocks
{
    public class MockLoginAttemptDAO : Mock<ILoginAttemptDAO>
    {
        public void MockSaveLoginAttempt()
        {
            Setup(x => x.SaveLoginAttemptAsync(
                It.IsAny<LoginAttempt>()
                )).Returns<LoginAttempt>(s => Task.FromResult<LoginAttempt>(s));
        }

    }
}
