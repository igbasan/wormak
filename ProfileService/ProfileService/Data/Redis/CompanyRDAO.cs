using ProfileService.Data.Interfaces;
using ProfileService.Models.Implementations;

namespace ProfileService.Data.Redis
{
    public class CompanyRDAO : ProfileRDAO<Company>, ICompanyDAO
    {
        public CompanyRDAO(ICompanyDAO companyDAO, IRedisConnection connection) : base(companyDAO, connection, "COMPANYPROFILE")
        {
        }
    }
}
