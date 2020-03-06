using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Data.Redis
{
    public class UserRDAO : IUserDAO
    {
        readonly IRedisConnection connection;
        readonly IUserDAO userDao;
        readonly string Indentifier = "USERS";
        public UserRDAO(IUserDAO userDao, IRedisConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException("connection");
            this.userDao = userDao ?? throw new ArgumentNullException("userDao");
        }
        public async Task<User> CreateUserAsync(User proflie)
        {
            User theUser = await userDao.CreateUserAsync(proflie);

            //return user after cache
            if (theUser != null)
            {
                //update cached values too
                await connection.SetAsync($"{Indentifier}_Email_{theUser.Email}", theUser,1);
                await connection.SetAsync($"{Indentifier}_ID_{theUser.Id}", theUser);
            }
            return theUser;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            //get from cache
            User theUser = await connection.GetAsync<User>($"{Indentifier}_Email_{email}");
            if (theUser != null) return theUser;

            //get if cache doesn't have the value
            theUser = await userDao.GetUserByEmailAsync(email);

            //return user after cache
            if(theUser != null) await connection.SetAsync($"{Indentifier}_Email_{email}", theUser,1);
            return theUser;
        }

        public async Task<User> GetUserByIDAsync(string id)
        {
            //get from cache
            User theUser = await connection.GetAsync<User>($"{Indentifier}_ID_{id}");
            if (theUser != null) return theUser;

            //get if cache doesn't have the value
            theUser = await userDao.GetUserByIDAsync(id);

            //return user after cache
            if (theUser != null) await connection.SetAsync($"{Indentifier}_ID_{id}", theUser);
            return theUser;
        }
    }
}
