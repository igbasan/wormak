using ProfileService.Models;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProfileService.Test.Tests
{
    public class CompanyLogicTest : ProfileLogicTest<Company>
    {
        [Fact]
        public async Task SetupProfileAsync_NullTest()
        {
            await SetupProfileAsync_Null();
        }


        [Theory]
        [InlineData("KnownUserID", "knownName")] //When user already has a company with the name
        [InlineData("KnownUserID", "Name")] //When user does not have a company with the name
        [InlineData("UserID", "knownName")] 
        [InlineData("UserID", "Name")] 
        public async void SetupProfileAsync_NotNullTest(string userID, string name)
        {
            await SetupProfileAsync_NotNull_UniqueProfileName(userID, name);
        }

        [Fact]
        public async void UpdateProfileAsync_NullTest()
        {
            await UpdateProfileAsync_Null();
        }

        [Fact]
        public async void UpdateProfileAsync_FillOldInterestsTest()
        {
            await UpdateProfileAsync_FillOldInterests();
        }


        [Theory]
        [InlineData("KnownUserID")]
        [InlineData("UserID")]
        public async void UpdateProfileAsync_NotNull_ValidateUserIDTest(string userID)
        {
            await UpdateProfileAsync_NotNull_ValidateUserID(userID);
        }

        [Theory]
        [InlineData("KnownUserID", "knownName", "AnotherId")] //When user already has a company with the name asides the record to update
        [InlineData("KnownUserID", "Name", "AnotherId")] //every other state
        [InlineData("UserID", "knownName", "AnotherId")]
        [InlineData("UserID", "Name", "AnotherId")]
        [InlineData("KnownUserID", "knownName", "Id")] 
        [InlineData("KnownUserID", "Name", "Id")] 
        [InlineData("UserID", "knownName", "Id")]
        [InlineData("UserID", "Name","Id")]
        public async void UpdateProfileAsync_NotNull_UniqueProfileNameTest(string userID, string name, string nameMatchId)
        {
            await UpdateProfileAsync_NotNull_UniqueProfileName(userID, name, nameMatchId);
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

        [Fact]
        public async void GetAllProfilesByIDsAsync_NullTest()
        {
            await GetAllProfilesByIDsAsync_Null();
        }

        [Theory]
        [InlineData("UnknownUserId", "UnknownUserId2")]
        [InlineData("KnownUserId1", "UnknownUserId")]
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
