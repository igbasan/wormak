using Moq;
using AuthenticationService.WebAPI.Logic.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Test.Mocks
{
    public class MockUserLogic : Mock<IUserLogic>
    {
        public bool AccountCreated { get; set; }

        public void MockCreateUser(User user)
        {
            Setup(x => x.CreateUserAsync(
                It.Is<User>(c => c.Email == user.Email)
                )).Callback(() => AccountCreated = true)
                .Returns<User>(s => Task.FromResult<User>(s));
        }
        public void MockGetUserByEmail(string email, string knownEmail, User user)
        {
            User output = null;
            if (!string.IsNullOrWhiteSpace(email) && email == knownEmail)
            {
                output = user;
            }

            Setup(x => x.GetUserByEmailAsync(
                It.Is<string>(c => c == email)
                )).Returns(Task.FromResult<User>(output));
        }
    }
}
