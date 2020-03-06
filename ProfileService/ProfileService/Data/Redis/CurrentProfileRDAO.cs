using ProfileService.Data.Interfaces;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Redis
{
    public class CurrentProfileRDAO : ICurrentProfileDAO
    {
        readonly IRedisConnection connection;
        readonly ICurrentProfileDAO currentProfileDAO;
        protected readonly string Indentifier = "CURRENTPROFILE";
        public CurrentProfileRDAO(ICurrentProfileDAO currentProfileDAO, IRedisConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException("connection");
            this.currentProfileDAO = currentProfileDAO ?? throw new ArgumentNullException("currentProfileDAO");
        }

        public async Task<CurrentProfile> GetCurrentProfileAsync(string userID)
        {
            //get from cache
            CurrentProfile currentProfile = await connection.GetAsync<CurrentProfile>($"{Indentifier}_UserID_{userID}");
            if (currentProfile != null) return currentProfile;

            //get if cache doesn't have the value
            currentProfile = await currentProfileDAO.GetCurrentProfileAsync(userID);

            //return user after cache
            if (currentProfile != null) await connection.SetAsync($"{Indentifier}_UserID_{userID}", currentProfile, 1);
            return currentProfile;
        }

        public async Task<CurrentProfile> SaveCurrentProfileAsync(CurrentProfile currentProfile)
        {
            currentProfile = await currentProfileDAO.SaveCurrentProfileAsync(currentProfile);

            //return user after cache
            if (currentProfile != null)
            {
                //update cached values too
                await connection.SetAsync($"{Indentifier}_UserID_{currentProfile.UserID}", currentProfile, 1);
            }
            return currentProfile;
        }

        public async Task<CurrentProfile> UpdateCurrentProfileAsync(CurrentProfile currentProfile)
        {
            currentProfile = await currentProfileDAO.UpdateCurrentProfileAsync(currentProfile);

            //return user after cache
            if (currentProfile != null)
            {
                //update cached values too
                await connection.SetAsync($"{Indentifier}_UserID_{currentProfile.UserID}", currentProfile, 1);
            }
            return currentProfile;
        }
    }
}
