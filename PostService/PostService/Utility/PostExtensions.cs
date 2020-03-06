using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Utility
{
    public static class PostExtensions
    {
        public static PostResult ToResult(this Post post)
        {
            return new PostResult
            {
                Comments = post.Comments?.Select(c => c.ToResult()).ToList() ?? new List<CommentResult>(),
                DatePosted = post.DatePosted,
                IsEdited = post.IsEdited,
                Likes = post.Likes?.Count ?? 0,
                Message = post.Message,
                PostId = post.Id,
                ProfileID = post.ProfileIDRaw,
                ProfileName = post.ProfileName,
                ProfileType = post.ProfileType,
                Tags = post.Tags,
                Title = post.Title
            };
        }
        public static CommentResult ToResult(this Comment comment)
        {
            return new CommentResult
            {
                DateCommented = comment.DateAdded,
                Message = comment.Message,
                ProfileID = comment.ProfileIDRaw,
                ProfileName = comment.ProfileName,
                ProfileType = comment.ProfileType
            };
        }
    }
}
