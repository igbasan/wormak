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
    public class ProfileLogicTest
    {
        [Fact]
        public async Task GetProfilesAsync_null()
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            var mockAppTokenStore = new MockAppTokenStore();

            //Act
            var exception = await Record.ExceptionAsync(() => new ProfileLogic(mockHttpClient.Object, mockAppTokenStore.Object, string.Empty).GetProfilesAsync(null));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task GetProfilesAsync_EmptyDic()
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            var mockAppTokenStore = new MockAppTokenStore();

            //Act
            var exception = await Record.ExceptionAsync(() => new ProfileLogic(mockHttpClient.Object, mockAppTokenStore.Object, string.Empty).GetProfilesAsync(new Dictionary<string, ProfileType>()));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task GetProfilesAsync_Valid()
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            var mockAppTokenStore = new MockAppTokenStore();
            mockAppTokenStore.MockGetAppToken("AppToken");
            mockHttpClient.MockAddDefaultRequestHeaders();
            mockHttpClient.MockPostAsyncWithAuthHeader("IntApp AppToken");
            string profileUrl = "http://localhost:5002/api";

            var idTypePairs = new Dictionary<string, ProfileType> { { "5e20ad1651c7c1df345d41a1", ProfileType.Professional }, { "5e209dbe44dcb183fcecddcf", ProfileType.GeneralUser } };
            //Act
            List<Profile> profiles = await new ProfileLogic(mockHttpClient.Object, mockAppTokenStore.Object, profileUrl).GetProfilesAsync(idTypePairs);

            //Assert
            Assert.NotNull(profiles);
            Assert.True(profiles.Count == 2);
            Assert.True(profiles.All(v => idTypePairs.Keys.Contains(v.Id)));
            mockHttpClient.VerifyAll();
            mockAppTokenStore.VerifyAll();
        }

        [Fact]
        public async Task GetProfilesByInterestsAsync_null()
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            var mockAppTokenStore = new MockAppTokenStore();

            //Act
            var exception = await Record.ExceptionAsync(() => new ProfileLogic(mockHttpClient.Object, mockAppTokenStore.Object, string.Empty).GetProfilesByInterestsAsync(null));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task GetProfilesByInterestsAsync_EmptyList()
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            var mockAppTokenStore = new MockAppTokenStore();

            //Act
            var exception = await Record.ExceptionAsync(() => new ProfileLogic(mockHttpClient.Object, mockAppTokenStore.Object, string.Empty).GetProfilesByInterestsAsync(new List<Interest>()));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task GetProfilesByInterestsAsync_Valid()
        {
            //Arrange
            var mockHttpClient = new MockHttpClient();
            var mockAppTokenStore = new MockAppTokenStore();
            mockAppTokenStore.MockGetAppToken("AppToken");
            mockHttpClient.MockAddDefaultRequestHeaders();
            mockHttpClient.MockPostAsyncWithAuthHeader("IntApp AppToken");
            string profileUrl = "http://localhost:5002/api";

            var interestList = new List<Interest> { Interest.Art, Interest.Science, Interest.Business, Interest.Pleasure };
            //Act
            List<Profile> profiles = await new ProfileLogic(mockHttpClient.Object, mockAppTokenStore.Object, profileUrl).GetProfilesByInterestsAsync(interestList);

            //Assert
            Assert.NotNull(profiles);
            Assert.True(profiles.Count == 5);
            Assert.True(profiles.All(v => v.Interests?.Any(b => interestList.Contains(b)) ?? false));
            mockHttpClient.VerifyAll();
            mockAppTokenStore.VerifyAll();
        }
    }
}
