using PostService.Logic.Interfaces;
using PostService.Models;
using PostService.Models.Exceptions;
using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PostService.Logic.Implementations
{
    public class FollowerLogic : IFollowerLogic
    {
        readonly IHttpHandler httpHandler;
        readonly IAppTokenStore tokenStore;
        readonly string interUserUrl;
        public FollowerLogic(IHttpHandler httpHandler, IAppTokenStore tokenStore, string interUserUrl)
        {
            this.httpHandler = httpHandler ?? throw new ArgumentNullException("httpHandler");
            this.interUserUrl = interUserUrl ?? throw new ArgumentNullException("interUserUrl");
            this.tokenStore = tokenStore ?? throw new ArgumentNullException("tokenStore");
        }
        public async Task<List<Profile>> GetFollowerProfilesByProfileIDAsync(string profileId, ProfileType profileType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(profileId)) throw new ArgumentNullException("profileId");

                httpHandler.AddDefaultRequestHeaders("Authorization", $"IntApp {await tokenStore.GetAppTokenAsync()}");
                HttpResponseMessage response = await httpHandler.GetAsync($"{interUserUrl}/Follower/GetAllFollowers?profileId={profileId}&profileType={profileType}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new PostServiceException("InterUser Service Unavailable");
                }
                string profilesDetailsString = await response.Content.ReadAsStringAsync();
                PostListResponse<Profile> profileList = JsonSerializer.Deserialize<PostListResponse<Profile>>(profilesDetailsString);

                return profileList?.Result ?? new List<Profile>();
            }
            finally
            {
                httpHandler.RemoveDefaultRequestHeaders("Authorization");
            }
        }

        public async Task<List<Profile>> GetFollowingProfilesByProfileIDAsync(string profileId, ProfileType profileType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(profileId)) throw new ArgumentNullException("profileId");

                httpHandler.AddDefaultRequestHeaders("Authorization", $"IntApp {await tokenStore.GetAppTokenAsync()}");
                HttpResponseMessage response = await httpHandler.GetAsync($"{interUserUrl}/Follower/GetAllFollowing?profileId={profileId}&profileType={profileType}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new PostServiceException("InterUser Service Unavailable");
                }
                string profilesDetailsString = await response.Content.ReadAsStringAsync();
                PostListResponse<Profile> profileList = JsonSerializer.Deserialize<PostListResponse<Profile>>(profilesDetailsString);

                return profileList?.Result ?? new List<Profile>();
            }
            finally
            {
                httpHandler.RemoveDefaultRequestHeaders("Authorization");
            }
        }
    }
}
