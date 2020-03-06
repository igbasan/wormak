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
    public class ProfessionalControllerTest
    {
        private Professional profile = new Professional
        {
            Address = "Address",
            Description = "About Me",
            City = "City",
            Country = "Country",
            Type = "Type",
            Name = "Name",
            State = "State"
        };

        [Fact]
        public async Task SetupProfessional_NullProfile()
        {
            //arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();

            //act
            IActionResult response = await new ProfessionalController(mockProfessionalLogic.Object).SetupProfessional(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task SetupProfessional_InvalidModel()
        {
            //arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();

            var controller = new ProfessionalController(mockProfessionalLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.SetupProfessional(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }
        [Fact]
        public async Task SetupProfessional_ValidModelWithException()
        {
            //arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();
            mockProfessionalLogic.MockSetupProfileWithException(profile);
            string userID = "userID";

            //set user ID in claims
            ProfessionalController controller = SetUpController(mockProfessionalLogic, userID);

            //act
            IActionResult response = await controller.SetupProfessional(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockProfessionalLogic.ProfileCreated);
            mockProfessionalLogic.VerifyAll();
        }

        [Fact]
        public async Task SetupProfessional_ValidModel()
        {
            //arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();
            mockProfessionalLogic.MockSetupProfile(profile);
            string userID = "userID";

            //set user ID in claims
            ProfessionalController controller = SetUpController(mockProfessionalLogic, userID);

            //act
            IActionResult response = await controller.SetupProfessional(profile);

            //assert
            Assert.NotNull(profile.UserId);
            Assert.Equal(profile.UserId, userID);
            Assert.IsType<JsonResult>(response);
            Assert.True(mockProfessionalLogic.ProfileCreated);
            mockProfessionalLogic.VerifyAll();
        }

        private static ProfessionalController SetUpController(MockProfessionalLogic mockProfessionalLogic, string userID)
        {
            var controller = new ProfessionalController(mockProfessionalLogic.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id", userID));
            controller.ControllerContext.HttpContext.User.AddIdentity(identity);
            return controller;
        }

        [Fact]
        public async Task UpdateProfessional_NullProfile()
        {
            //arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();

            //act
            IActionResult response = await new ProfessionalController(mockProfessionalLogic.Object).UpdateProfessional(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task UpdateProfessional_InvalidModel()
        {
            //arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();

            var controller = new ProfessionalController(mockProfessionalLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.UpdateProfessional(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }
        [Fact]
        public async Task UpdateProfessional_ValidModelWithException()
        {
            //arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();
            mockProfessionalLogic.MockUpdateProfileWithException(profile);
            string userID = "userID";

            //set user ID in claims
            ProfessionalController controller = SetUpController(mockProfessionalLogic, userID);

            //act
            IActionResult response = await controller.UpdateProfessional(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockProfessionalLogic.ProfileUpdated);
            mockProfessionalLogic.VerifyAll();
        }

        [Fact]
        public async Task UpdateProfessional_ValidModel()
        {
            //arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();
            mockProfessionalLogic.MockUpdateProfile(profile);
            string userID = "userID";

            //set user ID in claims
            ProfessionalController controller = SetUpController(mockProfessionalLogic, userID);

            //act
            IActionResult response = await controller.UpdateProfessional(profile);

            //assert
            Assert.Equal(profile.UserId, userID);
            Assert.IsType<JsonResult>(response);
            Assert.True(mockProfessionalLogic.ProfileUpdated);
            mockProfessionalLogic.VerifyAll();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetDetailsProfessional_idNullOrEmpty(string id)
        {
            //Arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();

            //Act
            IActionResult response = await new ProfessionalController(mockProfessionalLogic.Object).GetProfessionalDetails(id);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetDetailsProfessional_invalidIdFormat()
        {
            //Arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();

            //Act
            IActionResult response = await new ProfessionalController(mockProfessionalLogic.Object).GetProfessionalDetails("VeryInvalidFormat");

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }


        [Theory]
        [InlineData("a123456f")]
        [InlineData("a1234")]
        public async Task GetDetailsProfessional_TokenNotNull(string id)
        {
            //Arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();
            profile.Id = id;
            string knownId = "a1234";
            mockProfessionalLogic.MockGetProfileByID(id, knownId, profile);
            string userID = "userID";

            //set user ID in claims
            ProfessionalController controller = SetUpController(mockProfessionalLogic, userID);

            //Act
            IActionResult response = await controller.GetProfessionalDetails(id);

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileObjectResponse<Professional>;
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

            mockProfessionalLogic.VerifyAll();
        }

        [Theory]
        [InlineData("InvalidUserId")]
        [InlineData("userID")]
        public async Task GetDetailsProfessional_WithUserID(string userID)
        {
            //Arrange
            MockProfessionalLogic mockProfessionalLogic = new MockProfessionalLogic();
            string knownUserID = "userID";
            profile.UserId = userID;
            mockProfessionalLogic.MockGetAllProfiles(userID, knownUserID);

            //set user ID in claims
            ProfessionalController controller = SetUpController(mockProfessionalLogic, userID);

            //Act
            IActionResult response = await controller.GetAllProfessionalsByUser();

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileListResponse<Professional>;

            //test Json data
            Assert.NotNull(resultValue);

            if (userID == knownUserID)
            {
                Assert.NotNull(resultValue.Result);
                Assert.True(resultValue.Result.Count > 0);
                Assert.True(resultValue.Result.All(c => c.UserId == userID));
            }
            else
            {
                Assert.NotNull(resultValue.Result);
                Assert.True(resultValue.Result.Count == 0);
            }

            mockProfessionalLogic.VerifyAll();
        }

        [Fact]
        public async Task GetAllProfilesByIDsAsync_NullList()
        {
            //Arrange
            var mockProfessionalLogic = new MockProfessionalLogic();

            //Act
            IActionResult response = await new ProfessionalController(mockProfessionalLogic.Object).GetAllProfessionalsByProfileIDs(null);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetAllProfilesByIDsAsync_EmptyList()
        {
            //Arrange
            var mockProfessionalLogic = new MockProfessionalLogic();

            //Act
            IActionResult response = await new ProfessionalController(mockProfessionalLogic.Object).GetAllProfessionalsByProfileIDs(new List<string>());

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
            var mockProfessionalLogic = new MockProfessionalLogic();
            List<string> ids = new List<string>() { item1, item2 };

            //Act
            IActionResult response = await new ProfessionalController(mockProfessionalLogic.Object).GetAllProfessionalsByProfileIDs(ids);

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
            var mockProfessionalLogic = new MockProfessionalLogic();
            List<string> knownIds = new List<string> { "a12345678", "a12345679" };
            mockProfessionalLogic.MockGetAllProfilesByIDs(ids, knownIds);

            //Act
            IActionResult response = await new ProfessionalController(mockProfessionalLogic.Object).GetAllProfessionalsByProfileIDs(ids);

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileListResponse<Professional>;

            Assert.NotNull(resultValue);
            Assert.NotNull(resultValue.Result);
            var mutualIds = ids.Where(x => knownIds.Contains(x)).ToList();

            Assert.True(resultValue.Result.Count == mutualIds.Count);
            Assert.True(resultValue.Result.All(v => mutualIds.Contains(v.Id)));

            mockProfessionalLogic.VerifyAll();
        }

    }
}