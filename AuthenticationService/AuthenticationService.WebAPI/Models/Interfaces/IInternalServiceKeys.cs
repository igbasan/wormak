using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Interfaces
{
   public interface IInternalServiceKeys
    {
        Dictionary<string,string> ServiceKeys { get; set; }
    }
}
