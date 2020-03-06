using Moq;
using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Test.Mocks
{
    public class MockUserDAO : Mock<IUserDAO>
    {
        public void MockCreateUser(User user)
        {
            Setup(x => x.CreateUserAsync(
                It.Is<User>(c => c == user)
                ))
                .Returns<User>(s => Task.FromResult<User>(s));
        }
        public void MockGetUserByEmail(string email, string knownEmail)
        {
            User output = null;
            if (!string.IsNullOrWhiteSpace(email) && email == knownEmail)
            {
                output = new User
                {
                    Email = email
                };
            }

            Setup(x => x.GetUserByEmailAsync(
                It.Is<string>(c => c == email)
                )).Returns(Task.FromResult<User>(output));
        }

        public void MockGetUserByID(string id)
        {
            User output = new User
            {
                Id = id
            };

            Setup(x => x.GetUserByIDAsync(
                It.Is<string>(c => c == id)
                )).Returns(Task.FromResult<User>(output));
        }
    }
}
