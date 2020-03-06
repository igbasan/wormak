using InterUserService.Logic.Implementation;
using InterUserService.Models;
using InterUserService.Models.Exceptions;
using InterUserService.Models.Implemetations;
using InterUserService.Test.Mocks.Data;
using InterUserService.Test.Mocks.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InterUserService.Test.Tests.Logic
{
    public class InterUserLogicGenericTest<T> where T : InterUser, new()
    {
        public async Task SetupProfileAsync_Null()
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            //Act
            var exception = await Record.ExceptionAsync(() => new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).SetUpRelationshipAsync(null, false));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        public async Task SetupProfileAsync_invalidProfileId()
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            string profileId = "InvalidProfileID";
            ProfileType profileType = ProfileType.Company;
            mockProfileLogic.MockGetProfiles(false);

            //Act
            var exception = await Record.ExceptionAsync(() => new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).SetUpRelationshipAsync(new T { PassiveProfileID = profileId, PassiveProfileType = profileType }));

            //Assert
            Assert.IsType<InterUserException>(exception);

            mockProfileLogic.VerifyAll();
        }

        public async Task SetupProfileAsync_SameProfile()
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            string profileId = "profileID";
            ProfileType profileType = ProfileType.Company;

            T interuser = new T
            {
                ActiveProfileID = profileId,
                ActiveProfileType = profileType,
                PassiveProfileID = profileId,
                PassiveProfileType = profileType
            };
            //Act
            var exception = await Record.ExceptionAsync(() => new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).SetUpRelationshipAsync(interuser));

            //Assert
            Assert.IsType<InterUserException>(exception);
        }

        public async Task SetupProfileAsync_ValidProfileId(string activeProfileId, string passiveProfileId)
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            string KnownActiveProfileID = "KnownActiveProfileID";
            string KnownPassiveProfileID = "KnownPassiveProfileID";
            ProfileType profileType = ProfileType.Company;

            T interuser = new T
            {
                ActiveProfileID = activeProfileId,
                ActiveProfileType = profileType,
                PassiveProfileID = passiveProfileId,
                PassiveProfileType = profileType
            };

            mockProfileLogic.MockGetProfiles(true); //valid profiles
            //determine if the profile is available or not
            mockInterUserDAO.MockGetByActiveProfileIDandPassiveProfileID($"{profileType}_{activeProfileId}", $"{profileType}_{KnownActiveProfileID}", $"{profileType}_{passiveProfileId}", $"{profileType}_{KnownPassiveProfileID}");

            if (activeProfileId != KnownActiveProfileID || passiveProfileId != KnownPassiveProfileID)
            {
                mockInterUserDAO.MockCreate();
            }

            //Act
            T interUser = await new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).SetUpRelationshipAsync(interuser);

            //Assert
            if (activeProfileId != KnownActiveProfileID || passiveProfileId != KnownPassiveProfileID)
            {
                Assert.True(mockInterUserDAO.InterUserCreated);
            }
            else
            {
                Assert.False(mockInterUserDAO.InterUserCreated);
            }
            Assert.False(mockInterUserDAO.InterUserUpdated);
            Assert.NotNull(interUser);
            Assert.True(interUser.IsActive);
            mockProfileLogic.VerifyAll();
            mockInterUserDAO.VerifyAll();
        }

        public async Task SetupProfileAsync_ValidProfileId_ChangeActiveState(bool activate, bool currrentState)
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            string activeProfileId = "KnownActiveProfileID";
            string passiveProfileId = "KnownPassiveProfileID";
            ProfileType profileType = ProfileType.Company;

            T interuser = new T
            {
                ActiveProfileID = activeProfileId,
                ActiveProfileType = profileType,
                PassiveProfileID = passiveProfileId,
                PassiveProfileType = profileType
            };

            mockProfileLogic.MockGetProfiles(true); //valid profiles
            //determine if the profile is available or not
            mockInterUserDAO.MockGetByActiveProfileIDandPassiveProfileID($"{profileType}_{activeProfileId}", $"{profileType}_{activeProfileId}", $"{profileType}_{passiveProfileId}", $"{profileType}_{passiveProfileId}", currrentState);
            if (currrentState != activate)
            {
                mockInterUserDAO.MockUpdate();
            }

            //Act
            T interUser = await new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).SetUpRelationshipAsync(interuser, !activate);

            //Assert
            //must not be recreated since it already exists
            Assert.False(mockInterUserDAO.InterUserCreated);

            //Dont update for the same state
            if (currrentState == activate)
            {
                Assert.False(mockInterUserDAO.InterUserUpdated);
            }
            else
            {
                Assert.True(mockInterUserDAO.InterUserUpdated);
            }

            Assert.NotNull(interUser);
            if (activate)
            {
                Assert.True(interUser.IsActive);
            }
            else
            {
                Assert.False(interUser.IsActive);
            }
            mockProfileLogic.VerifyAll();
            mockInterUserDAO.VerifyAll();
        }



        public async Task GetByActiveProfileIDandPassiveProfileIDAsync_NullOrEmptyActiveID(string profileId)
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            //Act
            var exception = await Record.ExceptionAsync(() => new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).GetByActiveProfileIDandPassiveProfileIDAsync(profileId, ProfileType.Company, "ProfileID", ProfileType.Company));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }
        public async Task GetByActiveProfileIDandPassiveProfileIDAsync_NullOrEmptyPassiveID(string profileId)
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            //Act
            var exception = await Record.ExceptionAsync(() => new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).GetByActiveProfileIDandPassiveProfileIDAsync("ProfileID", ProfileType.Company, profileId, ProfileType.Company));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }


        public async Task GetByActiveProfileIDandPassiveProfileIDAsync_NotNull(string activeProfileId, string passiveProfileId, ProfileType profileType)
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            string KnownActiveProfileID = "KnownActiveProfileID";
            string KnownPassiveProfileID = "KnownPassiveProfileID";

            //determine if the profile is available or not
            mockInterUserDAO.MockGetByActiveProfileIDandPassiveProfileID($"{profileType}_{activeProfileId}", $"{profileType}_{KnownActiveProfileID}", $"{profileType}_{passiveProfileId}", $"{profileType}_{KnownPassiveProfileID}");

            //Act
            T interUser = await new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).GetByActiveProfileIDandPassiveProfileIDAsync(activeProfileId, profileType, passiveProfileId, profileType);

            //Assert
            if (activeProfileId != KnownActiveProfileID || passiveProfileId != KnownPassiveProfileID)
            {
                Assert.Null(interUser);
            }
            else
            {
                Assert.NotNull(interUser);
                Assert.Equal(interUser.ActiveProfileID, $"{profileType}_{activeProfileId}");
                Assert.Equal(interUser.PassiveProfileID, $"{profileType}_{passiveProfileId}");
            }

            mockInterUserDAO.VerifyAll();
        }

        public async Task GetAllByActiveProfileIDAsync_NullOrEmpty(string profileId)
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            //Act
            var exception = await Record.ExceptionAsync(() => new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).GetAllByActiveProfileIDAsync(profileId, ProfileType.Company, 0, 10));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        public async Task GetAllByActiveProfileIDAsync_NotNull(string profileId, ProfileType profileType)
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            string KnownProfileID = "KnownProfileID";
            mockInterUserDAO.MockGetAllByProfileID($"{profileType}_{profileId}", $"{profileType}_{KnownProfileID}", true);
            mockProfileLogic.MockGetProfiles(true);

            //Act
            CountList<Profile> interUsers = await new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).GetAllByActiveProfileIDAsync(profileId, profileType, 0, 10);

            //Assert
            if (KnownProfileID != profileId)
            {
                Assert.NotNull(interUsers);
                Assert.True(interUsers.Count == 0);
                Assert.True(interUsers.TotalCount == 0);
            }
            else
            {
                Assert.NotNull(interUsers);
                Assert.True(interUsers.Count == 2);
                Assert.True(interUsers.TotalCount == 2);
                Assert.True(interUsers.All(v => v.Id.StartsWith("Result")));
            }

            mockInterUserDAO.VerifyAll();
        }

        public async Task GetAllByPassiveProfileIDAsync_NullOrEmpty(string profileId)
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            //Act
            var exception = await Record.ExceptionAsync(() => new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).GetAllByPassiveProfileIDAsync(profileId, ProfileType.Company, 0, 10));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        public async Task GetAllByPassiveProfileIDAsync_NotNull(string profileId, ProfileType profileType)
        {
            //Arrange
            var mockInterUserDAO = new MockInterUserDAO<T>();
            var mockProfileLogic = new MockProfileLogic();

            string KnownProfileID = "KnownProfileID";
            mockInterUserDAO.MockGetAllByProfileID($"{profileType}_{profileId}", $"{profileType}_{KnownProfileID}", false);
            mockProfileLogic.MockGetProfiles(true);

            //Act
            CountList<Profile> interUsers = await new InterUserLogic<T>(mockInterUserDAO.Object, mockProfileLogic.Object).GetAllByPassiveProfileIDAsync(profileId, profileType, 0, 10);

            //Assert
            if (KnownProfileID != profileId)
            {
                Assert.NotNull(interUsers);
                Assert.True(interUsers.Count == 0);
                Assert.True(interUsers.TotalCount == 0);
            }
            else
            {
                Assert.NotNull(interUsers);
                Assert.True(interUsers.Count == 2);
                Assert.True(interUsers.TotalCount == 2);
                Assert.True(interUsers.All(v => v.Id.StartsWith("Result")));
            }

            mockInterUserDAO.VerifyAll();
        }
    }
}
