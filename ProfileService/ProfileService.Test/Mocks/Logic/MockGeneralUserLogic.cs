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
    public class MockGeneralUserLogic : Mock<IGeneralUserLogic>
    {
        public bool ProfileCreated { get; set; }
        public bool ProfileUpdated { get; set; }

        public void MockSetupProfile(GeneralUser profile)
        {
            Setup(x => x.SetupProfileAsync(
                It.Is<GeneralUser>(c => c == profile)
                )).Callback(() => ProfileCreated = true)
                .Returns<GeneralUser>(s => Task.FromResult<GeneralUser>(s));
        }
        public void MockSetupProfileWithException(GeneralUser profile)
        {
            Setup(x => x.SetupProfileAsync(
                It.Is<GeneralUser>(c => c == profile)
                ))
                .Throws(new ProfileServiceException("Test Exception"));
        }
        public void MockUpdateProfile(GeneralUser profile)
        {
            Setup(x => x.UpdateProfileAsync(
                It.Is<GeneralUser>(c => c == profile)
                )).Callback(() => ProfileUpdated = true)
                .Returns<GeneralUser>(s => Task.FromResult<GeneralUser>(s));
        }
        public void MockUpdateProfileWithException(GeneralUser profile)
        {
            Setup(x => x.UpdateProfileAsync(
                It.Is<GeneralUser>(c => c == profile)
                ))
                .Throws(new ProfileServiceException("Test Exception"));
        }
        public void MockGetProfileByID(string id, string knownId, GeneralUser profile)
        {
            GeneralUser output = null;
            if (!string.IsNullOrWhiteSpace(id) && id == knownId)
            {
                output = profile;
            }

            Setup(x => x.GetProfileByIDAsync(
                It.Is<string>(c => c == id)
                )).Returns(Task.FromResult<GeneralUser>(output));
        }

        public void MockGetAllProfiles(string userId, string knownUserId)
        {
            List<GeneralUser> output = new List<GeneralUser>();
            if (!string.IsNullOrWhiteSpace(userId) && userId == knownUserId)
            {
                output = new List<GeneralUser> { new GeneralUser { UserId = userId, Id = "ProfileID" } };
            }

            Setup(x => x.GetAllProfilesAsync(
                It.Is<string>(c => c == userId)
                )).Returns(Task.FromResult(output));
        }

        public void MockGetAllProfilesByIDs(List<string> ids, List<string> knownIds)
        {
            List<GeneralUser> output = new List<GeneralUser>();
            if (ids != null && knownIds != null)
            {
                foreach (var id in ids)
                {
                    if (knownIds.Contains(id))
                        output.Add(new GeneralUser { Id = id });
                }
            }

            Setup(x => x.GetAllProfilesByIDsAsync(
                It.IsAny<List<string>>()
                )).Returns(Task.FromResult(output));
        }

        public void MockGetAllProfilesByInterests(List<Interest> interests, List<Interest> knownInterests)
        {
            List<GeneralUser> output = new List<GeneralUser>();
            if (interests != null && knownInterests != null)
            {
                foreach (var interest in interests)
                {
                    if (knownInterests.Contains(interest))
                        output.Add(new GeneralUser { Interests = new List<Interest> { interest } });
                }
            }

            Setup(x => x.GetAllProfilesByInterestsAsync(
                It.Is<List<Interest>>(c => c == interests)
                )).Returns(Task.FromResult(output));
        }
    }
}
