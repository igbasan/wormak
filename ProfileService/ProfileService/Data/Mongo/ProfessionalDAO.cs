using ProfileService.Data.Interfaces;
using ProfileService.Models.Implementations;
using ProfileService.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Mongo
{
    public class ProfessionalDAO : ProfileDAO<Professional>, IProfessionalDAO
    {
        public ProfessionalDAO(IStoreDatabaseSettings settings) : base(settings)
        {
            _profiles = database.GetCollection<Professional>(settings.ProfessionalsCollectionName);
        }
    }
}
