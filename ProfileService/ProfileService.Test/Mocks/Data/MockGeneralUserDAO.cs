using Moq;
using ProfileService.Data.Interfaces;
using ProfileService.Models;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProfileService.Test.Mocks
{
    public class MockGeneralUserDAO : Mock<IGeneralUserDAO>
    {
        public void MockSaveProfile(GeneralUser profile)
        {
            Setup(x => x.SaveProfileAsync(
                It.Is<GeneralUser>(c => c == profile)
                ))
                .Returns<GeneralUser>(s => Task.FromResult<GeneralUser>(s));
        }
        public void MockUpdateProfile(GeneralUser profile)
        {
            Setup(x => x.UpdateProfileAsync(
                It.Is<GeneralUser>(c => c == profile)
                ))
                .Returns<GeneralUser>(s => Task.FromResult<GeneralUser>(s));
        }
        public void MockGetProfileByID(string id, string knownId)
        {
            GeneralUser output = null;
            if (!string.IsNullOrWhiteSpace(id) && id == knownId)
            {
                output = new GeneralUser
                {
                    Id = id
                };
            }

            Setup(x => x.GetProfileByIDAsync(
                It.Is<string>(c => c == id)
                )).Returns(Task.FromResult<GeneralUser>(output));
        }

        public void MockGetProfileByID(string userid, List<Interest> interests = null )
        {
            GeneralUser output = new GeneralUser
            {
                UserId = userid,
                Interests = interests
            };

            Setup(x => x.GetProfileByIDAsync(
                It.IsAny<string>()
                )).Returns(Task.FromResult<GeneralUser>(output));
        }

        public void MockGetProfileByUserIDAsync(string userID, string knownUserID)
        {
            GeneralUser output = null;
            if (!string.IsNullOrWhiteSpace(userID) && userID == knownUserID)
            {
                output = new GeneralUser
                {
                    UserId = userID
                };
            }

            Setup(x => x.GetProfileByUserIDAsync(
                It.Is<string>(c => c == userID)
                )).Returns(Task.FromResult<GeneralUser>(output));
        }

        public void MockGetProfileByUserIDAndNameAsync(string userID, string knownUserID, string name, string knownName, string id = null)
        {
            GeneralUser output = null;
            if (!string.IsNullOrWhiteSpace(userID) && !string.IsNullOrWhiteSpace(name) && userID == knownUserID && name == knownName)
            {
                output = new GeneralUser
                {
                    UserId = userID,
                    Name = name,
                    Id = id
                };
            }

            Setup(x => x.GetProfileByUserIDAndNameAsync(
                It.Is<string>(c => c == userID),
                It.Is<string>(c => c == name)
                )).Returns(Task.FromResult<GeneralUser>(output));
        }

        public void MockGetAllProfilesAsync(string userID, string knownUserID)
        {
            List<GeneralUser> output = new List<GeneralUser>();
            if (!string.IsNullOrWhiteSpace(userID) && userID == knownUserID)
            {
                output = new List<GeneralUser>{
                new GeneralUser
                {
                    UserId = userID
                },
                 new GeneralUser
                {
                    UserId = userID
                }};
            }

            Setup(x => x.GetAllProfilesAsync(
                It.Is<string>(c => c == userID)
                )).Returns(Task.FromResult<List<GeneralUser>>(output));
        }

        public void MockGetAllProfilesByProfileIDs(List<string> ids, List<string> knownIds)
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

            Setup(x => x.GetAllProfilesByProfileIDsAsync(
                It.Is<List<string>>(c => c == ids)
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
