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
using Xunit;

namespace ProfileService.Test.Tests
{
    public class CompanyControllerTest
    {
        private Company profile = new Company
        {
            Address = "Address",
            Description = "About Me",
            City = "City",
            Country = "Country",
            Industry = "Industry",
            Name = "Name",
            State = "State",
            SizeRange = "10-11",
            VerificationStatus = "NotVerified"
        };

        [Fact]
        public async Task SetupCompany_NullProfile()
        {
            //arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();

            //act
            IActionResult response = await new CompanyController(mockCompanyLogic.Object).SetupCompany(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task SetupCompany_InvalidModel()
        {
            //arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();

            var controller = new CompanyController(mockCompanyLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.SetupCompany(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }
        [Fact]
        public async Task SetupCompany_ValidModelWithException()
        {
            //arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();
            mockCompanyLogic.MockSetupProfileWithException(profile);
            string userID = "userID";

            //set user ID in claims
            CompanyController controller = SetUpController(mockCompanyLogic, userID);

            //act
            IActionResult response = await controller.SetupCompany(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockCompanyLogic.ProfileCreated);
            mockCompanyLogic.VerifyAll();
        }

        [Fact]
        public async Task SetupCompany_ValidModel()
        {
            //arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();
            mockCompanyLogic.MockSetupProfile(profile);
            string userID = "userID";

            //set user ID in claims
            CompanyController controller = SetUpController(mockCompanyLogic, userID);

            //act
            IActionResult response = await controller.SetupCompany(profile);

            //assert
            Assert.NotNull(profile.UserId);
            Assert.Equal(profile.UserId, userID);
            Assert.True(mockCompanyLogic.ProfileCreated);
            Assert.IsType<JsonResult>(response);
            mockCompanyLogic.VerifyAll();
        }

        private static CompanyController SetUpController(MockCompanyLogic mockCompanyLogic, string userID)
        {
            var controller = new CompanyController(mockCompanyLogic.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/id", userID));
            controller.ControllerContext.HttpContext.User.AddIdentity(identity);
            return controller;
        }

        [Fact]
        public async Task UpdateCompany_NullProfile()
        {
            //arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();

            //act
            IActionResult response = await new CompanyController(mockCompanyLogic.Object).UpdateCompany(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task UpdateCompany_InvalidModel()
        {
            //arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();

            var controller = new CompanyController(mockCompanyLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.UpdateCompany(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }
        [Fact]
        public async Task UpdateCompany_ValidModelWithException()
        {
            //arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();
            mockCompanyLogic.MockUpdateProfileWithException(profile);
            string userID = "userID";

            //set user ID in claims
            CompanyController controller = SetUpController(mockCompanyLogic, userID);

            //act
            IActionResult response = await controller.UpdateCompany(profile);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockCompanyLogic.ProfileUpdated);
            mockCompanyLogic.VerifyAll();
        }

        [Fact]
        public async Task UpdateCompany_ValidModel()
        {
            //arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();
            mockCompanyLogic.MockUpdateProfile(profile);
            string userID = "userID";

            //set user ID in claims
            CompanyController controller = SetUpController(mockCompanyLogic, userID);

            //act
            IActionResult response = await controller.UpdateCompany(profile);

            //assert
            Assert.Equal(profile.UserId, userID);
            Assert.IsType<JsonResult>(response);
            Assert.True(mockCompanyLogic.ProfileUpdated);
            mockCompanyLogic.VerifyAll();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetDetailsCompany_idNullOrEmpty(string id)
        {
            //Arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();

            //Act
            IActionResult response = await new CompanyController(mockCompanyLogic.Object).GetCompanyDetails(id);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetDetailsCompany_invalidIdFormat()
        {
            //Arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();

            //Act
            IActionResult response = await new CompanyController(mockCompanyLogic.Object).GetCompanyDetails("VeryInvalidFormat");

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("a123456f")]
        [InlineData("a1234")]
        public async Task GetDetailsCompany_TokenNotNull(string id)
        {
            //Arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();
            profile.Id = id;
            string knownId = "a1234";
            mockCompanyLogic.MockGetProfileByID(id, knownId, profile);
            string userID = "userID";

            //set user ID in claims
            CompanyController controller = SetUpController(mockCompanyLogic, userID);

            //Act
            IActionResult response = await controller.GetCompanyDetails(id);

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileObjectResponse<Company>;
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

            mockCompanyLogic.VerifyAll();
        }

        [Theory]
        [InlineData("InvalidUserId")]
        [InlineData("userID")]
        public async Task GetDetailsCompany_WithUserID(string userID)
        {
            //Arrange
            MockCompanyLogic mockCompanyLogic = new MockCompanyLogic();
            string knownUserID = "userID";
            profile.UserId = userID;
            mockCompanyLogic.MockGetAllProfiles(userID, knownUserID);

            //set user ID in claims
            CompanyController controller = SetUpController(mockCompanyLogic, userID);

            //Act
            IActionResult response = await controller.GetAllCompaniesByUser();

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileListResponse<Company>;

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

            mockCompanyLogic.VerifyAll();
        }

        [Fact]
        public async Task GetAllProfilesByIDsAsync_NullList()
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();

            //Act
            IActionResult response = await new CompanyController(mockCompanyLogic.Object).GetAllCompaniesByProfileIDs(null);

            //Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetAllProfilesByIDsAsync_EmptyList()
        {
            //Arrange
            var mockCompanyLogic = new MockCompanyLogic();

            //Act
            IActionResult response = await new CompanyController(mockCompanyLogic.Object).GetAllCompaniesByProfileIDs(new List<string>());

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
            var mockCompanyLogic = new MockCompanyLogic();
            List<string> ids = new List<string>() { item1, item2 };

            //Act
            IActionResult response = await new CompanyController(mockCompanyLogic.Object).GetAllCompaniesByProfileIDs(ids);

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
            var mockCompanyLogic = new MockCompanyLogic();
            List<string> knownIds = new List<string> { "a12345678", "a12345679" };
            mockCompanyLogic.MockGetAllProfilesByIDs(ids, knownIds);

            //Act
            IActionResult response = await new CompanyController(mockCompanyLogic.Object).GetAllCompaniesByProfileIDs(ids);

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as ProfileListResponse<Company>;

            Assert.NotNull(resultValue);
            Assert.NotNull(resultValue.Result);
            var mutualIds = ids.Where(x => knownIds.Contains(x)).ToList();

            Assert.True(resultValue.Result.Count == mutualIds.Count);
            Assert.True(resultValue.Result.All(v => mutualIds.Contains(v.Id)));

            mockCompanyLogic.VerifyAll();
        }

    }
}
