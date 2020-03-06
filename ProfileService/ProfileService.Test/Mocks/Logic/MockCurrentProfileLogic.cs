using Moq;
using ProfileService.Logic.Interfaces;
using ProfileService.Models;
using ProfileService.Models.Exceptions;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProfileService.Test.Mocks.Logic
{
    public class MockCurrentProfileLogic : Mock<ICurrentProfileLogic>
    {
        public bool ProfileCreated { get; set; }
        public bool ProfileUpdated { get; set; }

        public void MockSetupProfile(string userId, string profileId, ProfileType profileType)
        {
            CurrentProfile output = new CurrentProfile
            {
                UserID = userId,
                ProfileID = profileId,
                ProfileType = profileType
            };

            Setup(x => x.SetCurrentProfileAsync(
                It.Is<string>(c => c == userId),
                It.Is<string>(c => c == profileId),
                It.Is<ProfileType>(c => c == profileType)
                )).Callback(() => ProfileCreated = true)
                .Returns(Task.FromResult(output));
        }
        public void MockSetupProfileWithException(string userId, string profileId, ProfileType profileType)
        {
            Setup(x => x.SetCurrentProfileAsync(
                It.Is<string>(c => c == userId),
                It.Is<string>(c => c == profileId),
                It.Is<ProfileType>(c => c == profileType)
                ))
                .Throws(new ProfileServiceException("Test Exception"));
        }
        public void MockGetCurrentProfile(string userId)
        {
            CurrentProfile output = new CurrentProfile { UserID = userId };

            Setup(x => x.GetCurrentProfileAsync(
                It.Is<string>(c => c == userId)
                )).Returns(Task.FromResult<CurrentProfile>(output));
        }

        public void MockGetCurrentProfileWithException(string userId)
        {
            Setup(x => x.GetCurrentProfileAsync(
                It.Is<string>(c => c == userId)
                ))
                .Throws(new ProfileServiceException("Test Exception"));
        }
    }
}
