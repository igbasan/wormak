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
    public class InternalServiceSessionLogic : IInternalServiceSessionLogic
    {
        private readonly IInternalServiceSessionDAO internalServiceSessionDAO;
        private readonly int tokenExpirationInHours = 24;

        public InternalServiceSessionLogic(IInternalServiceSessionDAO internalServiceSessionDAO)
        {
            this.internalServiceSessionDAO = internalServiceSessionDAO ?? throw new ArgumentNullException("internalServiceSessionDAO");
        }

        public async Task<InternalServiceSession> GetServiceSessionByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException("token");

            //retrieve Session
            InternalServiceSession theServiceSession = await internalServiceSessionDAO.GetServiceSessionByTokenAsync(token);

            return theServiceSession;
        }

        public async Task<InternalServiceSession> SetUpServiceSessionAsync(string appName, string appKey)
        {
            if (string.IsNullOrWhiteSpace(appName)) throw new ArgumentNullException("appName");
            if (string.IsNullOrWhiteSpace(appKey)) throw new ArgumentNullException("appKey");

            InternalServiceSession theServiceSession = await internalServiceSessionDAO.GetServiceSessionByAppKeyAsync(appKey);

            //HashToken 
            var hashedToken = ShaHashTool.Hash($"{appName}-{Guid.NewGuid().ToString()}");

            //Save or Update UserSession Object
            if (theServiceSession == null)
            {
                theServiceSession = new InternalServiceSession
                {
                    AuthToken = hashedToken,
                    AppKey = appKey,
                    AppName = appName,
                    ExpirationDate = DateTime.Now.AddHours(tokenExpirationInHours),
                    FirstKeyExchangeDate = DateTime.Now,
                    LastExchangeDate = DateTime.Now
                };
                theServiceSession = await internalServiceSessionDAO.CreateServiceSessionAsync(theServiceSession);
            }
            else
            {
                theServiceSession.AuthToken = hashedToken;
                theServiceSession.ExpirationDate = DateTime.Now.AddHours(tokenExpirationInHours);
                theServiceSession.LastExchangeDate = DateTime.Now;
                theServiceSession.AppName = appName;

                theServiceSession = await internalServiceSessionDAO.UpdateServiceSessionAsync(theServiceSession);
            }

            return theServiceSession;
        }
    }
}
