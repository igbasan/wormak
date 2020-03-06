using ProfileService.Logic.Implementations;
using ProfileService.Models;
using ProfileService.Models.Exceptions;
using ProfileService.Models.Implementations;
using ProfileService.Test.Mocks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProfileService.Test.Tests
{
    public class GeneralUserLogicTest : ProfileLogicTest<Company>
    {
        [Fact]
        public async Task SetupProfileAsync_NullTest()
        {
            await SetupProfileAsync_Null();
        }

        [Theory]
        [InlineData("KnownUserID")] //When user ID already has a general user set
        [InlineData("UserID")] //When user ID does not have a general user set
        public async void SetupProfileAsync_NotNullTest(string userID)
        {
            await SetupProfileAsync_NotNull_UniqueProfile(userID);
        }

        [Fact]
        public async void UpdateProfileAsync_NullTest()
        {
            await UpdateProfileAsync_Null();
        }

        [Theory]
        [InlineData("KnownUserID")]
        [InlineData("UserID")]
        public async void UpdateProfileAsync_NotNullTest(string userID)
        {
            await UpdateProfileAsync_NotNull_ValidateUserID(userID);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void GetProfileByIDAsync_NullOrEmptyTest(string id)
        {
            await GetProfileByIDAsync_NullOrEmpty(id);
        }


        [Theory]
        [InlineData("KnownId")]
        [InlineData("UnknownId")]
        public async void GetProfileByIDAsync_NotNullTest(string id)
        {
            await GetProfileByIDAsync_NotNull(id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async void GetAllProfilesAsync_NullOrEmptyTest(string userID)
        {
            await GetAllProfilesAsync_NullOrEmpty(userID);
        }


        [Theory]
        [InlineData("KnownUserId")]
        [InlineData("UnknownUserId")]
        public async void GetAllProfilesAsync_NotNullTest(string userID)
        {
            await GetAllProfilesAsync_NotNull(userID);
        }

        public override async Task UpdateProfileAsync_NotNull_ValidateUserID(string userID)
        {
            //Arrange
            GeneralUser profile = new GeneralUser
            {
                Address = "Address",
                City = "City",
                Country = "Country",
                Name = "Name",
                State = "State",
                UserId = userID,
                Id = "id"
            };
            string knownUserID = "KnownUserID"; //existing user ID with the database record
            var mockgeneralUserDAO = new MockGeneralUserDAO();
            mockgeneralUserDAO.MockUpdateProfile(profile);
            mockgeneralUserDAO.MockGetProfileByID(knownUserID);

            if (knownUserID == userID)
            {
                //Act
                GeneralUser result = await new GeneralUserLogic(mockgeneralUserDAO.Object).UpdateProfileAsync(profile);

                //Assert
                Assert.Equal(result, profile);
                mockgeneralUserDAO.VerifyAll();
            }
            else
            {
                //Act
                var exception = await Record.ExceptionAsync(() => new GeneralUserLogic(mockgeneralUserDAO.Object).UpdateProfileAsync(profile));

                //Assert
                Assert.IsType<ProfileServiceException>(exception);
            }
        }

        public virtual async Task SetupProfileAsync_NotNull_UniqueProfile(string userID)
        {
            //Arrange
            GeneralUser profile = new GeneralUser
            {
                Address = "Address",
                City = "City",
                Country = "Country",
                Name = "Name",
                State = "State",
                UserId = userID
            };
            string knownUserID = "KnownUserID";
            var mockgeneralUserDAO = new MockGeneralUserDAO();
            mockgeneralUserDAO.MockSaveProfile(profile);
            mockgeneralUserDAO.MockGetProfileByUserIDAsync(userID, knownUserID);

            if (knownUserID != userID)
            {
                //Act
                GeneralUser result = await new GeneralUserLogic(mockgeneralUserDAO.Object).SetupProfileAsync(profile);

                //Assert
                Assert.Equal(result, profile);
                mockgeneralUserDAO.VerifyAll();
            }
            else
            {
                //Act
                var exception = await Record.ExceptionAsync(() => new GeneralUserLogic(mockgeneralUserDAO.Object).SetupProfileAsync(profile));

                //Assert
                Assert.IsType<ProfileServiceException>(exception);
            }
        }

        [Fact]
        public override async Task UpdateProfileAsync_FillOldInterests()
        {
            //Arrange
            string userID = "KnownUserID";
            GeneralUser profile = new GeneralUser
            {
                Address = "Address",
                City = "City",
                Country = "Country",
                Name = "Name",
                State = "State",
                UserId = userID,
                Id = "id"
            };
            string knownUserID = "KnownUserID"; //existing user ID with the database record
            List<Interest> oldInterests = new List<Interest> { Interest.Art, Interest.Pleasure };
            var mockgeneralUserDAO = new MockGeneralUserDAO();
            mockgeneralUserDAO.MockUpdateProfile(profile);
            mockgeneralUserDAO.MockGetProfileByID(knownUserID, oldInterests);

            //Act
            GeneralUser result = await new ProfileLogic<GeneralUser>(mockgeneralUserDAO.Object).UpdateProfileAsync(profile);

            //Assert
            Assert.Equal(result, profile);
            Assert.Equal(result.InterestsBeforeUpdate, oldInterests);
            mockgeneralUserDAO.VerifyAll();
        }

        [Fact]
        public async void GetAllProfilesByIDsAsync_NullTest()
        {
            await GetAllProfilesByIDsAsync_Null();
        }

        [Theory]
        [InlineData("UnknownUserId", "UnknownUserId2")]
        [InlineData("KnownUserId1","UnknownUserId")]
        [InlineData("KnownUserId1", "KnownUserId2")]
        public async void GetAllProfilesByIDsAsync_NotNullTest(string item1, string item2)
        {
            //Arrange
            List<string> ids = new List<string>() { item1, item2 };
            await GetAllProfilesByIDsAsync_NotNull(ids);
        }
        [Fact]
        public async void GetAllProfilesByInterestsAsync_NullTest()
        {
            await GetAllProfilesByInterestsAsync_Null();
        }

        [Theory]
        [InlineData(Interest.Art, Interest.Business)]
        [InlineData(Interest.Art, Interest.Pleasure)]
        [InlineData(Interest.Business, Interest.Science)]
        public async void GetAllProfilesByInterestsAsync_NotNullTest(Interest item1, Interest item2)
        {
            //Arrange
            List<Interest> interests = new List<Interest>() { item1, item2 };
            await GetAllProfilesByInterestsAsync_NotNull(interests);
        }
    }

}
