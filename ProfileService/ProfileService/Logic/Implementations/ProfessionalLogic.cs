using ProfileService.Data.Interfaces;
using ProfileService.Logic.Interfaces;
using ProfileService.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Logic.Implementations
{
    public class ProfessionalLogic : ProfileLogic<Professional>, IProfessionalLogic
    {
        public ProfessionalLogic(IProfessionalDAO professionalDAO) : base(professionalDAO)
        {
        }
    }
}