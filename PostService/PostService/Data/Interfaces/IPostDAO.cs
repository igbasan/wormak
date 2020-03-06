using PostService.Models;
using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Data.Interfaces
{
    public interface IPostDAO
    {
        Task<Post> SaveAsync(Post post, List<string> feedProfileIds);
        Task<Post> UpdateAsync(Post post, List<Interest> oldInterestList);
        Task<Post> GetByIDAsync(string id);
        Task<List<Post>> GetByIDsAsync(List<string> ids);
        Task<CountList<Post>> GetPostFeedAsync(string profileId, List<string> profileIds, List<Interest> interests, int skip, int take);
    }
}
