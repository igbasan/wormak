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
    public class TestimonialControllerTest
    {

        [Fact]
        public async Task AddTestimonial_NullModel()
        {
            //arrange
            MockTestimonialLogic mockTestimonialLogic = new MockTestimonialLogic();

            var controller = new TestimonialController(mockTestimonialLogic.Object);

            //act
            IActionResult response = await controller.AddTestimonial(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task AddTestimonial_InvalidModel()
        {
            //arrange
            MockTestimonialLogic mockTestimonialLogic = new MockTestimonialLogic();

            var controller = new TestimonialController(mockTestimonialLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.AddTestimonial(new TestimonialRequest());

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        private static TestimonialController SetUpController(MockTestimonialLogic mockTestimonialLogic, string profileID, ProfileType profileType)
        {
            var controller = new TestimonialController(mockTestimonialLogic.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId", profileID));
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType", profileType.ToString()));
            controller.ControllerContext.HttpContext.User.AddIdentity(identity);
            return controller;
        }

        [Fact]
        public async Task AddTestimonial_WithException()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Testimonial testimonial = new Testimonial
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser,
                Rating = 2,
                Comment = "Test Comment"
            };

            MockTestimonialLogic mockTestimonialLogic = new MockTestimonialLogic();
            mockTestimonialLogic.MockSetUpRelationshipWithException(testimonial);

            //set user ID in claims
            TestimonialController controller = SetUpController(mockTestimonialLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.AddTestimonial(new TestimonialRequest
            {
                ProfileId = passiveProfileID,
                ProfileType = ProfileType.Company,
                Rating = 2,
                Comment = "Test Comment"
            });

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockTestimonialLogic.TestimonialSetUp);
            Assert.False(mockTestimonialLogic.TestimonialSetUpToDeactivate);
            mockTestimonialLogic.VerifyAll();
        }


        [Fact]
        public async Task AddTestimonial_Valid()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Testimonial testimonial = new Testimonial
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser,
                Rating = 2,
                Comment = "Test Comment"
            };

            MockTestimonialLogic mockTestimonialLogic = new MockTestimonialLogic();
            mockTestimonialLogic.MockSetUpRelationship(testimonial);

            //set user ID in claims
            TestimonialController controller = SetUpController(mockTestimonialLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.AddTestimonial(new TestimonialRequest
            {
                ProfileId = passiveProfileID,
                ProfileType = ProfileType.Company,
                Rating = 2,
                Comment = "Test Comment"
            });

            //assert
            Assert.True(mockTestimonialLogic.TestimonialSetUp);
            Assert.False(mockTestimonialLogic.TestimonialSetUpToDeactivate);
            Assert.IsType<JsonResult>(response);
            mockTestimonialLogic.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("NonHexadecimalID")]//non hexadecimal ID
        public async Task RemoveTestimonial_InvalidProfileID(string profileID)
        {
            //arrange
            MockTestimonialLogic mockTestimonialLogic = new MockTestimonialLogic();

            var controller = new TestimonialController(mockTestimonialLogic.Object);

            //act
            IActionResult response = await controller.RemoveTestimonial(profileID, "Company");

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("invalidProfileType")]//invalid ProfileType
        public async Task RemoveTestimonial_InvalidProfileType(string profileType)
        {
            //arrange
            MockTestimonialLogic mockTestimonialLogic = new MockTestimonialLogic();

            var controller = new TestimonialController(mockTestimonialLogic.Object);

            //act
            IActionResult response = await controller.RemoveTestimonial("1231231312abdc", profileType);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task RemoveTestimonial_WithException()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Testimonial testimonial = new Testimonial
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockTestimonialLogic mockTestimonialLogic = new MockTestimonialLogic();
            mockTestimonialLogic.MockSetUpRelationshipWithException(testimonial);

            //set profile ID in claims
            TestimonialController controller = SetUpController(mockTestimonialLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.RemoveTestimonial(passiveProfileID, ProfileType.Company.ToString());

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockTestimonialLogic.TestimonialSetUp);
            Assert.True(mockTestimonialLogic.TestimonialSetUpToDeactivate);
            mockTestimonialLogic.VerifyAll();
        }


        [Fact]
        public async Task RemoveTestimonial_Valid()
        {
            //arrange
            string passiveProfileID = "1232abdc";
            string activeProfileID = "1232abde";

            Testimonial testimonial = new Testimonial
            {
                PassiveProfileID = passiveProfileID,
                PassiveProfileType = ProfileType.Company,
                ActiveProfileID = activeProfileID,
                ActiveProfileType = ProfileType.GeneralUser
            };

            MockTestimonialLogic mockTestimonialLogic = new MockTestimonialLogic();
            mockTestimonialLogic.MockSetUpRelationship(testimonial);

            //set user ID in claims
            TestimonialController controller = SetUpController(mockTestimonialLogic, activeProfileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.RemoveTestimonial(passiveProfileID, ProfileType.Company.ToString());

            //assert
            Assert.True(mockTestimonialLogic.TestimonialSetUp);
            Assert.True(mockTestimonialLogic.TestimonialSetUpToDeactivate);
            Assert.IsType<JsonResult>(response);
            mockTestimonialLogic.VerifyAll();
        }

        [Theory]
        [InlineData("InvalidProfileId")]//test for empty scenario
        [InlineData("profileId")]
        public async Task GetTestimonials_WithProfieID(string profileId)
        {
            //Arrange
            MockTestimonialLogic mockTestimonialLogic = new MockTestimonialLogic();
            string knownProfileId = "profileId";

            mockTestimonialLogic.MockGetAllByPassiveProfileID(profileId, knownProfileId, ProfileType.Company, ProfileType.Company);


            //set profile ID in claims
            TestimonialController controller = SetUpController(mockTestimonialLogic, profileId, ProfileType.Company);

            //Act
            IActionResult response = await controller.GetTestimonials(0, 10);

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as InterUserListWithRatingResponse<Profile>;

            //test Json data
            Assert.NotNull(resultValue);

            if (profileId == knownProfileId)
            {
                Assert.NotNull(resultValue.Result);
                Assert.True(resultValue.Result.Count > 0);
                Assert.Equal(2, resultValue.TotalCount);
                Assert.Equal(1.5, resultValue.AverageRating);

                Assert.True(resultValue.Result.All(c => c.Id == "Result" && c.ProfileType == ProfileType.Professional));
            }
            else
            {
                Assert.NotNull(resultValue.Result);
                Assert.True(resultValue.Result.Count == 0);
            }

            mockTestimonialLogic.VerifyAll();
        }
    }
}
