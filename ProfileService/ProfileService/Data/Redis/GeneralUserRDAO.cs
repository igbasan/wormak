using ProfileService.Data.Interfaces;
using ProfileService.Models.Implementations;

namespace ProfileService.Data.Redis
{
    public class GeneralUserRDAO : ProfileRDAO<GeneralUser>, IGeneralUserDAO
    {
        public GeneralUserRDAO(IGeneralUserDAO generalUserDAO, IRedisConnection connection) : base(generalUserDAO, connection, "GENERALUSERPROFILE")
        {
        }
    }
}
