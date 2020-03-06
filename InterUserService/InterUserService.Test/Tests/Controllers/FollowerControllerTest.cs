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
    public class FollowerControllerTest
    {

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("NonHexadecimalID")]//non hexadecimal ID
        public async Task Follow_InvalidProfileID(string profileID)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.Follow(profileID, "Company");

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("invalidProfileType")]//invalid ProfileType
        public async Task Follow_InvalidProfileType(string profileType)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.Follow("1231231312abdc", profileType);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        private static FollowerController SetUpController(MockFollowerLogic mockFollowerLogic, string profileID, ProfileType profileType)
        {
            var controller = new FollowerController(mockFollowerLogic.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId", profileID));
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType", profileType.ToString()));
            controller.ControllerContext.HttpContext.User.AddIdentity(identity);
            return controller;
        }

        [Fact]
        public async Task Follow_WithException()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Follower follower = new Follower
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();
            mockFollowerLogic.MockSetUpRelationshipWithException(follower);

            //set user ID in claims
            FollowerController controller = SetUpController(mockFollowerLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.Follow(passiveProfileID, ProfileType.Company.ToString());

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockFollowerLogic.FollowerSetUp);
            Assert.False(mockFollowerLogic.FollowerSetUpToDeactivate);
            mockFollowerLogic.VerifyAll();
        }


        [Fact]
        public async Task Follow_Valid()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Follower follower = new Follower
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();
            mockFollowerLogic.MockSetUpRelationship(follower);

            //set user ID in claims
            FollowerController controller = SetUpController(mockFollowerLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.Follow(passiveProfileID, ProfileType.Company.ToString());

            //assert
            Assert.True(mockFollowerLogic.FollowerSetUp);
            Assert.False(mockFollowerLogic.FollowerSetUpToDeactivate);
            Assert.IsType<JsonResult>(response);
            mockFollowerLogic.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("NonHexadecimalID")]//non hexadecimal ID
        public async Task Unfollow_InvalidProfileID(string profileID)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.Unfollow(profileID, "Company");

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("invalidProfileType")]//invalid ProfileType
        public async Task Unfollow_InvalidProfileType(string profileType)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.Unfollow("1231231312abdc", profileType);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Unfollow_WithException()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Follower follower = new Follower
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();
            mockFollowerLogic.MockSetUpRelationshipWithException(follower);

            //set profile ID in claims
            FollowerController controller = SetUpController(mockFollowerLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.Unfollow(passiveProfileID, ProfileType.Company.ToString());

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockFollowerLogic.FollowerSetUp);
            Assert.True(mockFollowerLogic.FollowerSetUpToDeactivate);
            mockFollowerLogic.VerifyAll();
        }


        [Fact]
        public async Task Unfollow_Valid()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Follower follower = new Follower
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();
            mockFollowerLogic.MockSetUpRelationship(follower);

            //set user ID in claims
            FollowerController controller = SetUpController(mockFollowerLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.Unfollow(passiveProfileID, ProfileType.Company.ToString());

            //assert
            Assert.True(mockFollowerLogic.FollowerSetUp);
            Assert.True(mockFollowerLogic.FollowerSetUpToDeactivate);
            Assert.IsType<JsonResult>(response);
            mockFollowerLogic.VerifyAll();
        }

        [Theory]
        [InlineData("InvalidProfileId")]//test for empty scenario
        [InlineData("profileId")]
        public async Task GetFollowers_WithProfileID(string profileId)
        {
            //Arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();
            string knownProfileId = "profileId";

            mockFollowerLogic.MockGetAllByPassiveProfileID(profileId, knownProfileId, ProfileType.Company, ProfileType.Company);


            //set profile ID in claims
            FollowerController controller = SetUpController(mockFollowerLogic, profileId, ProfileType.Company);

            //Act
            IActionResult response = await controller.GetFollowers(0, 10);

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

            mockFollowerLogic.VerifyAll();
        }

        [Theory]
        [InlineData("InvalidProfileId")]//test for empty scenario
        [InlineData("profileId")]
        public async Task GetFollowing_WithProfileID(string profileId)
        {
            //Arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();
            string knownProfileId = "profileId";

            mockFollowerLogic.MockGetAllByActiveProfileID(profileId, knownProfileId, ProfileType.Company, ProfileType.Company);


            //set profile ID in claims
            FollowerController controller = SetUpController(mockFollowerLogic, profileId, ProfileType.Company);

            //Act
            IActionResult response = await controller.GetFollowing(0, 10);

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

            mockFollowerLogic.VerifyAll();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("NonHexadecimalID")]//non hexadecimal ID
        public async Task CheckFollowed_InvalidProfileID(string profileID)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.CheckFollowed(profileID, "Company");

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("invalidProfileType")]//invalid ProfileType
        public async Task CheckFollowed_InvalidProfileType(string profileType)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.CheckFollowed("1231231312abdc", profileType);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("1232abdc", "1232abde", true)] //Should return true, existing active Inter User setup
        [InlineData("1232abdc", "1232abde", false)] //Should return false, existing inActive Inter User setup
        [InlineData("1232abdd", "1232abdb", false)] //Should return false, non existing Inter User setup
        public async Task CheckFollowed_Valid(string passiveProfileID, string activeProfileID, bool interUserSetUpIsAcitve)
        {
            //arrange
            string knownPassiveProfileID = "1232abdc";
            string knownActiveProfileID = "1232abde";

            Follower follower = new Follower
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();
            mockFollowerLogic.MockGetByActiveProfileIDandPassiveProfileID(activeProfileID, knownActiveProfileID, passiveProfileID, knownPassiveProfileID, ProfileType.Professional, interUserSetUpIsAcitve);

            //set user ID in claims
            FollowerController controller = SetUpController(mockFollowerLogic, passiveProfileID, ProfileType.Professional);

            //act
            IActionResult response = await controller.CheckFollowed(activeProfileID, ProfileType.Professional.ToString());

            //assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as InterUserBoolResponse;

            if (activeProfileID == knownActiveProfileID && passiveProfileID == knownPassiveProfileID && interUserSetUpIsAcitve)
            {
                Assert.True(resultValue.Result);
            }
            else
            {
                Assert.False(resultValue.Result);
            }
            mockFollowerLogic.VerifyAll();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("NonHexadecimalID")]//non hexadecimal ID
        public async Task CheckFollowing_InvalidProfileID(string profileID)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.CheckFollowing(profileID, "Company");

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("invalidProfileType")]//invalid ProfileType
        public async Task CheckFollowing_InvalidProfileType(string profileType)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.CheckFollowing("1231231312abdc", profileType);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("1232abdc", "1232abde", true)] //Should return true, existing active Inter User setup
        [InlineData("1232abdc", "1232abde", false)] //Should return false, existing inActive Inter User setup
        [InlineData("1232abdd", "1232abdb", false)] //Should return false, non existing Inter User setup
        public async Task CheckFollowing_Valid(string passiveProfileID, string activeProfileID, bool interUserSetUpIsAcitve)
        {
            //arrange
            string knownPassiveProfileID = "1232abdc";
            string knownActiveProfileID = "1232abde";

            Follower follower = new Follower
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();
            mockFollowerLogic.MockGetByActiveProfileIDandPassiveProfileID(activeProfileID, knownActiveProfileID, passiveProfileID, knownPassiveProfileID, ProfileType.Professional, interUserSetUpIsAcitve);

            //set user ID in claims
            FollowerController controller = SetUpController(mockFollowerLogic, activeProfileID, ProfileType.Professional);

            //act
            IActionResult response = await controller.CheckFollowing(passiveProfileID, ProfileType.Professional.ToString());

            //assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as InterUserBoolResponse;

            if (activeProfileID == knownActiveProfileID && passiveProfileID == knownPassiveProfileID && interUserSetUpIsAcitve)
            {
                Assert.True(resultValue.Result);
            }
            else
            {
                Assert.False(resultValue.Result);
            }
            mockFollowerLogic.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("NonHexadecimalID")]//non hexadecimal ID
        public async Task GetAllFollowers_InvalidProfileID(string profileID)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.GetAllFollowers(profileID, "Company");

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("invalidProfileType")]//invalid ProfileType
        public async Task GetAllFollowers_InvalidProfileType(string profileType)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.GetAllFollowers("1231231312abdc", profileType);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("123456fd")]//test for empty scenario
        [InlineData("123456f")]
        public async Task GetAllFollowers_WithProfileID(string profileId)
        {
            //Arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();
            string knownProfileId = "123456f";

            mockFollowerLogic.MockGetAllByPassiveProfileID(profileId, knownProfileId, ProfileType.Company, ProfileType.Company);


            //set profile ID in claims
            FollowerController controller = SetUpController(mockFollowerLogic, profileId, ProfileType.Company);

            //Act
            IActionResult response = await controller.GetAllFollowers(profileId, "Company");

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

            mockFollowerLogic.VerifyAll();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("NonHexadecimalID")]//non hexadecimal ID
        public async Task GetAllFollowing_InvalidProfileID(string profileID)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.GetAllFollowing(profileID, "Company");

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("invalidProfileType")]//invalid ProfileType
        public async Task GetAllFollowing_InvalidProfileType(string profileType)
        {
            //arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();

            var controller = new FollowerController(mockFollowerLogic.Object);

            //act
            IActionResult response = await controller.GetAllFollowing("1231231312abdc", profileType);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("123456fd")]//test for empty scenario
        [InlineData("123456f")]
        public async Task GetAllFollowing_WithProfileID(string profileId)
        {
            //Arrange
            MockFollowerLogic mockFollowerLogic = new MockFollowerLogic();
            string knownProfileId = "123456f";

            mockFollowerLogic.MockGetAllByActiveProfileID(profileId, knownProfileId, ProfileType.Company, ProfileType.Company);


            //set profile ID in claims
            FollowerController controller = SetUpController(mockFollowerLogic, profileId, ProfileType.Company);

            //Act
            IActionResult response = await controller.GetAllFollowing(profileId, "Company");

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

            mockFollowerLogic.VerifyAll();
        }
    }
}