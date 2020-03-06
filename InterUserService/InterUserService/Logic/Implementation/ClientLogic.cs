using InterUserService.Data.Interfaces;
using InterUserService.Logic.Interfaces;
using InterUserService.Models.Implemetations;

namespace InterUserService.Logic.Implementation
{
    public class ClientLogic : InterUserLogic<Client>, IClientLogic
    {
        public ClientLogic(IClientDAO clientDAO, IProfileLogic profileLogic) : base(clientDAO, profileLogic)
        {
        }
    }
}
