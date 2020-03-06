using Moq;
using ProfileService.Logic.Interfaces;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProfileService.Test.Mocks
{
    public class MockTokenValidator : Mock<ITokenValidator>
    {
        public void MockValidateTokenAsync(string token, string knownToken)
        {
            User output = null;
            if (!string.IsNullOrWhiteSpace(token) && token == knownToken)
            {
                output = new User()
                {
                    Email = "Email",
                    FirstName = "FirstName",
                    Id = "Id",
                    LastName = "LastName"
                };
            }

            Setup(x => x.ValidateTokenAsync(
                It.Is<string>(c => c == token)
                )).Returns(Task.FromResult<User>(output));
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
