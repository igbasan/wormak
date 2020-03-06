using InterUserService.Logic.Interfaces;
using InterUserService.Models;
using InterUserService.Models.Implemetations;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterUserService.Test.Mocks.Logic
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
                        output = dict.Select(v => new Profile { Id = v.Key, ProfileType = v.Value, Name ="Name" }).ToList();
                    }
                    return Task.FromResult(output);
                });

        }
    }
}
