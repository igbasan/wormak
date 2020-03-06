using PostService.Data.Interfaces;
using PostService.Logic.Interfaces;
using PostService.Models;
using PostService.Models.Exceptions;
using PostService.Models.Implementations;
using PostService.Models.Implementations.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Logic.Implementations
{
    public class PostLogic : IPostLogic
    {
        readonly IPostDAO postDAO;
        readonly IProfileLogic profileLogic;
        readonly IFollowerLogic followerLogic;
        public PostLogic(IPostDAO postDAO, IProfileLogic profileLogic, IFollowerLogic followerLogic)
        {
            this.postDAO = postDAO ?? throw new ArgumentNullException("postDAO");
            this.profileLogic = profileLogic ?? throw new ArgumentNullException("profileLogic");
            this.followerLogic = followerLogic ?? throw new ArgumentNullException("followerLogic");
        }

        public async Task<Comment> CommentAsync(CommentRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            //get post by ID
            Post post = await postDAO.GetByIDAsync(request.PostID);
            if (post == null) throw new PostServiceException("The specified Post Id is invalid");

            Comment newComment = new Comment
            {
                DateAdded = DateTime.Now,
                Message = request.Message,
                ProfileID = $"{request.ProfileType}_{request.ProfileID}"
            };
            if (post.Comments == null)
            {
                post.Comments = new List<Comment>();
            }

            post.Comments.Add(newComment);

            await postDAO.UpdateAsync(post, null);
            return newComment;
        }

        public async Task<Post> GetByIDAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException("id");

            //get post by ID
            Post post = await postDAO.GetByIDAsync(id);

            if (post == null) return post;

            //set parameter for getting profileNames
            Dictionary<string, ProfileType> profilesDict = new Dictionary<string, ProfileType> { { post.ProfileIDRaw, post.ProfileType } };
            if (post.Comments == null) post.Comments = new List<Comment>();
            foreach (var item in post.Comments)
            {
                profilesDict[item.ProfileIDRaw] = item.ProfileType;
            }

            //get profile names
            List<Profile> profilesWithNames = await profileLogic.GetProfilesAsync(profilesDict);
            Dictionary<string, string> nameDict = new Dictionary<string, string>();
            profilesWithNames.ForEach(v => nameDict[$"{v.ProfileType}_{v.Id}"] = v.Name);

            //set Profile Names
            nameDict.TryGetValue(post.ProfileID, out string postProfileName);
            post.ProfileName = postProfileName;

            foreach (var item in post.Comments)
            {
                nameDict.TryGetValue(item.ProfileID, out string commentProfileName);
                item.ProfileName = commentProfileName;
            }

            return post;
        }

        public async Task<CountList<Post>> GetPostFeedAsync(string profileId, ProfileType profileType, List<Interest> interests, int skip, int take)
        {
            if (string.IsNullOrWhiteSpace(profileId)) throw new ArgumentNullException("profileId");

            //get user interests, get profile
            List<Profile> followerProfiles = (await followerLogic.GetFollowerProfilesByProfileIDAsync(profileId, profileType)) ?? new List<Profile>();
            List<string> followerProfileIds = followerProfiles.Select(c => $"{c.ProfileType}_{c.Id}").ToList();
            //get post feed 
            CountList<Post> feed = await postDAO.GetPostFeedAsync($"{profileType}_{profileId}", followerProfileIds, interests, skip, take);


            if (feed == null || feed.Count == 0) return feed;

            //set parameter for getting profileNames
            Dictionary<string, ProfileType> profilesDict = new Dictionary<string, ProfileType>();
            foreach (var post in feed)
            {
                profilesDict[post.ProfileIDRaw] = post.ProfileType;
                if (post.Comments == null) post.Comments = new List<Comment>();
                foreach (var item in post.Comments)
                {
                    profilesDict[item.ProfileIDRaw] = item.ProfileType;
                }
            }

            //get profile names
            List<Profile> profilesWithNames = await profileLogic.GetProfilesAsync(profilesDict);
            Dictionary<string, string> nameDict = new Dictionary<string, string>();
            profilesWithNames.ForEach(v => nameDict[$"{v.ProfileType}_{v.Id}"] = v.Name);

            //set Profile Names
            foreach (var post in feed)
            {
                nameDict.TryGetValue(post.ProfileID, out string postProfileName);
                post.ProfileName = postProfileName;

                foreach (var item in post.Comments)
                {
                    nameDict.TryGetValue(item.ProfileID, out string commentProfileName);
                    item.ProfileName = commentProfileName;
                }
            }

            return feed;
        }

        public async Task<Like> LikeAsync(Request request)
        {
            if (request == null) throw new ArgumentNullException("request");

            //get post by ID
            Post post = await postDAO.GetByIDAsync(request.PostID);
            if (post == null) throw new PostServiceException("The specified Post Id is invalid");

            Like newLike = new Like
            {
                DateLiked = DateTime.Now,
                ProfileID = $"{request.ProfileType}_{request.ProfileID}"
            };
            if (post.Likes == null)
            {
                post.Likes = new List<Like>();
            }

            //add and save like only if it is unique
            if (post.Likes.All(x => x.ProfileID != newLike.ProfileID))
            {
                post.Likes.Add(newLike);

                await postDAO.UpdateAsync(post, null);
            }
            return newLike;
        }

        public async Task<Post> PostAsync(PostNewRequest post)
        {
            if (post == null) throw new ArgumentNullException("post");

            Post newPost = new Post
            {
                DatePosted = DateTime.Now,
                ProfileID = $"{post.ProfileType}_{post.ProfileID}",
                Message = post.Message,
                Tags = post.Tags,
                Title = post.Title
            };

            //get follower get profile
            List<Profile> followerProfiles = (await followerLogic.GetFollowerProfilesByProfileIDAsync(newPost.ProfileIDRaw, newPost.ProfileType)) ?? new List<Profile>();
            List<string> profileIds = followerProfiles.Select(c => $"{c.ProfileType}_{c.Id}").ToList();

            //get interest profiles
            List<Profile> interestProfiles = new List<Profile>();
            if (newPost.Tags?.Count > 0)
            {
                interestProfiles = (await profileLogic.GetProfilesByInterestsAsync(newPost.Tags)) ?? new List<Profile>();
                profileIds.AddRange(interestProfiles.Select(c => $"{c.ProfileType}_{c.Id}").ToList());
            }
            profileIds.Distinct().ToList();

            await postDAO.SaveAsync(newPost, profileIds);
            return newPost;
        }

        public async Task<Post> UpdatePostAsync(PostRequest post)
        {
            if (post == null) throw new ArgumentNullException("post");

            //get post by ID
            Post existingPost = await postDAO.GetByIDAsync(post.PostID);
            if (existingPost == null) throw new PostServiceException("The specified Post Id is invalid");

            if (existingPost.ProfileIDRaw != post.ProfileID || existingPost.ProfileType != post.ProfileType)
            {
                throw new PostServiceException("The specified Post can only be edited by the post owner");
            }

            List<Interest> oldInterests = new List<Interest>();
            if (existingPost.Tags != null) oldInterests.AddRange(existingPost.Tags);

            existingPost.Message = post.Message;
            existingPost.Tags = post.Tags;
            existingPost.Title = post.Title;
            existingPost.IsEdited = true;

            //get follower get profile
            List<Profile> followerProfiles = (await followerLogic.GetFollowerProfilesByProfileIDAsync(existingPost.ProfileIDRaw, existingPost.ProfileType)) ?? new List<Profile>();
            List<string> profileIds = followerProfiles.Select(c => $"{c.ProfileType}_{c.Id}").ToList();

            //get interest profiles
            List<Profile> interestProfiles = new List<Profile>();
            if (existingPost.Tags?.Count > 0)
            {
                interestProfiles = (await profileLogic.GetProfilesByInterestsAsync(existingPost.Tags)) ?? new List<Profile>();
                profileIds.AddRange(interestProfiles.Select(c => $"{c.ProfileType}_{c.Id}").ToList());
            }
            profileIds.Distinct().ToList();

            await postDAO.UpdateAsync(existingPost, oldInterests);
            return existingPost;
        }
    }
}
