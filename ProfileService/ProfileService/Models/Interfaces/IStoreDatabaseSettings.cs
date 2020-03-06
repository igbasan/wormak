using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Models.Interfaces
{
    public interface IStoreDatabaseSettings
    {
        string GeneralUsersCollectionName { get; set; }
        string CompaniesCollectionName { get; set; }
        string ProfessionalsCollectionName { get; set; }
        string CurrentProfilesCollectionName { get; set; }   
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
