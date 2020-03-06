using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Redis
{
    public class UserSessionRDAO : IUserSessionDAO
    {
        readonly IRedisConnection connection;
        readonly IUserSessionDAO userSessionDao;
        readonly string Indentifier = "USERSESSIONS";
        public UserSessionRDAO(IUserSessionDAO userSessionDao, IRedisConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException("connection");
            this.userSessionDao = userSessionDao ?? throw new ArgumentNullException("userSessionDao");
        }

        public async Task<UserSession> CreateUserSessionAsync(UserSession session)
        {
            UserSession theUserSession = await userSessionDao.CreateUserSessionAsync(session);

            //return user after cache
            if (theUserSession != null)
            {
                //update cached values too
                await connection.SetAsync($"{Indentifier}_Token_{theUserSession.AuthToken}", theUserSession,1);
                await connection.SetAsync($"{Indentifier}_UserID_{theUserSession.UserID}", theUserSession,1);
            }
            return theUserSession;
        }

        public async Task<UserSession> GetUserSessionByTokenAsync(string token)
        {
            //get from cache
            UserSession theUserSession = await connection.GetAsync<UserSession>($"{Indentifier}_Token_{token}");
            if (theUserSession != null) return theUserSession;

            //get if cache doesn't have the value
            theUserSession = await userSessionDao.GetUserSessionByTokenAsync(token);

            //return user after cache
            if (theUserSession != null) await connection.SetAsync($"{Indentifier}_Token_{token}", theUserSession,1);
            return theUserSession;
        }

        public async Task<UserSession> GetUserSessionByUserIDAsync(string userID)
        {
            //get from cache
            UserSession theUserSession = await connection.GetAsync<UserSession>($"{Indentifier}_UserID_{userID}");
            if (theUserSession != null) return theUserSession;

            //get if cache doesn't have the value
            theUserSession = await userSessionDao.GetUserSessionByUserIDAsync(userID);

            //return user after cache
            if (theUserSession != null) await connection.SetAsync($"{Indentifier}_UserID_{userID}", theUserSession,1);
            return theUserSession;
        }

        public async Task<UserSession> UpdateUserSessionAsync(UserSession session)
        {
            UserSession theUserSession = await userSessionDao.UpdateUserSessionAsync(session);

            //return user after cache
            if (theUserSession != null)
            {
                //update cached values too
                await connection.SetAsync($"{Indentifier}_Token_{theUserSession.AuthToken}", theUserSession,1);
                await connection.SetAsync($"{Indentifier}_UserID_{theUserSession.UserID}", theUserSession,1);
            }
            return theUserSession;
        }
    }
}
