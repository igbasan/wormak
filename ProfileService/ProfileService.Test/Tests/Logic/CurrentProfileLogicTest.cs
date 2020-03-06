using ProfileService.Logic.Implementations;
using ProfileService.Models;
using ProfileService.Models.Exceptions;
using ProfileService.Models.Implementations;
using ProfileService.Test.Mocks;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ProfileService.Test.Tests
{
    public class CurrentProfileLogicTest
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "profileID")]
        [InlineData("userID", null)]
        public async Task SetCurrentProfileAsync_Null(string userID, string profileID)
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileDAO = new MockCurrentProfileDAO();

            //Act
            var exception = await Record.ExceptionAsync(() => new CurrentProfileLogic(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileDAO.Object)
            .SetCurrentProfileAsync(userID, profileID, Models.ProfileType.Company));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }


        [Theory]
        [InlineData("KnownUserID")]
        [InlineData("UserID")]
        public async Task SetCurrentProfileAsync_ValidateUserID(string userID)
        {
            //Arrange
            string knownUserID = "KnownUserID"; //existing user ID with the database record
            string knownProfileID = "knownProfileID"; //existing company profile ID with the database record
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileDAO = new MockCurrentProfileDAO();

            //the company user id is set to the known user Id, only this user Id should be allowed to process, the other should throw an exception
            mockCompanyLogic.MockGetProfileByID(knownProfileID, knownProfileID, new Company { Id = knownProfileID, UserId = knownUserID });
            mockCurrentProfileDAO.MockGetCurrentProfile(userID, userID);

            if (knownUserID == userID)
            {
                //Act
                CurrentProfile result = await new CurrentProfileLogic(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileDAO.Object)
                    .SetCurrentProfileAsync(userID, knownProfileID, Models.ProfileType.Company);

                //Assert
                Assert.NotNull(result);
                Assert.Equal(result.ProfileID, knownProfileID);
                Assert.Equal(result.UserID, knownUserID);
                Assert.InRange(result.LastSetDate, DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(1));
                mockCompanyLogic.VerifyAll();
                mockCurrentProfileDAO.VerifyAll();
            }
            else
            {
                //Act
                var exception = await Record.ExceptionAsync(() => new CurrentProfileLogic(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileDAO.Object)
                    .SetCurrentProfileAsync(userID, knownProfileID, Models.ProfileType.Company));

                //Assert
                mockCompanyLogic.VerifyAll();
                Assert.IsType<ProfileServiceException>(exception);
            }
        }

        [Theory]
        [InlineData("knownProfileID", ProfileType.Company)]
        [InlineData("knownProfileID", ProfileType.GeneralUser)]
        [InlineData("knownProfileID", ProfileType.Professional)]
        [InlineData("UnknownProfileID", ProfileType.Company)]
        [InlineData("UnknownProfileID", ProfileType.GeneralUser)]
        [InlineData("UnknownProfileID", ProfileType.Professional)]
        public async Task SetCurrentProfileAsync_ValidateProfile(string profileID, ProfileType profileType)
        {
            //Arrange
            string knownUserID = "KnownUserID"; //existing user ID with the database record
            string knownProfileID = "knownProfileID"; //existing company profile ID with the database record
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileDAO = new MockCurrentProfileDAO();

            switch (profileType)
            {
                case ProfileType.Company:
                    mockCompanyLogic.MockGetProfileByID(profileID, knownProfileID, new Company { Id = knownProfileID, UserId = knownUserID });
                    break;

                case ProfileType.GeneralUser:
                    mockGeneralUserLogic.MockGetProfileByID(profileID, knownProfileID, new GeneralUser { Id = knownProfileID, UserId = knownUserID });
                    break;

                case ProfileType.Professional:
                    mockProfessionalLogic.MockGetProfileByID(profileID, knownProfileID, new Professional { Id = knownProfileID, UserId = knownUserID });
                    break;
            }
            mockCurrentProfileDAO.MockGetCurrentProfile(knownUserID, knownUserID);
            mockCurrentProfileDAO.MockUpdateProfile();

            if (knownProfileID == profileID)
            {
                //Act
                CurrentProfile result = await new CurrentProfileLogic(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileDAO.Object)
                    .SetCurrentProfileAsync(knownUserID, profileID, profileType);

                //Assert
                Assert.NotNull(result);
                Assert.Equal(result.ProfileID, knownProfileID);
                Assert.Equal(result.UserID, knownUserID);
                Assert.InRange(result.LastSetDate, DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(1));
                mockCurrentProfileDAO.VerifyAll();
            }
            else
            {
                //Act
                var exception = await Record.ExceptionAsync(() => new CurrentProfileLogic(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileDAO.Object)
                    .SetCurrentProfileAsync(knownUserID, profileID, profileType));

                //Assert
                Assert.IsType<ProfileServiceException>(exception);
            }

            switch (profileType)
            {
                case ProfileType.Company:
                    mockCompanyLogic.VerifyAll();
                    break;

                case ProfileType.GeneralUser:
                    mockGeneralUserLogic.VerifyAll();
                    break;

                case ProfileType.Professional:
                    mockProfessionalLogic.VerifyAll();
                    break;
            }
        }

        [Fact]
        public async Task SetCurrentProfileAsync_nonExistingReccord()
        {
            //Arrange
            string userId = "UserID";
            string knownUserId = "KnownUserID";
            string knownProfileID = "knownProfileID"; //existing company profile ID with the database record
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileDAO = new MockCurrentProfileDAO();

            mockCompanyLogic.MockGetProfileByID(knownProfileID, knownProfileID, new Company { Id = knownProfileID, UserId = userId });
            //already has an existing Current profile record
            mockCurrentProfileDAO.MockGetCurrentProfile(userId, knownUserId, null);
            mockCurrentProfileDAO.MockSaveProfile();

            //Act
            var currentProfile = await new CurrentProfileLogic(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileDAO.Object)
            .SetCurrentProfileAsync(userId, knownProfileID, ProfileType.Company);

            //Assert
            Assert.NotNull(currentProfile);
            Assert.Equal(currentProfile.UserID, userId);
            Assert.Equal(ProfileType.Company, currentProfile.ProfileType);
            Assert.InRange(currentProfile.LastSetDate, DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(1));
            mockGeneralUserLogic.VerifyAll();
            mockCurrentProfileDAO.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task GetCurrentProfileAsync_NullOrEmpty(string userId)
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileDAO = new MockCurrentProfileDAO();

            //Act
            var exception = await Record.ExceptionAsync(() => new CurrentProfileLogic(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileDAO.Object)
            .GetCurrentProfileAsync(userId));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task GetCurrentProfileAsync_nonExistingReccord()
        {
            //Arrange
            string userId = "UserID";
            string knownUserId = "KnownUserID"; //existing user ID with the database record
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileDAO = new MockCurrentProfileDAO();

            //get general user record
            mockGeneralUserLogic.MockGetAllProfiles(userId, userId);
            //already has an existing Current profile record
            mockCurrentProfileDAO.MockGetCurrentProfile(userId, knownUserId, null);
            //ProfileID is "ProfileID", because the mockGeneralUserLogic.MockGetAllProfiles sets that field
            mockCurrentProfileDAO.MockSaveProfile();

            //Act
            var currentProfile = await new CurrentProfileLogic(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileDAO.Object)
            .GetCurrentProfileAsync(userId);

            //Assert
            Assert.NotNull(currentProfile);
            Assert.Equal(currentProfile.UserID, userId);
            Assert.Equal(ProfileType.GeneralUser, currentProfile.ProfileType);
            Assert.InRange(currentProfile.LastSetDate, DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(1));
            mockGeneralUserLogic.VerifyAll();
            mockCurrentProfileDAO.VerifyAll();
        }

        [Fact]
        public async Task GetCurrentProfileAsync_GeneralUserNotSetUp()
        {
            //Arrange
            string userId = "UserID"; 
            string knownUserId = "KnownUserID"; //existing user ID with the database record
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileDAO = new MockCurrentProfileDAO();

            //the company user id is set to the known user Id, only this user Id should be allowed to process, the other should throw an exception
            mockGeneralUserLogic.MockGetAllProfiles(userId, knownUserId);
            mockCurrentProfileDAO.MockGetCurrentProfile(userId, knownUserId);

            //Act
            var exception = await Record.ExceptionAsync(() => new CurrentProfileLogic(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileDAO.Object)
            .GetCurrentProfileAsync(userId));

            //Assert
            Assert.IsType<ProfileServiceException>(exception);

            mockGeneralUserLogic.VerifyAll();
            mockCurrentProfileDAO.VerifyAll();
        }

        [Theory]
        [InlineData("UserID")]
        [InlineData("KnownUserID")]
        public async Task GetCurrentProfileAsync_ValidateUserID(string userId)
        {
            //Arrange
            string knownUserId = "KnownUserID"; //existing user ID with the database record
            string knownProfileID = "knownProfileID"; //existing company profile ID with the database record
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileDAO = new MockCurrentProfileDAO();

            //the company user id is set to the known user Id, only this user Id should be allowed to process, the other should throw an exception
            mockCompanyLogic.MockGetProfileByID(knownProfileID, knownProfileID, new Company { Id = knownProfileID, UserId = knownUserId });
            mockCurrentProfileDAO.MockGetCurrentProfile(userId, userId, new CurrentProfile { ProfileID = knownProfileID,  UserID = userId, ProfileType = ProfileType.Company }); //already has an existing Current profile record
            if (knownUserId != userId)
            {
                mockCurrentProfileDAO.MockUpdateProfile();
                mockGeneralUserLogic.MockGetAllProfiles(userId, userId);
            }

            //Act
            var currentProfile = await new CurrentProfileLogic(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileDAO.Object)
            .GetCurrentProfileAsync(userId);

            //Assert
            Assert.NotNull(currentProfile);

            //Should pick the General user if the existing profile has been assigned to a different user
            if (knownUserId != userId)
            {
                Assert.Equal(currentProfile.UserID, userId);
                Assert.NotEqual(currentProfile.ProfileID, knownProfileID);
                Assert.Equal(ProfileType.GeneralUser, currentProfile.ProfileType);
                Assert.InRange(currentProfile.LastSetDate, DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(1));
                mockGeneralUserLogic.VerifyAll();
            }
            else
            {
                Assert.Equal(currentProfile.UserID, userId);
                Assert.Equal(currentProfile.ProfileID, knownProfileID);
                Assert.Equal(ProfileType.Company, currentProfile.ProfileType);
            }
            mockCompanyLogic.VerifyAll();
            mockCurrentProfileDAO.VerifyAll();
        }
             
    }

}
