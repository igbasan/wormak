using InterUserService.Data.Interfaces;
using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Data.Redis
{
    public class FollowerRDAO : InterUserRDAO<Follower>, IFollowerDAO
    {
        public FollowerRDAO(IFollowerDAO followerDAO, IRedisConnection connection) : base(followerDAO, connection, "FOLLOWER")
        {
        }
    }
}
