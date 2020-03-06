using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Models.Interfaces
{
    public interface IStoreDatabaseSettings
    {
        string ClientsCollectionName { get; set; }
        string TestimonialsCollectionName { get; set; }
        string FollowersCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
