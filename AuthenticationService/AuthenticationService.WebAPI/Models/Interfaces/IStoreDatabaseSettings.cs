using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Interfaces
{
    public interface IStoreDatabaseSettings
    {
        string UsersCollectionName { get; set; }
        string UserSessionsCollectionName { get; set; }
        string LoginAttemptsCollectionName { get; set; }
        string InternalServiceSessionsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
