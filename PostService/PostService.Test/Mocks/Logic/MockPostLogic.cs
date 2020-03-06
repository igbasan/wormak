using Moq;
using PostService.Logic.Interfaces;
using PostService.Models;
using PostService.Models.Exceptions;
using PostService.Models.Implementations;
using PostService.Models.Implementations.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Test.Mocks.Logic
{
    public class MockPostLogic : Mock<IPostLogic>
    {
        public bool PostCreated { get; set; }
        public bool PostUpdated { get; set; }
        public bool CommentAdded { get; set; }
        public bool Liked { get; set; }

        public void MockPost()
        {
            Setup(x => x.PostAsync(
                It.IsAny<PostNewRequest>()
                )).Callback(() => PostCreated = true)
                .Returns<PostNewRequest>(s =>
                {
                    return Task.FromResult<Post>(new Post
                    {
                        Message = s.Message,
                        ProfileID = s.ProfileID,
                        ProfileType = s.ProfileType,
                        Tags = s.Tags,
                        Title = s.Title
                    });
                });
        }

        public void MockPostWithException()
        {
            Setup(x => x.PostAsync(
                It.IsAny<PostNewRequest>()
                ))
                .Throws(new PostServiceException("Test Exception"));
        }
        public void MockUpdatePost()
        {
            Setup(x => x.UpdatePostAsync(
                It.IsAny<PostRequest>()
                )).Callback(() => PostUpdated = true)
                .Returns<PostRequest>(s =>
                {
                    return Task.FromResult<Post>(new Post
                    {
                        Message = s.Message,
                        ProfileID = s.ProfileID,
                        ProfileType = s.ProfileType,
                        Tags = s.Tags,
                        Title = s.Title,
                        Id = s.PostID
                    });
                });
        }
        public void MockUpdatePostWithException()
        {
            Setup(x => x.UpdatePostAsync(
                It.IsAny<PostRequest>()
                ))
                .Throws(new PostServiceException("Test Exception"));
        }
        public void MockComment()
        {
            Setup(x => x.CommentAsync(
                It.IsAny<CommentRequest>()
                )).Callback(() => CommentAdded = true)
                .Returns<CommentRequest>(s =>
                {
                    return Task.FromResult<Comment>(new Comment
                    {
                        Message = s.Message,
                        ProfileID = s.ProfileID
                    });
                });
        }
        public void MockCommentWithException()
        {
            Setup(x => x.CommentAsync(
                It.IsAny<CommentRequest>()
                ))
                .Throws(new PostServiceException("Test Exception"));
        }

        public void MockLike()
        {
            Setup(x => x.LikeAsync(
                It.IsAny<Request>()
                )).Callback(() => Liked = true)
                .Returns<Request>(s =>
                {
                    return Task.FromResult<Like>(new Like
                    {
                        ProfileID = s.ProfileID
                    });
                });
        }
        public void MockLikeWithException()
        {
            Setup(x => x.LikeAsync(
                It.IsAny<Request>()
                ))
                .Throws(new PostServiceException("Test Exception"));
        }

        public void MockGetByID(string id, string knownId, Post post)
        {
            Post output = null;
            if (!string.IsNullOrWhiteSpace(id) && id == knownId)
            {
                output = post;
            }

            Setup(x => x.GetByIDAsync(
                It.Is<string>(c => c == id)
                )).Returns(Task.FromResult<Post>(output));
        }

        public void MockGetPostFeed(string profileId, ProfileType profileType, bool hasFeed)
        {
            CountList<Post> output = new CountList<Post>();
            if (hasFeed)
            {
                output = new CountList<Post> { new Post { Message = "Message" }, new Post { Message = "Message" } };
                output.TotalCount = 2;
            }

            Setup(x => x.GetPostFeedAsync(
                It.Is<string>(c => c == profileId),
                It.Is<ProfileType>(c => c == profileType),
                It.Is<List<Interest>>(c => c != null && c.Count == 2),
                It.Is<int>(c => c == 0),
                It.Is<int>(c => c > 0)
                )).Returns(Task.FromResult(output));
        }
    }
}
