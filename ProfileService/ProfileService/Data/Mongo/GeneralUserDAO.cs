using ProfileService.Data.Interfaces;
using ProfileService.Models.Implementations;
using ProfileService.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Mongo
{
    public class GeneralUserDAO : ProfileDAO<GeneralUser>, IGeneralUserDAO 
    {
        public GeneralUserDAO(IStoreDatabaseSettings settings) : base(settings)
        {
            _profiles = database.GetCollection<GeneralUser>(settings.GeneralUsersCollectionName);
        }
    }
}
