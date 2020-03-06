using ProfileService.Logic.Implementations;
using ProfileService.Models;
using ProfileService.Models.Exceptions;
using ProfileService.Models.Implementations;
using ProfileService.Test.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProfileService.Test.Tests
{
    public class ProfileLogicTest<T> where T : Profile, new()
    {

        public async Task SetupProfileAsync_Null()
        {
            //Arrange
            var mockProfileDAO = new MockProfileDAO<T>();
            T profile = null;

            //Act
            var exception = await Record.ExceptionAsync(() => new ProfileLogic<T>(mockProfileDAO.Object).SetupProfileAsync(profile));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }


        public async Task SetupProfileAsync_NotNull_UniqueProfileName(string userID, string name)
        {
            //Arrange
            T profile = new T
            {
                Address = "Address",
                City = "City",
                Country = "Country",
                Name = name,
                State = "State",
                UserId = userID
            };
            string knownUserID = "KnownUserID";
            string knownName = "knownName";
            var mockProfileDAO = new MockProfileDAO<T>();
            mockProfileDAO.MockSaveProfile(profile);
            mockProfileDAO.MockGetProfileByUserIDAndNameAsync(userID, knownUserID, name, knownName);

            if (knownUserID != userID || knownName != name)
            {
                //Act
                T result = await new ProfileLogic<T>(mockProfileDAO.Object).SetupProfileAsync(profile);

                //Assert        
                Assert.Equal(result, profile);
                mockProfileDAO.VerifyAll();
            }
            else
            {
                //Act
                var exception = await Record.ExceptionAsync(() => new ProfileLogic<T>(mockProfileDAO.Object).SetupProfileAsync(profile));

                //Assert
                Assert.IsType<ProfileServiceException>(exception);
            }
        }

        public async Task UpdateProfileAsync_Null()
        {
            //Arrange
            var mockProfileDAO = new MockProfileDAO<T>();
            T profile = null;

            //Act
            var exception = await Record.ExceptionAsync(() => new ProfileLogic<T>(mockProfileDAO.Object).UpdateProfileAsync(profile));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }
        public virtual async Task UpdateProfileAsync_FillOldInterests()
        {
            //Arrange
            string userID = "KnownUserID";
            T profile = new T
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
            var mockProfileDAO = new MockProfileDAO<T>();
            mockProfileDAO.MockUpdateProfile(profile);
            mockProfileDAO.MockGetProfileByID(knownUserID, oldInterests);
            mockProfileDAO.MockGetProfileByUserIDAndNameAsync(userID, userID, profile.Name, profile.Name, profile.Id);

            //Act
            T result = await new ProfileLogic<T>(mockProfileDAO.Object).UpdateProfileAsync(profile);

            //Assert
            Assert.Same(result, profile);
            Assert.Equal(result.InterestsBeforeUpdate, oldInterests);
            mockProfileDAO.VerifyAll();
        }


        public virtual async Task UpdateProfileAsync_NotNull_ValidateUserID(string userID)
        {
            //Arrange
            T profile = new T
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
            var mockProfileDAO = new MockProfileDAO<T>();
            mockProfileDAO.MockUpdateProfile(profile);
            mockProfileDAO.MockGetProfileByID(knownUserID);
            mockProfileDAO.MockGetProfileByUserIDAndNameAsync(userID, userID, profile.Name, profile.Name, profile.Id);

            if (knownUserID == userID)
            {
                //Act
                T result = await new ProfileLogic<T>(mockProfileDAO.Object).UpdateProfileAsync(profile);

                //Assert
                Assert.Equal(result, profile);
                mockProfileDAO.VerifyAll();
            }
            else
            {
                //Act
                var exception = await Record.ExceptionAsync(() => new ProfileLogic<T>(mockProfileDAO.Object).UpdateProfileAsync(profile));

                //Assert
                Assert.IsType<ProfileServiceException>(exception);
            }
        }


        public async Task UpdateProfileAsync_NotNull_UniqueProfileName(string userID, string name, string nameMatchId)
        {
            //Arrange
            string knownUserID = "KnownUserID";
            string knownName = "knownName";
            string profileObjectId = "Id";
            T profile = new T
            {
                Address = "Address",
                City = "City",
                Country = "Country",
                Name = name,
                State = "State",
                UserId = userID,
                Id = profileObjectId
            };

            var mockProfileDAO = new MockProfileDAO<T>();
            mockProfileDAO.MockUpdateProfile(profile);
            mockProfileDAO.MockGetProfileByID(userID);
            mockProfileDAO.MockGetProfileByUserIDAndNameAsync(userID, knownUserID, name, knownName, nameMatchId);

            //would only pass when the names don't match or we want to save the already name matching object
            if (knownUserID == userID && knownName == name && profileObjectId != nameMatchId)
            {
                //Act
                var exception = await Record.ExceptionAsync(() => new ProfileLogic<T>(mockProfileDAO.Object).UpdateProfileAsync(profile));

                //Assert
                Assert.IsType<ProfileServiceException>(exception);
            }
            else
            {
                //Act
                T result = await new ProfileLogic<T>(mockProfileDAO.Object).UpdateProfileAsync(profile);

                //Assert
                Assert.Equal(result, profile);
                mockProfileDAO.VerifyAll();
            }
        }


        public async Task GetProfileByIDAsync_NullOrEmpty(string id)
        {
            //Arrange
            var mockProfileDAO = new MockProfileDAO<T>();

            //Act
            var exception = await Record.ExceptionAsync(() => new ProfileLogic<T>(mockProfileDAO.Object).GetProfileByIDAsync(id));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }


        public async Task GetProfileByIDAsync_NotNull(string id)
        {
            //Arrange
            var mockProfileDAO = new MockProfileDAO<T>();
            string knownId = "KnownId";
            mockProfileDAO.MockGetProfileByID(id, knownId);

            //Act
            var profile = await new ProfileLogic<T>(mockProfileDAO.Object).GetProfileByIDAsync(id);

            //Assert
            if (knownId != id) Assert.Null(profile);
            else
            {
                Assert.NotNull(profile);
                Assert.Equal(profile.Id, id);
            }

            mockProfileDAO.VerifyAll();
        }

        public async Task GetAllProfilesAsync_NullOrEmpty(string userID)
        {
            //Arrange
            var mockProfileDAO = new MockProfileDAO<T>();

            //Act
            var exception = await Record.ExceptionAsync(() => new ProfileLogic<T>(mockProfileDAO.Object).GetAllProfilesAsync(userID));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        public async Task GetAllProfilesAsync_NotNull(string userID)
        {
            //Arrange
            var mockProfileDAO = new MockProfileDAO<T>();
            string knownId = "KnownUserId";
            mockProfileDAO.MockGetAllProfilesAsync(userID, knownId);

            //Act
            List<T> profiles = await new ProfileLogic<T>(mockProfileDAO.Object).GetAllProfilesAsync(userID);

            //Assert
            if (knownId != userID)
            {
                Assert.NotNull(profiles);
                Assert.True(profiles.Count == 0);
            }
            else
            {
                Assert.NotNull(profiles);
                Assert.True(profiles.Count == 2);
                Assert.True(profiles.All(v => v.UserId == userID));
            }

            mockProfileDAO.VerifyAll();
        }
        public async Task GetAllProfilesByIDsAsync_Null()
        {
            //Arrange
            var mockProfileDAO = new MockProfileDAO<T>();

            //Act
            var exception = await Record.ExceptionAsync(() => new ProfileLogic<T>(mockProfileDAO.Object).GetAllProfilesByIDsAsync(null));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        public async Task GetAllProfilesByIDsAsync_NotNull(List<string> ids)
        {
            //Arrange
            var mockProfileDAO = new MockProfileDAO<T>();
            List<string> knownIds = new List<string> { "KnownUserId1", "KnownUserId2" };
            mockProfileDAO.MockGetAllProfilesByProfileIDs(ids, knownIds);

            //Act
            List<T> profiles = await new ProfileLogic<T>(mockProfileDAO.Object).GetAllProfilesByIDsAsync(ids);

            //Assert
            Assert.NotNull(profiles);
            var mutualIds = ids.Where(x => knownIds.Contains(x)).ToList();

            Assert.True(profiles.Count == mutualIds.Count);
            Assert.True(profiles.All(v => mutualIds.Contains(v.Id)));

            mockProfileDAO.VerifyAll();
        }

        public async Task GetAllProfilesByInterestsAsync_Null()
        {
            //Arrange
            var mockProfileDAO = new MockProfileDAO<T>();

            //Act
            var exception = await Record.ExceptionAsync(() => new ProfileLogic<T>(mockProfileDAO.Object).GetAllProfilesByInterestsAsync(null));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        public async Task GetAllProfilesByInterestsAsync_NotNull(List<Interest> interests)
        {
            //Arrange
            var mockProfileDAO = new MockProfileDAO<T>();
            List<Interest> knownInterests = new List<Interest> { Interest.Art, Interest.Pleasure };
            mockProfileDAO.MockGetAllProfilesByInterests(interests, knownInterests);

            //Act
            List<T> profiles = await new ProfileLogic<T>(mockProfileDAO.Object).GetAllProfilesByInterestsAsync(interests);

            //Assert
            Assert.NotNull(profiles);
            var mutualInterests = interests.Where(x => knownInterests.Contains(x)).ToList();

            Assert.True(profiles.Count == mutualInterests.Count);
            Assert.True(profiles.All(v => mutualInterests.Contains(v.Interests.First())));

            mockProfileDAO.VerifyAll();
        }

    }
}
