using InterUserService.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Models.Implemetations
{
    public class StoreDatabaseSettings : IStoreDatabaseSettings
    {
        public string ClientsCollectionName { get; set; }
        public string TestimonialsCollectionName { get; set; }
        public string FollowersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
