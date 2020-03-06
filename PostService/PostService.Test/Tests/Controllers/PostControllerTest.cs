using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostService.Controllers;
using PostService.Models;
using PostService.Models.Implementations;
using PostService.Models.Implementations.Requests;
using PostService.Test.Mocks.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PostService.Test.Tests.Controllers
{
    public class PostControllerTest
    {
        [Fact]
        public async Task Post_NullModel()
        {
            //arrange
            MockPostLogic mockPostLogic = new MockPostLogic();

            var controller = new PostController(mockPostLogic.Object);

            //act
            IActionResult response = await controller.PostAsync(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Post_InvalidModel()
        {
            //arrange
            MockPostLogic mockPostLogic = new MockPostLogic();

            var controller = new PostController(mockPostLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.PostAsync(new PostNewRequest());

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        private static PostController SetUpController(MockPostLogic mockPostLogic, string profileID, ProfileType profileType)
        {
            var controller = new PostController(mockPostLogic.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileId", profileID));
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/profileType", profileType.ToString()));
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/interests", "[\"Business\",\"Pleasure\"]"));
            controller.ControllerContext.HttpContext.User.AddIdentity(identity);
            return controller;
        }

        [Fact]
        public async Task Post_WithException()
        {
            //arrange
            string profileID = "1232abdc";

            PostNewRequest postRequest = new PostNewRequest
            {
                ProfileID = profileID,
                ProfileType = ProfileType.Company,
                Message = "Message",
                Title = "Title",
                Tags = new List<Interest>()
            };

            MockPostLogic mockPostLogic = new MockPostLogic();
            mockPostLogic.MockPostWithException();

            //set user ID in claims
            PostController controller = SetUpController(mockPostLogic, profileID, ProfileType.Company);

            //act
            IActionResult response = await controller.PostAsync(new PostNewRequest
            {
                Message = "Message",
                Title = "Title",
                Tags = new List<Interest>()
            });

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockPostLogic.PostCreated);
            mockPostLogic.VerifyAll();
        }


        [Fact]
        public async Task Post_Valid()
        {
            //arrange
            string profileID = "1232abdc";

            PostNewRequest postRequest = new PostNewRequest
            {
                ProfileID = profileID,
                ProfileType = ProfileType.Company,
                Message = "Message",
                Title = "Title",
                Tags = new List<Interest>()
            };

            MockPostLogic mockPostLogic = new MockPostLogic();
            mockPostLogic.MockPost();

            //set user ID in claims
            PostController controller = SetUpController(mockPostLogic, profileID, ProfileType.Company);

            //act
            PostNewRequest request = new PostNewRequest
            {
                Message = "Message",
                Title = "Title",
                Tags = new List<Interest>()
            };

            IActionResult response = await controller.PostAsync(request);

            //assert
            Assert.True(mockPostLogic.PostCreated);
            Assert.Equal(profileID, request.ProfileID);
            Assert.Equal(ProfileType.Company, request.ProfileType);
            Assert.IsType<JsonResult>(response);
            mockPostLogic.VerifyAll();
        }
        [Fact]
        public async Task UpdatePost_NullModel()
        {
            //arrange
            MockPostLogic mockPostLogic = new MockPostLogic();

            var controller = new PostController(mockPostLogic.Object);

            //act
            IActionResult response = await controller.UpdatePostAsync(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task UpdatePost_InvalidModel()
        {
            //arrange
            MockPostLogic mockPostLogic = new MockPostLogic();

            var controller = new PostController(mockPostLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.UpdatePostAsync(new PostRequest());

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task UpdatePost_WithException()
        {
            //arrange
            string profileID = "1232abdc";

            PostRequest postRequest = new PostRequest
            {
                ProfileID = profileID,
                ProfileType = ProfileType.Company,
                Message = "Message",
                Title = "Title",
                Tags = new List<Interest>()
            };

            MockPostLogic mockPostLogic = new MockPostLogic();
            mockPostLogic.MockUpdatePostWithException();

            //set user ID in claims
            PostController controller = SetUpController(mockPostLogic, profileID, ProfileType.Company);

            //act
            IActionResult response = await controller.UpdatePostAsync(new PostRequest
            {
                Message = "Message",
                Title = "Title",
                Tags = new List<Interest>()
            });

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockPostLogic.PostUpdated);
            mockPostLogic.VerifyAll();
        }

        [Fact]
        public async Task UpdatePost_Valid()
        {
            //arrange
            string profileID = "1232abdc";
            string postId = "1232abdcf";

            PostRequest postRequest = new PostRequest
            {
                ProfileID = profileID,
                ProfileType = ProfileType.Company,
                Message = "Message",
                Title = "Title",
                Tags = new List<Interest>(),
                PostID = postId
            };

            MockPostLogic mockPostLogic = new MockPostLogic();
            mockPostLogic.MockUpdatePost();

            //set user ID in claims
            PostController controller = SetUpController(mockPostLogic, profileID, ProfileType.Company);

            //act
            PostRequest request = new PostRequest
            {
                Message = "Message",
                Title = "Title",
                Tags = new List<Interest>(),
                PostID = postId
            };
            IActionResult response = await controller.UpdatePostAsync(request);

            //assert
            Assert.True(mockPostLogic.PostUpdated);
            Assert.Equal(profileID, request.ProfileID);
            Assert.Equal(ProfileType.Company, request.ProfileType);
            Assert.IsType<JsonResult>(response);
            mockPostLogic.VerifyAll();
        }

        [Fact]
        public async Task Comment_NullModel()
        {
            //arrange
            MockPostLogic mockPostLogic = new MockPostLogic();

            var controller = new PostController(mockPostLogic.Object);

            //act
            IActionResult response = await controller.CommentAsync(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Comment_InvalidModel()
        {
            //arrange
            MockPostLogic mockPostLogic = new MockPostLogic();

            var controller = new PostController(mockPostLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.CommentAsync(new CommentRequest());

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Comment_WithException()
        {
            //arrange
            string profileID = "1232abdc";

            CommentRequest commentRequest = new CommentRequest
            {
                ProfileID = profileID,
                ProfileType = ProfileType.Company,
                Message = "Message"
            };

            MockPostLogic mockPostLogic = new MockPostLogic();
            mockPostLogic.MockCommentWithException();

            //set user ID in claims
            PostController controller = SetUpController(mockPostLogic, profileID, ProfileType.Company);

            //act
            IActionResult response = await controller.CommentAsync(commentRequest);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockPostLogic.CommentAdded);
            mockPostLogic.VerifyAll();
        }

        [Fact]
        public async Task Comment_Valid()
        {
            //arrange
            string profileID = "1232abdc";
            string postId = "1232abdcf";

            CommentRequest commentRequest = new CommentRequest
            {
                ProfileID = profileID,
                ProfileType = ProfileType.Company,
                Message = "Message",
                PostID = postId
            };

            MockPostLogic mockPostLogic = new MockPostLogic();
            mockPostLogic.MockComment();

            //set user ID in claims
            PostController controller = SetUpController(mockPostLogic, profileID, ProfileType.GeneralUser);

            //act

            CommentRequest request = new CommentRequest
            {
                Message = "Message",
                PostID = postId
            };
            IActionResult response = await controller.CommentAsync(request);

            //assert
            Assert.True(mockPostLogic.CommentAdded);
            Assert.Equal(request.ProfileID, profileID);
            Assert.Equal(ProfileType.GeneralUser, request.ProfileType);
            Assert.IsType<JsonResult>(response);
            mockPostLogic.VerifyAll();
        }

        [Fact]
        public async Task Like_NullModel()
        {
            //arrange
            MockPostLogic mockPostLogic = new MockPostLogic();

            var controller = new PostController(mockPostLogic.Object);

            //act
            IActionResult response = await controller.LikeAsync(null);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Like_InvalidModel()
        {
            //arrange
            MockPostLogic mockPostLogic = new MockPostLogic();

            var controller = new PostController(mockPostLogic.Object);
            controller.ModelState.AddModelError("Test", "Test Error");

            //act
            IActionResult response = await controller.LikeAsync(new Request());

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Like_WithException()
        {
            //arrange
            string profileID = "1232abdc";

            Request likeRequest = new Request
            {
                ProfileID = profileID,
                ProfileType = ProfileType.Company,
                PostID = "1232abdcf"
            };

            MockPostLogic mockPostLogic = new MockPostLogic();
            mockPostLogic.MockLikeWithException();

            //set user ID in claims
            PostController controller = SetUpController(mockPostLogic, profileID, ProfileType.Company);

            //act
            IActionResult response = await controller.LikeAsync(likeRequest);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.False(mockPostLogic.Liked);
            mockPostLogic.VerifyAll();
        }

        [Fact]
        public async Task Like_Valid()
        {
            //arrange
            string profileID = "1232abdc";
            string postId = "1232abdcf";

            Request likeRequest = new Request
            {
                ProfileID = profileID,
                ProfileType = ProfileType.Company,
                PostID = postId
            };

            MockPostLogic mockPostLogic = new MockPostLogic();
            mockPostLogic.MockLike();

            //set user ID in claims

            Request request = new Request
            {
                PostID = postId
            };
            PostController controller = SetUpController(mockPostLogic, profileID, ProfileType.GeneralUser);

            //act
            IActionResult response = await controller.LikeAsync(request);

            //assert
            Assert.True(mockPostLogic.Liked);
            Assert.Equal(profileID, request.ProfileID);
            Assert.Equal(ProfileType.GeneralUser, request.ProfileType);
            Assert.IsType<JsonResult>(response);
            mockPostLogic.VerifyAll();
        }

        [Theory]
        [InlineData("InvalidProfileId")]//test for empty scenario
        [InlineData("profileId")]
        public async Task GetPostFeed_WithProfieID(string profileId)
        {
            //Arrange
            MockPostLogic mockPostLogic = new MockPostLogic();
            string knownProfileId = "profileId";

            mockPostLogic.MockGetPostFeed(profileId, ProfileType.Company, knownProfileId == profileId);


            //set profile ID in claims
            PostController controller = SetUpController(mockPostLogic, profileId, ProfileType.Company);

            //Act
            IActionResult response = await controller.GetPostFeedAsync(0, 10);

            //Assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as PostListResponse<PostResult>;

            //test Json data
            Assert.NotNull(resultValue);

            if (profileId == knownProfileId)
            {
                Assert.NotNull(resultValue.Result);
                Assert.True(resultValue.Result.Count > 0);
                Assert.Equal(2, resultValue.TotalCount);

                Assert.True(resultValue.Result.All(c => c.Message == "Message"));
            }
            else
            {
                Assert.NotNull(resultValue.Result);
                Assert.True(resultValue.Result.Count == 0);
            }

            mockPostLogic.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]//whitespace
        [InlineData("NonHexadecimalID")]//non hexadecimal ID
        public async Task GetDetailsByID_InvalidProfileID(string postId)
        {
            //arrange
            MockPostLogic mockPostLogic = new MockPostLogic();

            var controller = new PostController(mockPostLogic.Object);

            //act
            IActionResult response = await controller.GetDetailsByIDAsync(postId);

            //assert
            Assert.IsType<BadRequestObjectResult>(response);
        }


        [Theory]
        [InlineData("1232abdc")] //has a post
        [InlineData("1232abdce")] //doesn't exist
        public async Task GetDetailsByID_Valid(string postID)
        {
            //arrange
            string profileID = "1232abdc";
            string knownPostID = "1232abdc";

            Post post = new Post
            {
                Comments = new List<Comment>(),
                ProfileID = profileID,
                ProfileType = ProfileType.GeneralUser,
                Message = "Message here",
                Likes = new List<Like>()
            };


            MockPostLogic mockPostLogic = new MockPostLogic();
            mockPostLogic.MockGetByID(postID, knownPostID, post);

            var controller = SetUpController(mockPostLogic, profileID, ProfileType.Professional);

            //act
            IActionResult response = await controller.GetDetailsByIDAsync(postID);

            //assert
            Assert.IsType<JsonResult>(response);
            JsonResult result = response as JsonResult;
            var resultValue = result.Value as PostObjectResponse<PostResult>;

            if (postID == knownPostID)
            {
                Assert.NotNull(resultValue.Result);
                Assert.Equal(post.Id, resultValue.Result.PostId);
            }
            else
            {
                Assert.Null(resultValue.Result);
            }
            mockPostLogic.VerifyAll();
        }

    }
}
