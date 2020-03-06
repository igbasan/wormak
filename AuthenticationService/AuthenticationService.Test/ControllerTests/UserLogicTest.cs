using Moq;
using AuthenticationService.Test.Mocks;
using AuthenticationService.WebAPI.Logic.Implementations;
using AuthenticationService.WebAPI.Models.Implementations;
using AuthenticationService.WebAPI.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticationService.Test.ControllerTests
{
    public class UserLogicTest
    {
        [Fact]
        public async void CreateUser_Null()
        {
            //Arrange
            var mockUserDAO = new MockUserDAO();
            User user = null;

            mockUserDAO.MockCreateUser(user);

            //Act
            var exception = await Record.ExceptionAsync(() => new UserLogic(mockUserDAO.Object).CreateUserAsync(user));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async void CreateUser_NotNull()
        {
            //Arrange
            User user = new User { Email = "theyknow@gmail.com", FirstName = "We", LastName = "Test", UserName = "Things" };
            var mockUserDAO = new MockUserDAO();
            mockUserDAO.MockCreateUser(user);

            //Act
            User output = await new UserLogic(mockUserDAO.Object).CreateUserAsync(user);

            //Assert
            Assert.NotNull(output);
            Assert.Equal(user, output);
            mockUserDAO.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async void GetUserByEmail_NullOrEmpty(string email)
        {
            //Arrange
            string knownEmail = "theyknow@gmail.com";
            var mockUserDAO = new MockUserDAO();

            mockUserDAO.MockGetUserByEmail(email, knownEmail);

            //Act
            var exception = await Record.ExceptionAsync(() =>
             new UserLogic(mockUserDAO.Object).GetUserByEmailAsync(email));

            //Assert

            Assert.IsType<ArgumentNullException>(exception);
            return;
        }

        [Theory]
        [InlineData("theyknow@gmail.com")]
        [InlineData("unkonwn@gmail.com")]
        public async void GetUserByEmail_KnownEmail(string email)
        {
            //Arrange
            string knownEmail = "theyknow@gmail.com";
            var mockUserDAO = new MockUserDAO();

            mockUserDAO.MockGetUserByEmail(email, knownEmail);

            //Act
            User user = await new UserLogic(mockUserDAO.Object).GetUserByEmailAsync(email);

            //Assert
            if (email != knownEmail)
            {
                Assert.Null(user);
                return;
            }

            Assert.NotNull(user);
            Assert.NotNull(user.Email);
            Assert.Equal(user.Email, email);
            mockUserDAO.VerifyAll();
        }
    }
}