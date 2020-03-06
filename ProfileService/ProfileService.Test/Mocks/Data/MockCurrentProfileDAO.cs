using Moq;
using ProfileService.Data.Interfaces;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProfileService.Test.Mocks
{
    public class MockCurrentProfileDAO : Mock<ICurrentProfileDAO>
    {
        public void MockSaveProfile()
        {
            Setup(x => x.SaveCurrentProfileAsync(
                It.IsAny<CurrentProfile>()
                ))
                .Returns<CurrentProfile>(s => Task.FromResult<CurrentProfile>(s));
        }
        public void MockUpdateProfile()
        {
            Setup(x => x.UpdateCurrentProfileAsync(
                It.IsAny<CurrentProfile>()
                ))
                .Returns<CurrentProfile>(s => Task.FromResult<CurrentProfile>(s));
        }
        public void MockGetCurrentProfile(string userId, string knownUserId, CurrentProfile currentProfile = null)
        {
            CurrentProfile output = null;
            if (!string.IsNullOrWhiteSpace(userId) && userId == knownUserId)
            {
                output = currentProfile ?? new CurrentProfile
                {
                    UserID = userId
                };
            }

            Setup(x => x.GetCurrentProfileAsync(
                It.Is<string>(c => c == userId)
                )).Returns(Task.FromResult<CurrentProfile>(output));
        }
    }
}
