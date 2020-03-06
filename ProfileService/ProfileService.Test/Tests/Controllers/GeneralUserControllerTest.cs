using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Controllers;
using ProfileService.Models.Implementations;
using ProfileService.Test.Mocks;
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
    public class GeneralUserControllerTest
    {
        private GeneralUser profile = new GeneralUser
        {
            Address = "Address",
            Bio = "About Me",
            City = "City",
            Country = "Country",
            DateOfBirth = DateTime.Now,
            FirstName = "FirstName",
            LastName = "LastName",
            Occupation = "Occupation",
            State = "State"
        };

        [Fact]
        public async Task SetupGeneralUser_NullProfile()
        {
            //arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();

            //act
            IActionResult response = await new GeneralUserController(mockGeneralUserLogic.Object).SetupGeneralUser(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task SetupGeneralUser_InvalidModel()
        {
            //arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();

            var controller = new GeneralUserController(mockGeneralUserLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.SetupGeneralUser(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }
        [Fact]
        public async Task SetupGeneralUser_ValidModelWithException()
        {
            //arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();
            mockGeneralUserLogic.MockSetupProfileWithException(profile);
            string userID = "userID";

            //set user ID in claims
            GeneralUserController controller = SetUpController(mockGeneralUserLogic, userID);

            //act
            IActionResult response = await controller.SetupGeneralUser(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockGeneralUserLogic.ProfileCreated);
            mockGeneralUserLogic.VerifyAll();
        }

        [Fact]
        public async Task SetupGeneralUser_ValidModel()
        {
            //arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();
            mockGeneralUserLogic.MockSetupProfile(profile);
            string userID = "userID";

            //set user ID in claims
            GeneralUserController controller = SetUpController(mockGeneralUserLogic, userID);

            //act
            IActionResult response = await controller.SetupGeneralUser(profile);

            //assert
            Assert.NotNull(profile.UserId);
            Assert.Equal(profile.UserId, userID);
            Assert.IsType<JsonResult>(response);
            Assert.True(mockGeneralUserLogic.ProfileCreated);
            mockGeneralUserLogic.VerifyAll();
        }

        private static GeneralUserController SetUpController(MockGeneralUserLogic mockGeneralUserLogic, string userID)
        {
            var controller = new GeneralUserController(mockGeneralUserLogic.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id", userID));
            controller.ControllerContext.HttpContext.User.AddIdentity(identity);
            return controller;
        }

        [Fact]
        public async Task UpdateGeneralUser_NullProfile()
        {
            //arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();

            //act
            IActionResult response = await new GeneralUserController(mockGeneralUserLogic.Object).UpdateGeneralUser(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task UpdateGeneralUser_InvalidModel()
        {
            //arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();

            var controller = new GeneralUserController(mockGeneralUserLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.UpdateGeneralUser(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }
        [Fact]
        public async Task UpdateGeneralUser_ValidModelWithException()
        {
            //arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();
            mockGeneralUserLogic.MockUpdateProfileWithException(profile);
            string userID = "userID";

            //set user ID in claims
            GeneralUserController controller = SetUpController(mockGeneralUserLogic, userID);

            //act
            IActionResult response = await controller.UpdateGeneralUser(profile);

            //assert
            Assert.False(mockGeneralUserLogic.ProfileUpdated);
            Assert.IsType<BadRequestObjectResult>(response);
            mockGeneralUserLogic.VerifyAll();
        }

        [Fact]
        public async Task UpdateGeneralUser_ValidModel()
        {
            //arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();
            mockGeneralUserLogic.MockUpdateProfile(profile);
            string userID = "userID";

            //set user ID in claims
            GeneralUserController controller = SetUpController(mockGeneralUserLogic, userID);

            //act
            IActionResult response = await controller.UpdateGeneralUser(profile);

            //assert
            Assert.Equal(profile.UserId, userID);
            Assert.IsType<JsonResult>(response);
            Assert.True(mockGeneralUserLogic.ProfileUpdated);
            mockGeneralUserLogic.VerifyAll();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetDetailsGeneralUser_idNullOrEmpty(string id)
        {
            //Arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();

            //Act
            IActionResult response = await new GeneralUserController(mockGeneralUserLogic.Object).GetGeneralUserDetails(id);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetDetailsGeneralUser_invalidIdFormat()
        {
            //Arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();

            //Act
            IActionResult response = await new GeneralUserController(mockGeneralUserLogic.Object).GetGeneralUserDetails("VeryInvalidFormat");

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("a123456f")]
        [InlineData("a1234")]
        public async Task GetDetailsGeneralUser_TokenNotNull(string id)
        {
            //Arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();
            profile.Id = id;
            string knownId = "Id";
            mockGeneralUserLogic.MockGetProfileByID(id, knownId, profile);
            string userID = "a1234";

            //set user ID in claims
            GeneralUserController controller = SetUpController(mockGeneralUserLogic, userID);

            //Act
            IActionResult response = await controller.GetGeneralUserDetails(id);

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileObjectResponse<GeneralUser>;
            Assert.NotNull(resultValue);

            if (id == knownId)
            {
                Assert.NotNull(resultValue.Result);
                Assert.Equal(id, resultValue.Result.Id);
            }
            else
            {
                Assert.Null(resultValue.Result);
            }

            mockGeneralUserLogic.VerifyAll();
        }

        [Theory]
        [InlineData("InvalidUserId")]
        [InlineData("userID")]
        public async Task GetDetailsGeneralUser_WithUserID(string userID)
        {
            //Arrange
            MockGeneralUserLogic mockGeneralUserLogic = new MockGeneralUserLogic();
            string knownUserID = "userID";
            profile.UserId = userID;
            mockGeneralUserLogic.MockGetAllProfiles(userID, knownUserID);

            //set user ID in claims
            GeneralUserController controller = SetUpController(mockGeneralUserLogic, userID);

            //Act
            IActionResult response = await controller.GetGeneralUserDetails();

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileObjectResponse<GeneralUser>;

            //test Json data
            Assert.NotNull(resultValue);

            if (userID == knownUserID)
            {
                Assert.NotNull(resultValue.Result);
                Assert.Equal(userID, resultValue.Result.UserId);
            }
            else
            {
                Assert.Null(resultValue.Result);
            }

            mockGeneralUserLogic.VerifyAll();
        }

        [Fact]
        public async Task GetAllProfilesByIDsAsync_NullList()
        {
            //Arrange
            var mockGeneralUserLogic = new MockGeneralUserLogic();

            //Act
            IActionResult response = await new GeneralUserController(mockGeneralUserLogic.Object).GetAllGeneralUsersByProfileIDs(null);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetAllProfilesByIDsAsync_EmptyList()
        {
            //Arrange
            var mockGeneralUserLogic = new MockGeneralUserLogic();

            //Act
            IActionResult response = await new GeneralUserController(mockGeneralUserLogic.Object).GetAllGeneralUsersByProfileIDs(new List<string>());

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("Invalid", "Invalid")]
        [InlineData("Invalid", "a12345678")]
        [InlineData("a12345678", "Invalid")]
        public async Task GetAllProfilesByIDsAsync_InvalidList(string item1, string item2)
        {
            //Arrange
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            List<string> ids = new List<string>() { item1, item2 };

            //Act
            IActionResult response = await new GeneralUserController(mockGeneralUserLogic.Object).GetAllGeneralUsersByProfileIDs(ids);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("a12345677", "a12345670")] //two not known
        [InlineData("a12345679", "a12345670")] //one known id
        [InlineData("a12345678", "a12345679")] //both known
        public async Task GetAllProfilesByIDsAsync_NotNull(string item1, string item2)
        {
            //Arrange
            List<string> ids = new List<string>() { item1, item2 };
            var mockGeneralUserLogic = new MockGeneralUserLogic();
            List<string> knownIds = new List<string> { "a12345678", "a12345679" };
            mockGeneralUserLogic.MockGetAllProfilesByIDs(ids, knownIds);

            //Act
            IActionResult response = await new GeneralUserController(mockGeneralUserLogic.Object).GetAllGeneralUsersByProfileIDs(ids);

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileListResponse<GeneralUser>;

            Assert.NotNull(resultValue);
            Assert.NotNull(resultValue.Result);
            var mutualIds = ids.Where(x => knownIds.Contains(x)).ToList();

            Assert.True(resultValue.Result.Count == mutualIds.Count);
            Assert.True(resultValue.Result.All(v => mutualIds.Contains(v.Id)));

            mockGeneralUserLogic.VerifyAll();
        }

    }
}