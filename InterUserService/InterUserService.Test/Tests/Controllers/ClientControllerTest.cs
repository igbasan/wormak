using InterUserService.Controllers;
using InterUserService.Models;
using InterUserService.Models.Implemetations;
using InterUserService.Test.Mocks.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InterUserService.Test.Tests.Controllers
{
    public class ClientControllerTest
    {

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("NonHexadecimalID")]//non hexadecimal ID
        public async Task AddClient_InvalidProfileID(string profileID)
        {
            //arrange
            MockClientLogic mockClientLogic = new MockClientLogic();

            var controller = new ClientController(mockClientLogic.Object);

            //act
            IActionResult response = await controller.AddClient(profileID, "Company");

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("invalidProfileType")]//invalid ProfileType
        public async Task AddClient_InvalidProfileType(string profileType)
        {
            //arrange
            MockClientLogic mockClientLogic = new MockClientLogic();

            var controller = new ClientController(mockClientLogic.Object);

            //act
            IActionResult response = await controller.AddClient("1231231312abdc", profileType);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        private static ClientController SetUpController(MockClientLogic mockClientLogic, string profileID, ProfileType profileType)
        {
            var controller = new ClientController(mockClientLogic.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId", profileID));
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType", profileType.ToString()));
            controller.ControllerContext.HttpContext.User.AddIdentity(identity);
            return controller;
        }

        [Fact]
        public async Task AddClient_WithException()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Client client = new Client
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockClientLogic mockClientLogic = new MockClientLogic();
            mockClientLogic.MockSetUpRelationshipWithException(client);

            //set user ID in claims
            ClientController controller = SetUpController(mockClientLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.AddClient(passiveProfileID, ProfileType.Company.ToString());

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockClientLogic.ClientSetUp);
            Assert.False(mockClientLogic.ClientSetUpToDeactivate);
            mockClientLogic.VerifyAll();
        }


        [Fact]
        public async Task AddClient_Valid()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Client client = new Client
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockClientLogic mockClientLogic = new MockClientLogic();
            mockClientLogic.MockSetUpRelationship(client);

            //set user ID in claims
            ClientController controller = SetUpController(mockClientLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.AddClient(passiveProfileID, ProfileType.Company.ToString());

            //assert
            Assert.True(mockClientLogic.ClientSetUp);
            Assert.False(mockClientLogic.ClientSetUpToDeactivate);
            Assert.IsType<JsonResult>(response);
            mockClientLogic.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("NonHexadecimalID")]//non hexadecimal ID
        public async Task RemoveClient_InvalidProfileID(string profileID)
        {
            //arrange
            MockClientLogic mockClientLogic = new MockClientLogic();

            var controller = new ClientController(mockClientLogic.Object);

            //act
            IActionResult response = await controller.RemoveClient(profileID, "Company");

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("invalidProfileType")]//invalid ProfileType
        public async Task RemoveClient_InvalidProfileType(string profileType)
        {
            //arrange
            MockClientLogic mockClientLogic = new MockClientLogic();

            var controller = new ClientController(mockClientLogic.Object);

            //act
            IActionResult response = await controller.RemoveClient("1231231312abdc", profileType);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task RemoveClient_WithException()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Client client = new Client
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockClientLogic mockClientLogic = new MockClientLogic();
            mockClientLogic.MockSetUpRelationshipWithException(client);

            //set profile ID in claims
            ClientController controller = SetUpController(mockClientLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.RemoveClient(passiveProfileID, ProfileType.Company.ToString());

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockClientLogic.ClientSetUp);
            Assert.True(mockClientLogic.ClientSetUpToDeactivate);
            mockClientLogic.VerifyAll();
        }


        [Fact]
        public async Task RemoveClient_Valid()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Client client = new Client
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockClientLogic mockClientLogic = new MockClientLogic();
            mockClientLogic.MockSetUpRelationship(client);

            //set user ID in claims
            ClientController controller = SetUpController(mockClientLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.RemoveClient(passiveProfileID, ProfileType.Company.ToString());

            //assert
            Assert.True(mockClientLogic.ClientSetUp);
            Assert.True(mockClientLogic.ClientSetUpToDeactivate);
            Assert.IsType<JsonResult>(response);
            mockClientLogic.VerifyAll();
        }

        [Theory]
        [InlineData("InvalidProfileId")]//test for empty scenario
        [InlineData("profileId")]
        public async Task GetClients_WithProfieID(string profileId)
        {
            //Arrange
            MockClientLogic mockClientLogic = new MockClientLogic();
            string knownProfileId = "profileId";

            mockClientLogic.MockGetAllByPassiveProfileID(profileId, knownProfileId, ProfileType.Company, ProfileType.Company);


            //set profile ID in claims
            ClientController controller = SetUpController(mockClientLogic, profileId, ProfileType.Company);

            //Act
            IActionResult response = await controller.GetClients(0, 10);

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as InterUserListResponse<Profile>;

            //test Json data
            Assert.NotNull(resultValue);

            if (profileId == knownProfileId)
            {
                Assert.NotNull(resultValue.Result);
                Assert.True(resultValue.Result.Count > 0);
                Assert.Equal(2, resultValue.TotalCount);
                Assert.True(resultValue.Result.All(c => c.Id == "Result" && c.ProfileType == ProfileType.Professional));
            }
            else
            {
                Assert.NotNull(resultValue.Result);
                Assert.True(resultValue.Result.Count == 0);
            }

            mockClientLogic.VerifyAll();
        }
    }
}
