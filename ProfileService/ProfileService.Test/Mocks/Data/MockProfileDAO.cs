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
    public class MockProfileDAO<T> : Mock<IProfileDAO<T>> where T : Profile, new()
    {
        public void MockSaveProfile(T profile)
        {
            Setup(x => x.SaveProfileAsync(
                It.Is<T>(c => c == profile)
                ))
                .Returns<T>(s => Task.FromResult<T>(s));
        }
        public void MockUpdateProfile(T profile)
        {
            Setup(x => x.UpdateProfileAsync(
                It.Is<T>(c => c == profile)
                ))
                .Returns<T>(s => Task.FromResult<T>(s));
        }
        public void MockGetProfileByID(string id, string knownId)
        {
            T output = null;
            if (!string.IsNullOrWhiteSpace(id) && id == knownId)
            {
                output = new T
                {
                    Id = id
                };
            }

            Setup(x => x.GetProfileByIDAsync(
                It.Is<string>(c => c == id)
                )).Returns(Task.FromResult<T>(output));
        }

        public void MockGetProfileByID(string userid, List<Interest> interests = null)
        {
            T output = new T
            {
                UserId = userid,
                Interests = interests
            };

            Setup(x => x.GetProfileByIDAsync(
                It.IsAny<string>()
                )).Returns(Task.FromResult<T>(output));
        }

        public void MockGetProfileByUserIDAsync(string userID, string knownUserID)
        {
            T output = null;
            if (!string.IsNullOrWhiteSpace(userID) && userID == knownUserID)
            {
                output = new T
                {
                    UserId = userID
                };
            }

            Setup(x => x.GetProfileByUserIDAsync(
                It.Is<string>(c => c == userID)
                )).Returns(Task.FromResult<T>(output));
        }

        public void MockGetProfileByUserIDAndNameAsync(string userID, string knownUserID, string name, string knownName, string id = null)
        {
            T output = null;
            if (!string.IsNullOrWhiteSpace(userID) && !string.IsNullOrWhiteSpace(name) && userID == knownUserID && name == knownName)
            {
                output = new T
                {
                    UserId = userID,
                    Name = name,
                    Id = id
                };
            }

            Setup(x => x.GetProfileByUserIDAndNameAsync(
                It.Is<string>(c => c == userID),
                It.Is<string>(c => c == name)
                )).Returns(Task.FromResult<T>(output));
        }

        public void MockGetAllProfilesAsync(string userID, string knownUserID)
        {
            List<T> output = new List<T>();
            if (!string.IsNullOrWhiteSpace(userID) && userID == knownUserID)
            {
                output = new List<T>{
                new T
                {
                    UserId = userID
                },
                 new T
                {
                    UserId = userID
                }};
            }

            Setup(x => x.GetAllProfilesAsync(
                It.Is<string>(c => c == userID)
                )).Returns(Task.FromResult<List<T>>(output));
        }

        public void MockGetAllProfilesByProfileIDs(List<string> ids, List<string> knownIds)
        {
            List<T> output = new List<T>();
            if (ids != null && knownIds != null)
            {
                foreach (var id in ids)
                {
                    if (knownIds.Contains(id))
                        output.Add(new T { Id = id });
                }
            }

            Setup(x => x.GetAllProfilesByProfileIDsAsync(
                It.Is<List<string>>(c => c == ids)
                )).Returns(Task.FromResult(output));
        }
        public void MockGetAllProfilesByInterests(List<Interest> interests, List<Interest> knownInterests)
        {
            List<T> output = new List<T>();
            if (interests != null && knownInterests != null)
            {
                foreach (var interest in interests)
                {
                    if (knownInterests.Contains(interest))
                        output.Add(new T { Interests = new List<Interest> { interest } });
                }
            }

            Setup(x => x.GetAllProfilesByInterestsAsync(
                It.Is<List<Interest>>(c => c == interests)
                )).Returns(Task.FromResult(output));
        }
    }
}
