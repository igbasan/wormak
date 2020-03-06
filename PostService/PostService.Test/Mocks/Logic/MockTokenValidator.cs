using Moq;
using PostService.Logic.Interfaces;
using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Test.Mocks.Logic
{
    public class MockTokenValidator : Mock<ITokenValidator>
    {
        public void MockValidateTokenAsync(string token, string knownToken)
        {
            Profile output = null;
            if (!string.IsNullOrWhiteSpace(token) && token == knownToken)
            {
                output = new Profile()
                {
                    Name = "Name",
                    Id = "Id",
                    ProfileType = Models.ProfileType.Company
                };
            }

            Setup(x => x.ValidateTokenAsync(
                It.Is<string>(c => c == token)
                )).Returns(Task.FromResult<Profile>(output));
        }

        public void MockValidateAppTokenAsync(string token, string knownToken)
        {
            string output = null;
            if (!string.IsNullOrWhiteSpace(token) && token == knownToken)
            {
                output = "TestApp";
            }

            Setup(x => x.ValidateAppTokenAsync(
                It.Is<string>(c => c == token)
                )).Returns(Task.FromResult(output));
        }
    }
}
