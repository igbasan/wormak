using AuthenticationService.WebAPI.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.WebAPI.Models.Implementations
{
    public class InternalServiceKeys : IInternalServiceKeys
    {
        public Dictionary<string, string> ServiceKeys { get; set; }
    }
}
