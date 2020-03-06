using PostService.Logic.Implementations;
using PostService.Models;
using PostService.Models.Exceptions;
using PostService.Models.Implementations;
using PostService.Models.Implementations.Requests;
using PostService.Test.Mocks.Data;
using PostService.Test.Mocks.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PostService.Test.Tests.Logic
{
    public class PostLogicTest
    {
        [Fact]
        public async Task PostAsync_Null()
        {
            //Arrange
            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();
            PostNewRequest post = null;

            //Act
            var exception = await Record.ExceptionAsync(() => new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .PostAsync(post));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task PostAsync_Valid()
        {
            //Arrange
            PostNewRequest post = new PostNewRequest
            {
                Message = "Message",
                ProfileID = "ProfileID",
                ProfileType = ProfileType.Company,
                Tags = new List<Interest> { Interest.Art },
                Title = "Title"
            };

            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();

            mockFollowerLogic.MockGetFollowerProfilesByProfileID(post.ProfileID, post.ProfileType, true);
            mockProfileLogic.MockGetProfilesByInterests(true);

            mockPostDAO.MockSave();

            //Act
            Post result = await new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .PostAsync(post);

            //Assert        
            Assert.Equal(result.ProfileID, $"{post.ProfileType}_{post.ProfileID}");
            Assert.Equal(result.Message, post.Message);
            Assert.Equal(result.Tags, post.Tags);
            Assert.Equal(result.Title, post.Title);
            Assert.InRange(result.DatePosted, DateTime.Now.AddSeconds(-10), DateTime.Now.AddSeconds(10));
            Assert.True(mockPostDAO.PostCreated);
            Assert.False(result.IsEdited);
            mockPostDAO.VerifyAll();
            mockFollowerLogic.VerifyAll();
            mockPostDAO.VerifyAll();

        }

        [Fact]
        public async Task UpdatePostAsync_Null()
        {
            //Arrange
            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();
            PostRequest post = null;

            //Act
            var exception = await Record.ExceptionAsync(() => new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .UpdatePostAsync(post));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task UpdatePostAsync_InvalidPostID()
        {
            //Arrange
            PostRequest post = new PostRequest
            {
                Message = "Message",
                ProfileID = "ProfileID",
                ProfileType = ProfileType.Company,
                Tags = new List<Interest> { Interest.Art },
                Title = "Title",
                PostID = "PostID"
            };

            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();

            //PostID is not known, the method would return null
            mockPostDAO.MockGetByID(post.PostID, "KnownPostID");

            //Act
            var exception = await Record.ExceptionAsync(() => new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .UpdatePostAsync(post));

            //Assert
            Assert.IsType<PostServiceException>(exception);
            mockPostDAO.VerifyAll();

        }

        [Theory]
        [InlineData("KnownProfileID")]
        [InlineData("UnKnownProfileID")]
        public async Task UpdatePostAsync_Valid(string profileID)
        {
            //Arrange
            PostRequest post = new PostRequest
            {
                Message = "Message",
                ProfileID = profileID,
                ProfileType = ProfileType.Company,
                Tags = new List<Interest> { Interest.Art },
                Title = "Title",
                PostID = "PostID"
            };

            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();
            string knownProfileID = "KnownProfileID";

            //get post with specified profile ID
            mockPostDAO.MockGetByID(post.PostID, "PostID", $"Company_{knownProfileID}");
            mockFollowerLogic.MockGetFollowerProfilesByProfileID(post.ProfileID, post.ProfileType, true);
            mockProfileLogic.MockGetProfilesByInterests(true);

            if (profileID == knownProfileID)
            {
                mockPostDAO.MockUpdate();

                //Act
                Post result = await new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
                .UpdatePostAsync(post);

                //Assert        
                Assert.Equal(result.ProfileID, $"{post.ProfileType}_{post.ProfileID}");
                Assert.Equal(result.Message, post.Message);
                Assert.Equal(result.Tags, post.Tags);
                Assert.Equal(result.Title, post.Title);
                Assert.Equal(result.Id, post.PostID);
                Assert.True(mockPostDAO.PostUpdated);
                Assert.True(result.IsEdited);
                mockProfileLogic.VerifyAll();
                mockFollowerLogic.VerifyAll();
            }
            else //when the update is not done by the originating user
            {
                //Act
                var exception = await Record.ExceptionAsync(() => new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
                .UpdatePostAsync(post));

                //Assert
                Assert.IsType<PostServiceException>(exception);
            }
            mockPostDAO.VerifyAll();

        }

        [Fact]
        public async Task CommentAsync_Null()
        {
            //Arrange
            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();
            CommentRequest comment = null;

            //Act
            var exception = await Record.ExceptionAsync(() => new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .CommentAsync(comment));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task CommentAsync_InvalidPostID()
        {
            //Arrange
            CommentRequest comment = new CommentRequest
            {
                Message = "Message",
                ProfileID = "ProfileID",
                ProfileType = ProfileType.Company,
                PostID = "PostID"
            };

            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();

            //PostID is not known, the method would return null
            mockPostDAO.MockGetByID(comment.PostID, "KnownPostID");

            //Act
            var exception = await Record.ExceptionAsync(() => new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .CommentAsync(comment));

            //Assert
            Assert.IsType<PostServiceException>(exception);
            mockPostDAO.VerifyAll();

        }

        [Fact]
        public async Task CommentAsync_Valid()
        {
            //Arrange
            CommentRequest comment = new CommentRequest
            {
                Message = "Message",
                ProfileID = "ProfileID",
                ProfileType = ProfileType.Company,
                PostID = "PostID"
            };

            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();

            //get post with specified profile ID
            mockPostDAO.MockGetByID(comment.PostID, "PostID", null, null);
            mockPostDAO.MockUpdate(true);

            //Act
            Comment result = await new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .CommentAsync(comment);

            //Assert        
            Assert.Equal(result.ProfileID, $"{comment.ProfileType}_{comment.ProfileID}");
            Assert.Equal(result.Message, comment.Message);
            Assert.InRange(result.DateAdded, DateTime.Now.AddSeconds(-10), DateTime.Now.AddSeconds(10));
            Assert.True(mockPostDAO.PostUpdated);

            mockPostDAO.VerifyAll();
        }

        [Fact]
        public async Task LikeAsync_Null()
        {
            //Arrange
            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();
            Request likeRequest = null;

            //Act
            var exception = await Record.ExceptionAsync(() => new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .LikeAsync(likeRequest));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task LikeAsync_InvalidPostID()
        {
            //Arrange
            Request likeRequest = new Request
            {
                ProfileID = "ProfileID",
                ProfileType = ProfileType.Company,
                PostID = "PostID"
            };

            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();

            //PostID is not known, the method would return null
            mockPostDAO.MockGetByID(likeRequest.PostID, "KnownPostID");

            //Act
            var exception = await Record.ExceptionAsync(() => new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .LikeAsync(likeRequest));

            //Assert
            Assert.IsType<PostServiceException>(exception);
            mockPostDAO.VerifyAll();

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task LikeAsync_Valid(bool likeExists)
        {
            //Arrange
            Request likeRequest = new Request
            {
                ProfileID = "ProfileID",
                ProfileType = ProfileType.Company,
                PostID = "PostID"
            };

            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();

            //get post with specified profile ID
            List<Like> likes = new List<Like>();
            if (likeExists) likes.Add(new Like { ProfileID = $"{likeRequest.ProfileType}_{likeRequest.ProfileID}" });
            int likeCountbefore = likes.Count;

            mockPostDAO.MockGetByID(likeRequest.PostID, "PostID", null, null, likes);
            if (!likeExists) mockPostDAO.MockUpdate(true);

            //Act
            Like result = await new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .LikeAsync(likeRequest);

            //Assert        
            Assert.Equal(result.ProfileID, $"{likeRequest.ProfileType}_{likeRequest.ProfileID}");

            if (likeExists) Assert.Equal(likeCountbefore, likes.Count);
            else
            {
                Assert.Equal(likeCountbefore + 1, likes.Count);
                Assert.InRange(result.DateLiked, DateTime.Now.AddSeconds(-10), DateTime.Now.AddSeconds(10));
                Assert.True(mockPostDAO.PostUpdated);
            }

            mockPostDAO.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task GetPostFeedAsync_NullOrEmpty(string profileId)
        {
            //Arrange
            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();

            //Act
            var exception = await Record.ExceptionAsync(() => new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .GetPostFeedAsync(profileId, ProfileType.Company, null, 0, 10));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("KnownProfileID", ProfileType.Company)]
        [InlineData("ProfileID", ProfileType.Professional)]
        public async Task GetPostFeedAsync_withNoCache(string profileId, ProfileType profileType)
        {
            //Arrange
            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();

            string KnownProfileID = "KnownProfileID";
            bool hasFeed = KnownProfileID == profileId;

            List<Comment> comments = new List<Comment> {
                new Comment {
                    ProfileID = "ProfileID"
                },
                new Comment {
                    ProfileID = "ProfileID"
                }
            };
            mockPostDAO.MockGetPostFeed(hasFeed, "PostProfileID", comments);
            mockFollowerLogic.MockGetFollowingProfilesByProfileID(profileId, profileType, hasFeed);
            mockProfileLogic.MockGetProfiles(true);

            //Act
            CountList<Post> feeds = await new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
                .GetPostFeedAsync(profileId, profileType, new List<Interest> { Interest.Business, Interest.Pleasure }, 0, 10);

            //Assert
            if (KnownProfileID != profileId)
            {
                Assert.NotNull(feeds);
                Assert.True(feeds.Count == 0);
                Assert.True(feeds.TotalCount == 0);
            }
            else
            {
                Assert.NotNull(feeds);
                Assert.True(feeds.Count == 2);
                Assert.True(feeds.TotalCount == 2);
                Assert.True(feeds.All(v => v.Id.StartsWith("Result")));
                Assert.True(feeds.All(v => v.ProfileName == "Name"));
                Assert.True(feeds.All(v => v.Comments.All(v => v.ProfileName == "Name")));
                mockProfileLogic.VerifyAll();
            }

            mockPostDAO.VerifyAll();
            mockFollowerLogic.VerifyAll();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task GetByID_NullOrEmpty(string id)
        {
            //Arrange
            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();

            //Act
            var exception = await Record.ExceptionAsync(() => new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
            .GetByIDAsync(id));

            //Assert
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("KnownId")]
        [InlineData("Id")]
        public async Task GetByID_NotNull(string id)
        {
            //Arrange
            var mockPostDAO = new MockPostDAO();
            var mockProfileLogic = new MockProfileLogic();
            var mockFollowerLogic = new MockFollowerLogic();

            string KnownId = "KnownId";
            List<Comment> comments = new List<Comment> {
                new Comment {
                    ProfileID = "ProfileID"
                },
                new Comment {
                    ProfileID = "ProfileID"
                }
            };
            mockPostDAO.MockGetByID(id, KnownId, "ProfileID", comments);
            mockProfileLogic.MockGetProfiles(true);

            //Act
            Post post = await new PostLogic(mockPostDAO.Object, mockProfileLogic.Object, mockFollowerLogic.Object)
                .GetByIDAsync(id);

            //Assert
            if (KnownId == id)
            {
                Assert.NotNull(post);
                Assert.Equal("Name", post.ProfileName);
                Assert.True(post.Comments.All(v => v.ProfileName == "Name"));
                mockProfileLogic.VerifyAll();
            }
            else
            {
                Assert.Null(post);
            }

            mockPostDAO.VerifyAll();
        }
    }
}
