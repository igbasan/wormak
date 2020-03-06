using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Redis
{
    public class InternalServiceSessionRDAO : IInternalServiceSessionDAO
    {
        readonly IRedisConnection connection;
        readonly IInternalServiceSessionDAO internalServiceSessionDAO;
        readonly string Indentifier = "INTERNALSERVICESESSIONS";
        public InternalServiceSessionRDAO(IInternalServiceSessionDAO internalServiceSessionDAO, IRedisConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException("connection");
            this.internalServiceSessionDAO = internalServiceSessionDAO ?? throw new ArgumentNullException("internalServiceSessionDAO");
        }

        public async Task<InternalServiceSession> CreateServiceSessionAsync(InternalServiceSession session)
        {
            InternalServiceSession theServiceSession = await internalServiceSessionDAO.CreateServiceSessionAsync(session);

            //return user after cache
            if (theServiceSession != null)
            {
                //update cached values too
                await connection.SetAsync($"{Indentifier}_Token_{theServiceSession.AuthToken}", theServiceSession, 1);
                await connection.SetAsync($"{Indentifier}_AppKey_{theServiceSession.AppKey}", theServiceSession, 1);
            }
            return theServiceSession;
        }

        public async Task<InternalServiceSession> GetServiceSessionByAppKeyAsync(string appKey)
        {
            //get from cache
            InternalServiceSession theServiceSession  = await connection.GetAsync<InternalServiceSession>($"{Indentifier}_AppKey_{appKey}");
            if (theServiceSession != null) return theServiceSession;

            //get if cache doesn't have the value
            theServiceSession = await internalServiceSessionDAO.GetServiceSessionByAppKeyAsync(appKey);

            //return user after cache
            if (theServiceSession != null) await connection.SetAsync($"{Indentifier}_AppKey_{appKey}", theServiceSession, 1);
            return theServiceSession;
        }

        public async Task<InternalServiceSession> GetServiceSessionByTokenAsync(string token)
        {
            //get from cache
            InternalServiceSession theServiceSession = await connection.GetAsync<InternalServiceSession>($"{Indentifier}_Token_{token}");
            if (theServiceSession != null) return theServiceSession;

            //get if cache doesn't have the value
            theServiceSession = await internalServiceSessionDAO.GetServiceSessionByTokenAsync(token);

            //return user after cache
            if (theServiceSession != null) await connection.SetAsync($"{Indentifier}_Token_{token}", theServiceSession, 1);
            return theServiceSession;
        }

        public async Task<InternalServiceSession> UpdateServiceSessionAsync(InternalServiceSession session)
        {
            InternalServiceSession theServiceSession = await internalServiceSessionDAO.UpdateServiceSessionAsync(session);

            //return user after cache
            if (theServiceSession != null)
            {
                //update cached values too
                await connection.SetAsync($"{Indentifier}_Token_{theServiceSession.AuthToken}", theServiceSession, 1);
                await connection.SetAsync($"{Indentifier}_AppKey_{theServiceSession.AppKey}", theServiceSession, 1);
            }
            return theServiceSession;
        }
    }
}
