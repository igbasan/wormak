using MongoDB.Driver;
using PostService.Data.Interfaces;
using PostService.Models;
using PostService.Models.Implementations;
using PostService.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Data.Mongo
{
    public class PostDAO : IPostDAO
    {
        protected IMongoCollection<Post> _posts;
        protected IMongoDatabase database;
        public PostDAO(IStoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);
            _posts = database.GetCollection<Post>(settings.PostsCollectionName);
        }
        public async Task<Post> GetByIDAsync(string id)
        {
            var posts = await _posts.FindAsync(g => g.Id == id);
            return await posts.FirstOrDefaultAsync();
        }

        public async Task<List<Post>> GetByIDsAsync(List<string> ids)
        {
            var posts = await _posts.FindAsync(g => ids.Contains(g.Id));
            return posts.ToList() ?? new List<Post>();
        }

        public async Task<CountList<Post>> GetPostFeedAsync(string profileId, List<string> profileIds, List<Interest> interests, int skip, int take)
        {
            List<string> profileIdsToSearch = new List<string>() { profileId };
            if (profileIds != null) profileIdsToSearch.AddRange(profileIds);
            profileIdsToSearch = profileIdsToSearch.Distinct().ToList();

            var fluentFinder = _posts.Find<Post>(g => profileIdsToSearch.Contains(g.ProfileID) || g.Tags.Any(n => interests.Contains(n)));

            if (take >= 0) fluentFinder.Skip(skip).Limit(take);

            var interUsers = await fluentFinder.SortBy(x => x.Id).ToCursorAsync();

            List<Post> postList = await interUsers.ToListAsync();

            CountList<Post> result = new CountList<Post>();
            if (postList != null) result.AddRange(postList);

            return result;
        }

        public async Task<Post> SaveAsync(Post post, List<string> feedProfileIds)
        {
            await _posts.InsertOneAsync(post);
            return post;
        }

        public async Task<Post> UpdateAsync(Post post, List<Interest> oldInterestList)
        {
            var filter = Builders<Post>.Filter.Eq(s => s.Id, post.Id);
            await _posts.ReplaceOneAsync(filter, post);
            return post;
        }
    }
}
