using AuthenticationService.WebAPI.Models.Interfaces;

namespace AuthenticationService.WebAPI.Models.Implementations
{
    public class StoreDatabaseSettings : IStoreDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string UserSessionsCollectionName { get; set; }
        public string LoginAttemptsCollectionName { get; set; }
        public string InternalServiceSessionsCollectionName { get; set; }      
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}