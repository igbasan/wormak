using ProfileService.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Models.Implementations
{
    public class StoreDatabaseSettings : IStoreDatabaseSettings
    {
        public string GeneralUsersCollectionName { get; set; }
        public string CompaniesCollectionName { get; set; }
        public string ProfessionalsCollectionName { get; set; }
        public string CurrentProfilesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
