using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Data.Interfaces
{
    public interface ICompanyDAO : IProfileDAO<Company>
    { 
    }
}
