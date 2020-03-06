using ProfileService.Data.Interfaces;
using ProfileService.Models.Implementations;

namespace ProfileService.Data.Redis
{
    public class ProfessionalRDAO : ProfileRDAO<Professional>, IProfessionalDAO
    {
        public ProfessionalRDAO(IProfessionalDAO professionalDAO, IRedisConnection connection) : base(professionalDAO, connection, "PROFESSIONALPROFILE")
        {
        }
    }
}
