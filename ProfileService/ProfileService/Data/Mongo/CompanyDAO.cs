using ProfileService.Data.Interfaces;
using ProfileService.Models.Implementations;
using ProfileService.Models.Interfaces;

namespace ProfileService.Data.Mongo
{
    public class CompanyDAO : ProfileDAO<Company>, ICompanyDAO
    {
        public CompanyDAO(IStoreDatabaseSettings settings) : base(settings)
        {
            _profiles = database.GetCollection<Company>(settings.CompaniesCollectionName);
        }
    }
}
