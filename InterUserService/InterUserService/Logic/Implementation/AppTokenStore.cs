﻿using InterUserService.Logic.Interfaces;
using InterUserService.Models.Exceptions;
using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace InterUserService.Logic.Implementation
{
    public class AppTokenStore : IAppTokenStore
    {
        protected static DateTime ExpirationTime;
        protected static string Token;
        private readonly IHttpHandler httpHandler;
        private readonly string authUrl;
        private readonly string appID;
        public AppTokenStore(IHttpHandler httpHandler, string authUrl, string appID)
        {
            this.httpHandler = httpHandler ?? throw new ArgumentNullException("httpHandler");
            this.authUrl = authUrl ?? throw new ArgumentNullException("authUrl");
            this.appID = appID ?? throw new ArgumentNullException("appID");
        }

        public async Task<string> GetAppTokenAsync()
        {
            if(ExpirationTime > DateTime.Now)
            {
                return Token;
            }
            HttpResponseMessage response = await httpHandler.GetAsync($"{authUrl}/internalService/SignIn?appKey={appID}");
            if (!response.IsSuccessStatusCode)
            {
                throw new InterUserException("Authentication Service Unavailable");
            }
            string tokenDetailsString = await response.Content.ReadAsStringAsync();
            AppToken appToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AppToken>(tokenDetailsString);
            Token = appToken.AccessToken;

            //ensure the token is refreshed 2 minutes before expiration
            ExpirationTime = DateTime.Now.AddSeconds(appToken.ExpiresIn).AddMinutes(-2);

            return Token;
        }
    }
}
