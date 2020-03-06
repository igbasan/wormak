using PostService.Logic.Implementations;
using PostService.Models;
using PostService.Models.Implementations;
using PostService.Test.Mocks.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PostService.Test.Tests.Logic
{
    public class FollowerLogicTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetFollowerProfilesByProfileIDAsync_NullOrEmptyProfileId(string profileId)
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            var mockAppTokenStore = new MockAppTokenStore();

            //Act
            var exception = await Record.ExceptionAsync(() => new FollowerLogic(mockHttpClient.Object, mockAppTokenStore.Object, string.Empty)
            .GetFollowerProfilesByProfileIDAsync(profileId, ProfileType.Company));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task GetFollowerProfilesByProfileIDAsync_Valid()
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            var mockAppTokenStore = new MockAppTokenStore();
            mockAppTokenStore.MockGetAppToken("AppToken");
            mockHttpClient.MockAddDefaultRequestHeaders();
            mockHttpClient.MockGetAsyncWithAuthHeader("IntApp AppToken");
            string followerUrl = "http://localhost:5004/api";

            //Act
            List<Profile> profiles = await new FollowerLogic(mockHttpClient.Object, mockAppTokenStore.Object, followerUrl)
                .GetFollowerProfilesByProfileIDAsync("5e20ad1651c7c1df345d41a1", ProfileType.Professional);

            //Assert
            Assert.NotNull(profiles);
            Assert.True(profiles.Count == 1);
            Assert.True(profiles.All(v => v.Name == "My FirstName My LastName"));
            mockHttpClient.VerifyAll();
            mockAppTokenStore.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetFollowingProfilesByProfileIDAsync_NullOrEmptyProfileId(string profileId)
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            var mockAppTokenStore = new MockAppTokenStore();

            //Act
            var exception = await Record.ExceptionAsync(() => new FollowerLogic(mockHttpClient.Object, mockAppTokenStore.Object, string.Empty)
            .GetFollowingProfilesByProfileIDAsync(profileId, ProfileType.Company));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task GetFollowingProfilesByProfileIDAsync_Valid()
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            var mockAppTokenStore = new MockAppTokenStore();
            mockAppTokenStore.MockGetAppToken("AppToken");
            mockHttpClient.MockAddDefaultRequestHeaders();
            mockHttpClient.MockGetAsyncWithAuthHeader("IntApp AppToken");
            string followerUrl = "http://localhost:5004/api";

            //Act
            List<Profile> profiles = await new FollowerLogic(mockHttpClient.Object, mockAppTokenStore.Object, followerUrl)
                .GetFollowingProfilesByProfileIDAsync("5e209dbe44dcb183fcecddcf", ProfileType.GeneralUser);

            //Assert
            Assert.NotNull(profiles);
            Assert.True(profiles.Count == 1);
            Assert.True(profiles.All(v => v.Name == "name"));
            mockHttpClient.VerifyAll();
            mockAppTokenStore.VerifyAll();
        }
    }
}
