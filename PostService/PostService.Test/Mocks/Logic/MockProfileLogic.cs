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
    public class MockProfileLogic : Mock<IProfileLogic>
    {
        public void MockGetProfiles(bool valid)
        {
            List<Profile> output = new List<Profile>();

            Setup(c => c.GetProfilesAsync(
                It.IsAny<Dictionary<string, ProfileType>>()))
                .Returns<Dictionary<string, ProfileType>>(dict =>
                {
                    if (valid && dict != null)
                    {
                        output = dict.Select(v => new Profile { Id = v.Key, ProfileType = v.Value, Name = "Name" }).ToList();
                    }
                    return Task.FromResult(output);
                });

        }
        public void MockGetProfilesByInterests(bool valid)
        {
            List<Profile> output = new List<Profile>();

            Setup(c => c.GetProfilesByInterestsAsync(
                It.IsAny<List<Interest>>()))
                .Returns<List<Interest>>(list =>
                {
                    if (valid && list != null)
                    {
                        output = list.Select(v => new Profile { Interests = new List<Interest> { v } }).ToList();
                    }
                    return Task.FromResult(output);
                });

        }
    }
}
