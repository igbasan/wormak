using System;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AuthenticationService.WebAPI.Logic.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using AuthenticationService.WebAPI.Models.Implementations.Facebook;
using AuthenticationService.WebAPI.Models.Implementations.Google;
using AuthenticationService.WebAPI.Models.Implementations.LinkedIn;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
//using static AuthenticationService.Startup;

namespace AuthenticationService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IUserLogic userLogic;
        private readonly IUserSessionLogic userSessionLogic;
        private readonly IHttpHandler client;
        private readonly ILogger logger;

        public AccountController(IConfiguration configuration, IUserLogic userLogic, IUserSessionLogic userSessionLogic, IHttpHandler client, ILogger<AccountController> logger)
        {
            this.configuration = configuration;
            this.userLogic = userLogic ?? throw new ArgumentNullException("userLogic");
            this.userSessionLogic = userSessionLogic ?? throw new ArgumentNullException("userSessionLogic");
            this.client = client ?? throw new ArgumentNullException("client");
            this.logger = logger ?? throw new ArgumentNullException("logger");
        }

        [HttpGet]
        [Route("GoogleSignIn")]
        public async Task<IActionResult> HandleExternalLoginWithGoogle(string authorizationCode)
        {
            if (string.IsNullOrWhiteSpace(authorizationCode))
            {
                return Unauthorized("Invalid authorization Code");
            }
            string clientID = configuration["Authentication_Google_ClientId"];
            string clientSecret = configuration["Authentication_Google_ClientSecret"];
            string redirectUrl = configuration["Authentication_Google_CallbackUrl"];

            if (string.IsNullOrWhiteSpace(clientID)) return Unauthorized($"ClientId: {clientID}");


            //get access Token
            string requestUrl = $"https://oauth2.googleapis.com/token?code={authorizationCode}&client_id={clientID}&client_secret={clientSecret}&redirect_uri={redirectUrl}&grant_type=authorization_code";
            logger.LogInformation(requestUrl);

            var appAccessTokenResponseObj = await client.PostAsJsonAsync(requestUrl, new { });
            if (!appAccessTokenResponseObj.IsSuccessStatusCode)
            {
                return Unauthorized("Invalid authorization Code");
            }
            var appAccessTokenResponse = await appAccessTokenResponseObj.Content.ReadAsStringAsync();
            var appAccessToken = JsonConvert.DeserializeObject<GoogleAppAccessToken>(appAccessTokenResponse);

            //get user details
            var userInfoResponse = await client.GetStringAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={appAccessToken.AccessToken}");
            var userInfo = JsonConvert.DeserializeObject<GoogleUserData>(userInfoResponse);


            return await HandleExternalLogin(userInfo.GetUserDetails(), appAccessToken.AccessToken);
        }

        [HttpGet]
        [Route("FacebookSignIn")]
        public async Task<IActionResult> HandleExternalLoginWithFacebook(string token)
        {

            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Invalid token");
            }

            string appID = configuration["Authentication_Facebook_AppId"];
            string AppSecret = configuration["Authentication_Facebook_AppSecret"];

            //get access Token
            var appAccessTokenResponse = await client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={appID}&client_secret={AppSecret}&grant_type=client_credentials");
            var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);

            // 2. validate the user access token
            var userAccessTokenValidationResponse = await client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={token}&access_token={appAccessToken.AccessToken}");
            var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

            if (!userAccessTokenValidation.Data.IsValid)
            {
                return Unauthorized("login_failure: Invalid facebook token.");
            }

            var userInfoResponse = await client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={token}");
            var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);


            return await HandleExternalLogin(userInfo.GetUserDetails(), token);
        }

        [HttpGet]
        [Route("LinkedInSignIn")]
        public async Task<IActionResult> HandleExternalLoginWithLinkedIn(string authorizationCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authorizationCode))
                {
                    return Unauthorized("Invalid authorization Code");
                }

                string clientID = configuration["Authentication_LinkedIn_ClientId"];
                string clientSecret = configuration["Authentication_LinkedIn_ClientSecret"];
                string redirectUrl = configuration["Authentication_LinkedIn_CallbackUrl"];

                //get access Token
                var appAccessTokenResponseObj = await client.PostAsJsonAsync($"https://www.linkedin.com/oauth/v2/accessToken?grant_type=authorization_code&code={authorizationCode}&redirect_uri={redirectUrl}&client_id={clientID}&client_secret={clientSecret}", new { });
                if (!appAccessTokenResponseObj.IsSuccessStatusCode)
                {
                    return Unauthorized("Invalid authorization Code");
                }
                var appAccessTokenResponse = await appAccessTokenResponseObj.Content.ReadAsStringAsync();
                var appAccessToken = JsonConvert.DeserializeObject<LinkedInAppAccessToken>(appAccessTokenResponse);

                //get user details
                client.AddDefaultRequestHeaders("Authorization", $"Bearer {appAccessToken.AccessToken}");
                var userInfoResponse = await client.GetStringAsync($"https://api.linkedin.com/v2/me");
                var userInfo = JsonConvert.DeserializeObject<LinkedInUserData>(userInfoResponse);

                //get email
                var emailResponse = await client.GetStringAsync($"https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~))");
                string email = JObject.Parse(emailResponse)["elements"][0]["handle~"]["emailAddress"].Value<string>();

                User userDetails = userInfo.GetUserDetails();
                userDetails.Email = email;

                return await HandleExternalLogin(userDetails, appAccessToken.AccessToken);
            }
            catch(Exception ex)
            {
                return Unauthorized("Invalid authorization Code");
            }
            finally
            {
                client.RemoveDefaultRequestHeaders("Authorization");
            }
        }

        public async virtual Task<IActionResult> HandleExternalLogin(User userDetails, string token)
        {

            if (string.IsNullOrWhiteSpace(token)) return Unauthorized("Invalid token");
            if (userDetails == null) return Unauthorized("Invalid user details from service provider");

            //check if user exists
            var user = await userLogic.GetUserByEmailAsync(userDetails.Email);

            if (user == null)
            {
                user = new User
                {
                    Email = userDetails.Email,
                    LastName = userDetails.LastName,
                    UserName = userDetails.UserName,
                    FirstName = userDetails.FirstName,
                    LoginServiceProvider = userDetails.LoginServiceProvider
                };
                await userLogic.CreateUserAsync(user);
            }
            else
            {
                user.LoginServiceProvider = userDetails.LoginServiceProvider;
            }
            //var token = await httpContextAccessor.ContextGetTokenAsync("access_token");

            UserSession session = await userSessionLogic.SetUpUserSessionAsync(token, user);

            return new JsonResult(new AuthToken { TokenType = "Bearer", AccessToken = session.AuthToken, ExpiresIn = 24 * 3600 });
        }

        [HttpGet]
        [Route("GetUserByToken")]
        public async Task<IActionResult> GetUserByToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Invalid Token");
            }

            UserSession session = await userSessionLogic.GetUserSessionByTokenAsync(token);
            if (session == null)
            {
                return Unauthorized("Invalid Token");
            }
            if (session.ExpirationDate < DateTime.Now)
            {
                return Unauthorized("Session Expired, please login again.");
            }

            return new JsonResult(session.User);
        }
    }
}
