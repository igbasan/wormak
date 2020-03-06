using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Controllers;
using ProfileService.Models;
using ProfileService.Models.Implementations;
using ProfileService.Test.Mocks;
using ProfileService.Test.Mocks.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Xunit;

namespace ProfileService.Test.Tests
{
    public class ProfileControllerTest
    {
        private CurrentProfile currentProfile = new CurrentProfile
        {
            ProfileID = "ProfileID",
            ProfileType = Models.ProfileType.Company
        };
        [Fact]
        public async Task SetCurrentProfile_NullProfile()
        {
            //arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            //act
            IActionResult response = await new ProfileController(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileLogic.Object)
                .SetCurrentProfile(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task SetupGeneralUser_InvalidModel()
        {
            //arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            var controller = new ProfileController(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act

            IActionResult response = await controller.SetCurrentProfile(currentProfile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task SetupGeneralUser_ValidModelWithException()
        {
            //arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            string userID = "userID";
            mockCurrentProfileLogic.MockSetupProfileWithException(userID, "ProfileID", Models.ProfileType.Company);

            //set user ID in claims
            ProfileController controller = SetUpController(mockCompanyLogic, mockGeneralUserLogic, mockProfessionalLogic, mockCurrentProfileLogic, userID);

            //act
            IActionResult response = await controller.SetCurrentProfile(currentProfile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockCurrentProfileLogic.ProfileCreated);
            mockCurrentProfileLogic.VerifyAll();
        }

        [Fact]
        public async Task SetupGeneralUser_ValidModel()
        {
            //arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            string userID = "userID";
            mockCurrentProfileLogic.MockSetupProfile(userID, "ProfileID", Models.ProfileType.Company);

            //set user ID in claims
            ProfileController controller = SetUpController(mockCompanyLogic, mockGeneralUserLogic, mockProfessionalLogic, mockCurrentProfileLogic, userID);

            //act
            IActionResult response = await controller.SetCurrentProfile(currentProfile);

            //assert
            Assert.NotNull(currentProfile.UserID);
            Assert.Equal(currentProfile.UserID, userID);
            Assert.IsType<JsonResult>(response);
            Assert.True(mockCurrentProfileLogic.ProfileCreated);
            mockCurrentProfileLogic.VerifyAll();
        }

        private static ProfileController SetUpController(MockCompanyLogic mockCompanyLogic, MockGeneralUserLogic mockGeneralUserLogic, MockProfessionalLogic mockProfessionalLogic, MockCurrentProfileLogic mockCurrentProfileLogic, string userID)
        {
            var controller = new ProfileController(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileLogic.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id", userID));
            controller.ControllerContext.HttpContext.User.AddIdentity(identity);
            return controller;
        }


        [Fact]
        public async Task GetCurrentProfile_WithException()
        {
            //arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            string userID = "userID";
            mockCurrentProfileLogic.MockGetCurrentProfileWithException(userID);

            //set user ID in claims
            ProfileController controller = SetUpController(mockCompanyLogic, mockGeneralUserLogic, mockProfessionalLogic, mockCurrentProfileLogic, userID);

            //Act
            IActionResult response = await controller.GetCurrentProfile();

            //Assert
            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            mockCurrentProfileLogic.VerifyAll();
        }

        [Fact]
        public async Task GetCurrentProfile_Valid()
        {
            //arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            string userID = "userID";
            mockCurrentProfileLogic.MockGetCurrentProfile(userID);

            //set user ID in claims
            ProfileController controller = SetUpController(mockCompanyLogic, mockGeneralUserLogic, mockProfessionalLogic, mockCurrentProfileLogic, userID);

            //Act
            IActionResult response = await controller.GetCurrentProfile();

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileObjectResponse<ProfileLite>;

            //test Json data
            Assert.NotNull(resultValue);
            Assert.NotNull(resultValue.Result);

            mockCurrentProfileLogic.VerifyAll();
        }

        [Fact]
        public async Task GetProfiles_NullList()
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            IActionResult response = await new ProfileController(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileLogic.Object)
                .GetProfiles(null);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetProfiles_EmptyList()
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            IActionResult response = await new ProfileController(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileLogic.Object)
                .GetProfiles(new List<ProfileLite>());

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("Invalid", "Invalid")]
        [InlineData("Invalid", "a12345678")]
        [InlineData("a12345678", "Invalid")]
        public async Task GetProfiles_InvalidList(string id1, string id2)
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            IActionResult response = await new ProfileController(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileLogic.Object)
                .GetProfiles(new List<ProfileLite>() { new ProfileLite { Id = id1 }, new ProfileLite { Id = id2 } });

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetProfiles_NotNull()
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            var profileList = new List<ProfileLite>() {
                new ProfileLite { Id = "a12345678", ProfileType = ProfileType.Company },
                new ProfileLite { Id = "b12345678", ProfileType = ProfileType.GeneralUser },
                new ProfileLite { Id = "c12345678", ProfileType = ProfileType.Professional },
                new ProfileLite { Id = "a12345670", ProfileType = ProfileType.Company }
            };

            var companyIDs = new List<string> { "a12345670", "a12345678" };
            var generalUserIDs = new List<string> { "b12345678" };
            var professionalIDs = new List<string> { "c12345678" };

            mockCompanyLogic.MockGetAllProfilesByIDs(companyIDs, companyIDs);
            mockGeneralUserLogic.MockGetAllProfilesByIDs(generalUserIDs, generalUserIDs);
            mockProfessionalLogic.MockGetAllProfilesByIDs(professionalIDs, professionalIDs);

            //Act
            IActionResult response = await new ProfileController(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileLogic.Object)
                .GetProfiles(profileList);


            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileListResponse<ProfileLite>;

            Assert.NotNull(resultValue);
            Assert.NotNull(resultValue.Result);

            Assert.True(resultValue.Result.Count(c => c.ProfileType == ProfileType.Company) == 2);
            Assert.True(resultValue.Result.Count(c => c.ProfileType == ProfileType.GeneralUser) == 1);
            Assert.True(resultValue.Result.Count(c => c.ProfileType == ProfileType.Professional) == 1);
            Assert.True(resultValue.Result.Where(c => c.ProfileType == ProfileType.Company).All(v => companyIDs.Contains(v.Id)));
            Assert.True(resultValue.Result.Where(c => c.ProfileType == ProfileType.GeneralUser).All(v => generalUserIDs.Contains(v.Id)));
            Assert.True(resultValue.Result.Where(c => c.ProfileType == ProfileType.Professional).All(v => professionalIDs.Contains(v.Id)));

            mockCompanyLogic.VerifyAll();
            mockGeneralUserLogic.VerifyAll();
            mockProfessionalLogic.VerifyAll();
        }

        [Fact]
        public async Task GetInterests_NullList()
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            IActionResult response = await new ProfileController(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileLogic.Object)
                .GetInterests(null);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetInterests_EmptyList()
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            IActionResult response = await new ProfileController(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileLogic.Object)
                .GetInterests(new List<Interest>());

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetInterests_NotNull()
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            var interestList = new List<Interest>() { Interest.Art, Interest.Pleasure};

            mockCompanyLogic.MockGetAllProfilesByInterests(interestList, interestList);
            mockGeneralUserLogic.MockGetAllProfilesByInterests(interestList, interestList);
            mockProfessionalLogic.MockGetAllProfilesByInterests(interestList, interestList);

            //Act
            IActionResult response = await new ProfileController(mockCompanyLogic.Object, mockGeneralUserLogic.Object, mockProfessionalLogic.Object, mockCurrentProfileLogic.Object)
                .GetInterests(interestList);


            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileListResponse<InterestProfileLite>;

            Assert.NotNull(resultValue);
            Assert.NotNull(resultValue.Result);

            Assert.True(resultValue.Result.Count(c => c.ProfileType == ProfileType.Company) == 2);
            Assert.True(resultValue.Result.Count(c => c.ProfileType == ProfileType.GeneralUser) == 2);
            Assert.True(resultValue.Result.Count(c => c.ProfileType == ProfileType.Professional) == 2);
            Assert.True(resultValue.Result.Where(c => c.ProfileType == ProfileType.Company).All(v => interestList.Contains(v.Interests.First())));
            Assert.True(resultValue.Result.Where(c => c.ProfileType == ProfileType.GeneralUser).All(v => interestList.Contains(v.Interests.First())));
            Assert.True(resultValue.Result.Where(c => c.ProfileType == ProfileType.Professional).All(v => interestList.Contains(v.Interests.First())));

            mockCompanyLogic.VerifyAll();
            mockGeneralUserLogic.VerifyAll();
            mockProfessionalLogic.VerifyAll();
        }
        [Fact]
        public async Task GetCurrentInterestProfile_WithException()
        {
            //arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            string userID = "userID";
            mockCurrentProfileLogic.MockGetCurrentProfileWithException(userID);

            //set user ID in claims
            ProfileController controller = SetUpController(mockCompanyLogic, mockGeneralUserLogic, mockProfessionalLogic, mockCurrentProfileLogic, userID);

            //Act
            IActionResult response = await controller.GetCurrentInterestProfile();

            //Assert
            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            mockCurrentProfileLogic.VerifyAll();
        }

        [Fact]
        public async Task GetCurrentInterestProfile_Valid()
        {
            //arrange
            var mockCompanyLogic = new MockCompanyLogic();
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            var mockProfessionalLogic = new MockProfessionalLogic();
            var mockCurrentProfileLogic = new MockCurrentProfileLogic();

            string userID = "userID";
            mockCurrentProfileLogic.MockGetCurrentProfile(userID);

            //set user ID in claims
            ProfileController controller = SetUpController(mockCompanyLogic, mockGeneralUserLogic, mockProfessionalLogic, mockCurrentProfileLogic, userID);

            //Act
            IActionResult response = await controller.GetCurrentInterestProfile();

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileObjectResponse<InterestProfileLite>;

            //test Json data
            Assert.NotNull(resultValue);
            Assert.NotNull(resultValue.Result);

            mockCurrentProfileLogic.VerifyAll();
        }
    }
}