using PostService.Logic.Interfaces;
using PostService.Models;
using PostService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PostService.Logic.Implementations
{
    public class TokenValidator : ITokenValidator
    {
        readonly IHttpHandler httpHandler;
        readonly string authUrl;
        readonly string profileUrl;
        public TokenValidator(IHttpHandler httpHandler, string authUrl, string profileUrl)
        {
            this.httpHandler = httpHandler ?? throw new ArgumentNullException("httpHandler");
            this.authUrl = authUrl ?? throw new ArgumentNullException("authUrl");
            this.profileUrl = profileUrl ?? throw new ArgumentNullException("profileUrl");
        }
        public async Task<string> ValidateAppTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            string appName = string.Empty;

            HttpResponseMessage response = await httpHandler.GetAsync($"{authUrl}/internalService/VaidateServiceToken?token={token}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            appName = await response.Content.ReadAsStringAsync();

            return appName;
        }

        /// <summary>
        /// Returns null if the token is invalid
        /// </summary>
        /// <param name="token"></param>
        /// <returns>User details</returns>
        public async Task<Profile> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");
            try
            {
                Profile theProfile = null;

                httpHandler.AddDefaultRequestHeaders("Authorization", $"Bearer {token}");
                HttpResponseMessage response = await httpHandler.GetAsync($"{profileUrl}/Profile/GetCurrentInterestProfile");
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                string detailsString = await response.Content.ReadAsStringAsync();
                theProfile = JsonSerializer.Deserialize<PostObjectResponse<Profile>>(detailsString)?.Result;

                return theProfile;
            }
            catch //(Exception ex)
            {
                return null;
            }
            finally
            {
                httpHandler.RemoveDefaultRequestHeaders("Authorization");
            }
        }
    }
}
