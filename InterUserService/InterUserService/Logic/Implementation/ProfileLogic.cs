using InterUserService.Logic.Interfaces;
using InterUserService.Models;
using InterUserService.Models.Exceptions;
using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace InterUserService.Logic.Implementation
{
    public class ProfileLogic : IProfileLogic
    {
        readonly IHttpHandler httpHandler;
        readonly IAppTokenStore tokenStore;
        readonly string profileUrl;
        public ProfileLogic(IHttpHandler httpHandler, IAppTokenStore tokenStore, string profileUrl)
        {
            this.httpHandler = httpHandler ?? throw new ArgumentNullException("httpHandler");
            this.profileUrl = profileUrl ?? throw new ArgumentNullException("profileUrl");
            this.tokenStore = tokenStore ?? throw new ArgumentNullException("tokenStore");
        }
        public async Task<List<Profile>> GetProfilesAsync(Dictionary<string, ProfileType> idTypePairs)
        {
            try
            {
                if (idTypePairs == null || idTypePairs.Count == 0) throw new ArgumentNullException("idTypePairs");

                List<Profile> request = new List<Profile>();
                foreach (var item in idTypePairs)
                {
                    request.Add(new Profile { Id = item.Key, ProfileType = item.Value });
                }

                httpHandler.AddDefaultRequestHeaders("Authorization", $"IntApp {await tokenStore.GetAppTokenAsync()}");
                HttpResponseMessage response = await httpHandler.PostAsJSONAsync($"{profileUrl}/profile/GetProfiles", request);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InterUserException("Profile Service Unavailable");
                }
                string profilesDetailsString = await response.Content.ReadAsStringAsync();
                InterUserListResponse<Profile> profileList = JsonSerializer.Deserialize<InterUserListResponse<Profile>>(profilesDetailsString);

                return profileList?.Result ?? new List<Profile>();
            }
            finally
            {
                httpHandler.RemoveDefaultRequestHeaders("Authorization");
            }
        }
    }
}
