using ProfileService.Logic.Interfaces;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProfileService.Logic.Implementations
{
    public class TokenValidator : ITokenValidator
    {
        readonly IHttpHandler httpHandler;
        readonly string authUrl;
        public TokenValidator(IHttpHandler httpHandler, string authUrl)
        {
            this.httpHandler = httpHandler ?? throw new ArgumentNullException("httpHandler");
            this.authUrl = authUrl ?? throw new ArgumentNullException("authUrl");
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
        public async Task<User> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");
            User theUser = null;

            HttpResponseMessage response = await httpHandler.GetAsync($"{authUrl}/account/GetUserByToken?token={token}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            string detailsString = await response.Content.ReadAsStringAsync();
            theUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(detailsString);

            return theUser;
        }
    }
}
