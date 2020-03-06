using InterUserService.Data.Interfaces;
using InterUserService.Models.Implemetations;
using InterUserService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Data.Redis
{
    public class InterUserRDAO<T> : IInterUserDAO<T> where T : InterUser
    {
        readonly IRedisConnection connection;
        readonly IInterUserDAO<T> interUserDAO;
        protected readonly string Indentifier;
        public InterUserRDAO(IInterUserDAO<T> interUserDAO, IRedisConnection connection, string indentifier)
        {
            this.connection = connection ?? throw new ArgumentNullException("connection");
            this.interUserDAO = interUserDAO ?? throw new ArgumentNullException("interUserDAO");
            this.Indentifier = indentifier ?? throw new ArgumentNullException("indentifier");
        }

        public async Task<CountList<T>> GetAllByActiveProfileIDAsync(string profileID, int skip, int take)
        {
            string key = $"{Indentifier}_Sorted_ByActiveID_{profileID}";
            //get from cache
            CountList<T> theInterUsers = await connection.GetSortedSetAsync<T>(key, skip, take);
            if (theInterUsers?.Count > 0) return theInterUsers;

            //get if cache doesn't have the value, get all values
            theInterUsers = await interUserDAO.GetAllByActiveProfileIDAsync(profileID, 0, -1);

            //return user after cache
            if (theInterUsers == null || theInterUsers.Count == 0)
                return theInterUsers;

            Dictionary<double, T> valueScorePairs = new Dictionary<double, T>();
            
            foreach(var item in theInterUsers)
            {
                //convert from mongo's hexadecimal keys to double
                valueScorePairs.Add(HexToDoubleTool.ConvertToDouble(item.Id), item);
            }
            //cache all values on redis, it is individual so very quick
            await connection.SetSortedSetAsync(key, valueScorePairs);

            //get from individual cache
            return await connection.GetSortedSetAsync<T>(key, skip, take);
        }

        public async Task<CountList<T>> GetAllByPassiveProfileIDAsync(string profileID, int skip, int take)
        {
            string key = $"{Indentifier}_Sorted_ByPassiveID_{profileID}";
            //get from cache
            CountList<T> theInterUsers = await connection.GetSortedSetAsync<T>(key, skip, take);
            if (theInterUsers?.Count > 0) return theInterUsers;

            //get if cache doesn't have the value, get all values
            theInterUsers = await interUserDAO.GetAllByPassiveProfileIDAsync(profileID, 0, -1);

            //return user after cache
            if (theInterUsers == null || theInterUsers.Count == 0)
                return theInterUsers;

            Dictionary<double, T> valueScorePairs = new Dictionary<double, T>();

            foreach (var item in theInterUsers)
            {
                //convert from mongo's hexadecimal keys to double
                valueScorePairs.Add(HexToDoubleTool.ConvertToDouble(item.Id), item);
            }
            //cache all values on redis, it is individual so very quick
            await connection.SetSortedSetAsync(key, valueScorePairs);

            //get from individual cache
            return await connection.GetSortedSetAsync<T>(key, skip, take);
        }

        public async Task<T> CreateAsync(T interUser)
        {
            interUser = await interUserDAO.CreateAsync(interUser);

            //return user after cache
            if (interUser != null)
            {
                //update cached values too
                await connection.SetAsync($"{Indentifier}_IDs_{interUser.ActiveProfileID}_{interUser.PassiveProfileID}", interUser, 1);

                //remove from redis, to refresh
                await connection.RemoveSortedSetAsync($"{Indentifier}_Sorted_ByActiveID_{interUser.ActiveProfileID}");
                await connection.RemoveSortedSetAsync($"{Indentifier}_Sorted_ByPassiveID_{interUser.PassiveProfileID}");
            }
            return interUser;
        }

        public async Task<T> UpdateAsync(T interUser)
        {
            interUser = await interUserDAO.UpdateAsync(interUser);

            //return user after cache
            if (interUser != null)
            {
                //update cached values too
                await connection.SetAsync($"{Indentifier}_IDs_{interUser.ActiveProfileID}_{interUser.PassiveProfileID}", interUser, 1);

                //remove from redis, to refresh
                await connection.RemoveSortedSetAsync($"{Indentifier}_Sorted_ByActiveID_{interUser.ActiveProfileID}");
                await connection.RemoveSortedSetAsync($"{Indentifier}_Sorted_ByPassiveID_{interUser.PassiveProfileID}");
            }
            return interUser;
        }

        public async Task<T> GetByActiveProfileIDandPassiveProfileIDAsync(string activeProfileID, string passiveProfileID)
        {
            string key = $"{Indentifier}_IDs_{activeProfileID}_{passiveProfileID}";
            //get from cache
            T theProfile = await connection.GetAsync<T>(key);
            if (theProfile != null) return theProfile;

            //get if cache doesn't have the value
            theProfile = await interUserDAO.GetByActiveProfileIDandPassiveProfileIDAsync(activeProfileID, passiveProfileID);

            //return user after cache
            if (theProfile != null) await connection.SetAsync(key, theProfile, 1);
            return theProfile;
        }
    }
}
