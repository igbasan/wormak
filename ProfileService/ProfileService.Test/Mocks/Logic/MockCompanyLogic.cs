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
    public class MockCompanyLogic : Mock<ICompanyLogic>
    {
        public bool ProfileCreated { get; set; }
        public bool ProfileUpdated { get; set; }

        public void MockSetupProfile(Company profile)
        {
            Setup(x => x.SetupProfileAsync(
                It.Is<Company>(c => c == profile)
                )).Callback(() => ProfileCreated = true)
                .Returns<Company>(s => Task.FromResult<Company>(s));
        }
        public void MockSetupProfileWithException(Company profile)
        {
            Setup(x => x.SetupProfileAsync(
                It.Is<Company>(c => c == profile)
                ))
                .Throws(new ProfileServiceException("Test Exception"));
        }
        public void MockUpdateProfile(Company profile)
        {
            Setup(x => x.UpdateProfileAsync(
                It.Is<Company>(c => c == profile)
                )).Callback(() => ProfileUpdated = true)
                .Returns<Company>(s => Task.FromResult<Company>(s));
        }
        public void MockUpdateProfileWithException(Company profile)
        {
            Setup(x => x.UpdateProfileAsync(
                It.Is<Company>(c => c == profile)
                ))
                .Throws(new ProfileServiceException("Test Exception"));
        }
        public void MockGetProfileByID(string id, string knownId, Company profile)
        {
            Company output = null;
            if (!string.IsNullOrWhiteSpace(id) && id == knownId)
            {
                output = profile;
            }

            Setup(x => x.GetProfileByIDAsync(
                It.Is<string>(c => c == id)
                )).Returns(Task.FromResult<Company>(output));
        }

        public void MockGetAllProfiles(string userId, string knownUserId)
        {
            List<Company> output = new List<Company>();
            if (!string.IsNullOrWhiteSpace(userId) && userId == knownUserId)
            {
                output = new List<Company> { new Company { UserId = userId }, new Company { UserId = userId } };
            }

            Setup(x => x.GetAllProfilesAsync(
                It.Is<string>(c => c == userId)
                )).Returns(Task.FromResult(output));
        }

        public void MockGetAllProfilesByIDs(List<string> ids, List<string> knownIds)
        {
            List<Company> output = new List<Company>();
            if (ids != null && knownIds != null)
            {
                foreach (var id in ids)
                {
                    if (knownIds.Contains(id))
                        output.Add(new Company { Id = id });
                }
            }

            Setup(x => x.GetAllProfilesByIDsAsync(
                It.IsAny<List<string>>()
                )).Returns(Task.FromResult(output));
        }

        public void MockGetAllProfilesByInterests(List<Interest> interests, List<Interest> knownInterests)
        {
            List<Company> output = new List<Company>();
            if (interests != null && knownInterests != null)
            {
                foreach (var interest in interests)
                {
                    if (knownInterests.Contains(interest))
                        output.Add(new Company { Interests = new List<Interest> { interest } });
                }
            }

            Setup(x => x.GetAllProfilesByInterestsAsync(
                It.Is<List<Interest>>(c => c == interests)
                )).Returns(Task.FromResult(output));
        }
    }
}
