using Moq;
using ProfileService.Logic.Interfaces;
using ProfileService.Models;
using ProfileService.Models.Exceptions;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProfileService.Test.Mocks
{
    public class MockProfessionalLogic : Mock<IProfessionalLogic>
    {
        public bool ProfileCreated { get; set; }
        public bool ProfileUpdated { get; set; }

        public void MockSetupProfile(Professional profile)
        {
            Setup(x => x.SetupProfileAsync(
                It.Is<Professional>(c => c == profile)
                )).Callback(() => ProfileCreated = true)
                .Returns<Professional>(s => Task.FromResult<Professional>(s));
        }
        public void MockSetupProfileWithException(Professional profile)
        {
            Setup(x => x.SetupProfileAsync(
                It.Is<Professional>(c => c == profile)
                ))
                .Throws(new ProfileServiceException("Test Exception"));
        }
        public void MockUpdateProfile(Professional profile)
        {
            Setup(x => x.UpdateProfileAsync(
                It.Is<Professional>(c => c == profile)
                )).Callback(() => ProfileUpdated = true)
                .Returns<Professional>(s => Task.FromResult<Professional>(s));
        }
        public void MockUpdateProfileWithException(Professional profile)
        {
            Setup(x => x.UpdateProfileAsync(
                It.Is<Professional>(c => c == profile)
                ))
                .Throws(new ProfileServiceException("Test Exception"));
        }
        public void MockGetProfileByID(string id, string knownId, Professional profile)
        {
            Professional output = null;
            if (!string.IsNullOrWhiteSpace(id) && id == knownId)
            {
                output = profile;
            }

            Setup(x => x.GetProfileByIDAsync(
                It.Is<string>(c => c == id)
                )).Returns(Task.FromResult<Professional>(output));
        }

        public void MockGetAllProfiles(string userId, string knownUserId)
        {
            List<Professional> output = new List<Professional>();
            if (!string.IsNullOrWhiteSpace(userId) && userId == knownUserId)
            {
                output = new List<Professional> { new Professional { UserId = userId }, new Professional { UserId = userId } };
            }

            Setup(x => x.GetAllProfilesAsync(
                It.Is<string>(c => c == userId)
                )).Returns(Task.FromResult(output));
        }

        public void MockGetAllProfilesByIDs(List<string> ids, List<string> knownIds)
        {
            List<Professional> output = new List<Professional>();
            if (ids != null && knownIds != null)
            {
                foreach (var id in ids)
                {
                    if (knownIds.Contains(id))
                        output.Add(new Professional { Id = id });
                }
            }

            Setup(x => x.GetAllProfilesByIDsAsync(
                It.IsAny<List<string>>()
                )).Returns(Task.FromResult(output));
        }
        public void MockGetAllProfilesByInterests(List<Interest> interests, List<Interest> knownInterests)
        {
            List<Professional> output = new List<Professional>();
            if (interests != null && knownInterests != null)
            {
                foreach (var interest in interests)
                {
                    if (knownInterests.Contains(interest))
                        output.Add(new Professional { Interests = new List<Interest> { interest } });
                }
            }

            Setup(x => x.GetAllProfilesByInterestsAsync(
                It.Is<List<Interest>>(c => c == interests)
                )).Returns(Task.FromResult(output));
        }
    }
}
