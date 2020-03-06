using PostService.Data.Interfaces;
using PostService.Models;
using PostService.Models.Exceptions;
using PostService.Models.Implementations;
using PostService.Utility;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Data.Redis
{
    public class PostRDAO : IPostDAO
    {
        readonly IRedisConnection connection;
        readonly IPostDAO postDAO;
        protected readonly string Indentifier = "POST";
        public PostRDAO(IPostDAO postDAO, IRedisConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException("connection");
            this.postDAO = postDAO ?? throw new ArgumentNullException("postDAO");
        }
        public async Task<Post> GetByIDAsync(string id)
        {
            //get from cache
            Post thePost = await connection.GetAsync<Post>($"{Indentifier}_ID_{id}");
            if (thePost != null) return thePost;

            //get if cache doesn't have the value
            thePost = await postDAO.GetByIDAsync(id);

            //return user after cache
            if (thePost != null) await connection.SetAsync($"{Indentifier}_ID_{id}", thePost, 1);
            return thePost;
        }

        public async Task<List<Post>> GetByIDsAsync(List<string> ids)
        {
            ids = ids.Distinct().ToList();
            //get from cache
            List<string> idKeys = ids.Select(c => $"{Indentifier}_ID_{c}").ToList();
            List<Post> posts = await connection.GetAsync<Post>(idKeys) ?? new List<Post>();

            //check list to confirm if all keys were gotten
            if (posts != null && idKeys.Count == posts.Count) return posts;

            //only get what we need to from the Db
            var gottenKeys = posts?.Select(c => c.Id);
            List<string> notGottenIds = idKeys.Where(v => !gottenKeys?.Contains(v) ?? true).Select(v => v.Replace($"{Indentifier}_ID_", string.Empty)).ToList();

            //get if cache doesn't have the value
            var newPosts = await postDAO.GetByIDsAsync(notGottenIds);

            //return profiles after cache
            if (newPosts != null && newPosts.Count > 0)
            {
                Dictionary<string, Post> newProfileDict = new Dictionary<string, Post>();
                newPosts.ForEach(b => newProfileDict.Add($"{Indentifier}_ID_{b.Id}", b));
                await connection.SetAsync(newProfileDict);

                // add new proflies
                posts.AddRange(newPosts);
            }
            return posts;
        }

        public async Task<CountList<Post>> GetPostFeedAsync(string profileId, List<string> profileIds, List<Interest> interests, int skip, int take)
        {
            //check user profile interests and followers
            RedisProfile profile = await connection.GetAsync<RedisProfile>($"{Indentifier}_PROFILE_{profileId}");
            List<string> newProfileIds = new List<string>();
            List<Interest> newInterests = new List<Interest>();

            if (profile == null)
            {
                if (profileIds != null) newProfileIds.AddRange(profileIds);
                if (interests != null) newInterests.AddRange(interests);

                profile = new RedisProfile
                {
                    Id = profileId,
                    Interests = interests,
                    ProfileIds = profileIds
                };
                await connection.SetAsync<RedisProfile>($"{Indentifier}_PROFILE_{profileId}", profile);
            }
            else
            {
                if (profile.ProfileIds == null) profile.ProfileIds = new List<string>();
                if (profile.Interests == null) profile.Interests = new List<Interest>();

                newProfileIds = profileIds?.Where(v => profile.ProfileIds.Contains(v)).ToList() ?? new List<string>();
                newInterests = interests?.Where(v => profile.Interests.Contains(v)).ToList() ?? new List<Interest>();

                profile.Interests = interests;
                profile.ProfileIds = profileIds;
                await connection.SetAsync<RedisProfile>($"{Indentifier}_PROFILE_{profileId}", profile);
            }

            //confirm if there are any new interests or followers
            List<SortedSetEntry> allNewFeed = new List<SortedSetEntry>();
            foreach (var id in newProfileIds)
            {
                //get last 100 posts
                var posts = await connection.GetSortedSetWithScoresAsync($"{Indentifier}_POSTED_{id}", 0, 100, Order.Descending);
                if (posts?.Count > 0) allNewFeed.AddRange(posts);
            }

            foreach (var interest in newInterests)
            {
                //get last 100 posts
                var posts = await connection.GetSortedSetWithScoresAsync($"{Indentifier}_POSTTAGFEED_{interest}", 0, 100, Order.Descending);
                if (posts?.Count > 0) allNewFeed.AddRange(posts);
            }


            //add last 100 new interest stories/followers
            if (allNewFeed.Count > 0) await connection.SetSortedSetAsync($"{Indentifier}_FEED_{profileId}", allNewFeed);

            //get feed
            var feed = await connection.GetSortedSetAsync($"{Indentifier}_FEED_{profileId}", skip, take, Order.Descending);
            if (feed.TotalCount > 0)
            {
                if (feed.Count == 0) return new CountList<Post> { TotalCount = 0 };

                List<Post> posts = await GetByIDsAsync(feed);
                if (posts.Count != feed.Count) throw new PostServiceException("Posts retrieve malfunction");

                CountList<Post> postFeeds = new CountList<Post>();
                postFeeds.AddRange(posts);
                postFeeds.TotalCount = feed.TotalCount;

                return postFeeds;
            }

            //if feed is null, get feed from db
            return await postDAO.GetPostFeedAsync(profileId, profileIds, interests, skip, take);
        }

        public async Task<Post> SaveAsync(Post post, List<string> feedProfileIds)
        {
            post = await postDAO.SaveAsync(post, feedProfileIds);

            //return user after cache
            if (post == null) return post;

            //update cached values too
            await connection.SetAsync($"{Indentifier}_ID_{post.Id}", post, 1);

            //add to posted list
            await connection.SetSortedSetAsync($"{Indentifier}_POSTED_{post.ProfileID}", HexToDoubleTool.ConvertToDouble(post.DatePosted), post.Id);
            //add to current user feed
            await connection.SetSortedSetAsync($"{Indentifier}_FEED_{post.ProfileID}", HexToDoubleTool.ConvertToDouble(post.DatePosted), post.Id);

            //add to follower feeds
            if (feedProfileIds?.Count > 0)
            {
                foreach (var feedProfileId in feedProfileIds)
                {
                    await connection.SetSortedSetAsync($"{Indentifier}_FEED_{feedProfileId}", HexToDoubleTool.ConvertToDouble(post.DatePosted), post.Id);
                }
            }
            //add to interest feeds
            if (post.Tags?.Count > 0)
            {
                foreach (var tag in post.Tags)
                {
                    await connection.SetSortedSetAsync($"{Indentifier}_POSTTAGFEED_{tag}", HexToDoubleTool.ConvertToDouble(post.DatePosted), post.Id);
                }
            }

            return post;
        }

        public async Task<Post> UpdateAsync(Post post, List<Interest> oldInterestList)
        {
            post = await postDAO.UpdateAsync(post, oldInterestList);

            //return user after cache
            if (post == null) return post;

            //update cached values too
            await connection.SetAsync($"{Indentifier}_ID_{post.Id}", post, 1);

            if (oldInterestList != null)
            {
                List<Interest> newInterests = post.Tags?.Where(v => !oldInterestList.Contains(v)).ToList() ?? new List<Interest>();
                List<Interest> removeInterests = oldInterestList?.Where(v => !post.Tags?.Contains(v) ?? false).ToList() ?? new List<Interest>();
                //add to interest feeds
                if (newInterests.Count > 0)
                {
                    foreach (var tag in newInterests)
                    {
                        await connection.SetSortedSetAsync($"{Indentifier}_POSTTAGFEED_{tag}", HexToDoubleTool.ConvertToDouble(post.DatePosted), post.Id);
                    }
                }
                //remove from interest feeds
                if (removeInterests.Count > 0)
                {
                    foreach (var tag in removeInterests)
                    {
                        await connection.RemoveSortedSetAsync($"{Indentifier}_POSTTAGFEED_{tag}", post.Id);
                    }
                }
            }
            return post;
        }
    }
}
