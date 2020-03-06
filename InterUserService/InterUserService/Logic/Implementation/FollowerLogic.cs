using InterUserService.Data.Interfaces;
using InterUserService.Logic.Interfaces;
using InterUserService.Models.Implemetations;

namespace InterUserService.Logic.Implementation
{
    public class FollowerLogic : InterUserLogic<Follower>, IFollowerLogic
    {
        public FollowerLogic(IFollowerDAO followerDAO, IProfileLogic profileLogic) : base(followerDAO, profileLogic)
        {
        }
    }
}
