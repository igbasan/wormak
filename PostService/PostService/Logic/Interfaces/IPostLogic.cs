using PostService.Models;
using PostService.Models.Implementations;
using PostService.Models.Implementations.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Logic.Interfaces
{
    public interface IPostLogic
    {
        Task<Post> PostAsync(PostNewRequest post);
        Task<Post> UpdatePostAsync(PostRequest post);
        Task<Post> GetByIDAsync(string id);
        Task<Comment> CommentAsync(CommentRequest request);
        Task<Like> LikeAsync(Request request);
        Task<CountList<Post>> GetPostFeedAsync(string profileId, ProfileType profileType, List<Interest> interests, int skip, int take);
    }
}
