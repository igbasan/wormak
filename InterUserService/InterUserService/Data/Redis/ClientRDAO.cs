using InterUserService.Data.Interfaces;
using InterUserService.Models.Implemetations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Data.Redis
{
    public class ClientRDAO : InterUserRDAO<Client>, IClientDAO
    {
        public ClientRDAO(IClientDAO clientDAO, IRedisConnection connection) : base(clientDAO, connection, "CLIENT")
        {
        }
    }
}
