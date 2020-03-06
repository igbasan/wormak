using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Logic.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using AuthenticationService.WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Logic.Implementations
{
    public class UserSessionLogic : IUserSessionLogic
    {
        private readonly IUserSessionDAO userSessionDAO;
        private readonly ILoginAttemptDAO loginAttemptDAO;
        private readonly IUserDAO userDAO;
        private readonly int tokenExpirationInHours = 24;

        public UserSessionLogic(IUserSessionDAO userSessionDAO, ILoginAttemptDAO loginAttemptDAO, IUserDAO userDAO)
        {
            this.userSessionDAO = userSessionDAO ?? throw new ArgumentNullException("userSessionDAO");
            this.loginAttemptDAO = loginAttemptDAO ?? throw new ArgumentNullException("loginAttemptDAO");
            this.userDAO = userDAO ?? throw new ArgumentNullException("userDAO");
        }
        public async Task<UserSession> GetUserSessionByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            //retrieve Session
            UserSession theUserSession = await userSessionDAO.GetUserSessionByTokenAsync(token);

            if (theUserSession != null)
            {
                //Retrieve User
                User theUser = await userDAO.GetUserByIDAsync(theUserSession.UserID);
                theUserSession.User = theUser;
            }

            return theUserSession;
        }

        public async Task<UserSession> SetUpUserSessionAsync(string token, User user)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");
            if (user == null) throw new ArgumentNullException("user");

            UserSession theUserSession = await userSessionDAO.GetUserSessionByUserIDAsync(user.Id);

            //HashToken 
            var hashedToken = ShaHashTool.Hash($"{user.LoginServiceProvider}-{token}");

            //Save or Update UserSession Object
            if (theUserSession == null)
            {
                theUserSession = new UserSession
                {
                    AuthToken = hashedToken,
                    UserID = user.Id,
                    User = user,
                    ExpirationDate = DateTime.Now.AddHours(tokenExpirationInHours),
                    FirstloginDate = DateTime.Now,
                    LastloginDate = DateTime.Now,
                    LoginServiceProvider = user.LoginServiceProvider,
                    ProviderAuthCode = token
                };
                theUserSession = await userSessionDAO.CreateUserSessionAsync(theUserSession);
            }
            else
            {
                theUserSession.AuthToken = hashedToken;
                theUserSession.ExpirationDate = DateTime.Now.AddHours(tokenExpirationInHours);
                theUserSession.LastloginDate = DateTime.Now;
                theUserSession.User = user;
                theUserSession.LoginServiceProvider = user.LoginServiceProvider;
                theUserSession.ProviderAuthCode = token;
                theUserSession = await userSessionDAO.UpdateUserSessionAsync(theUserSession);
            }

            //Save login attempt
            LoginAttempt loginAttempt = new LoginAttempt
            {
                AttemptDate = DateTime.Now,
                AuthToken = hashedToken,
                UserID = user.Id,
                SessionExpirationDate = theUserSession.ExpirationDate,
                SuccessfulAttempt = true,
                LoginServiceProvider = user.LoginServiceProvider,
                ProviderAuthCode = token
            };
            loginAttemptDAO.SaveLoginAttemptAsync(loginAttempt);

            return theUserSession;
        }
    }
}
