using Moq;
using PostService.Logic.Interfaces;
using PostService.Models;
using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Test.Mocks.Logic
{
    public class MockFollowerLogic : Mock<IFollowerLogic>
    {
        public void MockGetFollowerProfilesByProfileID(string profileID, ProfileType profileType, bool valid)
        {
            List<Profile> output = new List<Profile>();

            Setup(c => c.GetFollowerProfilesByProfileIDAsync(
                It.Is<string>(c => c == profileID),
                It.Is<ProfileType>(c => c == profileType)))
                .Returns<string, ProfileType>((pId, ptype) =>
                {
                    if (valid)
                    {
                        output = new List<Profile> { new Profile { Name = "Test" }, new Profile { Name = "Test" } };
                    }
                    return Task.FromResult(output);
                });

        }

        public void MockGetFollowingProfilesByProfileID(string profileID, ProfileType profileType, bool valid)
        {
            List<Profile> output = new List<Profile>();

            Setup(c => c.GetFollowerProfilesByProfileIDAsync(
                It.Is<string>(c => c == profileID),
                It.Is<ProfileType>(c => c == profileType)))
                .Returns<string, ProfileType>((pId, ptype) =>
                {
                    if (valid)
                    {
                        output = new List<Profile> { new Profile { Name = "Test" }, new Profile { Name = "Test" } };
                    }
                    return Task.FromResult(output);
                });

        }
    }
}
